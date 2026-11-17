using System;
using System.Collections.Generic;
using System.Text;
using WE.Core.ECS;

namespace WE.Core.Systems
{
    public static class CollisionSystem
    {
        public static bool Overlaps(Registry registry, int first, int second)
        {
            if (first == second
            || !registry.Transforms.TryGetValue(first, out var firstTransform)
            || !registry.Transforms.TryGetValue(second, out var secondTransform)
            || !registry.BoxColliders.TryGetValue(first, out var firstBox)
            || !registry.BoxColliders.TryGetValue(second, out var secondBox))
            {
                return false;
            }

            float distanceX = MathF.Abs(firstTransform.X - secondTransform.X);
            float distanceY = MathF.Abs(firstTransform.Y - secondTransform.Y);

            float reachX = (firstBox.Width + secondBox.Width) * 0.5f;
            float reachY = (firstBox.Height + secondBox.Height) * 0.5f;

            return distanceX < reachX && distanceY < reachY;
        }
    }
}
