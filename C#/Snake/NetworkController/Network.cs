using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace NetworkController
{
    /// <summary>
    /// Delegate used for various callback functions for network connections
    /// </summary>
    /// <param name="ss"></param>
    public delegate void CallBackFunction(SocketState ss);
    

    /// <summary>
    /// Static class that uses a SocketState to connect to clients and servers
    /// </summary>
    public static class Network
    {
        /// <summary>
        /// Default port used for network connections
        /// </summary>
        public const int DEFAULT_PORT = 11000;

        /// <summary>
        /// Handles the initial connection between client and server
        /// Attempts to connect to the server via a provided hostname.
        /// It saves the callback function (in a socket state object) for use when data arrives.
        /// This method takes the "state" object and "regurgitates" it back to you when a
        /// connection is made, thus allowing "communication" between this function and the ConnectedToServer function.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="hostName"></param>
        /// <returns></returns>
        public static SocketState ConnectToServer(CallBackFunction callback, string hostName)
        {
            // Debug
            System.Diagnostics.Debug.WriteLine("connecting  to " + hostName);

            // Connect to a remote device.
            try
            {

                // Establish the remote endpoint for the socket.
                IPHostEntry ipHostInfo;
                IPAddress ipAddress = IPAddress.None;

                // Determine if the server address is a URL or an IP
                try
                {
                    ipHostInfo = Dns.GetHostEntry(hostName);
                    bool foundIPV4 = false;
                    foreach (IPAddress addr in ipHostInfo.AddressList)
                        if (addr.AddressFamily != AddressFamily.InterNetworkV6)
                        {
                            foundIPV4 = true;
                            ipAddress = addr;
                            break;
                        }
                    // Didn't find any IPV4 addresses
                    if (!foundIPV4)
                    {
                        // Debug
                        System.Diagnostics.Debug.WriteLine("Invalid addres: " + hostName);
                        return null;
                    }
                }
                catch (Exception e1)
                {
                    // see if host name is actually an ipaddress, i.e., 155.99.123.456
                    System.Diagnostics.Debug.WriteLine("using IP");
                    ipAddress = IPAddress.Parse(hostName);
                }

                // Create a TCP/IP socket.
                Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

                SocketState theServer = new SocketState(socket, -1);

                theServer.callBackFunction = callback;

                // It will need to open a socket and then use the BeginConnect method.
                theServer.theSocket.BeginConnect(ipAddress, Network.DEFAULT_PORT, ConnectedToServer, theServer);

                return theServer;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Unable to connect to server. Error occured: " + e);
                return null;
            }

        }

        /// <summary>
        /// Helper method that runs after a connection has been made and calls the callback
        /// function given in the ConnectToServer method
        /// </summary>
        /// <param name="state_in_an_ar_object"></param>
        private static void ConnectedToServer(IAsyncResult state_in_an_ar_object)
        {
            // This function is referenced by the BeginConnect method above and is "called" by the OS when the 
            // socket connects to the server. The "state_in_an_ar_object" object contains a field "AsyncState" 
            // which contains the "state" object saved away in the above function.

            SocketState ss = (SocketState)state_in_an_ar_object.AsyncState;

            try
            {
                //Complete the connection.
                ss.theSocket.EndConnect(state_in_an_ar_object);
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to locate server.");
                System.Diagnostics.Debug.WriteLine("Unable to connect to server. Error occured: " + e);
                return;
            }

            // Once a connection is established the "saved away" callbackFunction needs to called. This function
            // is saved in the socket state, and was originally passed in to ConnectToServer.
            ss.callBackFunction(ss);
            ss.theSocket.BeginReceive(ss.messageBuffer, 0, ss.messageBuffer.Length, SocketFlags.None, ReceiveCallback, ss);
        }

        /// <summary>
        /// The ReceiveCallback method is called by the OS when new data arrives. This method should check to
        /// see how much data has arrived. If 0, the connection has been closed (presumably by the server).
        /// On greater than zero data, this method should call the callback function provided above.
        /// </summary>
        /// <param name="state_in_an_ar_object"></param>
        private static void ReceiveCallback(IAsyncResult state_in_an_ar_object)
        {
            SocketState ss = (SocketState)state_in_an_ar_object.AsyncState;

            int bytesRead = -1;

            // Catch if the socket closed withouth calling ShutDown()
            try
            {
                bytesRead = ss.theSocket.EndReceive(state_in_an_ar_object);
            }
            catch (Exception)
            {
                ss.Disconnected = true;
            }

            // If the socket is still open
            if (bytesRead > 0)
            {
                string theMessage = Encoding.UTF8.GetString(ss.messageBuffer, 0, bytesRead);
                // Append the received data to the growable buffer.
                // It may be an incomplete message, so we need to start building it up piece by piece
                ss.sb.Append(theMessage);
            }
            // The socket has been disconnected
            else
            {
                ss.Disconnected = true;
            }

            ss.callBackFunction(ss);
        }

        /// <summary>
        /// USED BY SERVER AND CLIENT 
        /// This is a small helper function that the client View code will call whenever it wants more data.
        /// Note: the client will probably want more data every time it gets data, and has finished processing
        /// it in its callbackFunction.
        /// </summary>
        /// <param name="state"></param>
        public static void GetData(SocketState state)
        {
            try
            {
                state.theSocket.BeginReceive(state.messageBuffer, 0, state.messageBuffer.Length, SocketFlags.None, ReceiveCallback, state);

            }
            catch (Exception)
            {
                // TODO: Determine how to handle an exception when GetData fails
                return;
            }
        }

        /// <summary>
        /// This function (along with its helper 'SendCallback') will allow a program to send data over a socket.
        /// This function needs to convert the data into bytes and then send them using socket.BeginSend.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        public static void Send(Socket socket, string data)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(data);
            try
            {
                socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, SendCallback, socket);
            }
            catch
            {
                //socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }

        /// <summary>
        /// This function "assists" the Send function. If all the data has been sent, then life 
        /// is good and nothing needs to be done
        /// </summary>
        /// <param name="state_in_an_ar_object"></param>
        private static void SendCallback(IAsyncResult state_in_an_ar_object)
        {
            Socket ss = (Socket)state_in_an_ar_object.AsyncState;
            // Nothing much to do here, just conclude the send operation so the socket is happy.
            ss.EndSend(state_in_an_ar_object);
        }



        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                                              SERVER SPECIFIC CODE BELOW
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// This is the heart of the server code. It should start a TCP listener for new connections and pass the listener, along 
        /// with the callMe function, to BeginAcceptSocket as the state parameter. Note: you should create a new type of state class 
        /// to hold a TcpListener plus the delegate. This is not the same as a SocketState. Upon a connection request coming in the 
        /// OS should invoke the AcceptNewClient as the callback method (see below).
        ///
        /// Note: while this method is called "Loop", it is not a traditional loop, but an "event loop" (i.e., this method sets
        /// up the connection listener, which, when a connection occurs, sets up a new connection listener for another connection).
        /// </summary>
        /// <param name="firstCallback"></param>
        public static void ServerAwaitingClientLoop(CallBackFunction firstCallback)
        {
            TcpListenerState listener = new TcpListenerState();
            listener.callBack = firstCallback;

            // Start the TcpListener to listen for new clients
            listener.TcpListener.Start();
            listener.TcpListener.BeginAcceptSocket(AcceptNewClient, listener);
        }

        /// <summary>
        /// This is the callback that BeginAcceptSocket should use. This code will be invoked by the OS when a connection request comes in. It should:
        /// 1. Extract the state containing the TcpListener and the callMe delegate from "ar"
        /// 2. Create a new socket with by using listener.EndAcceptSocket
        /// 3. Save the socket in a new SocketState
        /// 4. Call the callMe method and pass it the new SocketState
        /// 5. Await a new connection request(continue the event loop) with BeginAcceptSocket.
        /// </summary>
        /// <param name="state_in_an_ar_object"></param>
        public static void AcceptNewClient(IAsyncResult state_in_an_ar_object)
        {
            TcpListenerState listener = (TcpListenerState)state_in_an_ar_object.AsyncState;

            Socket socket = listener.TcpListener.EndAcceptSocket(state_in_an_ar_object);
            
            // Save the socket in a SocketState, 
            // so we can pass it to the receive callback, so we know which client we are dealing with.
            SocketState newClient = new SocketState(socket, -1);  // ID is -1 as a placeholder. It can be set in the server code
            
            // Start listening for more clients, goes back to the AcceptNewClient method
            listener.TcpListener.BeginAcceptSocket(AcceptNewClient, listener);

            // Call the callback function provided by the server
            listener.callBack(newClient);
        }
        
    }
}