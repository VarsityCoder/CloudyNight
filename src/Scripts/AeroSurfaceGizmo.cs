using Godot;
namespace CloudyNight.Scripts;
public partial class AeroSurfaceGizmo : EditorNode3DGizmoPlugin {
  private static readonly float _wingOpacity = 0.2f;
  private StandardMaterial3D _wingMaterial = new();
  private Color _wingColor = new Color(1,1,1, _wingOpacity);
  private Color _flapColor = new Color(1, 1, 0, _wingOpacity);

  private void Init() {
    _wingMaterial.SetShadingMode(BaseMaterial3D.ShadingModeEnum.Unshaded);
    _wingMaterial.SetTransparency(BaseMaterial3D.TransparencyEnum.Disabled);
    _wingMaterial.SetCullMode(BaseMaterial3D.CullModeEnum.Disabled);
    _wingMaterial.VertexColorUseAsAlbedo = true;
    _wingMaterial.SetFlag(BaseMaterial3D.Flags.DisableDepthTest, true);
  }

  private string GetGizmoName() => "AeroSurfaceGizmo";

  private bool HasGizmo(Node3D forNode3D) {
    //TODO need to change this to AeroSurface3d
    return forNode3D is AeroBody3d;
  }
  private void Redraw(EditorNode3DGizmo gizmo) {
    gizmo.Clear();
    var spatial = gizmo.GetNode3D();
    var st = new SurfaceTool();

    var halfChord = (float) spatial.Get("wing_config.chord") / 2f;
  }
}
