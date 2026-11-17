using System;
using System.Collections.Generic;
using System.Text;
using WE.Core.Components;

namespace WE.Core.ECS
{
    public class Registry
    {
        private int _nextEntityId = 0;

        public Dictionary<int, TransformComponent> Transforms { get; } = new();
        public Dictionary<int, SpriteComponent> Sprites { get; } = new();
        public Dictionary<int, MeshComponent> Meshes { get; } = new();
        public Dictionary<int, BoxColliderComponent> BoxColliders { get; } = new();

        public List<int> Entities { get; } = new();

        public int CreateEntity()
        {
            int id = _nextEntityId++;
            Entities.Add(id);
            return id;
        }

        public void DestroyEntity(int entityId)
        {
            Entities.Remove(entityId);
            Transforms.Remove(entityId);
            Sprites.Remove(entityId);
            Meshes.Remove(entityId);
            BoxColliders.Remove(entityId);
        }
    }
}
