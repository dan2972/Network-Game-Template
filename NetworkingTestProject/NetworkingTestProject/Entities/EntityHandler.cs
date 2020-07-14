using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkingTestProject.Content;
using NetworkingTestProject.Misc;
using NetworkingTestProject.Networking;
using NetworkingTestProjectLibrary.Entities;
using NetworkingTestProjectLibrary.Entities.Characters;
using NetworkingTestProjectLibrary.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingTestProject.Entities
{
    class EntityHandler
    {
        private Camera camera;
        private EntityList eList;
        private ClientHandler cHandler;

        public EntityHandler(Camera camera, EntityList eList, ClientHandler cHandler)
        {
            this.camera = camera;
            this.eList = eList;
            this.cHandler = cHandler;
        }

        public void tick(float gTime)
        {
            //client-side linear interpolation for player position
            for (int i = 0; i < eList.playerList.Count; i++)
            {
                Player tempPlayer = (Player)eList.playerList[i];
                tempPlayer.x = MathHelper.Lerp(tempPlayer.lastState.x, tempPlayer.currentState.x, cHandler.timeSinceLastUpdate * GlobalVariables.SERVER_TICK_RATE);
                tempPlayer.y = MathHelper.Lerp(tempPlayer.lastState.y, tempPlayer.currentState.y, cHandler.timeSinceLastUpdate * GlobalVariables.SERVER_TICK_RATE);

                if (tempPlayer.getUsername().Equals(Game1.CLIENTUSERNAME))
                {
                    camera.follow(tempPlayer.x, tempPlayer.y, tempPlayer.sizeX, tempPlayer.sizeY, gTime);
                }
            }

            for (int i = 0; i < eList.projectileList.Count; i++)
            {
                GameObject obj = (GameObject)eList.projectileList[i];
                obj.tickAsClient(gTime);
            }

            cHandler.timeSinceLastUpdate += gTime;
        }

        public void render(SpriteBatch sb)
        {
            eList.render(sb, camera.getX(), camera.getY());
        }

    }
}
