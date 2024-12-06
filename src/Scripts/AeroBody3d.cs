using Godot;
namespace CloudyNight.Scripts;

[Tool]
public partial class AeroBody3d : VehicleBody3D {
  private Script _aeroMathUtils = GD.Load<Script>("../utils/math_utils.cs");
  private Script _aeroNodehUtils = GD.Load<Script>("../utils/node_utils.cs");

  // Backing Store
  private int _subStepsOverride = -1;

  [Export] private int SubStepsOverride {
    get => _subStepsOverride;
    set {
      _subStepsOverride = value;
    }
  }

  [ExportGroup("Control")]
  [Export] private Vector3 _controlCommand = Vector3.Zero;
  [Export] private float _throttleCommand;
  [Export] private float _brakeCommand;

  public override void _Ready() {
  }
}
