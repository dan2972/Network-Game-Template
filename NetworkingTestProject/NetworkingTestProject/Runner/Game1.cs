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
using NetworkingTestProjectLibrary.Misc;
using System;
using NetworkingTestProjectLibrary.Entities.Blocks;
using NetworkingTestProjectLibrary;
using System.Threading.Tasks;

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
        private GameMap gMap;
        private Camera camera;

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

            //tweak graphics variables
            graphics.PreferredBackBufferWidth = TRUEGAMEWIDTH;
            graphics.PreferredBackBufferHeight = TRUEGAMEHEIGHT;
            //graphics.IsFullScreen = true;
            graphics.HardwareModeSwitch = false;
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            graphics.ApplyChanges();

            GlobalVariables.GAME_SCALE = (float)TRUEGAMEWIDTH / (float)GlobalVariables.GAME_WIDTH;

            //initialize variables
            gMap = new GameMap(100,100);
            MapGenerator.generateMap(gMap);
            camera = new Camera();
            entityList = new EntityList(gMap);
            clientHandler = new ClientHandler(entityList, gMap, inputManager);
            entityHandler = new EntityHandler(camera, entityList, clientHandler);

            //take input from user to create a username
            System.Console.WriteLine("Enter a name for the player");
            string playerName = System.Console.ReadLine();
            CLIENTUSERNAME = playerName;

            //create and add a new player
            Player player = new Player(100, 100, CLIENTUSERNAME, gMap, entityList);
            entityList.addPlayer(player);
            //initialize input manager for the player
            inputManager = new InputManager(player, camera, entityList, clientHandler);

            requestPlayerCreation();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            TextureLibrary.loadTextures(Content);
        }

        protected override void UnloadContent()
        {
            TextureLibrary.unloadTextures();
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            float gTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsActive)
            {
                inputManager.readInput(gTime);
            }

            clientHandler.update();

            entityHandler.tick(gTime);

            //Console.WriteLine(entityList.projectileList.Count);

            base.Update(gameTime);
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            NetOutgoingMessage disconnectMsg = clientHandler.client.CreateMessage();
            disconnectMsg.Write("disconnected");
            disconnectMsg.Write(CLIENTUSERNAME);
            clientHandler.sendMessage(disconnectMsg, NetDeliveryMethod.ReliableOrdered);
            Task.Delay(1000);
            clientHandler.client.Shutdown("Bye server..");

            base.OnExiting(sender, args);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, camera.getTransform());

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
