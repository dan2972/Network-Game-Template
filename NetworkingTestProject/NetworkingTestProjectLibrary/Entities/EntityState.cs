using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingTestProjectLibrary.Entities
{
    public class EntityState
    {

        public float x { get; set; }
        public float y { get; set; }
        public float vx { get; set; }
        public float vy { get; set; }

        public EntityState(float x, float y)
        {
            this.x = x;
            this.y = y;
            vx = 0;
            vy = 0;
        }

        public EntityState(float x, float y, float vx, float vy)
        {
            this.x = x;
            this.y = y;
            this.vx = vx;
            this.vy = vy;
        }

        public EntityState()
        {
            x = 0;
            y = 0;
            vx = 0;
            vy = 0;
        }

        public void update(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public void update(float x, float y, float vx, float vy)
        {
            this.x = x;
            this.y = y;
            this.vx = vx;
            this.vy = vy;
        }


    }
}
