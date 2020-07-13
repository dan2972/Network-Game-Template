using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Lidgren.Network;
using NetworkingTestProject.Networking;
using NetworkingTestProject.Content;
using NetworkingTestProjectLibrary.Entities;
using NetworkingTestProject.Entities;
using NetworkingTestProject.Misc;
using NetworkingTestProjectLibrary.Entities.Characters;
using System;

namespace NetworkingTestProject
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static int TRUEGAMEWIDTH = 1280;//GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        public static int TRUEGAMEHEIGHT = 720;//GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        public static string CLIENTUSERNAME;

        private EntityList entityList;
        private EntityHandler entityHandler;

        private ClientHandler clientHandler;
        private InputManager inputManager;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = TRUEGAMEWIDTH;
            graphics.PreferredBackBufferHeight = TRUEGAMEHEIGHT;
            //graphics.IsFullScreen = true;
            graphics.HardwareModeSwitch = false;
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            graphics.ApplyChanges();

            //initialize variables
            entityList = new EntityList();
            clientHandler = new ClientHandler(entityList, inputManager);
            entityHandler = new EntityHandler(entityList, clientHandler);

            //take input from user to create a username
            System.Console.WriteLine("Enter a name for the player");
            string playerName = System.Console.ReadLine();
            CLIENTUSERNAME = playerName;

            //create and add a new player
            Player player = new Player(100, 100, CLIENTUSERNAME, entityList);
            entityList.addPlayer(player);
            //initialize input manager for the player
            inputManager = new InputManager(player, clientHandler);

            requestPlayerCreation();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            TextureManager.loadTextures(Content);
        }

        protected override void UnloadContent()
        {
            TextureManager.unloadTextures();
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                NetOutgoingMessage disconnectMsg = clientHandler.client.CreateMessage();
                disconnectMsg.Write("disconnected");
                disconnectMsg.Write(CLIENTUSERNAME);
                clientHandler.sendMessage(disconnectMsg, NetDeliveryMethod.ReliableOrdered);
                clientHandler.client.Shutdown("Bye server..");
                Exit();
            }
            float gTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            inputManager.readInput();

            clientHandler.update();

            entityHandler.tick(gTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);

            entityHandler.render(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void requestPlayerCreation()
        {
            NetOutgoingMessage playerCreationMsg = clientHandler.client.CreateMessage();
            playerCreationMsg.Write("create player");
            playerCreationMsg.Write(CLIENTUSERNAME);
            playerCreationMsg.Write(100f);
            playerCreationMsg.Write(100f);
            clientHandler.sendMessage(playerCreationMsg, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
