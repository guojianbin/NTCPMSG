﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

using NTCPMSG.Server;
using NTCPMSG.Event;

namespace Example
{
    class Server
    {
        /// <summary>
        /// DataReceived event will be called back when server get message from client which connect to.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        static void ReceiveEventHandler(object sender, ReceiveEventArgs args)
        {
            switch ((Event)args.Event)
            {
                case Event.OneWay:
                    //Get OneWay message from client
                    if (args.Data != null)
                    {
                        try
                        {
                            Encoding.UTF8.GetString(args.Data);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    break;
                case Event.Return:
                    //Get return message from client
                    if (args.Data != null)
                    {
                        try
                        {
                            int fromClient = BitConverter.ToInt32(args.Data, 0);

                            args.ReturnData = BitConverter.GetBytes(++fromClient);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// RemoteDisconnected event will be called back when specified client disconnected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        static void DisconnectEventHandler(object sender, DisconnectEventArgs args)
        {
            Console.WriteLine("Remote socket:{0} disconnected.", args.RemoteIPEndPoint);
        }

        /// <summary>
        /// Accepted event will be called back when specified client connected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        static void AcceptedEventHandler(object sender, AcceptEventArgs args)
        {
            Console.WriteLine("Remote socket:{0} connected.", args.RemoteIPEndPoint);
        }

        public static void Run(string[] args)
        {
            NTCPMSG.Server.NTcpListener listener;

            //Create a tcp listener that listen 2500 TCP port.
            listener = new NTcpListener(new IPEndPoint(IPAddress.Any, 2500));

            //DataReceived event will be called back when server get message from client which connect to.
            listener.DataReceived += new EventHandler<ReceiveEventArgs>(ReceiveEventHandler);

            //RemoteDisconnected event will be called back when specified client disconnected.
            listener.RemoteDisconnected += new EventHandler<DisconnectEventArgs>(DisconnectEventHandler);

            //Accepted event will be called back when specified client connected
            listener.Accepted += new EventHandler<AcceptEventArgs>(AcceptedEventHandler);

            //Start listening.
            //This function will not block current thread.
            listener.Listen();

            Console.WriteLine("Listening...");
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }
    }
}