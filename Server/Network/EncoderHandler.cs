using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NORSU.EncodeMe.Models;
using NORSU.EncodeMe.Properties;

namespace NORSU.EncodeMe.Network
{
    static partial class Server
    {
        public static async void LoginHandler(PacketHeader packetheader, Connection connection, Login login)
        {
            var ip = ((IPEndPoint)connection.ConnectionInfo.RemoteEndPoint);
            var client = Client.Cache.FirstOrDefault(x => x.IpAddress == ip.Address.ToString());
            if (!(client?.IsEnabled ?? false))
            {
                Activity.Log(
                    Activity.Categories.Network,
                    Activity.Types.Warning,
                    $"Login attempted at an unauthorized terminal ({ip}).");
                return;
            }

            client.IsOnline = true;
            if ((DateTime.Now - client.LastHeartBeat).TotalSeconds > Settings.Default.LoginAttemptTimeout)
                client.LoginAttempts = 0;
            
            client.Update("LastHeartBeat", DateTime.Now);
            client.LoginAttempts++;

            if (client.LoginAttempts > Settings.Default.MaxLoginAttempts)
            {
                TerminalLog.Add(client.Id, "Login is disabled. Maximum attempts reached.", TerminalLog.Types.Warning);
                await new LoginResult(ResultCodes.Error, "Too many failed attempts").Send((IPEndPoint)connection.ConnectionInfo.RemoteEndPoint);
                return;
            }
            
            var encoder = Models.Encoder.Cache.FirstOrDefault(
                x => string.Equals(x.Username.Trim(), login.Username, StringComparison.CurrentCultureIgnoreCase) &&
                     !string.IsNullOrWhiteSpace(x.Username));

            if (encoder != null && !string.IsNullOrEmpty(login.Password) &&
                (encoder.Password == login.Password || string.IsNullOrEmpty(encoder.Password)))
            {
                encoder.Update(nameof(encoder.Password), login.Password);
                TerminalLog.Add(client.Id, $"{encoder.Username} has logged in.");
                //Logout previous session if any.
                var cl = Client.Cache.FirstOrDefault(x => x.Encoder?.Id == encoder.Id);
                if (cl != null)
                {
                    await new Logout() {Reason = $"You were logged in at another terminal ({cl.Hostname})."}
                        .Send(new IPEndPoint(ip.Address, cl.Port));
                    cl.Encoder = null;
                    cl.CancelRequest();
                }
                

                client.Encoder = encoder;
                await new LoginResult(new Encoder()
                {
                    Username = encoder.Username,
                    FullName = encoder.FullName,
                    Picture = encoder.Thumbnail,
                    WorkCount = Request.Cache.Count(x=>x.Status > Request.Statuses.Proccessing && x.EncoderId == encoder.Id),
                    Rate = encoder.WorkRate,
                    BestTime = TimeSpan.FromSeconds(encoder.BestTime).ToString(),
                    AverageTime = TimeSpan.FromSeconds(encoder.AverageTime).ToString(),
                }).Send((IPEndPoint) connection.ConnectionInfo.RemoteEndPoint);
                client.LoginAttempts = 0;
            }
            else
            {
                TerminalLog.Add(client.Id, $"Login attempt failed. Username: {login.Username}", TerminalLog.Types.Warning);
                await new LoginResult(ResultCodes.Error, "Invalid username/password").Send((IPEndPoint)connection.ConnectionInfo.RemoteEndPoint);
            }
         //   List<Client> clients;
         //   lock (_clients)
         //       clients = _clients.ToList();
            await SendEncoderUpdates(Client.Cache.ToList());
        }

        private static object _clientsLock = new object();
        private static Queue<Task> _handshakeTasks = new Queue<Task>();
        private static bool _handshakeTasksRunning;
       // private static List<Client> _clients = new List<Client>();
        private static Task Pinger;

        private static async void StartPinger(Client client)
        {
            while (client.IsOnline)
            {
                await TaskEx.Delay(5555);
                client.IsOnline = false;
                await new Ping().Send(new IPEndPoint(IPAddress.Parse(client.IpAddress), client.Port));
                await TaskEx.Delay(1111);
                if (!client.IsOnline)
                {
                    client.Encoder = null;
                    client.CancelRequest();
                }
            }
        }

