using Godot;
namespace CloudyNight.Scripts;

public partial class Thruster : Marker3D {
  [Export] private bool _enabled = true;
  [Export] private float _force = 100f;

  public override void _PhysicsProcess(double delta) {
    if (_enabled) {
      GetParent<RigidBody3D>().ApplyForce(-GlobalTransform.Basis.Z * _force, GlobalTransform.Basis * Position);
    }
  }
}
