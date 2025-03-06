using Godot;
namespace CloudyNight.Scripts;

[Tool]
public partial class AeroBody3d : VehicleBody3D {
  private Script _aeroMathUtils = GD.Load<Script>("../utils/math_utils.cs");
  private Script _aeroNodehUtils = GD.Load<Script>("../utils/node_utils.cs");


  private Vector3 _currentForce = Vector3.Zero;
  private Vector3 _currentTorque = Vector3.Zero;
  private Vector3 _currentGravity = Vector3.Zero;
  private Vector3 _lastLinearVelocity;
  private Vector3 _lastAngularVelocity;
  private Vector3 _wind = Vector3.Zero;
  private Vector3 _airVelocity = Vector3.Zero;
  private Vector3 _localAirVelocity = Vector3.Zero;
  private Vector3 _localAngularVelocity = Vector3.Zero;
  private float _airSpeed;
  private float _mach;
  private float _airDensity;
  private float _airPressure;
  private float _angleOfAttack;
  private float _sideslipAngle;
  private float _altitude;
  private float _bankAngle;
  private float _heading;
  private float _inclination;
  private Vector3 _linearVelocityPrediction;
  private Vector3 _angularVelocityPrediction;
  private float _subStepDelta;


  // Backing Store
  private int _subStepsOverride = -1;

  [Export]
  private int SubStepsOverride {
    get => _subStepsOverride;
    set {
      _subStepsOverride = value;
    }
  }

  [ExportGroup("Control")] [Export] private Vector3 _controlCommand = Vector3.Zero;
  [Export] private float _throttleCommand;
  [Export] private float _brakeCommand;

  [ExportGroup("Debug")] [ExportSubgroup("Visibility ")]

  // Backing Store
  private bool _showDebug;

  [Export]
  public bool ShowDebug {
    get => _showDebug;
    set {
      _showDebug = value;
      UpdateDebugVisibility();

    }
  }

  private Vector3 GetRelativePosition() => GlobalBasis * -CenterOfMass;

  private Vector3 GetLinearVelocity() => _linearVelocityPrediction;

  private Vector3 GetAngularVelocity() => _angularVelocityPrediction;

  private Vector3 GetLinearAcceleration() => (_linearVelocityPrediction - _lastLinearVelocity) / _subStepDelta;

  private Vector3 GetAngularAcceleration() => (_angularVelocityPrediction - _lastAngularVelocity) / _subStepDelta;

  private void UpdateDebugVisibility() {

  }


  public override void _Ready() {
    _lastAngularVelocity = LinearVelocity;
    _lastAngularVelocity = AngularVelocity;
    _linearVelocityPrediction = LinearVelocity;
    _angularVelocityPrediction = AngularVelocity;
    _subStepDelta = (float)GetPhysicsProcessDeltaTime() / 1f;
  }
}
