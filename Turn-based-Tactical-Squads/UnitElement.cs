using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitElement : MonoBehaviour
{
    private const float BLEEDOUT_PROBABILITY = 0.33f;

    [SerializeField] private Animator m_ElementAnimator;
    [SerializeField] private GameObject m_RagdollPrefab;
    [SerializeField] private GameObject m_RootBone;
    [SerializeField] private Transform m_WeaponHoldPoint;
    [SerializeField] private ArmouryScriptableObject m_Armoury;

    public event EventHandler OnElementStatusChange;
    public event EventHandler OnElementDestruction;

    private string m_Name;
    public string GetName() { return m_Name; }

    private int m_Health = 4; //Healthy, Walking Wounded, Badly Wounded, Unconcious, Dead
    public int GetHealth() { return m_Health; }

    private int m_Marksmanship;

    [SerializeField] private List<WeaponScriptableObject> m_Weapons;
    public List<WeaponScriptableObject> GetAllWeapons() { return m_Weapons; }

    private WeaponScriptableObject m_SelectedWeapon;
    public WeaponScriptableObject GetSelectedWeapon() { return m_SelectedWeapon; }
    private WeaponScriptableObject.WeaponFireMode m_SelectedFireMode;
    public WeaponScriptableObject.WeaponFireMode GetSelectedFireMode() { return m_SelectedFireMode; }

    private Dictionary<WeaponScriptableObject, Weapon> m_WeaponObjects;

    private Unit m_ParentUnit;

    private bool m_IsStunned;
    private bool m_IsStunnedNextTurn;
    public bool IsStunned() { return m_IsStunned; }

    private bool m_IsBleeding;
    public bool IsBleeding() { return m_IsBleeding; }

    private List<ConsumableScriptableObject> m_ConsumableList;

    public void Setup(Unit parent_unit, int marksmanship, WeaponScriptableObject weapon_override)
    {
        m_ParentUnit = parent_unit;
        m_Marksmanship = marksmanship;
        m_Name = NameGenerator.GetInstance().GetRandomName();
        m_IsStunned = false;
        m_IsStunnedNextTurn = false;
        m_IsBleeding = false;

        if (weapon_override != null)
        {
            m_Weapons.Clear();
            m_Weapons.Add(weapon_override);
        }
        m_SelectedWeapon = m_Weapons[0];
        m_SelectedFireMode = m_SelectedWeapon.all_fire_modes[0];

        m_WeaponObjects = new Dictionary<WeaponScriptableObject, Weapon>();
        foreach (WeaponScriptableObject weapon in m_Weapons)
        {   
            GameObject l_WeaponObject = Instantiate(m_Armoury.GetWeapon(weapon), m_WeaponHoldPoint);
            Weapon l_Weapon = l_WeaponObject.GetComponent<Weapon>();
            m_WeaponObjects.Add(weapon, l_Weapon);
            if (weapon != m_SelectedWeapon)
                l_WeaponObject.SetActive(false);
        }
    }

    public void SetWeaponAndFiringMode(WeaponScriptableObject weapon, WeaponScriptableObject.WeaponFireMode firing_mode)
    {
        if (weapon != m_SelectedWeapon)
        {
            m_WeaponObjects[m_SelectedWeapon].gameObject.SetActive(false);
            m_WeaponObjects[weapon].gameObject.SetActive(true);
        }
        m_SelectedWeapon = weapon;
        m_SelectedFireMode = firing_mode;
    }

    public void TriggerAnimation(string animation)
    {
        m_ElementAnimator.SetTrigger(animation);
    }

    public void SetAnimationBool(string animation_name, bool value)
    {
        m_ElementAnimator.SetBool(animation_name, value);
    }

    public void FireWeapon(Unit target_unit, int shots_this_action, float cover_modifier)
    {
        TriggerAnimation("FireRifle");
        RollToHit(target_unit, shots_this_action, cover_modifier);
    }
    
    public int CalculateHitChance(Unit target_unit, WeaponScriptableObject weapon, WeaponScriptableObject.WeaponFireMode fire_mode, float cover_modifier)
    {
        int base_hit_chance = 25 + (m_Marksmanship * 10);

        int cover_penalty = Mathf.RoundToInt(cover_modifier * 100);

        int enemy_defence_penalty = target_unit.GetDefence() * 5;

        int range_penalty = CalculateRangeModifier(weapon, fire_mode, Vector3.Distance(m_ParentUnit.GetWorldPosition(), target_unit.GetWorldPosition()));

        int injury_penalty = (4 - m_Health) * 33;

        int all_penalties = cover_penalty + enemy_defence_penalty + range_penalty + injury_penalty;

        float enemy_numbers_multiplier = 0.6f + (target_unit.GetSurvivingElements().Count * 0.1f);

        int total_hit_chance = Mathf.RoundToInt((base_hit_chance - all_penalties) * enemy_numbers_multiplier);

        return Mathf.Clamp(total_hit_chance, 0, 100);
    }

    private int CalculateRangeModifier(WeaponScriptableObject weapon, WeaponScriptableObject.WeaponFireMode fire_mode, float distance)
    {
        float l_minValue = fire_mode.effective_range * 0.5f;
        float l_MaxValue = fire_mode.effective_range * 1.5f;

        if (!fire_mode.direct_fire)
            l_minValue = 0f;

        float l_NormalisedValue = Mathf.Max(distance - l_minValue, 0f) / (l_MaxValue - l_minValue);

        return Mathf.RoundToInt(100 * l_NormalisedValue);
    }

    private void RollToHit(Unit target_unit, int shots_this_action, float cover_modifier)
    {
        int l_HitPercentage = CalculateHitChance(target_unit, m_SelectedWeapon, m_SelectedFireMode, cover_modifier) - (m_SelectedFireMode.recoil * shots_this_action);
        //Debug.Log($"Firing shot {shots_this_action + 1} with {l_HitPercentage}% chance to hit");
        int l_Rolled = (UnityEngine.Random.Range(0, 100) + 1);
        if ((l_Rolled > l_HitPercentage) || (target_unit.GetSurvivingElements().Count == 0))
            m_WeaponObjects[m_SelectedWeapon].SpawnWeaponProjectileMiss(target_unit, m_SelectedWeapon, m_SelectedFireMode, l_Rolled - l_HitPercentage);
        else
            m_WeaponObjects[m_SelectedWeapon].SpawnWeaponProjectileHit(target_unit, m_SelectedWeapon, m_SelectedFireMode, this);
    }

    public void SelectTargets(Unit target_unit)
    {
        m_WeaponObjects[m_SelectedWeapon].SelectTargets(target_unit);
    }

    public void FinishFiring()
    {
        m_WeaponObjects[m_SelectedWeapon].FinishFiring();
    }

    public void ResolveBulletHit(UnitElement firer, WeaponScriptableObject.WeaponFireMode fire_mode_used)
    {
        RollForLethality(fire_mode_used);
        
        if (m_Health == 0)
            Die(firer.transform.position, fire_mode_used);
    }

    public void ResolveBlastHit(WeaponScriptableObject.WeaponFireMode fire_mode_used)
    {
        RollForLethality(fire_mode_used);

        if (m_Health == 0)
            Die(m_ParentUnit.GetWorldPosition(), fire_mode_used);
    }

    private void RollForLethality(WeaponScriptableObject.WeaponFireMode fire_mode_used)
    {
        if (UnityEngine.Random.Range(0f, 1f) < fire_mode_used.lethality)
        {
            m_Health = 0;
        }
        else
        {
            if (UnityEngine.Random.Range(0f, 1f) < fire_mode_used.lethality)
            {
                m_IsStunned = true;
                m_IsStunnedNextTurn = true;
            }

            int current_injuries = 4 - m_Health;
            int random_injury_level = UnityEngine.Random.Range(1, 5);
            if (random_injury_level >= m_Health) //if random injury is better than current health
            {
                m_IsBleeding = true;
            }

            m_Health = Mathf.Max(0, random_injury_level - current_injuries);
        }

        if (m_Health != 0)
            OnElementStatusChange?.Invoke(this, EventArgs.Empty);
    }

    private void Die(Vector3 death_from_where, WeaponScriptableObject.WeaponFireMode fire_mode_used)
    {
        GameObject l_RagdollGameObject = Instantiate(m_RagdollPrefab, transform.position, transform.rotation);
        UnitElementRagdoll l_Ragdoll = l_RagdollGameObject.GetComponent<UnitElementRagdoll>();

        m_IsStunned = false;
        m_IsStunnedNextTurn = false;
        m_IsBleeding = false;
        
        if (fire_mode_used != null)
        {
            if (fire_mode_used.blast_chance_to_hit > 0f)
                l_Ragdoll.SetupDeathByExplosion(m_RootBone, death_from_where, fire_mode_used);
            else
                l_Ragdoll.SetupDeathByBullet(m_RootBone, death_from_where, fire_mode_used);
        }
        else
        {
            l_Ragdoll.SetupDeathByBleedout(m_RootBone);
        }

        OnElementDestruction?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
    }

    public bool IsAbleToAct()
    {
        return (m_Health > 1);
    }

    public void OnNewTurn()
    {
        if (!m_IsStunnedNextTurn)
            m_IsStunned = false;

        m_IsStunnedNextTurn = false;

        if (m_IsBleeding && (UnityEngine.Random.Range(0f, 1f) < BLEEDOUT_PROBABILITY))
        {
            m_Health--;
            if (m_Health == 0)
                Die(transform.position, null);
        }
    }
}
