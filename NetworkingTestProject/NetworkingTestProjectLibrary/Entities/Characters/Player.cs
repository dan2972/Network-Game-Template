using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkingTestProject.Content;
using NetworkingTestProjectLibrary.Calculations;
using NetworkingTestProjectLibrary.Entities.Blocks;
using NetworkingTestProjectLibrary.Entities.Projectiles.Commander;
using NetworkingTestProjectLibrary.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingTestProjectLibrary.Entities.Characters
{
    public class Player : GameObject
    {
        public bool movingRight { get; set; }
        public bool movingLeft { get; set; }
        public bool jumpPressed { get; set; }
        public bool primaryPressed { get; set; }
        public bool secondaryPressed { get; set; }
        public bool utilityPressed { get; set; }
        public bool ultimatePressed { get; set; }
        public bool inAir = true;

        public string username { get; set; }

        private EntityList eList;
        private GameMap gMap;

        private float maxMovementSpeed = 5;
        private float movementAccel = 0.8f;
        private float gravity = 0.33f;
        private float jumpPower = 7f;
        private float friction = 0.21f;

        private float primaryCooldownTime = 20;
        private float primaryCooldownTimer = 0;
        private float secondaryCooldownTime = 60;
        private float secondaryCooldownTimer = 0;
        private float utilityCooldownTime = 20;
        private float utilityCooldownTimer = 0;
        private float ultimateCooldownTime = 20;
        private float ultimateCooldownTimer = 0;

        private int onObjectCount = 0;

        public Player(float x, float y, string username, GameMap gMap, EntityList eList) : base(x, y)
        {
            this.x = x;
            this.y = y;
            this.gMap = gMap;
            this.eList = eList;

            this.username = username;

            sizeX = 36;
            sizeY = 78;

            vx = 0;
            vy = 0;

            mass = 60f;

            lastState = new EntityState();
            currentState = new EntityState(x, y);
        }

        public override void tick()
        {
            primaryCooldownTimer--;
            primaryCooldownTimer = primaryCooldownTimer < 0 ? 0 : primaryCooldownTimer;

            x += vx;
            y += vy;

            handleMovement();
            handleCollision();
        }

        private void handleMovement()
        {
            if (movingLeft)
            {
                if(vx > -maxMovementSpeed)
                    vx -= movementAccel;
            }
            else if (movingRight)
            {
                if(vx < maxMovementSpeed)
                    vx += movementAccel;
            }
            else
            {
                if (vx > 0.26)
                    vx -= friction;
                else if (vx < -0.26)
                    vx += friction;
                else
                    vx = 0;
            }


            if (inAir)
                vy += gravity;
            if (vy >= 27)
                vy = 27;

            if (!inAir && jumpPressed)
            {
                vy -= jumpPower;
                inAir = true;
            }

        }

        private void handleCollision()
        {
            onObjectCount = 0;
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
                        if (CollisionDetector.intersects(x, y + sizeY + (vy), sizeX, 2, obj.x, obj.y, obj.sizeX, obj.sizeY))
                            onObjectCount++;
                        if (CollisionDetector.intersects(x + (vx), y + (vy), sizeX, sizeY, obj.x, obj.y, obj.sizeX, obj.sizeY))
                        {

                            if (CollisionDetector.intersects(x, y + sizeY + (vy) + 1, sizeX, 1, obj.x, obj.y, obj.sizeX, obj.sizeY) && inAir && vy > 0)
                            {
                                inAir = false;
                                vy = 0;
                                y = obj.y - sizeY;
                            }
                            else if (CollisionDetector.intersects(x, y + (vy) - 1, sizeX, 1, obj.x, obj.y, obj.sizeX, obj.sizeY) && inAir && vy < 0)
                            {
                                vy = 0;
                                y = obj.y + obj.sizeY;
                            }
                            else if (!CollisionDetector.intersects(x, y + sizeY + 1, sizeX, 1, obj.x, obj.y, obj.sizeX, obj.sizeY) && !CollisionDetector.intersects(x, y - 1, sizeX, 1, obj.x, obj.y, obj.sizeX, obj.sizeY))
                            {
                                if (vx > 0)
                                {
                                    x = obj.x - sizeX;
                                    vx = 0;
                                }
                                else if (vx < 0)
                                {
                                    x = obj.x + obj.sizeX;
                                    vx = 0;
                                }
                            }
                        }
                    }
                }
            }

            if (onObjectCount == 0)
                inAir = true;

            if (x + (vx) < 0)
            {
                vx = 0;
                x = 0;
            }
            if (x + (vx) > GlobalVariables.MAP_SIZE_X * GlobalVariables.BLOCK_SIZE)
            {
                vx = 0;
                x = GlobalVariables.MAP_SIZE_X * GlobalVariables.BLOCK_SIZE;
            }
        }

        public override void render(SpriteBatch sb)
        {
            int scaledX = (int)(x * GlobalVariables.GAME_SCALE);
            int scaledY = (int)(y * GlobalVariables.GAME_SCALE);
            int scaledSizeX = (int)(sizeX * GlobalVariables.GAME_SCALE);
            int scaledSizeY = (int)(sizeY * GlobalVariables.GAME_SCALE);

            sb.Draw(TextureLibrary.blank_texture, destinationRectangle: new Rectangle(scaledX, scaledY,
                scaledSizeX, scaledSizeY), color: Color.White);
        }

        public void usePrimary(int mouseX, int mouseY, NetServer server)
        {
            if (primaryCooldownTimer == 0)
            {
                primaryCooldownTimer = primaryCooldownTime;
                float bulletX = x + sizeX / 2;
                float bulletY = y + sizeY / 2;
                CommanderBullet bullet = new CommanderBullet(eList.getUsableID(ObjectType.ID.Projectile), bulletX, bulletY, mouseX, mouseY, this, gMap, eList, server);
                eList.addObject(bullet, ObjectType.ID.Projectile);
                NetOutgoingMessage outMsg = server.CreateMessage();
                outMsg.Write("create projectile");
                outMsg.Write("primary");
                outMsg.Write(bullet.uniqueID);
                outMsg.Write(bullet.x);
                outMsg.Write(bullet.y);
                outMsg.Write(bullet.vx);
                outMsg.Write(bullet.vy);
                server.SendToAll(outMsg, NetDeliveryMethod.Unreliable);
            }
        }

        public override ObjectType.ID getType()
        {
            return ObjectType.ID.Player;
        }

        public string getUsername()
        {
            return username;
        }

        public override void tickAsClient(float gTime)
        {
            throw new NotImplementedException();
        }

    }
}
