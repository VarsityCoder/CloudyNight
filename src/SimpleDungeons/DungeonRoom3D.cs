namespace CloudyNight.SimpleDungeons;

using Godot;
using Godot.Collections;

public partial class DungeonRoom3D : Node3D {
  [Signal]
  public delegate void DungeonDoneGeneratingEventHandler();

  // Backing Store
  private DungeonGenerator3D _dungeonGenerator3D;

  public DungeonGenerator3D DungeonGenerator3D {
    get {
      if (IsInsideTree() && GetParent() is DungeonGenerator3D) {
        return (DungeonGenerator3D)GetParent();
      }

      return _dungeonGenerator3D;
    }
    set => _dungeonGenerator3D = value;
  }

  // Backing Store
  private Vector3I _sizeInVoxels = new(1, 1, 1);

  [Export]
  public Vector3I SizeInVoxels {
    get => _sizeInVoxels;
    set => _sizeInVoxels = value.Clamp(new Vector3I(1, 1, 1), new Vector3I(9999, 9999, 9999));
  }

  // Backing Store
  private Vector3 _voxelScale = new(10, 10, 10);

  [Export]
  public Vector3 VoxelScale {
    get => _voxelScale;
    set => _voxelScale = value.Clamp(new Vector3(0.0001f, 0.0001f, 0.0001f), new Vector3(9999, 9999, 9999));
  }

  [Export] private int _minCount = 2;
  [Export] private int _maxCount = 5;

  [Export] private bool _isStairRoom;

  [ExportGroup("Pre-Place room options")]

  // Backing Store
  private bool _forceAlignWithGridButton;

  [Export]
  public bool ForceAlignWithGridButton {
    get => _forceAlignWithGridButton;
    set {
      _virtualTransform = Transform;
      SnapRoomToDungeonGrid();
    }
  }

  [ExportGroup("Debug View")] [Export] private bool _showDebugInEditor = true;

  [Export] private bool _showDebugInGame;
  [Export] private bool _showGridAabbWithDoors;

  // Backing Store
  private bool _wasReplaced;

  public bool WasReplaced {
    get {
      if (Engine.IsEditorHint()) {
        return IsInsideTree() && GetParent() is DungeonGenerator3D;
      }
      return _wasReplaced;
    }
  }

  // Backing Store
  private int _roomRotations;

  public int RoomRotations {
    // Using int for Vector 3 because I think original intent was to use Wrap Int
    get => (int)Mathf.Round(Mathf.Wrap(Mathf.Round(_virtualTransform.Basis.GetEuler().Y / Mathf.DegToRad(90.0)),0,4));
    set =>
      _virtualTransform.Basis = Basis.FromEuler(new Vector3((int)0, (int)(Mathf.Wrap(value, 0, 4) * Mathf.DegToRad(90.0)), (int)0))
        .Scaled(_virtualTransform.Basis.Scale);
  }

  // Backing Store
  private DungeonRoom3D _virtualSelf;

  public DungeonRoom3D VirtualSelf {
    get {
      if (VirtualizedFrom is null) {
        return VirtualizedFrom;
      }
      return this;
    }
    set => _virtualSelf = value;
  }

  private Transform3D _virtualTransform;




  public DungeonRoom3D VirtualizedFrom;



  public void AddDebugViewIfNotExist() {

  }

  private void SnapRoomToDungeonGrid() {

  }

  // Have no clue what this is doing. May skip since I don't know what he is doing with .name and .usage in the dictionary
  private void ValidateProperty(Dictionary<string,GodotObject> property) {
    if (property.ContainsKey("force_align_with_grid_button") && !(GetParent() is DungeonGenerator3D)) {
    }


  }

}
