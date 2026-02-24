using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Variable Objects/Animation Curve Variable", fileName = "ACV - ", order = 1)]
public class AnimationCurveVariable : VariableObject<AnimationCurve> { }
[Serializable]
public class AnimationCurveReference : VariableReference<AnimationCurve> { }