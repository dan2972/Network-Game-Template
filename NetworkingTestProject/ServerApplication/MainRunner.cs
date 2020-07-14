using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkingTestProjectLibrary;
using NetworkingTestProjectLibrary.Entities;
using NetworkingTestProjectLibrary.Entities.Blocks;
using NetworkingTestProjectLibrary.Misc;

namespace ServerApplication
{
    class MainRunner
    {
        public static EntityList eList;
        public static ServerHandler serverHandler;
        public static GameManager gm;
        public static GameMap gMap;

        static void Main(string[] args)
        {
            //initialize variables
            gMap = new GameMap(100, 100);
            MapGenerator.generateMap(gMap);
            eList = new EntityList(gMap);
            serverHandler = new ServerHandler(eList, gMap);
            gm = new GameManager(eList, serverHandler);

            //main game loop (server)
            long lastTime = nanoTime();
            double amountOfTicks = GlobalVariables.SERVER_TICK_RATE;
            double ns = 1000000000 / amountOfTicks;
            double delta = 0;
            long timer = currentTimeMillis();
            int updates = 0;
            while (true)
            {
                long now = nanoTime();
                delta += (now - lastTime) / ns;
                lastTime = now;
                while (delta >= 1)
                {
                    serverHandler.update();
                    gm.tick();
                    updates++;
                    delta--;
                }
                if (currentTimeMillis() - timer > 1000)
                {
                    timer += 1000;
                    Console.WriteLine(updates);
                    updates = 0;
                }
            }

        }

        private static long nanoTime()
        {
            double timestamp = Stopwatch.GetTimestamp();
            double nanoseconds = 1000000000.0 * timestamp / Stopwatch.Frequency;

            return (long)nanoseconds;
        }
        private static long currentTimeMillis()
        {
            double timestamp = Stopwatch.GetTimestamp();
            double milliseconds = 1000 * timestamp / Stopwatch.Frequency;

            return (long)milliseconds;
        }

    }
}
