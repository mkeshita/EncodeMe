using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.DPSBase;
using NetworkCommsDotNet.Tools;
using NORSU.EncodeMe.ViewModels;

namespace NORSU.EncodeMe.Network
{
    static class Client
    {
        private static bool _started;
        
        public static void Start()
        {
            if (_started) return;
            _started = true;
            
            NetworkComms.EnableLogging(new LiteLogger(LiteLogger.LogMode.ConsoleOnly));
            
            NetworkComms.IgnoreUnknownPacketTypes = true;
            var serializer = DPSManager.GetDataSerializer<ProtobufSerializer>();
            
            NetworkComms.DefaultSendReceiveOptions = new SendReceiveOptions(serializer,
                NetworkComms.DefaultSendReceiveOptions.DataProcessors, NetworkComms.DefaultSendReceiveOptions.Options);
            
            NetworkComms.AppendGlobalIncomingPacketHandler<ServerInfo>(ServerInfo.GetHeader(), ServerInfoReceived);
            NetworkComms.AppendGlobalIncomingPacketHandler<Logout>(Logout.GetHeader(), LogoutHandlger);
            
            
            PeerDiscovery.EnableDiscoverable(PeerDiscovery.DiscoveryMethod.UDPBroadcast);

            PeerDiscovery.OnPeerDiscovered += OnPeerDiscovered;
            Connection.StartListening(ConnectionType.UDP, new IPEndPoint(IPAddress.Any, 0));
            
            PeerDiscovery.DiscoverPeersAsync(PeerDiscovery.DiscoveryMethod.UDPBroadcast);
            
            
        }

        private static void LogoutHandlger(PacketHeader packetHeader, Connection connection, Logout incomingObject)
        {
            Encoder = null;
            MainViewModel.Instance.Encoder = null;
        }

        public static void Stop()
        {
            if(!_started) return;
            Connection.StopListening();
            NetworkComms.Shutdown();
        }

        private static ServerInfo Server;
        private static void ServerInfoReceived(PacketHeader packetheader, Connection connection, ServerInfo incomingobject)
        {
            Server = incomingobject;
        }

        private static void OnPeerDiscovered(ShortGuid peeridentifier, Dictionary<ConnectionType, List<EndPoint>> endPoints)
        {
            var info = new EndPointInfo(Environment.MachineName);
            
            var eps = endPoints[ConnectionType.UDP];
            var localEPs = Connection.AllExistingLocalListenEndPoints();
            
            foreach (var value in eps)
            {
                var ip = value as IPEndPoint;
                if (ip?.AddressFamily != AddressFamily.InterNetwork) continue;

                foreach (var localEP in localEPs[ConnectionType.UDP])
                {
                    var lEp = (IPEndPoint)localEP;
                    if (!ip.Address.IsInSameSubnet(lEp.Address)) continue;
                    info.IP = lEp.Address.ToString();
                    info.Port = lEp.Port;                   
                    info.Send(ip);
                }
            }
        }

        public static async Task FindServer()
        {
            Start();
            var start = DateTime.Now;
            PeerDiscovery.DiscoverPeersAsync(PeerDiscovery.DiscoveryMethod.UDPBroadcast);
            while ((DateTime.Now-start).TotalSeconds<20)
            {
                if(Server!=null) break;
                await TaskEx.Delay(TimeSpan.FromSeconds(7));
            }
        }
        
        public static Encoder Encoder { get; private set; }
        
        public static async Task<LoginResult> Login(string username, string password)
        {
                if(Server==null) await FindServer();
                if (Server == null) return new LoginResult(ResultCodes.Offline);
                
                var login = new Login(){Username = username,Password = password};

                LoginResult result = null;
                
                NetworkComms.AppendGlobalIncomingPacketHandler<LoginResult>(LoginResult.GetHeader(),
                    (h, c, res) =>
                    {
                        NetworkComms.RemoveGlobalIncomingPacketHandler(LoginResult.GetHeader());
                        result = res;
                        Encoder = res?.Encoder;
                    });
            
                await login.Send(new IPEndPoint(IPAddress.Parse(Server.IP), Server.Port));

            var start = DateTime.Now;
            while ((DateTime.Now-start).TotalSeconds<17)
                {
                    if (result != null) return result;
                    await TaskEx.Delay(TimeSpan.FromSeconds(1));
                }

            Server = null;

            NetworkComms.RemoveGlobalIncomingPacketHandler(LoginResult.GetHeader());
            
            return new LoginResult(ResultCodes.Timeout);
        }

        public static async Task<SaveWorkResult> SaveWork(SaveWork work)
        {
            if (Server == null) await FindServer();
            if (Server == null) return new SaveWorkResult() { Result = ResultCodes.Offline};

            SaveWorkResult result = null;
            
            NetworkComms.AppendGlobalIncomingPacketHandler<SaveWorkResult>(SaveWorkResult.GetHeader(),
                (h, c, i) =>
                {
                    NetworkComms.RemoveGlobalIncomingPacketHandler(SaveWorkResult.GetHeader());
                    result = i;
                });

            await work.Send(new IPEndPoint(IPAddress.Parse(Server.IP), Server.Port));

            var start = DateTime.Now;
            while ((DateTime.Now - start).TotalSeconds < 17)
            {
                if (result != null) return result;
                await TaskEx.Delay(TimeSpan.FromSeconds(1));
            }

            Server = null;
            NetworkComms.RemoveGlobalIncomingPacketHandler(SaveWorkResult.GetHeader());
            return new SaveWorkResult(){Result = ResultCodes.Timeout};
        }

        public static async Task<GetWorkResult> GetNextWork(string username="")
        {
            if (Server == null) await FindServer();
            if (Server == null) return new GetWorkResult(ResultCodes.Offline);
            
            var request = new GetWork(username);
            GetWorkResult result = null;
            
            NetworkComms.AppendGlobalIncomingPacketHandler<GetWorkResult>(GetWorkResult.GetHeader(),
                (h, c, i) =>
                {
                    NetworkComms.RemoveGlobalIncomingPacketHandler(GetWorkResult.GetHeader());
                    result = i;
                });

            await request.Send(new IPEndPoint(IPAddress.Parse(Server.IP), Server.Port));

            var start = DateTime.Now;
            while ((DateTime.Now - start).TotalSeconds < 17)
            {
                if (result != null) return result;
                await TaskEx.Delay(TimeSpan.FromSeconds(1));
            }

            Server = null;
            NetworkComms.RemoveGlobalIncomingPacketHandler(GetWorkResult.GetHeader());
            return new GetWorkResult(ResultCodes.Timeout);
        }
    }
}
