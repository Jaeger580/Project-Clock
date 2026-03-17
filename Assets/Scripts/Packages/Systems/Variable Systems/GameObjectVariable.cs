using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Variable Objects/Game Object Variable", fileName = "GOV - ", order = 1)]
public class GameObjectVariable : VariableObject<GameObject> { }

[Serializable]
public class GameObjectReference : VariableReference<GameObject> { }
