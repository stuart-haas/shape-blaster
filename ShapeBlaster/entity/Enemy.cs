using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShapeBlaster.core;
using ShapeBlaster.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeBlaster
{
    class Enemy: Entity
    {
        public static Random rand = new Random();

        public int PointValue { get; private set; }
        public bool IsActive { get { return timeUntilStart <= 0; } }

        private int timeUntilStart = 60;
        private List<IEnumerator<int>> behaviors = new List<IEnumerator<int>>();

        public static Enemy CreateSeeker(Vector2 position)
        {
            var enemy = new Enemy(Art.Seeker, position);
            enemy.AddBehavior(enemy.FollowPlayer());
            enemy.PointValue = 2;
            return enemy;
        }

        public static Enemy CreateWanderer(Vector2 position)
        {
            var enemy = new Enemy(Art.Wanderer, position);
            enemy.AddBehavior(enemy.MoveRandomly());
            enemy.PointValue = 1;
            return enemy;
        }


        public Enemy(Texture2D image, Vector2 position)
        {
            this.image = image;
            Position = position;
            Radius = image.Width / 2f;
            color = Color.Transparent;
        }

        public override void Update()
        {
            if(timeUntilStart <= 0)
            {
                ApplyBehaviors();
            }
            else
            {
                timeUntilStart--;
                color = Color.White * (1 - timeUntilStart / 60f);
            }

            Position += Velocity;
            Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size / 2);

            Velocity *= 0.8f;
        }

        public void HandleCollision(Enemy other)
        {
            var d = Position - other.Position;
            Velocity += 10 * d / (d.LengthSquared() + 1);
        }

        public void WasShot()
        {
            IsExpired = true;
            Sound.Explosion.Play(0.5f, rand.NextFloat(-0.2f, 0.2f), 0);

            PlayerStatus.AddPoints(PointValue);
            PlayerStatus.IncreaseMultiplier();
        }

        private void AddBehavior(IEnumerable<int> behavior)
        {
            behaviors.Add(behavior.GetEnumerator());
        }

        private void ApplyBehaviors()
        {
            for(int i = 0; i < behaviors.Count; i ++)
            {
                if (!behaviors[i].MoveNext())
                    behaviors.RemoveAt(i--);
            }
        }

        IEnumerable<int> FollowPlayer(float acceleration = 1f)
        {
            while(true)
            {
                Velocity += (PlayerShip.Instance.Position - Position).ScaleTo(acceleration);
                if (Velocity != Vector2.Zero)
                    Orientation = Velocity.ToAngle();

                yield return 0;
            }
        }

        IEnumerable<int> MoveRandomly()
        {
            float direction = rand.NextFloat(0, MathHelper.TwoPi);

            while(true)
            {
                direction += rand.NextFloat(-0.1f, 0.1f);
                direction = MathHelper.WrapAngle(direction);

                for(int i = 0; i < 6; i ++)
                {
                    Velocity += MathUtil.FromPolar(direction, 0.4f);
                    Orientation -= 0.05f;

                    var bounds = GameRoot.Viewport.Bounds;
                    bounds.Inflate(-image.Width, -image.Height);

                    if (!bounds.Contains(Position.ToPoint()))
                        direction = (GameRoot.ScreenSize / 2 - Position).ToAngle() + rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);

                    yield return 0;
                }
            }
        }
    }
}
