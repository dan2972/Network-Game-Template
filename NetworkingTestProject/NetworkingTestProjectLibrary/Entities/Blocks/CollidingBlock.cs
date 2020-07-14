using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkingTestProject.Content;
using NetworkingTestProjectLibrary.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingTestProjectLibrary.Entities.Blocks
{
    public class CollidingBlock : GameObject
    {

        private bool highlighted = false;
        public CollidingBlock(float x, float y) : base(x, y)
        {
            this.x = x;
            this.y = y;

            sizeX = GlobalVariables.BLOCK_SIZE;
            sizeY = GlobalVariables.BLOCK_SIZE;
        }
        public override void tick()
        {

        }

        public override void tickAsClient(float gTime)
        {
            throw new NotImplementedException();
        }
        public override void render(SpriteBatch sb)
        {
            int scaledX = (int)(x * GlobalVariables.GAME_SCALE);
            int scaledY = (int)(y * GlobalVariables.GAME_SCALE);
            int scaledSizeX = (int)(sizeX * GlobalVariables.GAME_SCALE);
            int scaledSizeY = (int)(sizeY * GlobalVariables.GAME_SCALE);

            if (!highlighted)
            {
                sb.Draw(TextureLibrary.blank_texture, destinationRectangle: new Rectangle(scaledX, scaledY,
                    scaledSizeX, scaledSizeY), color: Color.White);
            }
            else
            {
                sb.Draw(TextureLibrary.blank_texture, destinationRectangle: new Rectangle(scaledX, scaledY,
                    scaledSizeX, scaledSizeY), color: Color.Green);
            }
            highlighted = false;
        }

        public override ObjectType.ID getType()
        {
            return ObjectType.ID.Block;
        }

        public void highlight()
        {
            highlighted = true;
        }
    }
}
