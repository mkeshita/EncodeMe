using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using NetworkCommsDotNet.Connections.UDP;
using NetworkCommsDotNet.Tools;
using NORSU.EncodeMe.Annotations;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    abstract class Message :  INotifyPropertyChanged
    {
        public static implicit operator string([NotNull] Message value)
        {
            return value.Value;
        }

        private readonly string Value;
        protected Message(string message)
        {
            Value = message;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Task Send(IPEndPoint ip)
        {
            return Send(Value, this, ip);
        }

        private static async Task Send<T>(string msgType, T message, IPEndPoint ip)
        {
            var sent = false;
            while (!sent)
            {
                try
                {
                    UDPConnection.SendObject(msgType, message, ip, NetworkComms.DefaultSendReceiveOptions, ApplicationLayerProtocolStatus.Enabled);
                    sent = true;
                    break;
                }
                catch (Exception ex)
                {
                    await TaskEx.Delay(100);
                }
            }
        }

        private static IPEndPoint _broadcastEP;

        public static Task Broadcast<T>(string type, T message)
        {
            if (_broadcastEP == null)
                _broadcastEP = new IPEndPoint(IPAddress.Broadcast, Config.ServerPort);
            return Send(type, message, _broadcastEP);
        }
    }
}
