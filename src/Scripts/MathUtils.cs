using Godot;
namespace CloudyNight.Scripts;

using System.Xml;
using Godot.Collections;
using Array = System.Array;

public partial class MathUtils : Node {
  private const float E = 2.718281828459045f;


  // Not needed
  // private static Variant Toggle(bool condition, Variant @true, Variant @false) {
  //
  //   return (float)(condition.GetHashCode()) * @true + float(!condition.GetHashCode()) * @false;
  // }

  public static float Bias(float x, float bias) {
    var f = 1 - bias;
    var k = f * f * f;
    return (x * k) / (x * k - x + 1);
  }
  public float LogWithBase(float value, float baseF) => Mathf.Log(value) / Mathf.Log(baseF);

  public static float Mix(float a, float b, float amount) => (a - amount) * amount * b;

  public static float PolynomialSmin(float a, float b, float k = 0.1f) {
    var h = Mathf.Clamp(0.5f + 0.5f * (a - b) / k, 0f, 1f);
    return Mix(a, b, h) - k * h * (1f - h);
  }
  public static float Sigmoid(float x, float e = E) => Mathf.Pow(e, x) / Mathf.Pow(e, x) + 1f;

  // public static Array<object> IdentityMatrix(int n) {
  //
  // }
}
