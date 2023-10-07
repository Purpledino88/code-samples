using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentScriptableObject", menuName = "ScriptableObjects/Equipment", order = 0)]
public class EquipmentScriptableObject : ScriptableObject 
{
    public string equipment_name;
    public Sprite sprite;
}
