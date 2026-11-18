using System;
using System.Collections.Generic;
using System.Text;
using WE.Core.Geometry;

namespace WE.AssetPipeline.Serializers;

public class SceneData
{
    public string SceneName { get; set; } = "Level1";
    public int? PlayerEntityId { get; set; }
    public List<EntityData> Entities { get; set; } = new();
}

public class EntityData
{
    public int Id { get; set; }
    public MeshData? Mesh { get; set; }
}

public class MeshData
{
    public string PrimitiveType { get; set; } = "Cube";
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float PosZ { get; set; }

    public float ScaleX { get; set; } = 1f;
    public float ScaleY { get; set; } = 1f;
    public float ScaleZ { get; set; } = 1f;

    public float ColorR { get; set; } = 1f;
    public float ColorG { get; set; } = 1f;
    public float ColorB { get; set; } = 1f;
}