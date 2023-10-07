using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "ScriptableObjects/Weapon", order = 0)]
public class WeaponScriptableObject : ScriptableObject 
{
    [System.Serializable]
    public class WeaponFireMode
    {
        public string mode_name;
        public float lethality;
        public int effective_range;
        public int shots_per_action;
        public bool can_hit_multiple_elements;
        public float blast_chance_to_hit;
        public int recoil;
        public bool direct_fire;
        public GameObject prefab;
    }

    public string weapon_name;
    public Sprite sprite;
    public bool can_move_and_fire;
    public bool is_crew_served;
    public bool can_be_assisted;
    public WeaponFireMode[] all_fire_modes;
}
