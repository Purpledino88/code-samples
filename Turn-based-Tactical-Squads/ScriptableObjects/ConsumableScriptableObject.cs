using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableScriptableObject", menuName = "ScriptableObjects/Consumable", order = 0)]
public class ConsumableScriptableObject : ScriptableObject
{
    public string consumable_name;
    public Sprite sprite;
}
