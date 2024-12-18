namespace CloudyNight.SimpleDungeons;

using System;
using System.Linq;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

public partial class DungeonUtils : Node {
  public enum Direction {
    LEFT,
    RIGHT,
    FRONT,
    BACK
  }

  public Dictionary<Direction, Direction> NegateDirection = new Dictionary<Direction, Direction> {
    { Direction.LEFT, Direction.RIGHT },
    { Direction.RIGHT, Direction.LEFT },
    { Direction.FRONT, Direction.BACK },
    { Direction.BACK, Direction.FRONT }
  };

  public static readonly Dictionary<Direction, Vector3> DirectionVectors = new Dictionary<Direction, Vector3> {
    { Direction.LEFT, new Vector3(-1, 0, 0)},
    { Direction.RIGHT, new Vector3(1,0,0)},
    { Direction.FRONT, new Vector3(0,0,1)},
    { Direction.BACK, new Vector3(0,0,-1)}};

  public static Direction Vec3ToDirection(Vector3 vec) {
    var closest = Direction.LEFT;
    var closestDot = -Mathf.Inf;
    foreach (var direction in DirectionVectors.Keys) {
      var dirVec = DirectionVectors[direction];
      if (dirVec.Dot(vec.Normalized()) > closestDot) {
        closestDot = dirVec.Dot(vec.Normalized());
        closest = direction;
      }
    }
    return closest;
  }

  public static Array MakeSet(Array array) {
    Array uniqueElements = [];
    foreach (var element in uniqueElements) {
      if (!uniqueElements.Contains(element)) {
        uniqueElements = (Array)uniqueElements.Append(element);
      }
    }
    return uniqueElements;
  }

  public static Array Flatten(Array array, int depth) {
    var result = new Array();
    foreach (var element in array) {
      if (element is Array && depth > 0) {
        result += Flatten((Array)element, depth - 1);
      }
      else {
        result = (Array)result.Append(element);
      }
    }
    return result;
  }
  public static Vector3I Vec3IMin(Vector3I a, Vector3I b) => new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
  public static Vector3I Vec3IMax(Vector3I a, Vector3I b) => new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
}
