using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkingTestProject.Content;
using NetworkingTestProjectLibrary.Calculations;
using NetworkingTestProjectLibrary.Entities.Blocks;
using NetworkingTestProjectLibrary.Entities.Characters;
using NetworkingTestProjectLibrary.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingTestProjectLibrary.Entities.Projectiles.Commander
{
    public class CommanderBullet : GameObject
    {
        public int mouseX { get; set; }
        public int mouseY { get; set; }

        private NetServer server;

        private Player user;
        private GameMap gMap;
        private EntityList eList;

        private float speed = 25f;
        private float gravity = 0.33f;

        private int lifetime = 120;

        //server-side initialization
        public CommanderBullet(int id, float x, float y, int mouseX, int mouseY, Player user, GameMap gMap, EntityList eList, NetServer server) : base(x, y)
        {
            this.uniqueID = id;
            this.x = x;
            this.y = y;
            this.mouseX = mouseX;
            this.mouseY = mouseY;
            this.user = user;
            this.gMap = gMap;
            this.eList = eList;
            this.server = server;

            sizeX = 12;
            sizeY = 12;

            float distanceToMouse = (float)Math.Sqrt(Math.Pow((mouseX - x), 2) + Math.Pow((mouseY - y), 2));

            vx = (mouseX - x) / distanceToMouse * speed;
            vy = (mouseY - y) / distanceToMouse * speed;

            rotation = (float)Math.Atan2((mouseX - y), (mouseY - x));

            lastState = new EntityState();
            currentState = new EntityState(x, y);
        }

        //client-side initialization
        public CommanderBullet(int id, float x, float y, EntityList eList) : base(x, y)
        {
            this.uniqueID = id;
            this.x = x;
            this.y = y;
            this.eList = eList;

            sizeX = 12;
            sizeY = 12;

            lastState = new EntityState();
            currentState = new EntityState(x, y);
        }

        public override void tick()
        {
            //vy += gravity;
            if (vy >= 27)
                vy = 27;

            x += vx;
            y += vy;

            checkCollision();

            lifetime--;
            if (lifetime == 0)
                destroy();
        }
        public override void tickAsClient(float gTime)
        {
            //vy += gravity * GlobalVariables.SERVER_TICK_RATE * gTime;
            if (vy >= 27 * GlobalVariables.SERVER_TICK_RATE)
                vy = 27 * GlobalVariables.SERVER_TICK_RATE;

            x += vx * GlobalVariables.SERVER_TICK_RATE * gTime;
            y += vy * GlobalVariables.SERVER_TICK_RATE * gTime;
        }

        private void checkCollision()
        {
            for(int i = 0; i < eList.playerList.Count; i++)
            {
                Player tempPlayer = (Player)eList.playerList[i];
                if (!tempPlayer.getUsername().Equals(user.getUsername()))
                {
                    if (CollisionDetector.intersects(x, y, sizeX, sizeY, tempPlayer.x, tempPlayer.y, tempPlayer.sizeX, tempPlayer.sizeY))
                    {
                        tempPlayer.vx += vx / 5;
                        destroy();
                    }
                }
            }
            int xMap = (int)((x + sizeX / 2) / GlobalVariables.BLOCK_SIZE);
            int yMap = (int)((y + sizeY / 2) / GlobalVariables.BLOCK_SIZE);
            int minCheckX = xMap - 5 < 0 ? 0 : xMap - 5;
            int maxCheckX = xMap + 5 > GlobalVariables.MAP_SIZE_X ? GlobalVariables.MAP_SIZE_X : xMap + 5;
            int minCheckY = yMap - 5 < 0 ? 0 : yMap - 5;
            int maxCheckY = yMap + 5 > GlobalVariables.MAP_SIZE_Y ? GlobalVariables.MAP_SIZE_Y : yMap + 5;
            for (int i = minCheckY; i < maxCheckY; i++)
            {
                for (int j = minCheckX; j < maxCheckX; j++)
                {
                    CollidingBlock obj = gMap.gMap[i, j];
                    if (obj != null)
                    {
                        if (CollisionDetector.intersects(x, y, sizeX, sizeY, obj.x, obj.y, obj.sizeX, obj.sizeY))
                        {
                            destroy();
                        }
                    }
                }
            }
        }

        private void destroy()
        {
            NetOutgoingMessage outMsg = server.CreateMessage();
            outMsg.Write("destroy projectile");
            outMsg.Write(this.uniqueID);
            server.SendToAll(outMsg, NetDeliveryMethod.Unreliable);
            eList.removeObject(this, ObjectType.ID.Projectile);
        }

        public override void render(SpriteBatch sb)
        {
            int scaledX = (int)(x * GlobalVariables.GAME_SCALE);
            int scaledY = (int)(y * GlobalVariables.GAME_SCALE);
            int scaledSizeX = (int)(sizeX * GlobalVariables.GAME_SCALE);
            int scaledSizeY = (int)(sizeY * GlobalVariables.GAME_SCALE);

            sb.Draw(TextureLibrary.blank_texture, destinationRectangle: new Rectangle(scaledX, scaledY,
                scaledSizeX, scaledSizeY), color: Color.Gray);
        }

        public override ObjectType.ID getType()
        {
            return ObjectType.ID.Projectile;
        }

    }
}
