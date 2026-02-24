using System;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Variable Objects/Float Variable", fileName = "FV - ", order = 0)]
public class FloatVariable : VariableObject<float> { }
[Serializable]
public class FloatReference : VariableReference<float> { }