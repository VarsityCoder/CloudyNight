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
    // need to change this to AeroSurface3d
    return forNode3D is AeroBody3d;
  }
  private void Redraw(EditorNode3DGizmo gizmo) {
    gizmo.Clear();
    var spatial = gizmo.GetNode3D();
    var st = new SurfaceTool();

    // origin
    var halfChord = (float) spatial.Get("wing_config.chord") / 2f;
    var quarterChord = (float)spatial.Get("wing_config.chord") / 4f;
    var halfSpan = (float)spatial.Get("wing_config.span") / 2f;

    st.Begin(Mesh.PrimitiveType.Triangles);
    // flap section
    var tl = new Vector3(-halfSpan, 0, halfChord);
    var tr = new Vector3(halfSpan, 0, halfChord);
    tl.Z += quarterChord;
    tr.Z += quarterChord;
    var bl = new Vector3(-halfSpan, 0, quarterChord);
    var br = new Vector3(halfSpan, 0, quarterChord);

    //first triangle
    st.SetColor(_flapColor);
    st.AddVertex(tl);
    st.AddVertex(tr);
    st.AddVertex(bl);
    //second triangle
    st.AddVertex(bl);
    st.AddVertex(tr);
    st.AddVertex(br);

    //wing section
    var tlwing = new Vector3(-halfSpan, 0, quarterChord);
    var trwing = new Vector3(halfSpan, 0, quarterChord);
    var blwing = new Vector3(-halfSpan, 0, -halfChord + quarterChord);
    var brwing = new Vector3(halfSpan, 0, -halfChord + quarterChord);

    //first triangle
    st.SetColor(_wingColor);
    st.AddVertex(tlwing);
    st.AddVertex(trwing);
    st.AddVertex(blwing);
    //second triangle
    st.AddVertex(blwing);
    st.AddVertex(trwing);
    st.AddVertex(brwing);

    var mesh = st.Commit();
    gizmo.AddMesh(mesh, _wingMaterial);
    gizmo.AddCollisionTriangles(mesh.GenerateTriangleMesh());
  }
}
