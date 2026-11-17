using System;
using System.Collections.Generic;
using System.Text;
using WE.Core.ECS;

namespace WE.Core.Systems
{
    public static class MovementSystem
    {
        public static void Update(Registry registry, float deltaTimeSeconds)
        {
            foreach(int entity in registry.Entities)
            {
                if (!registry.Transforms.TryGetValue(entity, out var transform))
                    continue;

                transform.X += transform.VelocityX * deltaTimeSeconds;
                transform.Y += transform.VelocityY * deltaTimeSeconds;
                registry.Transforms[entity] = transform;
            }
        }
    }
}
