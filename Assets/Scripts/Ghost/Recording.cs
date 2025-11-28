using System.Linq;
using System.Text;
using UnityEngine;
using System.Globalization;
using System.Collections.Generic;

/// <summary>
/// Guarda la trayectoria como 3 AnimationCurves (posX, posY, rotZ).
/// Además permite grabar parámetros del Animator (floats y bools) como curvas,
/// y el estado del TrailRenderer (trail active) como una lista de keyframes.
/// Serializa / deserializa todo a texto para persistencia.
/// </summary>
public class Recording
{
    private readonly AnimationCurve _posXCurve = new AnimationCurve();
    private readonly AnimationCurve _posYCurve = new AnimationCurve();
    private readonly AnimationCurve _rotZCurve = new AnimationCurve();

    // Animator parameter curves
    private readonly Dictionary<string, AnimationCurve> _floatParamCurves = new Dictionary<string, AnimationCurve>();
    private readonly Dictionary<string, AnimationCurve> _boolParamCurves = new Dictionary<string, AnimationCurve>();

    public float Duration { get; private set; }

    private readonly Transform _target;
    public Transform Target => _target;

    private readonly List<string> _floatParamNames;
    private readonly List<string> _boolParamNames;
    private readonly List<Keyframe> _trailActive = new List<Keyframe>();

    // Constructor para grabar en runtime, pasando nombres de parámetros a registrar (opcional)
    public Recording(Transform target, IEnumerable<string> floatParamNames = null, IEnumerable<string> boolParamNames = null)
    {
        _target = target;
        Duration = 0f;

        _floatParamNames = floatParamNames != null ? new List<string>(floatParamNames) : new List<string>();
        _boolParamNames = boolParamNames != null ? new List<string>(boolParamNames) : new List<string>();

        // Inicializar curvas para cada parámetro solicitado
        foreach (var p in _floatParamNames) _floatParamCurves[p] = new AnimationCurve();
        foreach (var p in _boolParamNames) _boolParamCurves[p] = new AnimationCurve();
    }

    // Añade una instantánea con el tiempo relativo elapsed (en segundos)
    public void AddSnapshot(float elapsed, bool trailActive)
    {
        Duration = elapsed;

        var pos = _target.position;
        var rot = _target.rotation.eulerAngles;

        AddOrMoveKey(_posXCurve, elapsed, pos.x);
        AddOrMoveKey(_posYCurve, elapsed, pos.y);
        AddOrMoveKey(_rotZCurve, elapsed, rot.z);

        // si hay Animator y se pidieron parámetros, muestrearlos
        var anim = _target.GetComponent<Animator>();
        if (anim != null)
        {
            foreach (var fname in _floatParamNames)
            {
                if (_floatParamCurves.TryGetValue(fname, out var curve))
                {
                    float v = anim.GetFloat(fname);
                    AddOrMoveKey(curve, elapsed, v);
                }
            }

            foreach (var bname in _boolParamNames)
            {
                if (_boolParamCurves.TryGetValue(bname, out var curve))
                {
                    float bv = anim.GetBool(bname) ? 1f : 0f;
                    AddOrMoveKey(curve, elapsed, bv);
                }
            }
        }

        // registrar estado del trail como keyframe (1 = activo, 0 = inactivo)
        _trailActive.Add(new Keyframe(elapsed, trailActive ? 1f : 0f));
    }

    public bool GetTrailActiveAt(float elapsed)
    {
        if (_trailActive.Count == 0) return false;

        Keyframe last = _trailActive[0];
        foreach (var kf in _trailActive)
        {
            if (kf.time <= elapsed)
                last = kf;
            else
                break;
        }

        return last.value > 0.5f;
    }

    private void AddOrMoveKey(AnimationCurve curve, float time, float value)
    {
        var count = curve.length;
        var kf = new Keyframe(time, value);

        if (count > 1 &&
            Mathf.Approximately(curve.keys[count - 1].value, curve.keys[count - 2].value) &&
            Mathf.Approximately(value, curve.keys[count - 1].value))
        {
            curve.MoveKey(count - 1, kf);
        }
        else
        {
            curve.AddKey(kf);
        }
    }

