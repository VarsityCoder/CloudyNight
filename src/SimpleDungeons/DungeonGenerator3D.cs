namespace CloudyNight.SimpleDungeons;

using System;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Mutex = Godot.Mutex;

public partial class DungeonGenerator3D : Node3D {
  [Signal]
  public delegate void DoneGeneratingEventHandler();

  [Signal]
  public delegate void GeneratingFailedEventHandler();

  //TODO add the custom rooms function

  private Mutex _tMutex = new();

  public enum BuildStage {
    NOT_STARTED = -2,
    PREPARING =  -1,
    PLACE_ROOMS = 0,
    PLACE_STAIRS = 1,
    SEPARATE_ROOMS = 2,
    CONNECT_ROOMS = 3,
    FINALIZING = 4,
    DONE = 5
  }


  // Backing Store
  private BuildStage _stage = BuildStage.NOT_STARTED;
  public BuildStage Stage {
    get {
      Stage = _stage;
      _tMutex.Lock();
      var v = _stage;
      _tMutex.Unlock();
      return v;
    }
    set {
      _tMutex.Lock();
      _stage = value;
      _tMutex.Unlock();
    }
  }

  [Export] public Array<PackedScene?> RoomScenes = [];
  [Export] public PackedScene? CorridorRoomScene;

  //Backing store
  private Vector3I _dungeonSize = new(10,10,10);

  [Export]
  public Vector3I DungeonSize {
    get => _dungeonSize;
    set => _dungeonSize = value.Clamp(new Vector3I(1, 1, 1), new Vector3I(9999, 9999, 9999));
  }
  // Backing Store
  private Vector3 _voxelScale = new Vector3(10, 10, 10);

 [Export]
 public Vector3 VoxelScale {
    get => _voxelScale;
    set => _voxelScale = value.Clamp(new Vector3(0.0001f, 0.0001f, 0.0001f), new Vector3(9999f, 9999f, 9999f));
 }

 //Backing Store
 private static string _generateSeed = "";

 [Export]
 public string GenerateSeed {
   get => _generateSeed;
   set {
     var pattern = "r[^0-9-]";
     var strippedValue = value.StripEdges().Replace(pattern, "");
     if (strippedValue.StartsWith("-")) {
       strippedValue = "-" + strippedValue.Replace("-", "");
     }
     else {
       strippedValue = strippedValue.Replace("-", "");
     }
     _generateSeed = strippedValue;
   }
 }

 [Export] private bool _generateOnReady = true;
 [Export] private int _maxRetries = 1;
 [Export] private int _maxSafeIterations = 250;
 [Export] private bool _generateThreaded;

 //Backing Store
 private bool _editorButtonGenerateDungeon;

 [Export]
 public bool EditorButtonGenerateDungeon {
   get => _editorButtonGenerateDungeon;
   set => Generate();
 }

 // Backing Store
 private bool _abortEditorButton;

 [Export]
 public bool AbortEditorButton {
   get => _abortEditorButton;
   set => AbortGeneration();
 }

 private enum AStarHeuristics {
   NONE_DIJKSTRAS = 0,
   MANHATTAN = 1,
   EUCLIDEAN = 2
 }

 [ExportGroup("AStar room connection options")]
 [Export] private AStarHeuristics _aStarHeuristics = AStarHeuristics.EUCLIDEAN;

 [Export] private float _heuristicScale = 1f;
 [Export] private float _corridorCostMultiplier = 0.25f;
 [Export] private float _roomCostMultiplier = 0.025f;
 [Export] private float _roomCostAtEndForRequiredDoors = 2f;

 [ExportGroup("Debug Options")] [Export]
 private bool _showDebugInEditor = true;

 [Export] private bool _showDebugInGame;
 [Export] private bool _placeEvenIfFail;
 [Export] public bool VisualizeGenerationProgress;
 [Export] private bool _hideDebugVisualsForAllGeneratedRooms = true;
 [Export] private bool _cycleDebugDrawWhenPressN;

 [Export(PropertyHint.Range, "0, 1000, 1, or_greater, suffix:ms")]
 private int _visualizeGenerationWaitBetweenIterations = 100;

 private Node? _debugView;

