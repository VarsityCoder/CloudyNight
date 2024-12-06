using Godot;
namespace CloudyNight.Scripts.Utils;

public partial class TransformUtils : Node
{
  public static Quaternion QuatToAxisAngle(Quaternion quaternion) {
    var axisAngle = new Quaternion(0f,0f,0f,0f);
    if (quaternion.W > 1) {
      quaternion = quaternion.Normalized();
    }

    var angle = 2f * Mathf.Acos(quaternion.W);
    axisAngle.W = Mathf.Sqrt(1f - quaternion.W * quaternion.W);

    if (axisAngle.W < 0.00001f) {
      axisAngle.X = quaternion.X;
      axisAngle.Y = quaternion.Y;
      axisAngle.Z = quaternion.Z;
    }
    else {
      axisAngle.X = quaternion.X / axisAngle.W;
      axisAngle.Y = quaternion.Y / axisAngle.W;
      axisAngle.Z = quaternion.Z / axisAngle.W;
    }

    return axisAngle;
  }
}
