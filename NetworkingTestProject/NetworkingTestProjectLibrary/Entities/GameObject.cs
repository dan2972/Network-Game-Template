using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingTestProjectLibrary.Entities
{
    public abstract class GameObject
    {
        public EntityState lastState { get; set; }
        public EntityState currentState { get; set; }

        public int uniqueID { get; set; }

        public float x { get; set; }
        public float y { get; set; }
        public float vx { get; set; }
        public float vy { get; set; }
        public float sizeX { get; set; }
        public float sizeY { get; set; }
        public float mass { get; set; }
        public float rotation { get; set; }

        public GameObject(float x, float y)
        {
            this.x = x;
            this.y = y;
            vx = 0;
            vy = 0;

            lastState = new EntityState();
            currentState = new EntityState(x, y);
        }

        public abstract void tick();

        //this method will only be used if the server will not send positional data to the client, and the client
        //will have to "tick" it instead.
        public abstract void tickAsClient(float gTime);

        public abstract void render(SpriteBatch sb);

        public abstract ObjectType.ID getType();

    }
}
