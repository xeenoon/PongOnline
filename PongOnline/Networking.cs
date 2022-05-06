﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PongOnline
{
    public static class Networking
    {
        private static byte[] _buffer = new byte[10];
        public static List<Socket> _clientSockets = new List<Socket>();
        private static Socket _serversocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public static void SetupServer()
        {
            //Setting up server
            //    Console.Title = "Server";
            ShowText("IP=" + LocalIP());
            ShowText("Setting up server");
            _serversocket.Bind(new IPEndPoint(IPAddress.Any, 9999));
            _serversocket.Listen(5); //listen 1
            _serversocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            ShowText("Setup complete");
        }
        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket = _serversocket.EndAccept(AR);
            _clientSockets.Add(socket); //Socket is the client
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), socket);
            _serversocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }
        private static void RecieveCallback(IAsyncResult AR)
        {
            try
            {
                Socket socket = (Socket)AR.AsyncState;
                int recieved = socket.EndReceive(AR);
                byte[] dataBuf = new byte[recieved];
                Array.Copy(_buffer, dataBuf, recieved);

                string texts = Encoding.ASCII.GetString(dataBuf);

                var player = PongGameWindow.player1; //TODO: create a dictionary of sockets, each connected to a different Bat

                //Databuf can ONLY have a length of 1
                //This will contain 1 byte, which signifies what movement type this is
                switch (dataBuf[0])
                {
                    case 1: //UP key has been pressed
                        player.direction = Direction.UP;
                        break;
                    case 2: //DOWN key has been pressed
                        player.direction = Direction.DOWN;
                        break;
                    case 3: //Key has been RELEASED
                        player.direction = Direction.NONE;
                        break;
                }

                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), socket);
            }
            catch
            {

            }
        }
        public static void SendToAll(byte[] data)
        {
            foreach (var socket in _clientSockets)
            {
                socket.Send(data);
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), socket);
            }
        }
        public static string LocalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        private static void ShowText(string s)
        {
            //Do nothing, will implement logging later
        }
    }
}
