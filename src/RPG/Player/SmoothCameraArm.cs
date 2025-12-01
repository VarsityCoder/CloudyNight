using Godot;
namespace CloudyNight.RPG.Player;

public partial class SmoothCameraArm : SpringArm3D {
  [Export] private Node3D? _target;
  [Export] private float _decay = 20.0f;

  public override void _PhysicsProcess(double delta) {
    if (_target != null) {
      GlobalTransform = GlobalTransform.InterpolateWith(_target.GlobalTransform ,1f- (float)Mathf.Exp(-_decay * delta));
    }
  }
}
