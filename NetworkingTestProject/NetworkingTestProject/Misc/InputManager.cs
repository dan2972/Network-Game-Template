using Lidgren.Network;
using Microsoft.Xna.Framework.Input;
using NetworkingTestProject.Networking;
using NetworkingTestProjectLibrary.Entities;
using NetworkingTestProjectLibrary.Entities.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingTestProject.Misc
{
    class InputManager
    {

        private KeyboardState keyboardState;

        private Player player;
        private ClientHandler cHandler;

        public InputManager(Player player, ClientHandler cHandler)
        {
            this.player = player;
            this.cHandler = cHandler;
            Console.WriteLine("initialized");
        }

        public void readInput()
        {
            keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.A))
            {
                if (!player.movingLeft)
                {
                    writeMovementMessage("left", true);
                }
            }
            else
            {
                if (player.movingLeft)
                {
                    writeMovementMessage("left", false);
                }
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {
                if (!player.movingRight)
                {
                    writeMovementMessage("right", true);
                }
            }
            else
            {
                if (player.movingRight)
                {
                    writeMovementMessage("right", false);
                }
            }

            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Space))
            {
                if (!player.jumpPressed)
                {
                    writeMovementMessage("jump", true);
                }
            }
            else
            {
                if (player.jumpPressed)
                {
                    writeMovementMessage("jump", false);
                }
            }
            
        }

        private void writeMovementMessage(string movementType, bool b)
        {
            NetOutgoingMessage msg = cHandler.client.CreateMessage();
            msg.Write("update keyboard state player");
            msg.Write(player.name);
            msg.Write(movementType);
            msg.Write(b);
            cHandler.sendMessage(msg, NetDeliveryMethod.Unreliable);
        }

    }
}
