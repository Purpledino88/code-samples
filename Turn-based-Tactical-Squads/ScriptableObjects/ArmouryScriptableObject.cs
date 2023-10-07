using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArmouryScriptableObject", menuName = "ScriptableObjects/Armoury", order = 0)]
public class ArmouryScriptableObject : ScriptableObject 
{
    [System.Serializable]
    public struct WeaponLookup
    {
        public WeaponScriptableObject key_weapon;
        public GameObject value_prefab;
    }

    public WeaponLookup[] m_Weapons;

    public GameObject GetWeapon(WeaponScriptableObject weapon)
    {
        foreach (WeaponLookup pair in m_Weapons)
        {
            if (pair.key_weapon == weapon)
                return pair.value_prefab;
        }
        return null;
    }
}
