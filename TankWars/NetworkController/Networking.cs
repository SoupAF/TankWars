using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetworkUtil
{

    public static class Networking
    {
        /////////////////////////////////////////////////////////////////////////////////////////
        // Server-Side Code
        /////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Starts a TcpListener on the specified port and starts an event-loop to accept new clients.
        /// The event-loop is started with BeginAcceptSocket and uses AcceptNewClient as the callback.
        /// AcceptNewClient will continue the event-loop.
        /// </summary>
        /// <param name="toCall">The method to call when a new connection is made</param>
        /// <param name="port">The the port to listen on</param>
        public static TcpListener StartServer(Action<SocketState> toCall, int port)
        {
            //Start the TCPListener
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            //Start the event loop to add new clients
            listener.BeginAcceptSocket(AcceptNewClient, Tuple.Create(toCall, listener));

            return listener;
        }

        /// <summary>
        /// To be used as the callback for accepting a new client that was initiated by StartServer, and 
        /// continues an event-loop to accept additional clients.
        ///
        /// Uses EndAcceptSocket to finalize the connection and create a new SocketState. The SocketState's
        /// OnNetworkAction should be set to the delegate that was passed to StartServer.
        /// Then invokes the OnNetworkAction delegate with the new SocketState so the user can take action. 
        /// 
        /// If anything goes wrong during the connection process (such as the server being stopped externally), 
        /// the OnNetworkAction delegate should be invoked with a new SocketState with its ErrorOccured flag set to true 
        /// and an appropriate message placed in its ErrorMessage field. The event-loop should not continue if
        /// an error occurs.
        ///
        /// If an error does not occur, after invoking OnNetworkAction with the new SocketState, an event-loop to accept 
        /// new clients should be continued by calling BeginAcceptSocket again with this method as the callback.
        /// </summary>
        /// <param name="ar">The object asynchronously passed via BeginAcceptSocket. It must contain a tuple with 
        /// 1) a delegate so the user can take action (a SocketState Action), and 2) the TcpListener</param>
        private static void AcceptNewClient(IAsyncResult ar)
        {
            //Attempt to finalize the connection process
            try
            {
                //Finalize the connection and create a socket to represent it
                Tuple<Action<SocketState>, TcpListener> args = (Tuple<Action<SocketState>, TcpListener>)ar.AsyncState;
                Socket s = args.Item2.EndAcceptSocket(ar);




                //Make a SocketState object
                SocketState state = new SocketState(args.Item1, s);
                state.OnNetworkAction(state);

                if (state.ErrorOccured)
                    return;

                //Start the event loop for getting data
                //GetData(state);

                //Contine the AcceptNewClient event loop
                args.Item2.BeginAcceptSocket(AcceptNewClient, args);
            }
            //If the connection fails, continue the event loop
            catch (System.ObjectDisposedException)
            { }

        }

        /// <summary>
        /// Stops the given TcpListener.
        /// </summary>
        public static void StopServer(TcpListener listener)
        {
            listener.Stop();
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        // Client-Side Code
        /////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Begins the asynchronous process of connecting to a server via BeginConnect, 
        /// and using ConnectedCallback as the method to finalize the connection once it's made.
        /// 
        /// If anything goes wrong during the connection process, toCall should be invoked 
        /// with a new SocketState with its ErrorOccured flag set to true and an appropriate message 
        /// placed in its ErrorMessage field. Between this method and ConnectedCallback, toCall should 
        /// only be invoked once on error.
        ///
        /// This connection process should timeout and produce an error (as discussed above) 
        /// if a connection can't be established within 3 seconds of starting BeginConnect.
        /// 
        /// </summary>
        /// <param name="toCall">The action to take once the connection is open or an error occurs</param>
        /// <param name="hostName">The server to connect to</param>
        /// <param name="port">The port on which the server is listening</param>
        public static void ConnectToServer(Action<SocketState> toCall, string hostName, int port)
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
                    //Create a new SocketState, mark it as an error, and invoke its toCall method
                    SocketState s = new SocketState(toCall, new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
                    s.ErrorOccured = true;
                    s.ErrorMessage = "The IPV4 address you entered could not be found";
                    toCall(s);

                }
            }
            catch (Exception)
            {
                // see if host name is a valid ipaddress
                try
                {
                    ipAddress = IPAddress.Parse(hostName);
                }
                catch (Exception)
                {
                    //Create a new SocketState, mark it as an error, and invoke its toCall method
                    SocketState s = new SocketState(toCall, new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
                    s.ErrorOccured = true;
                    s.ErrorMessage = "The hostname you entered could not be found";
                    toCall(s);

                }
            }

            // Create a TCP/IP socket.
            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // This disables Nagle's algorithm (google if curious!)
            // Nagle's algorithm can cause problems for a latency-sensitive 
            // game like ours will be 
            socket.NoDelay = true;

            //Create a new SocketState object to pass to the callback method
            SocketState state = new SocketState(toCall, socket);



            //Begin the connection
            IAsyncResult result = state.TheSocket.BeginConnect(ipAddress, port, ConnectedCallback, state);

            //Wait for 3 seconds and timeout the connection if it is not already finalized
            if (!result.AsyncWaitHandle.WaitOne(3000, true))
            {
                socket.Close();
                state.ErrorOccured = true;
                state.ErrorMessage = "failed to connect on timeout";
                state.OnNetworkAction(state);
            }


        }

        /// <summary>
        /// To be used as the callback for finalizing a connection process that was initiated by ConnectToServer.
        ///
        /// Uses EndConnect to finalize the connection.
        /// 
        /// As stated in the ConnectToServer documentation, if an error occurs during the connection process,
        /// either this method or ConnectToServer (not both) should indicate the error appropriately.
        /// 
        /// If a connection is successfully established, invokes the toCall Action that was provided to ConnectToServer (above)
        /// with a new SocketState representing the new connection.
        /// 
        /// </summary>
        /// <param name="ar">The object asynchronously passed via BeginConnect</param>
        private static void ConnectedCallback(IAsyncResult ar)
        {
            //Attempt to finalize the connection
            try
            {
                SocketState state = (SocketState)ar.AsyncState;
                state.TheSocket.EndConnect(ar);
                state.OnNetworkAction(state);
            }
            //If the connection failed, notify the user
            catch (Exception)
            {
                SocketState state = (SocketState)ar.AsyncState;
                state.ErrorMessage = "No server was found at the specified address";
                state.ErrorOccured = true;
                state.OnNetworkAction(state);

            }

        }


        /////////////////////////////////////////////////////////////////////////////////////////
        // Server and Client Common Code
        /////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Begins the asynchronous process of receiving data via BeginReceive, using ReceiveCallback 
        /// as the callback to finalize the receive and store data once it has arrived.
        /// The object passed to ReceiveCallback via the AsyncResult should be the SocketState.
        /// 
        /// If anything goes wrong during the receive process, the SocketState's ErrorOccured flag should 
        /// be set to true, and an appropriate message placed in ErrorMessage, then the SocketState's
        /// OnNetworkAction should be invoked. Between this method and ReceiveCallback, OnNetworkAction should only be 
        /// invoked once on error.
        /// 
        /// </summary>
        /// <param name="state">The SocketState to begin receiving</param>
        public static void GetData(SocketState state)
        {

            if (state.TheSocket.Connected)
                state.TheSocket.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, ReceiveCallback, state);
            else
            {
                state.ErrorMessage = "The socket was disconnected";
                state.ErrorOccured = true;
                state.OnNetworkAction(state);
            }
        }

        /// <summary>
        /// To be used as the callback for finalizing a receive operation that was initiated by GetData.
        /// 
        /// Uses EndReceive to finalize the receive.
        ///
        /// As stated in the GetData documentation, if an error occurs during the receive process,
        /// either this method or GetData (not both) should indicate the error appropriately.
        /// 
        /// If data is successfully received:
        ///  (1) Read the characters as UTF8 and put them in the SocketState's unprocessed data buffer (its string builder).
        ///      This must be done in a thread-safe manner with respect to the SocketState methods that access or modify its 
        ///      string builder.
        ///  (2) Call the saved delegate (OnNetworkAction) allowing the user to deal with this data.
        /// </summary>
        /// <param name="ar"> 
        /// This contains the SocketState that is stored with the callback when the initial BeginReceive is called.
        /// </param>
        private static void ReceiveCallback(IAsyncResult ar)
        {


            //makes a new SocketState and finalizes recieve
            SocketState state = (SocketState)ar.AsyncState;

            try
            {
                if (state.TheSocket.Connected)
                {
                    state.TheSocket.EndReceive(ar);

                    //create a string from the buffer and appends it to the string builder.
                    String s = Encoding.UTF8.GetString(state.buffer);

                    //If the message is not the as large as the entire buffer, the message is complete
                    int endof = s.IndexOf('\0');
                    if (endof != -1)
                    {
                        //Add the data to the final message, and clear the buffer
                        s = s.Substring(0, endof);
                        lock (state.data)
                        {
                            state.data.Append(s);
                            byte[] newBuff = new byte[4096];
                            state.buffer = newBuff;
                        }
                        //Trigger the user-defined callback method
                        state.OnNetworkAction(state);
                    }
                    //Otherwise, the event loop for recieving messages needs to continue
                    else
                    {
                        //Add the data to the final message, and clear the buffer
                        lock (state.data)
                        {
                            state.data.Append(s);
                            byte[] newBuff = new byte[4096];
                            state.buffer = newBuff;
                        }
                        GetData(state);
                    }
                }
            }
            catch (System.Net.Sockets.SocketException)
            {
                state.ErrorMessage = "The socket was disconnected";
                state.ErrorOccured = true;
                state.OnNetworkAction(state);
            }
        }

        /// <summary>
        /// Begin the asynchronous process of sending data via BeginSend, using SendCallback to finalize the send process.
        /// 
        /// If the socket is closed, does not attempt to send.
        /// 
        /// If a send fails for any reason, this method ensures that the Socket is closed before returning.
        /// </summary>
        /// <param name="socket">The socket on which to send the data</param>
        /// <param name="data">The string to send</param>
        /// <returns>True if the send process was started, false if an error occurs or the socket is already closed</returns>
        public static bool Send(Socket socket, string data)
        {

            //Convert the data string to a byte array
            byte[] buffer = Encoding.UTF8.GetBytes(data);

            //Begin to send the data using the provided socket
            try
            {
                socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallback, socket);
            }
            //If sending failed for any reason, return false 
            catch (System.Net.Sockets.SocketException)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// To be used as the callback for finalizing a send operation that was initiated by Send.
        ///
        /// Uses EndSend to finalize the send.
        /// 
        /// This method must not throw, even if an error occured during the Send operation.
        /// </summary>
        /// <param name="ar">
        /// This is the Socket (not SocketState) that is stored with the callback when
        /// the initial BeginSend is called.
        /// </param>
        private static void SendCallback(IAsyncResult ar)
        {
            //End the sending process
            Socket s = (Socket)ar.AsyncState;
            s.EndSend(ar);
        }


        /// <summary>
        /// Begin the asynchronous process of sending data via BeginSend, using SendAndCloseCallback to finalize the send process.
        /// This variant closes the socket in the callback once complete. This is useful for HTTP servers.
        /// 
        /// If the socket is closed, does not attempt to send.
        /// 
        /// If a send fails for any reason, this method ensures that the Socket is closed before returning.
        /// </summary>
        /// <param name="socket">The socket on which to send the data</param>
        /// <param name="data">The string to send</param>
        /// <returns>True if the send process was started, false if an error occurs or the socket is already closed</returns>
        public static bool SendAndClose(Socket socket, string data)
        {
            //Convert the data string to a byte array
            byte[] buffer = Encoding.UTF8.GetBytes(data);

            //Begin to send the data using the provided socket
            try
            {
                socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendAndCloseCallback, socket);
            }
            //If sending failed for any reason, close the socket and return false
            catch (System.Net.Sockets.SocketException)
            {
                socket.Close();
                return false;
            }

            return true;
        }

        /// <summary>
        /// To be used as the callback for finalizing a send operation that was initiated by SendAndClose.
        ///
        /// Uses EndSend to finalize the send, then closes the socket.
        /// 
        /// This method must not throw, even if an error occured during the Send operation.
        /// 
        /// This method ensures that the socket is closed before returning.
        /// </summary>
        /// <param name="ar">
        /// This is the Socket (not SocketState) that is stored with the callback when
        /// the initial BeginSend is called.
        /// </param>
        private static void SendAndCloseCallback(IAsyncResult ar)
        {
            //End the sending process, and close the socket
            Socket s = (Socket)ar.AsyncState;
            s.EndSend(ar);
            s.Close();
        }

    }
}