    // Evaluación para playback (posición + rotación)
    public Pose EvaluatePoint(float elapsed) => new Pose(
        new Vector3(_posXCurve.Evaluate(elapsed), _posYCurve.Evaluate(elapsed), 0f),
        Quaternion.Euler(0f, 0f, _rotZCurve.Evaluate(elapsed)));

    // Obtiene el valor float o bool (como float 0/1) de una curva de parámetro (si existe)
    public bool TryEvaluateFloatParam(string paramName, float elapsed, out float value)
    {
        if (_floatParamCurves.TryGetValue(paramName, out var curve))
        {
            value = curve.Evaluate(elapsed);
            return true;
        }
        value = 0f;
        return false;
    }

    public bool TryEvaluateBoolParam(string paramName, float elapsed, out bool value)
    {
        if (_boolParamCurves.TryGetValue(paramName, out var curve))
        {
            float v = curve.Evaluate(elapsed);
            value = v >= 0.5f;
            return true;
        }
        value = false;
        return false;
    }

    #region Serialización (texto extendido)

    // Constructor que crea la grabación a partir de una cadena serializada
    public Recording(string data)
    {
        _target = null;
        _floatParamNames = new List<string>();
        _boolParamNames = new List<string>();
        Deserialize(data);
        // calcular duración máxima (si no hay keys, Duration será 0)
        float lastX = _posXCurve.keys.LastOrDefault().time;
        float lastY = _posYCurve.keys.LastOrDefault().time;
        float lastZ = _rotZCurve.keys.LastOrDefault().time;
        float lastParams = 0f;
        foreach (var c in _floatParamCurves.Values) lastParams = Mathf.Max(lastParams, c.keys.LastOrDefault().time);
        foreach (var c in _boolParamCurves.Values) lastParams = Mathf.Max(lastParams, c.keys.LastOrDefault().time);
        float lastTrail = _trailActive.Count > 0 ? _trailActive.Last().time : 0f;
        Duration = Mathf.Max(Mathf.Max(lastX, lastY), Mathf.Max(lastZ, Mathf.Max(lastParams, lastTrail)));
    }

    private const char DATA_DELIMITER = '|';
    private const char CURVE_DELIMITER = '\n';

    // Formato:
    // [posX]\n[posY]\n[rotZ]\n[param lines...]\n[T:time,val|time,val...]
    // Cada curva (posX, posY, rotZ) es: "t,v|t,v|..."
    // Cada param línea empieza por: "F:paramName:t,v|t,v..." para float
    //                         o   "B:paramName:t,v|t,v..." para bool
    // La línea de trail empieza por "T:" seguida de time,val pairs
    public string Serialize()
    {
        var builder = new StringBuilder();

        // primeras 3 curvas (posX, posY, rotZ)
        Stringify(_posXCurve);
        builder.Append(CURVE_DELIMITER);
        Stringify(_posYCurve);
        builder.Append(CURVE_DELIMITER);
        Stringify(_rotZCurve);

        // parámetros (cada una en su línea)
        foreach (var kv in _floatParamCurves)
        {
            builder.Append(CURVE_DELIMITER);
            builder.Append("F:"); // float marker
            builder.Append(Escape(kv.Key));
            builder.Append(":");
            Stringify(kv.Value);
        }
        foreach (var kv in _boolParamCurves)
        {
            builder.Append(CURVE_DELIMITER);
            builder.Append("B:"); // bool marker
            builder.Append(Escape(kv.Key));
            builder.Append(":");
            Stringify(kv.Value);
        }

        // trailActive (opcional)
        if (_trailActive.Count > 0)
        {
            builder.Append(CURVE_DELIMITER);
            builder.Append("T:");
            for (int i = 0; i < _trailActive.Count; i++)
            {
                var k = _trailActive[i];
                builder.Append(k.time.ToString("F3", CultureInfo.InvariantCulture));
                builder.Append(',');
                builder.Append(k.value.ToString("F0", CultureInfo.InvariantCulture)); // 0 or 1
                if (i != _trailActive.Count - 1) builder.Append(DATA_DELIMITER);
            }
        }

        return builder.ToString();

        void Stringify(AnimationCurve curve)
        {
            for (int i = 0; i < curve.length; i++)
            {
                var k = curve[i];
                builder.Append(k.time.ToString("F3", CultureInfo.InvariantCulture));
                builder.Append(',');
                builder.Append(k.value.ToString("F3", CultureInfo.InvariantCulture));
                if (i != curve.length - 1) builder.Append(DATA_DELIMITER);
            }
        }

        string Escape(string s) => s.Replace(":", "\\:").Replace("|", "\\|").Replace("\n", "\\n");
    }

