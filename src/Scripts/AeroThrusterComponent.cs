using Godot;
namespace CloudyNight.Scripts;

public partial class AeroThrusterComponent : Marker3D {
  private RigidBody3D _rigidBody3D;
  [Export] private bool _enabled = true;

  [ExportCategory("Control")]
  [Export] private bool _getThrottleFromAeroBody = true;
  [Export] private float _throttle = 1f;

  [ExportCategory("Simulation Parameters")]
  [Export] private float _intakeArea = 0.5f;

  [Export] private float _intakeFanMaxVelocity = 100f;
  [Export] private float _exhaustVelocity = 1000f;
  [Export] private float _maxFuelFlow = 0.1f;

  public override void _PhysicsProcess(double delta) {
    if (!_enabled) {
      return;
    }

    var massAccelerationRate = CalculateMassFlowAcceleration();
    var forceMagnitude = massAccelerationRate * (float)delta;
    GD.Print(massAccelerationRate);
    if(_rigidBody3D != null) {

    }
    if(_rigidBody3D is AeroThrusterComponent) {

    }
  }
  private float CalculateMassFlowAcceleration() {
    var altitude = 0f;
    var airVelocity = _rigidBody3D.LinearVelocity;

    var intakeAirVelocty = airVelocity.Dot(-GlobalBasis.Z);
    return 1f;
  }

  private float CalculateExhaustVelocity() {
    return _exhaustVelocity;
  }
}
