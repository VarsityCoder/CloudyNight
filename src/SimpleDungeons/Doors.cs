namespace CloudyNight.SimpleDungeons;
using Godot;
using Godot.Collections;

public partial class Doors : Node {
  public Doors(Vector3 localPos, DungeonUtils.Direction direction, bool optional, DungeonRoom3D room, Node3D doorNode) {
    LocalPos = (Vector3I)localPos.Round();
    _direction = direction;
    _optional = optional;
    _room = room;
    _doorNode = doorNode;
  }

  private DungeonRoom3D _room;
  public Vector3I LocalPos;

  // Backing Store
  private Vector3I _gridPos;

  public Vector3I GridPos {
    get => _room.LocalGridPosToDungeonGridPos(LocalPos);
    set => _gridPos = value;
  }

  // Backing Store
  private Vector3I _exitPosLocal;

  public Vector3I ExitPosLocal {
    // TODO change this for the dungeon utils
    get => LocalPos + new Vector3I(1, 1, 1);
    set => _exitPosLocal = value;
  }

  // Backing Store
  private Vector3I _exitPosGrid;

  public Vector3I ExitPosGrid {
    get => _room.LocalGridPosToDungeonGridPos(ExitPosLocal);
    set => _exitPosGrid = value;
  }

  private DungeonUtils.Direction _direction;
  private bool _optional;
  private Node3D _doorNode;

  private bool FitsOtherDoor(Doors otherRoomDoor) => otherRoomDoor.ExitPosGrid == GridPos && otherRoomDoor.GridPos == ExitPosGrid;

  private Array FindDuplicates() {
    return new Array();
  }

  private bool ValidateDoor() {
    return false;
  }

  private DungeonRoom3D GetRoomLeadsTo() {
    return new DungeonRoom3D();
  }
}
