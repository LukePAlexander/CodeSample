using System.Net.Sockets;
using System.Net;

namespace NetworkController
{
    class TcpListenerState
    {
        public TcpListener TcpListener = new TcpListener(IPAddress.Any, Network.DEFAULT_PORT);

        public CallBackFunction callBack;
    }
}