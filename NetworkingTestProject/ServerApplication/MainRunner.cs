using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkingTestProjectLibrary.Entities;

namespace ServerApplication
{
    class MainRunner
    {
        public static EntityList eList;
        public static ServerHandler serverHandler;
        public static GameManager gm;

        static void Main(string[] args)
        {
            //initialize variables
            eList = new EntityList();
            serverHandler = new ServerHandler(eList);
            gm = new GameManager(eList, serverHandler);

            //main game loop (server)
            long lastTime = nanoTime();
            double amountOfTicks = 60.0;
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
