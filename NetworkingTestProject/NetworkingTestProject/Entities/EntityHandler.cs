using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkingTestProject.Content;
using NetworkingTestProject.Networking;
using NetworkingTestProjectLibrary.Entities;
using NetworkingTestProjectLibrary.Entities.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingTestProject.Entities
{
    class EntityHandler
    {
        private EntityList eList;
        private ClientHandler cHandler;

        public EntityHandler(EntityList eList, ClientHandler cHandler)
        {
            this.eList = eList;
            this.cHandler = cHandler;
        }

        public void tick(float gTime)
        {
            //client-side "fake" linear interpolation
            for (int i = 0; i < eList.playerList.Count; i++)
            {
                Player tempPlayer = (Player)eList.playerList[i];
                /*
                tempPlayer.x += tempPlayer.vx * (20f * gTime);
                tempPlayer.y += tempPlayer.vy * (20f * gTime);*/
                tempPlayer.x = MathHelper.Lerp(tempPlayer.lastPlayerState.x, tempPlayer.currentPlayerState.x, cHandler.timeSinceLastUpdate * 60);
                tempPlayer.y = MathHelper.Lerp(tempPlayer.lastPlayerState.y, tempPlayer.currentPlayerState.y, cHandler.timeSinceLastUpdate * 60);
            }
            
            cHandler.timeSinceLastUpdate += gTime;
        }

        public void render(SpriteBatch sb)
        {
            for (int i = 0; i < eList.playerList.Count; i++)
            {
                Player tempPlayer = (Player)eList.playerList[i];

                int x = (int)tempPlayer.x;
                int y = (int)tempPlayer.y;
                int sizeY = (int)tempPlayer.sizeY;
                int sizeX = (int)tempPlayer.sizeX;

                sb.Draw(TextureManager.blank_texture, destinationRectangle: new Rectangle(x, y, sizeX, sizeY), color: Color.White);
            }
        }

    }
}
