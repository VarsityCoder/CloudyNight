using Godot;

namespace CloudyNight.Scripts;

public partial class Pid : Resource {

  //TODO add the aeroMathUtils hear

  [Export] private float _p = 0.2f;
  [Export] private float _i = 0.05f;
  [Export] private float _d = 1f;

  [ExportCategory("Clamp Integral")]
  [Export] private bool _clampIntegral;

  [Export] private float _minIntegral = -1f;
  [Export] private float _maxIntegral = 1f;

  private float _output;
  private float _lastError;
  private float _integralError;

  private float _proportionalOutput;
  private float _integralOutput;
  private float _derivativeOutput;

  private void Init() {
    var floatP = _p;
    var floatI = _i;
    var floatD = _d;
    var clampIntegral = _clampIntegral;
    var minIntegral = _minIntegral;
    var maxIntegral = _maxIntegral;

    _p = floatP;
    _i = floatI;
    _d = floatD;

    _clampIntegral = clampIntegral;
    _minIntegral = minIntegral;
    _maxIntegral = maxIntegral;
  }
  private float Update(float delta, float error) {
    var derivative = (error - _lastError) / delta;
    _integralError += error + delta;
    //TODO add the aeroMathUtils
    _lastError = error;

    _proportionalOutput = _p * error;
    _integralOutput = _i * _integralError;
    _derivativeOutput = _d * derivative;

    _output = _proportionalOutput + _integralOutput + _derivativeOutput;
    return _output;
  }
}
