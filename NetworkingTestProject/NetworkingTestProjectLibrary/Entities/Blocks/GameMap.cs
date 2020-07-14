using Microsoft.Xna.Framework.Graphics;
using NetworkingTestProjectLibrary.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingTestProjectLibrary.Entities.Blocks
{
    public class GameMap
    {

        public CollidingBlock[,] gMap { get; set; }

        public int sizeX { get; set; }
        public int sizeY { get; set; }

        public GameMap(int sizeX, int sizeY)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            gMap = new CollidingBlock[sizeY, sizeX];
        }

        public void render(SpriteBatch sb, float camX, float camY)
        {
            int xMap = (int)-(camX / GlobalVariables.GAME_SCALE) / (int)GlobalVariables.BLOCK_SIZE;
            int yMap = (int)-(camY / GlobalVariables.GAME_SCALE) / (int)GlobalVariables.BLOCK_SIZE;
            int renderDistX = GlobalVariables.GAME_WIDTH / (int)GlobalVariables.BLOCK_SIZE / 2 + 2;
            int renderDistY = GlobalVariables.GAME_HEIGHT / (int)GlobalVariables.BLOCK_SIZE / 2 + 3;
            int minCheckX = xMap - renderDistX < 0 ? 0 : xMap - renderDistX;
            int maxCheckX = xMap + renderDistX > sizeX ? sizeX : xMap + renderDistX;
            int minCheckY = yMap - renderDistY < 0 ? 0 : yMap - renderDistY;
            int maxCheckY = yMap + renderDistY > sizeY ? sizeY : yMap + renderDistY;
            for (int i = minCheckY; i < maxCheckY; i++)
            {
                for (int j = minCheckX; j < maxCheckX; j++)
                {
                    CollidingBlock block = gMap[i, j];
                    if (block != null)
                    {
                        block.render(sb);
                    }
                }
            }
        }

    }
}
