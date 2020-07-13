using Lidgren.Network;
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
        public bool inAir = true;

        public EntityState lastPlayerState { get; set; }
        public EntityState currentPlayerState { get; set; }

        public string name { get; set; }

        private EntityList eList;


        private float maxMovementSpeed = 5;
        private float movementAccel = 0.8f;
        private float gravity = 0.33f;
        private float jumpPower = 6.7f;
        private float friction = 0.21f;

        public Player(float x, float y, string name, EntityList eList) : base(x, y)
        {
            this.x = x;
            this.y = y;
            this.eList = eList;

            this.name = name;

            sizeX = 24;
            sizeY = 52;

            vx = 0;
            vy = 0;

            mass = 60f;

            lastPlayerState = new EntityState();
            currentPlayerState = new EntityState(x, y);
        }

        public override void tick()
        {
            handlePhysics();

            x += vx;
            y += vy;
        }

        private void handlePhysics()
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

            if (x + (vx) < 0 || x + (vx) + sizeX > 1280)
            {
                vx *= -1;
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

            if (y + (vy) + sizeY > 720)
            {
                y = 720 - sizeY;
                vy = 0;
                inAir = false;
            }
        }

        public override ObjectType.ID getType()
        {
            return ObjectType.ID.Player;
        }

        public string getName()
        {
            return name;
        }

    }
}
