using Godot;
namespace CloudyNight.Scripts;

using Godot.Collections;

public partial class AeroUnits : Node {
  [Export] private float _ratioOfSpecificHeat = 1.402f;
  [Export] private Dictionary<int, Dictionary<string, float>> _altitudeValues = new();
  [Export] private float _minAltitude;
  [Export] private float _maxAltitude = 80000.0f;
  private float _integrateForcesTime;
  [Export] private float _minMach;
  [Export] private float _maxMach;
  // Backing Store
  private float _minDensity;
  [Export] private float MinDensity {
    get => _minDensity;
    set {
      _minDensity = value;
      UpdateDensityCurve();
    }
  }
  // Backing Store
  private float _maxDensity = 100f;
  [Export] private float MaxDensity {
    get => _maxDensity;
    set {
      _maxDensity = value;
      UpdateDensityCurve();
    }
  }
  [Export] private Curve? _densityAtAltitudeCurve;
  // Backing Store
  private float _minMachSpeed;
  [Export] private float MinMachSpeed {
    get => _minMachSpeed;
    set {
      _minMachSpeed = value;
      UpdateMachSpeedCurve();
    }
  }
  // Backing Store
  private float _maxMachSpeed = 700f;
  [Export] private float MaxMachSpeed {
    get => _maxMachSpeed;
    set {
      _maxMachSpeed = value;
      UpdateMachSpeedCurve();
    }
  }
  [Export] private Curve? _machAtAltitudeCurve;
  // Backing Store
  private float _minPressure;
  [Export] private float MinPressure {
    get => _minPressure;
    set {
      _minPressure = value;
      UpdatePressureCurve();
    }
  }
  // Backing Store
  private float _maxPressure;
  [Export] private float MaxPressure {
    get => _maxPressure;
    set {
      _maxPressure = value;
      UpdatePressureCurve();
    }
  }

