namespace CloudyNight.SimpleDungeons;

using Godot;
using Godot.Collections;

public partial class DungeonRoom3D : Node3D {
  [Signal]
  public delegate void DungeonDoneGeneratingEventHandler();

  private static readonly Array<string> _dungeonRoomExportPropsNames = ["size_in_voxels","voxel_scale", "min_count",
  "max_count", "is_stair_room", "show_debug_in_editor", "show_debug_in_game", "show_grid_aabb_with_doors"];

  // Backing Store
  private DungeonGenerator3D? _dungeonGenerator;

  public DungeonGenerator3D? DungeonGenerator {
    get {
      if (IsInsideTree() && GetParent() is DungeonGenerator3D) {
        return (DungeonGenerator3D)GetParent();
      }

      return _dungeonGenerator;
    }
    set => _dungeonGenerator = value;
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
  private Mutex _threadFixMutex = new Mutex();
  // Backing Store
  private Array<Doors> _doorsCache = [];

  public Array<Doors> DoorsCache {
    get {
      _threadFixMutex.Lock();
      var tmp = _doorsCache;
      _threadFixMutex.Unlock();
      return tmp;
    }
    set {
      _threadFixMutex.Lock();
      _doorsCache = value;
      _threadFixMutex.Unlock();
    }
  }

  // Backing Store
  private int _roomRotations;

  public int RoomRotations {
    // Using int for Vector 3 because I think original intent was to use Wrap Int
    get => (int)Mathf.Round(Mathf.Wrap(Mathf.Round(VirtualTransform.Basis.GetEuler().Y / Mathf.DegToRad(90.0)),0,4));
    set =>
      _virtualTransform.Basis = Basis.FromEuler(new Vector3((int)0, (int)(Mathf.Wrap(value, 0, 4) * Mathf.DegToRad(90.0)), (int)0))
        .Scaled(VirtualTransform.Basis.Scale);
  }

  private DungeonRoom3D? _virtualizedFrom;

  // Backing Store
  private DungeonRoom3D? _virtualSelf;

  public DungeonRoom3D? VirtualSelf {
    get {
      if (VirtualizedFrom is null) {
        return VirtualizedFrom;
      }
      return this;
    }
    set => _virtualSelf = value;
  }

  // Backing Store
  private Transform3D _virtualTransform;

  public Transform3D VirtualTransform {
    get => _virtualTransform;
    set {
      _virtualTransform = value;
      if (IsInsideTree() && OS.GetMainThreadId() == OS.GetThreadCallerId()) {
        Transform = value;
      }
    }
  }

  private bool _originalReadyFuncCalled;


  public override void _Ready() {
    _originalReadyFuncCalled = true;
    if (_virtualizedFrom == null) {
      AddDebugViewIfNotExist();
    }

    if (!(VirtualTransform is Transform3D)) {
      Transform = VirtualTransform;
    }
    else if(!(Transform is Transform3D)) {
      VirtualTransform = Transform;
    }
    if (GetParent() is Node3D) {
      if (GetParent() is DungeonGenerator3D) {
        DungeonGenerator = GetParent() as DungeonGenerator3D;
      }
      if (GetParent().GetParent() is DungeonGenerator3D) {
        DungeonGenerator = GetParent().GetParent() as DungeonGenerator3D;
      }
    }
    if (Engine.IsEditorHint()) {
      return;
    }
  }

  public Vector3I LocalGridPosToDungeonGridPos(Vector3I localPos) {
    // TODO need to change this with actual vector3i
    return new Vector3I();
  }

  private Transform3D GetXformTo(SPACE from, SPACE to) {
    var t = new Transform3D();
    var inv = (to.GetHashCode() < from.GetHashCode()) ? to : from;

    //TODO need to change this for an actual transform 3d
    return new Transform3D();
  }

  public override void _Process(double delta) {
    if (Engine.IsEditorHint()) {
      return;
    }
  }

  private enum SPACE {
    LOCAL_GRID,
    LOCAL_SPACE,
    DUNGEON_SPACE,
    DUNGEON_GRID
  }

  private void _copyAllProps(DungeonRoom3D from, DungeonRoom3D to) {
    foreach (var prop in _dungeonRoomExportPropsNames) {
      if (from.Get(prop).ToString() != to.Get(prop).ToString()) {
        to.Set(prop, from.Get(prop));
      }
      to.Name = from.Name;
      to.DungeonGenerator = from.DungeonGenerator;
    }
  }

  private PackedScene? GetOriginalPackedScene() {
    if (DungeonGenerator != null && DungeonGenerator.CorridorRoomScene != null && VirtualSelf != null) {
      if (DungeonGenerator.CorridorRoomScene.ResourcePath == VirtualSelf.SceneFilePath) {
        return DungeonGenerator.CorridorRoomScene;
      }
      foreach (var scene in DungeonGenerator.RoomScenes) {
        if (scene?.ResourcePath == VirtualSelf.SceneFilePath) {
          return scene;
        }
      }
      if (VirtualSelf.SceneFilePath != null) {
        return GD.Load<PackedScene>(VirtualSelf.SceneFilePath);
      }
      GD.PrintErr(Name +
                  "Could not find DungeonRoom3D's original packed scene. This shouldn't happen. Are you manually spawning rooms?");
    }
    return null;
  }

  private DungeonRoom3D CreateCloneAndMakeVirtualUnlessVisualizing() {
    var makeCloneVirtual = !(DungeonGenerator != null && DungeonGenerator.VisualizeGenerationProgress);
    DungeonRoom3D clone;
    if (makeCloneVirtual) {
      clone = new DungeonRoom3D();
      clone.VirtualizedFrom = VirtualizedFrom;
      foreach (var door in DoorsCache) {
        // clone.DoorsCache.Append(new Doors(door))
      }
    }
    return null;
  }

  public DungeonRoom3D? VirtualizedFrom;


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
