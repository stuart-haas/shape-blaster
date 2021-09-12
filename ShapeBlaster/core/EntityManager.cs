using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShapeBlaster.core;
using ShapeBlaster.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeBlaster
{
    static class EntityManager
    {
        static List<Entity> entities = new List<Entity>();

        static bool isUpdating;
        static List<Entity> addedEntities = new List<Entity>();

        static List<Enemy> enemies = new List<Enemy>();
        static List<Bullet> bullets = new List<Bullet>();
        static List<BlackHole> blackHoles = new List<BlackHole>();

        public static int Count {  get { return entities.Count; } }
        public static int BlackHoleCount { get { return blackHoles.Count; } }

        private static void AddEntity(Entity entity)
        {
            entities.Add(entity);
            if (entity is Bullet)
                bullets.Add(entity as Bullet);
            else if (entity is Enemy)
                enemies.Add(entity as Enemy);
        }

        public static void Add(Entity entity)
        {
            if (!isUpdating)
                EntityManager.AddEntity(entity);
            else
                addedEntities.Add(entity);
        }

        public static void Update()
        {
            isUpdating = true;

            HandleCollisions();

            foreach (var entity in entities)
                entity.Update();

            isUpdating = false;

            foreach (var entity in addedEntities)
                EntityManager.Add(entity);

            addedEntities.Clear();

            entities = entities.Where(x => !x.IsExpired).ToList();
            bullets = bullets.Where(x => !x.IsExpired).ToList();
            enemies = enemies.Where(x => !x.IsExpired).ToList();
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var entity in entities)
                entity.Draw(spriteBatch);
        }

        static void HandleCollisions()
        {
            for(int i = 0; i < enemies.Count; i ++)
                for(int j = i + 1; j < enemies.Count; j ++)
                {
                    if(IsColliding(enemies[i], enemies[j]))
                    {
                        enemies[i].HandleCollision(enemies[j]);
                        enemies[j].HandleCollision(enemies[i]);
                    }
                }

            for(int i = 0; i < enemies.Count; i ++)
                for(int j = 0; j < bullets.Count; j ++)
                {
                    if(IsColliding(enemies[i], bullets[j]))
                    {
                        enemies[i].WasShot();
                        bullets[j].IsExpired = true;
                    }
                }

            for(int i = 0; i < enemies.Count; i ++)
            {
                if(enemies[i].IsActive && IsColliding(PlayerShip.Instance, enemies[i]))
                {
                    PlayerShip.Instance.Kill();
                    enemies.ForEach(x => x.WasShot());
                    break;
                }
            }

            for (int i = 0; i < blackHoles.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                    if (enemies[j].IsActive && IsColliding(blackHoles[i], enemies[j]))
                        enemies[j].WasShot();

                for (int j = 0; j < bullets.Count; j++)
                {
                    if (IsColliding(blackHoles[i], bullets[j]))
                    {
                        bullets[j].IsExpired = true;
                        blackHoles[i].WasShot();
                    }
                }

                if (IsColliding(PlayerShip.Instance, blackHoles[i]))
                {
                    KillPlayer();
                    break;
                }
            }
        }

        private static void KillPlayer()
        {
            PlayerShip.Instance.Kill();
            enemies.ForEach(x => x.WasShot());
            blackHoles.ForEach(x => x.Kill());
            EnemySpawner.Reset();
        }

        private static bool IsColliding(Entity a, Entity b)
        {
            float radius = a.Radius + b.Radius;
            return !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < radius * radius;
        }

        public static IEnumerable<Entity> GetNearbyEntities(Vector2 position, float radius)
        {
            return entities.Where(x => Vector2.DistanceSquared(position, x.Position) < radius * radius);
        }
    }
}