  [Export] private Curve? _pressureAtAltitudeCurve;
  private void UpdateDensityCurve() {
    if (_densityAtAltitudeCurve != null) {
      _densityAtAltitudeCurve.MinValue = MinDensity;
      _densityAtAltitudeCurve.MaxValue = MaxDensity;
      _densityAtAltitudeCurve.BakeResolution = 16;
      _densityAtAltitudeCurve.Bake();
    }
  }
  private float GetDensityAtAltitude(float altitude) {
    var lerp = AltitudeToLerp(altitude);
    if (_densityAtAltitudeCurve != null) {
      return _densityAtAltitudeCurve.SampleBaked(lerp);
    }
    GD.PrintErr("_densityAtAltitudeCurve is null! Returning empty float!");
    return new float();
  }
  private void UpdateMachSpeedCurve() {
    if (_machAtAltitudeCurve != null) {
      _machAtAltitudeCurve.MinValue = MinMachSpeed;
      _machAtAltitudeCurve.MaxValue = MaxMachSpeed;
      _machAtAltitudeCurve.BakeResolution = 16;
      _machAtAltitudeCurve.Bake();
    }
  }
  private float GetMachAtAltitude(float altitude) {
    var lerp = AltitudeToLerp(altitude);
    if (_machAtAltitudeCurve != null) {
      return _machAtAltitudeCurve.SampleBaked(lerp);
    }
    GD.PrintErr("_machAtAltitudeCurve is null! Returning empty float!");
    return new float();
  }
  private void UpdatePressureCurve() {
    if (_pressureAtAltitudeCurve != null) {
      _pressureAtAltitudeCurve.MinValue = _minPressure;
      _pressureAtAltitudeCurve.MaxValue = _maxPressure;
      _pressureAtAltitudeCurve.BakeResolution = 16;
      _pressureAtAltitudeCurve.Bake();
    }
  }
  private float GetPressureAtAltitude(float altitude) {
    var lerp = AltitudeToLerp(altitude);
    if (_pressureAtAltitudeCurve != null) {
      return _pressureAtAltitudeCurve.SampleBaked(lerp);
    }
    GD.PrintErr("_pressureAtAltitudeCurve is null! Returning empty float!");
    return new float();
  }
  private void PopulateDictionary() {
    _altitudeValues.Add(0, new Dictionary<string, float>(){{"temperature", 288.0f}});
    _altitudeValues.Add(0, new Dictionary<string, float>(){{"pressure", 101325.0f}});
    _altitudeValues.Add(0, new Dictionary<string, float>(){{"density", 1.225f}});
    _altitudeValues.Add(0, new Dictionary<string, float>(){{"viscosity", 0.00001812f}});

    _altitudeValues.Add(2000, new Dictionary<string, float>(){{"temperature", 275.0f}});
    _altitudeValues.Add(2000, new Dictionary<string, float>(){{"pressure", 79495.2f}});
    _altitudeValues.Add(2000, new Dictionary<string, float>(){{"density", 1.0065f}});
    _altitudeValues.Add(2000, new Dictionary<string, float>(){{"viscosity", 0.00001746f}});

    _altitudeValues.Add(5000, new Dictionary<string, float>(){{"temperature", 255.0f}});
    _altitudeValues.Add(5000, new Dictionary<string, float>(){{"pressure", 54019.9f}});
    _altitudeValues.Add(5000, new Dictionary<string, float>(){{"density", 0.7361f}});
    _altitudeValues.Add(5000, new Dictionary<string, float>(){{"viscosity", 0.00001645f}});

    _altitudeValues.Add(10000, new Dictionary<string, float>(){{"temperature", 223.0f}});
    _altitudeValues.Add(10000, new Dictionary<string, float>(){{"pressure", 26436.3f}});
    _altitudeValues.Add(10000, new Dictionary<string, float>(){{"density", 0.4127f}});
    _altitudeValues.Add(10000, new Dictionary<string, float>(){{"viscosity", 0.00001645f}});

    _altitudeValues.Add(15000, new Dictionary<string, float>(){{"temperature", 216.0f}});
    _altitudeValues.Add(15000, new Dictionary<string, float>(){{"pressure", 12044.6f}});
    _altitudeValues.Add(15000, new Dictionary<string, float>(){{"density", 0.1937f}});
    _altitudeValues.Add(15000, new Dictionary<string, float>(){{"viscosity", 0.00001432f}});

    _altitudeValues.Add(20000, new Dictionary<string, float>(){{"temperature", 216.0f}});
    _altitudeValues.Add(20000, new Dictionary<string, float>(){{"pressure", 5474.8f}});
    _altitudeValues.Add(20000, new Dictionary<string, float>(){{"density", 0.08803f}});
    _altitudeValues.Add(20000, new Dictionary<string, float>(){{"viscosity", 0.00001645f}});

    _altitudeValues.Add(30000, new Dictionary<string, float>(){{"temperature", 226.0f}});
    _altitudeValues.Add(30000, new Dictionary<string, float>(){{"pressure", 1171.8f}});
    _altitudeValues.Add(30000, new Dictionary<string, float>(){{"density", 0.01803f}});
    _altitudeValues.Add(30000, new Dictionary<string, float>(){{"viscosity", 0.00001432f}});

    _altitudeValues.Add(40000, new Dictionary<string, float>(){{"temperature", 251.0f}});
    _altitudeValues.Add(40000, new Dictionary<string, float>(){{"pressure", 277.5f}});
    _altitudeValues.Add(40000, new Dictionary<string, float>(){{"density", 0.003851f}});
    _altitudeValues.Add(40000, new Dictionary<string, float>(){{"viscosity", 0.00001621f}});

    _altitudeValues.Add(50000, new Dictionary<string, float>(){{"temperature", 270.0f}});
    _altitudeValues.Add(50000, new Dictionary<string, float>(){{"pressure", 75.9f}});
    _altitudeValues.Add(50000, new Dictionary<string, float>(){{"density", 0.0009775f}});
    _altitudeValues.Add(50000, new Dictionary<string, float>(){{"viscosity", 0.00001723f}});

    _altitudeValues.Add(60000, new Dictionary<string, float>(){{"temperature", 245.0f}});
    _altitudeValues.Add(60000, new Dictionary<string, float>(){{"pressure", 20.3f}});
    _altitudeValues.Add(60000, new Dictionary<string, float>(){{"density", 0.0002883f}});
    _altitudeValues.Add(60000, new Dictionary<string, float>(){{"viscosity", 0.00001591f}});

    _altitudeValues.Add(70000, new Dictionary<string, float>(){{"temperature", 217.0f}});
    _altitudeValues.Add(70000, new Dictionary<string, float>(){{"pressure", 4.6f}});
    _altitudeValues.Add(70000, new Dictionary<string, float>(){{"density", 0.00007424f}});
    _altitudeValues.Add(70000, new Dictionary<string, float>(){{"viscosity", 0.000014367f}});

    _altitudeValues.Add(70000, new Dictionary<string, float>(){{"temperature", 217.0f}});
    _altitudeValues.Add(70000, new Dictionary<string, float>(){{"pressure", 4.6f}});
    _altitudeValues.Add(70000, new Dictionary<string, float>(){{"density", 0.00007424f}});
    _altitudeValues.Add(70000, new Dictionary<string, float>(){{"viscosity", 0.000014367f}});

    _altitudeValues.Add(80000, new Dictionary<string, float>(){{"temperature", 196.0f}});
    _altitudeValues.Add(80000, new Dictionary<string, float>(){{"pressure", 0.89f}});
    _altitudeValues.Add(80000, new Dictionary<string, float>(){{"density", 0.0000157f}});
    _altitudeValues.Add(80000, new Dictionary<string, float>(){{"viscosity", 0.000013168f}});
  }
  public override void _Ready() {
    PopulateDictionary();
    _densityAtAltitudeCurve = new Curve();
    _machAtAltitudeCurve = new Curve();
    _pressureAtAltitudeCurve = new Curve();
  }
  private float AltitudeToLerp(float altitude) => Mathf.Remap(altitude, _minAltitude, _maxAltitude, 0f, 1f);

  private float LerpToAltitude(float lerp) => Mathf.Remap(lerp, 0f, 1f, _minAltitude, _maxAltitude);

  private static float RangeToLerp(float value, float min, float max) => Mathf.Remap(value, min, max, 0f, 1f);

  private float LerpToRange(float lerp, float min, float max) => Mathf.Remap(lerp, 0f, 1f, min, max);

  private float GetSpeedOfSoundAtPressureAndDensity(float pressure, float density) => Mathf.Sqrt(_ratioOfSpecificHeat * (pressure / density));

  private float SpeedToMachAtAltitude(float speed, float altitude) => speed / GetMachAtAltitude(altitude);

  private float GetAltitude(Node3D node) {
    while (true) {
      if (node.HasNode("/root/FloatingOriginHelper")) {
        continue;
      }
      return node.GlobalPosition.Y;
    }
  }
}
