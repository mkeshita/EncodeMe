using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.UDP;
using NetworkCommsDotNet.DPSBase;
using NetworkCommsDotNet.Tools;

namespace NORSU.EncodeMe.Network
{
    class Client
    {
        private static bool _started;
        
        private Client()
        {
           
        }

        private static Client _instance;
        private static Client Instance => _instance ?? (_instance = new Client());
        
        public static void Start()
        {
            if (_started) return;
            _started = true;
            
            NetworkComms.EnableLogging(new LiteLogger(LiteLogger.LogMode.ConsoleOnly));
            
            NetworkComms.IgnoreUnknownPacketTypes = true;
            var serializer = DPSManager.GetDataSerializer<ProtobufSerializer>();
            
            NetworkComms.DefaultSendReceiveOptions = new SendReceiveOptions(serializer,
                NetworkComms.DefaultSendReceiveOptions.DataProcessors, NetworkComms.DefaultSendReceiveOptions.Options);
            
            NetworkComms.AppendGlobalIncomingPacketHandler<ServerInfo>(nameof(ServerInfo), ServerInfoReceived);

            PeerDiscovery.EnableDiscoverable(PeerDiscovery.DiscoveryMethod.UDPBroadcast);

            PeerDiscovery.OnPeerDiscovered += OnPeerDiscovered;
            Connection.StartListening(ConnectionType.UDP, new IPEndPoint(IPAddress.Any, 0));
            
            PeerDiscovery.DiscoverPeersAsync(PeerDiscovery.DiscoveryMethod.UDPBroadcast);
            
            
        }

        public static void Stop()
        {
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

        private static async Task FindServer()
        {
            var start = DateTime.Now;
            PeerDiscovery.DiscoverPeersAsync(PeerDiscovery.DiscoveryMethod.UDPBroadcast);
            while ((DateTime.Now-start).TotalSeconds<20)
            {
                if(Server!=null) break;
                await TaskEx.Delay(TimeSpan.FromSeconds(7));
            }
        }
        
        public static async Task<LoginResult> Login(string username, string password)
        {
            //return Task.Factory.StartNew(() =>
            //{
                if(Server==null)
                    await FindServer();
                var result = new LoginResult();
                if (Server == null)
                {
                    result.Success = false;
                    result.Message = "Server is offline";
                    return result;
                }
                
                var login = new Login(){Username = username,Password = password};

                result = null;
                
                NetworkComms.AppendGlobalIncomingPacketHandler<LoginResult>(nameof(LoginResult),
                    (h, c, res) =>
                    {
                        NetworkComms.RemoveGlobalIncomingPacketHandler(nameof(LoginResult));
                        result = res;
                    });

                var start = DateTime.Now;
                await login.Send(new IPEndPoint(IPAddress.Parse(Server.IP), Server.Port));

                while ((DateTime.Now-start).TotalSeconds<17)
                {
                    if (result != null) return result;
                    await TaskEx.Delay(TimeSpan.FromSeconds(1));
                }
                
                result = new LoginResult("Request timeout");
                return result;
            //});
        }
      

    }
}
