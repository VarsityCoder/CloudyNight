using Godot;
namespace CloudyNight.RPG.Player;

public partial class PlayerMovement : CharacterBody3D
{
  private const float SPEED = 10.0f;
  private const float JUMP_VELOCITY = 4.5f;
  private Node3D? _verticalPivot;
  private Node3D? _horizontalPivot;
  private SpringArm3D? _springArm3D;
  private Vector2 _look = Vector2.Zero;
  private Vector3 _attackDirection = Vector3.Zero;
  private Rig? _rig = new();
  private AttackCast? _attackCast;

  private Node3D? _rigPivot;

  [Export]
  private float _mouseSensitivity = 0.00075f;

  [Export] private float _minBoundary = -75f;
  [Export] private float _maxBoundary = 10f;
  [Export] private float _animationDecay = 10f;
  [Export] private float _attackMoveSpeed = 3f;

  public override void _Ready() {
    Input.MouseMode = Input.MouseModeEnum.Captured;
    _horizontalPivot = GetNode<Node3D>("HorizontalPivot");
    _verticalPivot = GetNode<Node3D>("HorizontalPivot/VerticalPivot");
    _springArm3D = GetNode<SpringArm3D>("SmoothCameraArm");
    _rigPivot = GetNode<Node3D>("RigPivot");
    _rig = GetNode<Node3D>("RigPivot/Rig") as Rig;
    _attackCast = GetNode<AttackCast>("RigPivot/RayAttachment/AttackCast");
  }

  public override void _PhysicsProcess(double delta) {
    FrameCameraRotation();
		var velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JUMP_VELOCITY;
		}

    var direction = GetMovementDirection();
    _rig.UpdateAnimationTree(direction);
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * SPEED;
			velocity.Z = direction.Z * SPEED;
      LookTowardDirection(direction, delta);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, SPEED);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, SPEED);
		}

		Velocity = velocity;
    HandleSlashingPhysicsFrame((float)delta);
		MoveAndSlide();
	}

  public override void _UnhandledInput(InputEvent @event) {
    if (@event.IsActionPressed("ui_cancel")) {
      Input.MouseMode = Input.MouseModeEnum.Visible;
    }
    if(@event.IsActionPressed("click")) {
      Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    if (_rig.isIdle()) {
      if (@event.IsActionPressed("click")) {
        SlashAttack();
      }
    }

    if (@event is InputEventMouseMotion) {
      if (Input.MouseMode == Input.MouseModeEnum.Captured) {
        var m = (InputEventMouseMotion)@event;
        _look = -m.Relative * _mouseSensitivity;
      }
    }
  }

  private void FrameCameraRotation() {
    if (_springArm3D != null && _verticalPivot != null && _horizontalPivot != null) {
      _horizontalPivot.RotateY(_look.X);
      _verticalPivot.RotateX(_look.Y);
      var tempVector3 = _verticalPivot.Rotation;
      tempVector3.X = Mathf.Clamp(_verticalPivot.Rotation.X, Mathf.DegToRad(_minBoundary), Mathf.DegToRad(_maxBoundary));
      _verticalPivot.Rotation = tempVector3;
      _look = Vector2.Zero;
    }
  }

  private Vector3 GetMovementDirection() {
    var inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
    var inputVector = new Vector3(inputDir.X, 0, inputDir.Y).Normalized();
    Vector3 direction = default;
    if (_horizontalPivot != null) {
      direction = _horizontalPivot.GlobalTransform.Basis * inputVector;
    }
    return direction;
  }

  private void LookTowardDirection(Vector3 direction, double delta) {
    if (_rigPivot != null) {
      var targetTransform = _rigPivot.GlobalTransform.LookingAt(_rigPivot.GlobalPosition + direction, Vector3.Up, true);
      _rigPivot.GlobalTransform = _rigPivot.GlobalTransform.InterpolateWith(targetTransform, 1f- (float)Mathf.Exp(-_animationDecay * delta));
    }
  }
  private void SlashAttack() {
    _rig.Travel("attack_Root|Attack");
    _attackDirection = GetMovementDirection();
    if (_attackDirection.IsZeroApprox()) {
      _attackDirection = _rig.GlobalBasis * new Vector3(0, 0, 1);
    }
    _attackCast?.ClearExceptions();
  }

  private void HandleSlashingPhysicsFrame(float delta) {
    if (!_rig.isSlashing()) {
      return;
    }
    var tempVector3 = Velocity;
    tempVector3.X = _attackDirection.X * _attackMoveSpeed;
    tempVector3.Z = _attackDirection.Z * _attackMoveSpeed;
    Velocity = tempVector3;
    _attackCast?.DealDamage();
    LookTowardDirection(_attackDirection, delta);
  }
}