 private Array<DungeonRoom3D> _roomInstances = new();
 private DungeonRoom3D? CorridorRoomInstance;
 private int _iterations;
 private int _retryAttempts;
 private Node3D? _roomsContainer;
 private RandomNumberGenerator _rng = new();
 // TODO I need to figure out what this thread is doing
 // private Thread _runningThread = new Thread(new ThreadStart(Generate));


 //Backing Store
 private bool _failedToGenerate;

 public bool FailedToGenerate {
   get {
     _tMutex.Lock();
     FailedToGenerate = _failedToGenerate;
     var v = FailedToGenerate;
     _tMutex.Unlock();
     return v;
   }
   set {
     _tMutex.Lock();
     _failedToGenerate = value;
     _tMutex.Unlock();
   }
}
 // Backing Store
 private bool _fullAbortTriggered;

 public bool FullAbortTriggered {
   get {
     _tMutex.Lock();
     var v = _fullAbortTriggered;
     _tMutex.Unlock();
     return v;
   }
   set {
     _tMutex.Lock();
     _fullAbortTriggered = value;
     _tMutex.Unlock();
   }
 }
 // Backing Store
 private bool _isCurrentlyGenerating;

 public bool IsCurrentlyGenerating => !(_stage is BuildStage.NOT_STARTED or BuildStage.DONE) && !_failedToGenerate;

 private bool _isGenerateThreaded;
 private bool _visualizationInProgress;
 private int _lastIterationEndTime = (int)Time.GetTicksMsec();
 private Array<DungeonRoom3D> _preplacedRoomsCached = new();



 private void Init() => RenderingServer.SetDebugGenerateWireframes(true);

 public override void _Input(InputEvent @event) {
   if (_cycleDebugDrawWhenPressN && @event is InputEventKey && Input.IsKeyPressed(Key.N)) {
     var vp = GetViewport();
     vp.DebugDraw = (Viewport.DebugDrawEnum)((vp.DebugDraw.GetHashCode() + 1) % 4);
   }
 }


 private void AddDebugViewIfNotExist() {
   if (!_debugView.IsNodeReady()) {
    //TODO add debug view neod
   }
 }
 public override void _Ready() {
   AddDebugViewIfNotExist();
   if (Engine.IsEditorHint()) {
     return;
   }
   if (_generateOnReady) {
     Generate();
   }
 }

 public override void _Process(double delta) {
   if (Engine.IsEditorHint()) {
     foreach (var child in GetChildren()) {
       if (child is DungeonRoom3D) {
         DungeonRoom3D dungeonRoom3DChild = child as DungeonRoom3D;
         if (dungeonRoom3DChild.VirtualizedFrom != null) {
           dungeonRoom3DChild.AddDebugViewIfNotExist();
         }
       }
     }
   }
 }

 private int GenerateHelper() {
   if (GenerateSeed.IsValidInt()) {
     return GenerateSeed.ToInt();
   }
   var random = new Random();
   var randomInt = random.Next();
   return randomInt;
 }

 private void Generate() {
   var seed = GenerateHelper();
   if (_isCurrentlyGenerating) {
     GD.PrintErr("SimpleDungeonsC# Error: Dungeon currently generating, cannot generate.");
     return;
   }

   _stage = BuildStage.NOT_STARTED;
   if (!ValidateDungeon()) {
     GD.PrintErr("SimpleDungeons C#: Cannot generate.");
     return;
   }

   _rng = new RandomNumberGenerator();
   _rng.Seed = (ulong)seed;
   GD.Print("SimpleDungeonsC# generate() : Using seed ", seed);

   CleanAndResetDungeonGenerator();
   CreateOrRecreateRoomsContainer();
   GetPreplacedRooms();
 }

 private void AbortGeneration() {

 }

 private bool ValidateDungeon() {
   return true;
 }

 private void RunGenerateLoop() {

 }

 private void CleanAndResetDungeonGenerator() {

 }

 private void CreateOrRecreateRoomsContainer() {

 }

 private void FailGeneration(string error = "Aborted Generation") {
   GD.PrintErr("SimpleDungeonsC# Error: " + error);
   GD.PrintErr("SimpleDungeonsC# Error: Failed to generate dungeon");
   _failedToGenerate = true;
 }


 private Array<DungeonRoom3D> GetPreplacedRooms() {
   var rooms = new Array<DungeonRoom3D>();
   if (OS.GetThreadCallerId() != OS.GetMainThreadId()) {
     return _preplacedRoomsCached.Slice(0);
   }

   return rooms;
 }
}
