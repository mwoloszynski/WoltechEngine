using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Text.Json;
using WE.Core.Components;
using WE.Core.ECS;
using WE.Graphics;

namespace WE.AssetPipeline.Serializers
{
    public static class SceneSerializer
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        public static void SaveScene(Registry registry, string filePath, int? playerEntityId = null)
        {
            var sceneData = new SceneData { PlayerEntityId = playerEntityId };

            foreach (var entityId in registry.Entities)
            {
                var entityData = new EntityData { Id = entityId };

                if (registry.Meshes.TryGetValue(entityId, out var mesh))
                {
                    entityData.Mesh = new MeshData
                    {
                        PrimitiveType = mesh.Mesh == PrimitiveMeshes.Pyramid ? "Pyramid" : "Cube",
                        PosX = mesh.Position.X,
                        PosY = mesh.Position.Y,
                        PosZ = mesh.Position.Z,
                        ScaleX = mesh.Scale.X,
                        ScaleY = mesh.Scale.Y,
                        ScaleZ = mesh.Scale.Z,
                        ColorR = mesh.Color.X,
                        ColorG = mesh.Color.Y,
                        ColorB = mesh.Color.Z
                    };
                }

                sceneData.Entities.Add(entityData);
            }

            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string jsonString = JsonSerializer.Serialize(sceneData, _options);
            File.WriteAllText(filePath, jsonString);
        }

        public static int? LoadScene(Registry registry, string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Scene file not found at: {filePath}");
            }

            string jsonString = File.ReadAllText(filePath);
            var sceneData = JsonSerializer.Deserialize<SceneData>(jsonString, _options);

            if (sceneData == null)
                return null;

            registry.Entities.Clear();
            registry.Meshes.Clear();
            int? loadedPlayerId = null;

            foreach (var entityData in sceneData.Entities)
            {
                int newId = registry.CreateEntity();

                if (sceneData.PlayerEntityId == entityData.Id)
                {
                    loadedPlayerId = newId;
                }

                if (entityData.Mesh != null)
                {
                    var primitive = entityData.Mesh.PrimitiveType == "Pyramid"
                        ? PrimitiveMeshes.Pyramid
                        : PrimitiveMeshes.Cube;

                    var position = new Vector3(entityData.Mesh.PosX, entityData.Mesh.PosY, entityData.Mesh.PosZ);

                    registry.Meshes[newId] = new MeshComponent(position, primitive)
                    {
                        Scale = new Vector3(entityData.Mesh.ScaleX, entityData.Mesh.ScaleY, entityData.Mesh.ScaleZ),
                        Color = new Vector3(entityData.Mesh.ColorR, entityData.Mesh.ColorG, entityData.Mesh.ColorB)
                    };
                }
            }

            return loadedPlayerId;
        }
    }
}
