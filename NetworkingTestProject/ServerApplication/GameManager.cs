using Lidgren.Network;
using NetworkingTestProjectLibrary.Entities;
using NetworkingTestProjectLibrary.Entities.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication
{
    class GameManager
    {

        private EntityList eList;
        private ServerHandler sHandler;

        public GameManager(EntityList eList, ServerHandler sHandler)
        {
            this.eList = eList;
            this.sHandler = sHandler;
        }

        public void tick()
        {
            eList.tick();

            for(int i = 0; i < eList.playerList.Count; i++)
            {
                Player tempPlayer = (Player)eList.playerList[i];
                NetOutgoingMessage requestUpdate = sHandler.server.CreateMessage();
                requestUpdate.Write("update player position");
                requestUpdate.Write(tempPlayer.name);
                requestUpdate.Write(tempPlayer.x);
                requestUpdate.Write(tempPlayer.y);
                requestUpdate.Write(tempPlayer.vx);
                requestUpdate.Write(tempPlayer.vy);
                requestUpdate.Write(tempPlayer.movingLeft);
                requestUpdate.Write(tempPlayer.movingRight);
                requestUpdate.Write(tempPlayer.jumpPressed);
                sHandler.server.SendToAll(requestUpdate, NetDeliveryMethod.Unreliable);
            }
        }

    }
}
