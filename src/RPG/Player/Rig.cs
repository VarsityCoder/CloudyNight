using Godot;
namespace CloudyNight.RPG.Player;

public partial class Rig : Node3D {
  private string _runPath = "parameters/MoveSpace/blend_position";
  private AnimationNodeStateMachinePlayback? _playBack;
  private AnimationTree? _animationTree;
  private float _runWeightTarget = -1f;
  [Export] private float _animationSpeed = 8f;

  public override void _Ready() {
    _animationTree = GetNode<AnimationTree>("AnimationTree");
    _playBack =(AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
  }

  public override void _PhysicsProcess(double delta) {
    if (_animationTree != null) {
      _animationTree.Set(_runPath,
        Mathf.MoveToward((float)_animationTree.Get(_runPath), _runWeightTarget, (float)delta * _animationSpeed));
    }
  }

  public void UpdateAnimationTree(Vector3 direction) {
    if (direction.IsZeroApprox()) {
      _runWeightTarget = -1f;
    }
    else {
      _runWeightTarget = 1f;
    }
  }
  public void Travel(string animationName) {
    _playBack.Travel(animationName);
  }

  public bool isIdle() => _playBack.GetCurrentNode() ==  "MoveSpace";

  public bool isSlashing() => _playBack.GetCurrentNode() ==  "attack_Root|Attack";

}
