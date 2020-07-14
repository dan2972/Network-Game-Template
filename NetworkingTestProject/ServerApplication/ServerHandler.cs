using Lidgren.Network;
using NetworkingTestProjectLibrary.Entities;
using NetworkingTestProjectLibrary.Entities.Blocks;
using NetworkingTestProjectLibrary.Entities.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication
{
    class ServerHandler
    {
        public NetServer server { get; }

        private NetPeerConfiguration config;

        private EntityList eList;
        private GameMap gMap;

        public ServerHandler(EntityList eList, GameMap gMap)
        {
            this.eList = eList;
            this.gMap = gMap;

            config = new NetPeerConfiguration("NetworkTestGame")
            { Port = 14242 };

            config.EnableUPnP = true;
            //config.LocalAddress = IPAddress.Parse("172.30.1.32");
            config.MaximumConnections = 8;
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            server = new NetServer(config);

            server.Start();

            server.UPnP.ForwardPort(14242, "NetworkGameTest");

            if (server.Status == NetPeerStatus.Running)
                Console.WriteLine("Server is running on port " + config.Port);
            else
                Console.WriteLine("Server not started...");
        }

        public void update()
        {
            NetIncomingMessage message;
            while ((message = server.ReadMessage()) != null)
            {
                switch (message.MessageType)
                {

                    case NetIncomingMessageType.ConnectionApproval:
                        string s = message.ReadString();
                        if (s == "approval message")
                        {
                            message.SenderConnection.Approve();
                        }
                        else
                            message.SenderConnection.Deny();
                        break;

                    case NetIncomingMessageType.Data:
                        string headerMsg = message.ReadString();

                        if (headerMsg.Equals("create player"))
                        {
                            string name = message.ReadString();
                            float x = message.ReadFloat();
                            float y = message.ReadFloat();
                            eList.addPlayer(new Player(x, y, name, gMap, eList));
                            for (int i = 0; i < eList.playerList.Count; i++)
                            {
                                Player tempPlayer = (Player)eList.playerList[i];
                                NetOutgoingMessage outMsg = server.CreateMessage();
                                outMsg.Write("create player");
                                outMsg.Write(tempPlayer.username);
                                outMsg.Write(tempPlayer.x);
                                outMsg.Write(tempPlayer.y);
                                server.SendToAll(outMsg, NetDeliveryMethod.ReliableOrdered);
                            }
                        }
                        else if (headerMsg.Equals("player ability use"))
                        {
                            string playerName = message.ReadString();
                            string abilityType = message.ReadString();
                            int mx = message.ReadInt32();
                            int my = message.ReadInt32();
                            for (int i = 0; i < eList.playerList.Count; i++)
                            {
                                Player tempPlayer = (Player)eList.playerList[i];
                                if (playerName.Equals(tempPlayer.getUsername()))
                                {
                                    if (abilityType.Equals("primary"))
                                    {
                                        tempPlayer.usePrimary(mx, my, server);
                                    }
                                }
                            }
                        }
                        else if (headerMsg.Equals("disconnected"))
                        {
                            string name = message.ReadString();
                            for (int i = 0; i < eList.playerList.Count; i++)
                            {
                                Player tempPlayer = (Player)eList.playerList[i];
                                if (name.Equals(tempPlayer.getUsername()))
                                {
                                    eList.removePlayer(tempPlayer);
                                    Console.WriteLine("removed player " + name);
                                }
                            }
                            NetOutgoingMessage outMsg = server.CreateMessage();
                            outMsg.Write("disconnected");
                            outMsg.Write(name);
                            server.SendToAll(outMsg, NetDeliveryMethod.ReliableOrdered);
                        }
                        else if (headerMsg.Equals("update keyboard state player"))
                        {
                            string name = message.ReadString();
                            string direction = message.ReadString();
                            bool b = message.ReadBoolean();
                            for (int i = 0; i < eList.playerList.Count; i++)
                            {
                                Player tempPlayer = (Player)eList.playerList[i];
                                if (name.Equals(tempPlayer.getUsername()))
                                {
                                    if (direction.Equals("left"))
                                        tempPlayer.movingLeft = b;
                                    else if (direction.Equals("right"))
                                        tempPlayer.movingRight = b;
                                    else if (direction.Equals("jump"))
                                        tempPlayer.jumpPressed = b;

                                }
                            }
                        }
                        else
                        {
                            //NetOutgoingMessage outMsg = server.CreateMessage();
                            //outMsg.Write(message);

                            //server.SendToAll(outMsg, NetDeliveryMethod.Unreliable);
                        }
                        break;

                    case NetIncomingMessageType.DebugMessage:
                        Console.WriteLine(message.ReadString());
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        Console.WriteLine(message.SenderConnection.Status);
                        if (message.SenderConnection.Status == NetConnectionStatus.Connected)
                        {
                            Console.WriteLine($"{message.SenderEndPoint.Address} has connected.");
                            NetOutgoingMessage asdf = server.CreateMessage("Connected to server");
                            server.SendMessage(asdf, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                        }
                        if (message.SenderConnection.Status == NetConnectionStatus.Disconnected)
                        {
                            //message.SenderConnection.Peer.Configuration.LocalAddress
                            Console.WriteLine($"{message.SenderEndPoint.Address} has disconnected.");
                        }
                        break;

                    case NetIncomingMessageType.WarningMessage:
                        Console.WriteLine(message.ReadString());
                        break;

                    default:
                        Console.WriteLine($"Unhandled message type: {message.MessageType}");
                        break;
                }
                server.Recycle(message);
            }
        }

    }
}
