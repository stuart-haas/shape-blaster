using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShapeBlaster.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeBlaster.entity
{
    class BlackHole: Entity
    {
        private static Random rand = new Random();

        private int hitPoints = 10;

        public BlackHole(Vector2 position)
        {
            image = Art.BlackHole;
            Position = position;
            Radius = image.Width / 2f;
        }

        public void WasShot()
        {
            hitPoints--;
            if (hitPoints <= 0)
                IsExpired = true;

            Sound.Explosion.Play(0.5f, rand.NextFloat(-0.2f, 0.2f), 0);
        }

        public void Kill()
        {
            hitPoints = 0;
            WasShot();
        }

        public override void Update()
        {
            var entities = EntityManager.GetNearbyEntities(Position, 250);

            foreach(var entity in entities)
            {
                if (entity is Enemy && !(entity as Enemy).IsActive)
                    continue;

                if (entity is Bullet)
                    entity.Velocity += (entity.Position - Position).ScaleTo(0.3f);
                else
                {
                    var dPos = Position - entity.Position;
                    var length = dPos.Length();

                    entity.Velocity += dPos.ScaleTo(MathHelper.Lerp(2, 0, length / 250f));
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float scale = 1 + 0.1f * (float)Math.Sin(10 * GameRoot.GameTime.TotalGameTime.TotalSeconds);
            spriteBatch.Draw(image, Position, null, color, Orientation, Size / 2f, scale, 0, 0);
        }
    }
}
