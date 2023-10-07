using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform m_FiringPoint;

    private UnitElement m_TargetElement;

    public void SelectTargets(Unit target_unit)
    {
        m_TargetElement = target_unit.GetRandomElement();
    }

    public void SpawnWeaponProjectileHit(Unit target_unit, WeaponScriptableObject weapon, WeaponScriptableObject.WeaponFireMode fire_mode, UnitElement firing_element)
    {
        if (fire_mode.blast_chance_to_hit == 0f)
        {
            if (fire_mode.can_hit_multiple_elements)
                m_TargetElement = target_unit.GetRandomElement();

            if (m_TargetElement == null)
            {// Target is already dead
                SpawnWeaponProjectileMiss(target_unit, weapon, fire_mode, 0);
                return;
            }

            BulletProjectile l_Bullet = Instantiate(fire_mode.prefab, m_FiringPoint.position, Quaternion.identity).GetComponent<BulletProjectile>();
            Vector3 l_GunHeight = new Vector3(0, m_FiringPoint.position.y, 0);
            l_Bullet.Setup(m_TargetElement, firing_element, m_TargetElement.transform.position + l_GunHeight, fire_mode);
        }
        else
        {
            BlastProjectile l_Projectile = Instantiate(fire_mode.prefab, m_FiringPoint.position, Quaternion.identity).GetComponent<BlastProjectile>();
            Vector3 l_GunHeight = new Vector3(0, m_FiringPoint.position.y, 0);
            l_Projectile.Setup(target_unit.GetGridPosition(), fire_mode);
        }
    }

    public void SpawnWeaponProjectileMiss(Unit target_unit, WeaponScriptableObject weapon, WeaponScriptableObject.WeaponFireMode fire_mode, int missed_by_how_much)
    {
        if (fire_mode.blast_chance_to_hit == 0f)
        {
            BulletProjectile l_Bullet = Instantiate(fire_mode.prefab, m_FiringPoint.position, Quaternion.identity).GetComponent<BulletProjectile>();
            Vector3 l_OffsetVector = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), target_unit.GetWorldPosition().y, UnityEngine.Random.Range(-0.5f, 0.5f));
            l_Bullet.Setup(null, null, target_unit.GetWorldPosition() + l_OffsetVector, fire_mode);
        }
        else
        {
            Debug.Log("Roll to scatter");
            BlastProjectile l_Projectile = Instantiate(fire_mode.prefab, m_FiringPoint.position, Quaternion.identity).GetComponent<BlastProjectile>();
            Vector3 l_GunHeight = new Vector3(0, m_FiringPoint.position.y, 0);
            l_Projectile.Setup(target_unit.GetGridPosition(), fire_mode);
        }
    }

    public void FinishFiring()
    {
        m_TargetElement = null;
    }
}
