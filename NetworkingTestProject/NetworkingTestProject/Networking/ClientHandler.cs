using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using Microsoft.Xna.Framework.Content;
using NetworkingTestProject.Misc;
using NetworkingTestProjectLibrary.Entities;
using NetworkingTestProjectLibrary.Entities.Projectiles.Commander;
using NetworkingTestProjectLibrary.Entities.Characters;
using Microsoft.Xna.Framework;
using NetworkingTestProjectLibrary.Entities.Blocks;

namespace NetworkingTestProject.Networking
{
    class ClientHandler
    {

        public NetClient client { get; }
        public float timeSinceLastUpdate { get; set; }

        private static NetPeerConfiguration config;

        private EntityList eList;
        private GameMap gMap;
        private InputManager inputManager;

        public ClientHandler(EntityList eList, GameMap gMap, InputManager inputManager)
        {
            this.eList = eList;
            this.gMap = gMap;
            this.inputManager = inputManager;

            config = new NetPeerConfiguration("NetworkTestGame");
            client = new NetClient(config);

            client.Start();

            NetOutgoingMessage approval = client.CreateMessage();
            approval.Write("approval message");

            //127.0.0.1    local
            //172.30.1.32  dan's laptop ip
            //25.64.92.236 hamachi ip
            client.Connect("211.199.156.182", 14242, approval);

        }

        public void update()
        {
            NetIncomingMessage message;

            while ((message = client.ReadMessage()) != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        string headerMsg = message.ReadString();

                        if (headerMsg.Equals("create player"))
                        {
                            string name = message.ReadString();
                            float x = message.ReadFloat();
                            float y = message.ReadFloat();
                            Player p = new Player(x, y, name, gMap, eList);
                            bool samePlayerNameFound = false;
                            for (int i = 0; i < eList.playerList.Count; i++)
                            {
                                Player tempPlayer = (Player)eList.playerList[i];
                                if (tempPlayer.getUsername().Equals(name))
                                    samePlayerNameFound = true;
                            }
                            if (!samePlayerNameFound)
                            {
                                if (!name.Equals(Game1.CLIENTUSERNAME))
                                {
                                    eList.addPlayer(p);
                                }
                                Console.WriteLine(name + " has entered the server");
                            }
                        }
                        if (headerMsg.Equals("create projectile"))
                        {
                            string type = message.ReadString();
                            int id = message.ReadInt32();
                            float x = message.ReadFloat();
                            float y = message.ReadFloat();
                            float vx = message.ReadFloat();
                            float vy = message.ReadFloat();
                            if (type.Equals("primary"))
                            {
                                CommanderBullet bullet = new CommanderBullet(id, x, y, eList);
                                bullet.vx = vx;
                                bullet.vy = vy;
                                eList.addObject(bullet, ObjectType.ID.Projectile);
                            }
                        }
                        else if (headerMsg.Equals("update player position"))
                        {
                            string name = message.ReadString();
                            float x = message.ReadFloat();
                            float y = message.ReadFloat();
                            bool movingLeft = message.ReadBoolean();
                            bool movingRight = message.ReadBoolean();
                            bool jumpPressed = message.ReadBoolean();
                            for (int i = 0; i < eList.playerList.Count; i++)
                            {
                                Player tempPlayer = (Player)eList.playerList[i];
                                if (name.Equals(tempPlayer.getUsername()))
                                {
                                    tempPlayer.lastState.update(tempPlayer.x, tempPlayer.y);
                                    tempPlayer.x = x;
                                    tempPlayer.y = y;
                                    tempPlayer.movingLeft = movingLeft;
                                    tempPlayer.movingRight = movingRight;
                                    tempPlayer.jumpPressed = jumpPressed;
                                    tempPlayer.currentState.update(x, y);
                                    timeSinceLastUpdate = 0;
                                }
                            }
                        }
                        else if (headerMsg.Equals("destroy projectile"))
                        {
                            int id = message.ReadInt32();
                            for (int i = 0; i < eList.projectileList.Count; i++)
                            {
                                GameObject obj = (GameObject)eList.projectileList[i];
                                if(obj.uniqueID == id)
                                {
                                    eList.removeObject(obj, ObjectType.ID.Projectile);
                                }
                            }
                        }/*
                        else if (headerMsg.Equals("update projectile position"))
                        {
                            int ID = message.ReadInt32();
                            float x = message.ReadFloat();
                            float y = message.ReadFloat();
                            for (int i = 0; i < eList.projectileList.Count; i++)
                            {
                                GameObject obj = (GameObject)eList.projectileList[i];
                                if (ID == obj.uniqueID)
                                {
                                    obj.lastState.update(obj.x, obj.y);
                                    obj.x = x;
                                    obj.y = y;
                                    obj.currentState.update(x, y);
                                    timeSinceLastUpdate = 0;
                                }
                            }
                        }*/
                        else if (headerMsg.Equals("disconnected"))
                        {
                            string name = message.ReadString();
                            for (int i = 0; i < eList.playerList.Count; i++)
                            {
                                Player tempPlayer = (Player)eList.playerList[i];
                                if (name.Equals(tempPlayer.getUsername()))
                                {
                                    eList.removePlayer(tempPlayer);
                                    Console.WriteLine(name + " has disconnected from the server");
                                }
                            }
                        }
                        break;

                    case NetIncomingMessageType.DebugMessage:
                        Console.WriteLine(message.ReadString());
                        break;

                    case NetIncomingMessageType.WarningMessage:
                        Console.WriteLine(message.ReadString());
                        break;

                    default:
                        Console.WriteLine($"Unhandled message type: {message.MessageType}");
                        break;
                }
                client.Recycle(message);
            }
        }

        public void sendMessage(NetOutgoingMessage msg, NetDeliveryMethod method)
        {
            client.SendMessage(msg, method);
        }

    }
}
