namespace CloudyNight.RPG.Player;
using Godot;


public partial class AttackCast : RayCast3D {
  public void DealDamage() {
    if (!IsColliding()) {
      return;
    }
    var collider = GetCollider();
    GD.Print(collider);
  }
}
