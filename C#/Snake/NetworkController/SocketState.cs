using System.Net.Sockets;
using System.Text;

namespace NetworkController
{
    /// <summary>
    /// Wraps a Socket and gives it an ID and a method to interpret and send data via a buffer
    /// </summary>
    public class SocketState
    {
        /// <summary>
        /// The Socket
        /// </summary>
        public Socket theSocket;

        /// <summary>
        /// The SocketState's ID
        /// </summary>
        public int ID;

        /// <summary>
        /// This is the buffer where we will receive message data from the client
        /// </summary>
        public byte[] messageBuffer = new byte[1024];

        /// <summary>
        /// Keep track of whether the client using this SocketState is disconnected or not
        /// </summary>
        public bool Disconnected { get; set; }

        /// <summary>
        /// This is a larger (growable) buffer, in case a single receive does not contain the full message.
        /// </summary>
        public StringBuilder sb = new StringBuilder();

        /// <summary>
        /// CallBackFunction delegate to be called when Network connections are made
        /// </summary>
        public CallBackFunction callBackFunction;

        /// <summary>
        /// Constructs a new SocketState with the given Socket and ID
        /// </summary>
        /// <param name="s"></param>
        /// <param name="id"></param>
        public SocketState(Socket s, int id)
        {
            theSocket = s;
            ID = id;
            Disconnected = false;
        }
    }
}