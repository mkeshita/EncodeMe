using System;
using System.Linq;
using System.Net;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NORSU.EncodeMe.Models;

namespace NORSU.EncodeMe.Network
{
    static class AndroidHandler
    {

        public static void HandShakeHandler(PacketHeader packetheader, Connection connection,
            AndroidInfo ad)
        {
            var device = AndroidDevice.Cache.FirstOrDefault(x => x.IP == ad.IP);
            if (device == null)
            {
                device = new AndroidDevice();
            }

            device.IP = ad.IP;
            device.Port = ad.Port;
            device.DeviceId = ad.DeviceId;
            device.MAC = ad.MAC;
            device.Model = ad.Model;
            device.SIM = ad.Sim;
            device.Save();

            var localEPs = Connection.AllExistingLocalListenEndPoints();
            var serverInfo = new ServerInfo(Environment.MachineName);
            var ip = new IPEndPoint(IPAddress.Parse(ad.IP), ad.Port);

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

        public static void StudentInfoRequested(PacketHeader packetheader, Connection connection,
            StudentInfoRequest incomingobject)
        {
            var result = new StudentInfoResult();
            if (string.IsNullOrWhiteSpace(incomingobject.StudentId))
            {
                result.Result = ResultCodes.NotFound;
                SendStudentInfoResult(result, connection);
                return;
            }
            //Not for production
            var student = Models.Student.Cache.FirstOrDefault(x => x.StudentId == incomingobject.StudentId);
            if (student == null)
            {
                result.Result = ResultCodes.NotFound;
                SendStudentInfoResult(result, connection);
                return;
            }
            
            result.Result = ResultCodes.Success;
            result.Student = new Student()
            {
                Course = student.Course,
                FirstName = student.FirstName,
                Id = student.Id,
                LastName = student.LastName,
                Picture = student.Picture,
                StudentId = student.StudentId
            };
            
            SendStudentInfoResult(result,connection);
        }

        private static void SendStudentInfoResult(StudentInfoResult result, Connection con)
        {
            var dev = AndroidDevice.Cache.FirstOrDefault(
                d => d.IP == ((IPEndPoint) con.ConnectionInfo.RemoteEndPoint).Address.ToString());
            
            //Maybe do not ignore this on production
            if (dev == null) return;

            var ip = new IPEndPoint(IPAddress.Parse(dev.IP),dev.Port);
            result.Send(ip);
        }
    }
}
