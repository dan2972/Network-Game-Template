using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingTestProject.Content
{
    public class TextureLibrary
    {
        public static Texture2D blank_texture { get; set; }

        public static void loadTextures(ContentManager content)
        {
            blank_texture = content.Load<Texture2D>("blank_sprite");
        }

        public static void unloadTextures()
        {
            blank_texture.Dispose();
        }

    }
}