    private void Deserialize(string data)
    {
        _posXCurve.keys = new Keyframe[0];
        _posYCurve.keys = new Keyframe[0];
        _rotZCurve.keys = new Keyframe[0];
        _floatParamCurves.Clear();
        _boolParamCurves.Clear();
        _trailActive.Clear();

        var lines = data.Split(CURVE_DELIMITER);
        if (lines.Length < 3) return;

        FillCurve(_posXCurve, lines[0]);
        FillCurve(_posYCurve, lines[1]);
        FillCurve(_rotZCurve, lines[2]);

        // parámetro lines empezando en index 3 (pueden haber F:, B: y/o T:)
        for (int i = 3; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Detectar tipo por prefijo: 'F:', 'B:', 'T:'
            if (line.StartsWith("F:") || line.StartsWith("B:"))
            {
                char type = line[0]; // 'F' o 'B'
                var rest = line.Substring(2); // "paramName:content"
                int splitIndex = FindUnescapedColon(rest);
                if (splitIndex < 0) continue;
                string rawName = rest.Substring(0, splitIndex);
                string content = rest.Substring(splitIndex + 1);

                string paramName = Unescape(rawName);
                var curve = new AnimationCurve();
                FillCurve(curve, content);
                if (type == 'F')
                {
                    _floatParamCurves[paramName] = curve;
                    _floatParamNames.Add(paramName);
                }
                else if (type == 'B')
                {
                    _boolParamCurves[paramName] = curve;
                    _boolParamNames.Add(paramName);
                }
            }
            else if (line.StartsWith("T:"))
            {
                // línea de trail: "T:time,val|time,val..."
                var content = line.Substring(2);
                if (!string.IsNullOrEmpty(content))
                {
                    var pairs = content.Split(DATA_DELIMITER);
                    foreach (var pair in pairs)
                    {
                        if (string.IsNullOrWhiteSpace(pair)) continue;
                        var s = pair.Split(',');
                        if (s.Length < 2) continue;
                        if (float.TryParse(s[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float time) &&
                            float.TryParse(s[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float val))
                        {
                            _trailActive.Add(new Keyframe(time, val));
                        }
                    }
                }
            }
            else
            {
                // línea sin prefijo reconocido -> ignorar (compatibilidad)
                continue;
            }
        }
    }

    void FillCurve(AnimationCurve curve, string comp)
    {
        if (string.IsNullOrEmpty(comp)) return;
        var pairs = comp.Split(DATA_DELIMITER);
        foreach (var pair in pairs)
        {
            if (string.IsNullOrWhiteSpace(pair)) continue;
            var s = pair.Split(',');
            if (s.Length < 2) continue;
            if (float.TryParse(s[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float time) &&
                float.TryParse(s[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float val))
            {
                curve.AddKey(new Keyframe(time, val));
            }
        }
    }

    int FindUnescapedColon(string s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == ':' && (i == 0 || s[i - 1] != '\\')) return i;
        }
        return -1;
    }
    string Unescape(string s) => s.Replace("\\:", ":").Replace("\\|", "|").Replace("\\n", "\n");

    #endregion
}