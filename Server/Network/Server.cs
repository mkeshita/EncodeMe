using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.DPSBase;
using NetworkCommsDotNet.Tools;
using NORSU.EncodeMe.Models;

namespace NORSU.EncodeMe.Network
{
    static partial class Server
    {
        private static bool _started;
        
        public static void Start()
        {
            if (_started) return;
            _started = true;
            
            var serializer = DPSManager.GetDataSerializer<ProtobufSerializer>();
            
            NetworkComms.DefaultSendReceiveOptions = new SendReceiveOptions(serializer,
                NetworkComms.DefaultSendReceiveOptions.DataProcessors, NetworkComms.DefaultSendReceiveOptions.Options);
            
            PeerDiscovery.EnableDiscoverable(PeerDiscovery.DiscoveryMethod.UDPBroadcast);
            
            
            NetworkComms.AppendGlobalIncomingPacketHandler<AndroidInfo>(AndroidInfo.GetHeader(), AndroidHandler.HandShakeHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<StudentInfoRequest>(StudentInfoRequest.GetHeader(), AndroidHandler.StudentInfoRequested);
            NetworkComms.AppendGlobalIncomingPacketHandler<EndPointInfo>(EndPointInfo.GetHeader(), HandShakeHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<GetWork>(GetWork.GetHeader(), GetWorkHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<Login>(Login.GetHeader(), LoginHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<SchedulesRequest>(SchedulesRequest.GetHeader(), AndroidHandler.ScheduleRequestHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<EnrollRequest>(EnrollRequest.GetHeader(),AndroidHandler.EnrollRequestHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<RegisterStudent>(RegisterStudent.GetHeader(), AndroidHandler.RegisterStudentHandler);
            
            Connection.StartListening(ConnectionType.UDP, new IPEndPoint(IPAddress.Any, 0), true);
            
        }
        
        public static void Stop()
        {
            Connection.StopListening();
            NetworkComms.Shutdown();
        }


        public static async Task PushUpdate(string studentId)
        {
            
        }

        public static Task SendEncoderUpdates()
        {
            return Task.Factory.StartNew(() =>
            {
                var update = new ServerUpdate();
                update.Requests = Request.Cache.Count(x => x.Status == Request.Statuses.Pending);
                update.Encoders = Client.Cache.Count(x => x.Encoder != null);
                //Parallel.ForEach(Client.Cache, c => update.Send(new IPEndPoint(IPAddress.Parse(c.IP), c.Port)));
                foreach (var c in Client.Cache) update.Send(new IPEndPoint(IPAddress.Parse(c.IP), c.Port));
            });
        }
    }
}
