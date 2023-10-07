using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TroopQualityScriptableObject", menuName = "ScriptableObjects/Troop Quality", order = 0)]
public class TroopQualityScriptableObject : ScriptableObject 
{
    public int marksmanship;
    public int discipline;
    public int agility;
    public int defence;    
}