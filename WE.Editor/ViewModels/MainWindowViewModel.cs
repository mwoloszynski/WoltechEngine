using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Numerics;
using WE.AssetPipeline.Serializers;
using WE.Core.Components;
using WE.Core.ECS;
using WE.Graphics;

namespace WE.Editor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private Registry _activeRegistry = new();

    [ObservableProperty]
    private Camera3D _editorCamera;

    [ObservableProperty]
    private ObservableCollection<EntityItemViewModel> _entities = new();

    [ObservableProperty]
    private EntityItemViewModel? _selectedEntity;

    [ObservableProperty] private decimal _posX;
    [ObservableProperty] private decimal _posY;
    [ObservableProperty] private decimal _posZ;

    [ObservableProperty] private decimal _scaleX = 1;
    [ObservableProperty] private decimal _scaleY = 1;
    [ObservableProperty] private decimal _scaleZ = 1;

    public bool IsEntitySelected => SelectedEntity != null;

    private int? _playerEntityId;
    private bool _isLoadingEntity;

    public MainWindowViewModel()
    {
        EditorCamera = new Camera3D
        {
            Position = new Vector3(10f, 9.6f, 10f),
            Target = Vector3.Zero,
            ProjectionMode = CameraProjectionMode.Orthographic,
            OrthographicHeight = 8f
        };

        LoadScene();
    }

    partial void OnSelectedEntityChanged(EntityItemViewModel? value)
    {
        OnPropertyChanged(nameof(IsEntitySelected));

        if (value == null) return;

        _isLoadingEntity = true;
        if (ActiveRegistry.Meshes.TryGetValue(value.Id, out var mesh))
        {
            PosX = (decimal)mesh.Position.X;
            PosY = (decimal)mesh.Position.Y;
            PosZ = (decimal)mesh.Position.Z;

            ScaleX = (decimal)mesh.Scale.X;
            ScaleY = (decimal)mesh.Scale.Y;
            ScaleZ = (decimal)mesh.Scale.Z;
        }
        _isLoadingEntity = false;
    }

    partial void OnPosXChanged(decimal value) => UpdateSelectedMesh();
    partial void OnPosYChanged(decimal value) => UpdateSelectedMesh();
    partial void OnPosZChanged(decimal value) => UpdateSelectedMesh();
    partial void OnScaleXChanged(decimal value) => UpdateSelectedMesh();
    partial void OnScaleYChanged(decimal value) => UpdateSelectedMesh();
    partial void OnScaleZChanged(decimal value) => UpdateSelectedMesh();

    private void UpdateSelectedMesh()
    {
        if (_isLoadingEntity || SelectedEntity == null) return;

        int id = SelectedEntity.Id;
        if (ActiveRegistry.Meshes.TryGetValue(id, out var mesh))
        {
            var newPos = new Vector3((float)PosX, (float)PosY, (float)PosZ);
            var newScale = new Vector3((float)ScaleX, (float)ScaleY, (float)ScaleZ);
            ActiveRegistry.Meshes[id] = mesh with { Position = newPos, Scale = newScale };
        }
    }

    [RelayCommand]
    private void AddCube()
    {
        int id = ActiveRegistry.CreateEntity();
        ActiveRegistry.Meshes[id] = new MeshComponent(new Vector3(0f, 0.5f, 0f), PrimitiveMeshes.Cube)
        {
            Color = new Vector3(0.8f, 0.8f, 0.8f)
        };
        RefreshEntities();
    }

    [RelayCommand]
    private void AddPyramid()
    {
        int id = ActiveRegistry.CreateEntity();
        ActiveRegistry.Meshes[id] = new MeshComponent(new Vector3(0f, 0.5f, 0f), PrimitiveMeshes.Pyramid)
        {
            Color = new Vector3(1.0f, 0.55f, 0.2f)
        };
        RefreshEntities();
    }

    [RelayCommand]
    private void NewScene()
    {
        ActiveRegistry.Entities.Clear();
        ActiveRegistry.Meshes.Clear();
        _playerEntityId = null;
        SelectedEntity = null;
        RefreshEntities();
    }

    [RelayCommand]
    private void SaveScene()
    {
        SceneSerializer.SaveScene(ActiveRegistry, "Scenes/level1.json", _playerEntityId);
    }

    [RelayCommand]
    private void LoadScene()
    {
        if (!File.Exists("Scenes/level1.json")) return;

        _playerEntityId = SceneSerializer.LoadScene(ActiveRegistry, "Scenes/level1.json");
        SelectedEntity = null;
        RefreshEntities();
    }

    private void RefreshEntities()
    {
        Entities.Clear();
        foreach (var id in ActiveRegistry.Entities)
        {
            string label = id == _playerEntityId ? $"Entity #{id} [Player]" : $"Entity #{id}";
            Entities.Add(new EntityItemViewModel { Id = id, DisplayName = label });
        }
    }
}

public class EntityItemViewModel
{
    public int Id { get; init; }
    public string DisplayName { get; init; } = string.Empty;
}