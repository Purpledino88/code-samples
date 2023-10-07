using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SquadScriptableObject", menuName = "ScriptableObjects/Squad", order = 0)]
public class SquadScriptableObject : ScriptableObject
{
    [System.Serializable]
    public class FireTeam
    {
        public string team_name;
        public int number_of_elements;
        public int number_of_special_weapons;
        public WeaponScriptableObject[] special_weapon_choices;
    }

    public string squad_name;
    public WeaponScriptableObject[] primary_weapon_choices;
    public FireTeam[] fireteams;
    public TroopQualityScriptableObject[] possible_qualities_player;
    public TroopQualityScriptableObject[] possible_qualities_enemy;
}
