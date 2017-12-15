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
using NORSU.EncodeMe.Properties;

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

            
            if ((DateTime.Now - client.LastHeartBeat).TotalSeconds > Settings.Default.LoginAttemptTimeout)
                client.LoginAttempts = 0;
            
            client.LoginAttempts++;

            if (client.LoginAttempts > Settings.Default.MaxLoginAttempts)
            {
                client.LastHeartBeat = DateTime.Now;
                new LoginResult("Too many failed attempts").Send((IPEndPoint) connection.ConnectionInfo.RemoteEndPoint);
                return;
            }

            
            var encoder = Models.Encoder.Cache.FirstOrDefault(
                x => string.Equals(x.Username.Trim(), login.Username, StringComparison.CurrentCultureIgnoreCase) && 
                     !string.IsNullOrWhiteSpace(x.Username));

            if (encoder != null && !string.IsNullOrEmpty(login.Password) &&
                 (encoder.Password == login.Password || encoder.Password == ""))
            {
                encoder.Update(nameof(encoder.Password),login.Password);
                //Logout previous session if any.
                var cl = Client.Cache.FirstOrDefault(x => x.Encoder == encoder);
                cl?.Logout($"You are logged in at another terminal ({cl.IP}).");

                client.Encoder = encoder;
                new LoginResult(new Encoder()
                {
                    Username = encoder.Username,
                    FullName = encoder.FullName,
                    Picture = encoder.Thumbnail
                }).Send((IPEndPoint) connection.ConnectionInfo.RemoteEndPoint);
                client.LoginAttempts = 0;
            }
            else new LoginResult("Invalid username/password").Send((IPEndPoint) connection.ConnectionInfo.RemoteEndPoint);
            
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