        public static async void HandShakeHandler(PacketHeader packetheader, Connection connection, EndPointInfo ep)
        {
            
            lock(_clientsLock)
            _handshakeTasks.Enqueue(new Task(async () =>
            {
                //Get known client or create new one.
                var client = Client.Cache.FirstOrDefault(x => x.IpAddress == ep.IP); //.GetByName(ep.Hostname);
                if (client == null)
                {
                    client = new Client()
                    {
                        IpAddress = ep.IP
                    };
                        
                }
                
                if (client.IsDeleted && client.Id>0)
                {
                    client.Undelete();
                }
                //client.IpAddress = ep.IP;
                client.Hostname = ep.Hostname;
                client.Port = ep.Port;
                client.LastHeartBeat = DateTime.Now;
                var isNew = client.Id == 0;
                try
                {
                    client.Save();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                
                client.IsOnline = true;
                
                StartPinger(client);
                
                if (isNew)
                    TerminalLog.Add(client.Id, "Encoder terminal added.");

                TerminalLog.Add(client.Id, "Terminal has connected.");
            
                var localEPs = Connection.AllExistingLocalListenEndPoints();
                var serverInfo = new ServerInfo(Environment.MachineName);
                var ip = new IPEndPoint(IPAddress.Parse(ep.IP), ep.Port);

                foreach (var localEP in localEPs[ConnectionType.UDP])
                {
                    var lEp = localEP as IPEndPoint;
                
                    if (lEp == null) continue;
                    if(lEp.AddressFamily != AddressFamily.InterNetwork) continue;
                    if (!ip.Address.IsInSameSubnet(lEp.Address)) continue;

                    serverInfo.IP = lEp.Address.ToString();
                    serverInfo.Port = lEp.Port;
                    await serverInfo.Send(ip);
                    break;
                }
                
                await SendEncoderUpdates(Client.Cache.ToList());

                await TaskEx.Delay(100);
            }));
            
            if(_handshakeTasksRunning)
                return;
            _handshakeTasksRunning = true;

            await Task.Factory.StartNew(() =>
            {
                while(true)
                {
                    Task task = null;
                    lock(_clientsLock)
                    {
                        if(_handshakeTasks.Count == 0)
                            break;
                        task = _handshakeTasks.Dequeue();

                        if(task == null)
                            continue;
                        task.Start();
                        task.Wait();
                    }
                }
                _handshakeTasksRunning = false;
            });
        }
        
        public static async void GetWorkHandler(PacketHeader packetheader, Connection connection, GetWork req)
        {
            var ip = ((IPEndPoint) connection.ConnectionInfo.RemoteEndPoint);
            var client = Client.Cache.FirstOrDefault(x => x.IpAddress == ip.Address.ToString());
            if (!(client?.IsEnabled ?? false))
            {
                Activity.Log(
                    Activity.Categories.Network,
                    Activity.Types.Warning,
                    $"Work item requested at an unauthorized terminal ({ip}).");
                return;
            }

            if(client.Encoder == null) return;
            
            client.IsOnline = true;
            client.LastHeartBeat = DateTime.Now;
            TerminalLog.Add(client.Id, "Work item requested.");
            
            var work = Request.GetNextRequest(); 
            if (work == null)
            {
                await new GetWorkResult(ResultCodes.NotFound).Send(ip);//(new IPEndPoint(IPAddress.Parse(client.IP), client.Port));
                return;
            }
            
            //work.Update(nameof(work.Status),Request.Statuses.Proccessing);
            work.Status = Request.Statuses.Proccessing;

            var student = Models.Student.Cache.FirstOrDefault(x => x.Id == work.StudentId);
            if (student == null)
            {
                await new GetWorkResult(ResultCodes.NotFound).Send(ip); //(new IPEndPoint(IPAddress.Parse(client.IP),client.Port));
                return;
                
            }
            
            var result = new GetWorkResult(ResultCodes.Success)
            {
                RequestId = work.Id,
                Student = new Student()
                {
                    Address = student.Address,
                    BirthDate = student.BirthDate,
                    Course = student.Course.Acronym,
                    CourseId = student.CourseId,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    Major = student.Major,
                    Minor = student.Minor,
                    Male = student.Sex == Sexes.Male,
                    Id = student.Id,
                    Scholarship = student.Scholarship,
                    StudentId = student.StudentId,
                },
                Receipts = new List<Receipt>(),
            };
            
            foreach(var or in work.Receipts)
            {
                result.Receipts.Add(
                    new Receipt()
                    {
                        Amount = or.AmountPaid,
                        DatePaid = or.DatePaid,
                        Number = or.Number,
                    });
            }

            var items = RequestDetail.Cache.Where(x => x.RequestId == work.Id).ToList();
            foreach (var item in items)
            {
                var sched = Models.ClassSchedule.Cache.FirstOrDefault(x => x.Id == item.ScheduleId);
                result.ClassSchedules.Add(new ClassSchedule()
                {
                    ClassId = item.ScheduleId,
                    SubjectCode = item.Schedule.Subject.Code,
                    Instructor = sched?.Instructor,
                    Room = sched?.Room,
                    Schedule = sched?.Description,
                });
            }

            await result.Send(ip);
            
            await SendEncoderUpdates(Client.Cache.ToList());

            client.Encoder.StartWork();
            work.StartWorking();
            client.Request?.Update(nameof(Models.Request.Status),Request.Statuses.Pending);
            client.Request = work;
        }

        private static async void SaveWorkHandler(PacketHeader packetheader, Connection connection, SaveWork i)
        {
            var ip = ((IPEndPoint) connection.ConnectionInfo.RemoteEndPoint);
            var client = Client.Cache.FirstOrDefault(x => x.IpAddress == ip.Address.ToString());
            if (!(client?.IsEnabled ?? false))
                return;

            client.IsOnline = true;
            client.LastHeartBeat = DateTime.Now;

            if (client.Encoder == null)
            {
                var res = new SaveWorkResult();
                res.Success = false;
                res.ErrorMessage = "Request Denied";
                await res.Send(ip);// (new IPEndPoint(IPAddress.Parse(client.IpAddress), client.Port));
                return;
            }
            
            TerminalLog.Add(client.Id, $"Enrollment request completed. Encoder: {client.Encoder.Username}");

            var req = client.Request;
            if (req != null && req.Id != i.RequestId)
            {
                client.CancelRequest();
                req = Request.GetById(i.RequestId);
            }
            if (req == null) return;
            
            foreach (var sched in i.ClassSchedules)
            {
                var s = RequestDetail.Cache.FirstOrDefault(x => x.RequestId == req.Id && x.ScheduleId == sched.ClassId);
                if(s==null) continue;
                var stat = Request.Statuses.Pending;
                switch (sched.EnrollmentStatus)
                {
                    case ScheduleStatuses.Accepted:
                        stat = Request.Statuses.Accepted;
                        break;
                    case ScheduleStatuses.Conflict:
                        stat = Request.Statuses.Conflict;
                        break;
                    case ScheduleStatuses.Closed:
                        stat = Request.Statuses.Closed;
                        break;
                }
                s.Update(nameof(s.Status), stat);
            }

            if (i.ClassSchedules.Any(x => x.EnrollmentStatus == ScheduleStatuses.Closed))
            {
                req.Status = Request.Statuses.Closed;
            }
            else if (i.ClassSchedules.Any(x => x.EnrollmentStatus == ScheduleStatuses.Conflict))
            {
                req.Status = Request.Statuses.Conflict;
            }
            else if (i.ClassSchedules.All(x => x.EnrollmentStatus == ScheduleStatuses.Accepted))
            {
                req.Status = Request.Statuses.Accepted;
            }
            else if (i.ClassSchedules.All(x => x.EnrollmentStatus == ScheduleStatuses.Pending))
            {
                req.Status = Request.Statuses.Pending;
            }
            
            
            req.EncoderId = client.Encoder.Id;
            req.Save();

            client.Encoder.EndWork();
            req.StopWorking();

            //PushRequestUpdate(req);
            
            var result = new SaveWorkResult
            {
                Success = true,
                WorkCount = Request.Cache.Count(x =>x.Status > Request.Statuses.Proccessing && x.EncoderId == client.Encoder.Id),
                BestTime = TimeSpan.FromSeconds(client.Encoder.BestTime).ToString(),
                AverageTime = TimeSpan.FromSeconds(client.Encoder.AverageTime).ToString()
            };

            await result.Send(ip); //(new IPEndPoint(IPAddress.Parse(client.IP), client.Port));
            
            await SendEncoderUpdates(Client.Cache.ToList());
        }

        private static void LogoutHandler(PacketHeader packetheader, Connection connection, Logout incomingobject)
        {
            var ip = ((IPEndPoint) connection.ConnectionInfo.RemoteEndPoint);
            var client = Client.Cache.FirstOrDefault(x => x.IpAddress == ip.Address.ToString());
            if (client?.Encoder == null) return;
            
            TerminalLog.Add(client.Id, $"{client.Encoder.Username} has logged out.");
            client.Encoder = null;
            client.CancelRequest();
        }

        private static void PongHandler(PacketHeader packetheader, Connection connection, Pong incomingobject)
        {
            var ip = ((IPEndPoint) connection.ConnectionInfo.RemoteEndPoint);
            var client = Client.Cache.FirstOrDefault(x => x.IpAddress == ip.Address.ToString());
            if (client == null) return;
            client.IsOnline = true;
            
        }

        private static void GetCoursesHandler(PacketHeader packetheader, Connection connection, GetCoursesDesktop incomingobject)
        {
            var ip = ((IPEndPoint) connection.ConnectionInfo.RemoteEndPoint);
            var client = Client.Cache.FirstOrDefault(x => x.IpAddress == ip.Address.ToString());

            //Maybe do not ignore this on production
            if (client == null)
                return;

            var result = new GetCoursesResult();
            foreach (var course in Models.Course.Cache.ToList())
            {
                result.Courses.Add(new Course()
                {
                    Id = course.Id,
                    Name = course.Acronym,
                    Fullname = course.FullName
                });
            }
            var rep = (IPEndPoint) connection.ConnectionInfo.RemoteEndPoint;
            result.Send(ip);// (new IPEndPoint(IPAddress.Parse(client.IP), rep.Port));
        }

        private static async void EnrollStudentHandler(PacketHeader packetheader, Connection connection, EnrollStudent req)
        {
            var ip = ((IPEndPoint) connection.ConnectionInfo.RemoteEndPoint);
            var client = Client.Cache.FirstOrDefault(x => x.IpAddress == ip.Address.ToString());

            //Maybe do not ignore this on production
            if (client == null)
                return;

            if (Models.Student.Cache.Any(x => x.StudentId.ToLower() == req.Student.StudentId.ToLower()))
            {
                await new EnrollStudentResult()
                {
                    Success = false,
                    ErrorMessage = "Student ID Taken"
                }.Send(ip);// (new IPEndPoint(IPAddress.Parse(client.IP), client.Port));
                return;
            }
            var stud = new Models.Student()
            {
                StudentId = req.Student.StudentId,
                Address = req.Student.Address,
                BirthDate = req.Student.BirthDate,
                CourseId = req.Student.CourseId,
                FirstName = req.Student.FirstName,
                LastName = req.Student.LastName,
                Major = req.Student.Major,
                Minor = req.Student.Minor,
                Scholarship = req.Student.Scholarship,
                Sex = req.Student.Male?Sexes.Male:Sexes.Female,
                YearLevel = (YearLevels) req.Student.YearLevel,
                Status = (StudentStatus)req.Student.Status,
            };
            stud.Save();

            await new EnrollStudentResult()
            {
                Success = true,
                Id = stud.Id
            }.Send(ip);// (new IPEndPoint(IPAddress.Parse(client.IP), client.Port));
        }
    }
}
