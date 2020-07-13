using NetworkingTestProjectLibrary.Entities.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingTestProjectLibrary.Entities
{
    public class EntityList
    {

        public ArrayList playerList { get; set; }
        public ArrayList particleList { get; set; }
        public ArrayList projectileList { get; set; }

        public EntityList()
        {
            playerList = new ArrayList();
            particleList = new ArrayList();
            projectileList = new ArrayList();
        }

        public void tick()
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                GameObject obj = (GameObject)playerList[i];
                obj.tick();
            }
            for (int i = 0; i < particleList.Count; i++)
            {
                GameObject obj = (GameObject)particleList[i];
                obj.tick();
            }
            for (int i = 0; i < projectileList.Count; i++)
            {
                GameObject obj = (GameObject)projectileList[i];
                obj.tick();
            }
        }

        public void addPlayer(Player p)
        {
            playerList.Add(p);
        }

        public void addObject(GameObject obj, ObjectType.ID id)
        {
            switch (id)
            {
                case ObjectType.ID.Particle:
                    particleList.Add(obj);
                    break;
                case ObjectType.ID.Projectile:
                    projectileList.Add(obj);
                    break;
                default:
                    particleList.Add(obj);
                    break;
            }
        }

        public void removePlayer(Player p)
        {
            playerList.Remove(p);
        }

        public void removeObject(GameObject obj, ObjectType.ID id)
        {
            switch (id)
            {
                case ObjectType.ID.Particle:
                    particleList.Remove(obj);
                    break;
                case ObjectType.ID.Projectile:
                    projectileList.Remove(obj);
                    break;
                default:
                    particleList.Remove(obj);
                    break;
            }
        }

    }
}
