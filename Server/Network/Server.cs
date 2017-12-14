using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.UDP;
using NetworkCommsDotNet.DPSBase;
using NetworkCommsDotNet.Tools;
using NORSU.EncodeMe.Models;

namespace NORSU.EncodeMe.Network
{
    static class Server
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
            
            NetworkComms.AppendGlobalIncomingPacketHandler<EndPointInfo>(nameof(EndPointInfo), EndPointInfoReceived);
            NetworkComms.AppendGlobalIncomingPacketHandler<Login>(nameof(Login), LoginHandler);
            
            Connection.StartListening(ConnectionType.UDP, new IPEndPoint(IPAddress.Any, 0), true);
            
        }
        
        private static void LoginHandler(PacketHeader packetheader, Connection connection, Login login)
        {
            var ip = ((IPEndPoint) connection.ConnectionInfo.RemoteEndPoint).Address.ToString();
            var client = Client.Cache.FirstOrDefault(x => x.IP == ip);
            if (!(client?.IsEnabled ?? false))
            {
                Activity.Log(
                    Activity.Categories.Network,
                    Activity.Types.Warning,
                    $"Login attempted at an unauthorized terminal ({ip}).");
                return;
            }
            
            client.LoginAttempts++;
            
            var encoder = Models.Encoder.Cache.FirstOrDefault(x => x.Username == login.Username && x.Password==login.Password);
            
            if (encoder == null)
                new LoginResult(false).Send((IPEndPoint) connection.ConnectionInfo.RemoteEndPoint);
            else
            {
                //Logout previous session if any.
                var cl = Client.Cache.FirstOrDefault(x => x.Encoder == encoder);
                cl?.Logout($"You are logged in at another terminal ({cl.IP}).");

                client.Encoder = encoder;
                new LoginResult(new Encoder()
                {
                    Username = encoder.Username,
                    FullName = encoder.FullName,
                    Picture = encoder.Picture
                }).Send((IPEndPoint) connection.ConnectionInfo.RemoteEndPoint);
                client.LoginAttempts = 0;
            }
        }
        
        private static void EndPointInfoReceived(PacketHeader packetheader, Connection connection, EndPointInfo ep)
        {
            //Get known client or create new one.
            var client = Client.Cache.FirstOrDefault(x => x.IP == ep.IP);
            if (client == null)
            {
                client = new Client();
            }
            
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


        public static void Stop()
        {
            Connection.StopListening();
            NetworkComms.Shutdown();
        }

      


    }
}
