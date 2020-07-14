using Lidgren.Network;
using Microsoft.Xna.Framework.Input;
using NetworkingTestProject.Networking;
using NetworkingTestProjectLibrary.Entities;
using NetworkingTestProjectLibrary.Entities.Characters;
using NetworkingTestProjectLibrary.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingTestProject.Misc
{
    class InputManager
    {

        private KeyboardState keyboardState;
        private MouseState mouseState;

        private Player player;
        private Camera camera;
        private ClientHandler cHandler;
        private EntityList eList;

        private float serverWaitTime = 0;

        public InputManager(Player player, Camera camera, EntityList eList, ClientHandler cHandler)
        {
            this.player = player;
            this.camera = camera;
            this.eList = eList;
            this.cHandler = cHandler;
            Console.WriteLine("initialized");
        }

        public void readInput(float gTime)
        {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            serverWaitTime += gTime;

            basicPlayerMovement();

            if (serverWaitTime >= 1f / GlobalVariables.SERVER_TICK_RATE)
            {
                serverWaitTime = 0;
                float offsetX, offsetY;
                offsetX = Game1.TRUEGAMEWIDTH / 2 - (-camera.getX());
                offsetY = Game1.TRUEGAMEHEIGHT / 2 - (-camera.getY());
                int scaledMouseX = (int)((mouseState.X - offsetX) / GlobalVariables.GAME_SCALE);
                int scaledMouseY = (int)((mouseState.Y - offsetY) / GlobalVariables.GAME_SCALE);
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    writeAbilityUseMessage("primary", scaledMouseX, scaledMouseY);
                }
            }
            
        }

        private void basicPlayerMovement()
        {
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
            msg.Write(player.username);
            msg.Write(movementType);
            msg.Write(b);
            cHandler.sendMessage(msg, NetDeliveryMethod.Unreliable);
        }

        private void writeAbilityUseMessage(string abilityType, int mx, int my)
        {
            NetOutgoingMessage msg = cHandler.client.CreateMessage();
            msg.Write("player ability use");
            msg.Write(player.getUsername());
            msg.Write(abilityType);
            msg.Write(mx);
            msg.Write(my);
            cHandler.sendMessage(msg, NetDeliveryMethod.Unreliable);
        }

    }
}
