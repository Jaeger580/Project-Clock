using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Variable Objects/Vector 2 Variable", fileName = "V2V - ", order = 0)]
public class Vector2Variable : VariableObject<Vector2> { }
[Serializable]
public class Vector2Reference : VariableReference<Vector2> { }