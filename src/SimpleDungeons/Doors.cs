namespace CloudyNight.SimpleDungeons;
using Godot;


public partial class Doors : Node {

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
  }

  public void Init() {

  }

}
