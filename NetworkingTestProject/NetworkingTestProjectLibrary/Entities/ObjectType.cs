using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingTestProjectLibrary.Entities
{
    public class ObjectType
    {

        public enum ID:int
        {
            Player = 0,
            Mob = 1,
            Item = 2,
            Projectile = 3,
            Particle = 4
        }

    }
}
