using Microsoft.Xna.Framework;
using NetworkingTestProjectLibrary.Entities.Blocks;
using NetworkingTestProjectLibrary.Misc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingTestProjectLibrary
{
    public class MapGenerator
    {
        public static void generateMap(GameMap gMap)
        {
            string path = Path.Combine(Environment.CurrentDirectory, @"MapFiles\", "gameMap.png");
            
            Bitmap img = new Bitmap(path);

            GlobalVariables.MAP_SIZE_X = img.Width;
            GlobalVariables.MAP_SIZE_X = img.Height;

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    System.Drawing.Color pixel = img.GetPixel(j, i);

                    if(pixel.R == 255 && pixel.G == 255 && pixel.B == 255)
                    {
                        gMap.gMap[i, j] = new CollidingBlock(j * GlobalVariables.BLOCK_SIZE, i * GlobalVariables.BLOCK_SIZE);
                    }
                }
            }
        }

    }
}
