using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeBlaster
{
    static class Art
    {
        public static Texture2D Player { get; private set; }
        public static Texture2D Seeker { get; private set; }
        public static Texture2D Wanderer { get; private set; }

        public static Texture2D BlackHole { get; private set; }
        public static Texture2D Bullet { get; private set; }
        public static Texture2D Pointer { get; private set; }

        public static Texture2D Pixel { get; private set; }

        public static SpriteFont Font { get; private set; }

        public static void Load(ContentManager content)
        {
            Player = content.Load<Texture2D>("Art/Player");
            Seeker = content.Load<Texture2D>("Art/Seeker");
            Wanderer = content.Load<Texture2D>("Art/Wanderer");
            BlackHole = content.Load<Texture2D>("Art/Black Hole");
            Bullet = content.Load<Texture2D>("Art/Bullet");
            Pointer = content.Load<Texture2D>("Art/Pointer");
            Pixel = new Texture2D(Player.GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });

            Font = content.Load<SpriteFont>("Font");
        }
    }
}
