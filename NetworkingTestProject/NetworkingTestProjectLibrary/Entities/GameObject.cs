using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingTestProjectLibrary.Entities
{
    public abstract class GameObject
    {
        public float x { get; set; }
        public float y { get; set; }
        public float vx { get; set; }
        public float vy { get; set; }
        public float sizeX { get; set; }
        public float sizeY { get; set; }
        public float mass { get; set; }

        public GameObject(float x, float y)
        {
            this.x = x;
            this.y = y;
            vx = 0;
            vy = 0;
        }

        public abstract void tick();

        public abstract ObjectType.ID getType();

    }
}
