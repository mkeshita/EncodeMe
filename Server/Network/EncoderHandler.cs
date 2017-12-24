using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NORSU.EncodeMe.Models;
using NORSU.EncodeMe.Properties;

namespace NORSU.EncodeMe.Network
{
    static class EncoderHandler
    {
        public static void LoginHandler(PacketHeader packetheader, Connection connection, Login login)
        {
            var ip = ((IPEndPoint)connection.ConnectionInfo.RemoteEndPoint).Address.ToString();
            var client = Client.Cache.FirstOrDefault(x => x.IP == ip);
            if (!(client?.IsEnabled ?? false))
            {
                Activity.Log(
                    Activity.Categories.Network,
                    Activity.Types.Warning,
                    $"Login attempted at an unauthorized terminal ({ip}).");
                return;
            }


            if ((DateTime.Now - client.LastHeartBeat).TotalSeconds > Settings.Default.LoginAttemptTimeout)
                client.LoginAttempts = 0;

            client.LoginAttempts++;

            if (client.LoginAttempts > Settings.Default.MaxLoginAttempts)
            {
                client.LastHeartBeat = DateTime.Now;
                new LoginResult(ResultCodes.Error, "Too many failed attempts").Send((IPEndPoint)connection.ConnectionInfo.RemoteEndPoint);
                return;
            }


            var encoder = Models.Encoder.Cache.FirstOrDefault(
                x => string.Equals(x.Username.Trim(), login.Username, StringComparison.CurrentCultureIgnoreCase) &&
                     !string.IsNullOrWhiteSpace(x.Username));

            if (encoder != null && !string.IsNullOrEmpty(login.Password) &&
                 (encoder.Password == login.Password || string.IsNullOrEmpty(encoder.Password)))
            {
                encoder.Update(nameof(encoder.Password), login.Password);
                //Logout previous session if any.
                var cl = Client.Cache.FirstOrDefault(x => x.Encoder?.Id == encoder.Id);
                cl?.Logout($"You are logged in at another terminal ({cl.IP}).");

                client.Encoder = encoder;
                new LoginResult(new Encoder()
                {
                    Username = encoder.Username,
                    FullName = encoder.FullName,
                    Picture = encoder.Thumbnail
                }).Send((IPEndPoint)connection.ConnectionInfo.RemoteEndPoint);
                client.LoginAttempts = 0;
            }
            else new LoginResult(ResultCodes.Error, "Invalid username/password").Send((IPEndPoint)connection.ConnectionInfo.RemoteEndPoint);

        }

        public static void HandShakeHandler(PacketHeader packetheader, Connection connection, EndPointInfo ep)
        {
            //Get known client or create new one.
            var client = Client.Cache.FirstOrDefault(x => x.IP == ep.IP) ?? new Client();


            client.IP = ep.IP;
            client.Hostname = ep.Hostname;
            client.Port = ep.Port;
            client.LastHeartBeat = DateTime.Now;
            client.Save();

            var localEPs = Connection.AllExistingLocalListenEndPoints();
            var serverInfo = new ServerInfo(Environment.MachineName);
            var ip = new IPEndPoint(IPAddress.Parse(ep.IP), ep.Port);

            foreach (var localEP in localEPs[ConnectionType.UDP])
            {
                var lEp = localEP as IPEndPoint;

                if (lEp == null) continue;
                if (!ip.Address.IsInSameSubnet(lEp.Address)) continue;

                serverInfo.IP = lEp.Address.ToString();
                serverInfo.Port = lEp.Port;
                serverInfo.Send(ip);
                break;
            }
        }

        public static void GetWorkHandler(PacketHeader packetheader, Connection connection, GetWork req)
        {
            var ip = ((IPEndPoint) connection.ConnectionInfo.RemoteEndPoint).Address.ToString();
            var client = Client.Cache.FirstOrDefault(x => x.IP == ip);
            if (!(client?.IsEnabled ?? false))
            {
                Activity.Log(
                    Activity.Categories.Network,
                    Activity.Types.Warning,
                    $"Work item requested at an unauthorized terminal ({ip}).");
                return;
            }

            var work = Request.GetNextRequest();
            work.Update(nameof(work.IsProcessing),true);

            var result = new GetWorkResult(ResultCodes.Success)
            {
                RequestId = work.Id,
                StudentId = work.StudentId
            };

            var items = RequestDetail.Cache.Where(x => x.RequestId == work.Id).ToList();
            foreach (var item in items)
            {
                var sched = Models.ClassSchedule.Cache.FirstOrDefault(x => x.Id == item.ScheduleId);
                result.ClassSchedules.Add(new ClassSchedule()
                {
                    ClassId = item.ScheduleId,
                    SubjectCode = item.SubjectCode,
                    Instructor = sched?.Instructor,
                    Room = sched?.Room,
                    Schedule = sched?.Description,
                });
            }

            result.Send(new IPEndPoint(IPAddress.Parse(client.IP), client.Port));
            
        }
    }
}
