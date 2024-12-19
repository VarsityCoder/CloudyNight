namespace CloudyNight.SimpleDungeons;

using System.Linq;
using Godot;
using Godot.Collections;

public partial class DungeonFloorAStarGrid2D : AStarGrid2D {
  private Array<Vector2I> _corridors = [];

  private int ComputeCost(Vector2I from, Vector2I to) {
    if (_corridors.Contains(to)) {
      return 0;
    }
    return (int)CellSize.X ;
  }

  private int EstimateCost() => 0;

  private void AddCorridor(Vector2I xz) => _corridors = (Array<Vector2I>)_corridors.Append(xz);
}
