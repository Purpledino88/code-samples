using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitElementRagdoll : MonoBehaviour
{
    private const float BULLET_FORCE_MULTIPLIER = 50f;
    private const float EXPLOSIVE_FORCE_MULTIPLIER = 400f;
    private const float EXPLOSIVE_RANGE_MULTIPLIER = 50f;

    [SerializeField] private GameObject m_RootBone;
    [SerializeField] private Rigidbody m_CentralMassRigidBody;

    private void MatchChildTransforms(Transform original, Transform clone)
    {
        foreach (Transform original_child in original)
        {
            Transform clone_child = clone.Find(original_child.name);
            if (clone_child != null)
            {
                clone_child.position = original_child.position;
                clone_child.rotation = original_child.rotation;
                MatchChildTransforms(original_child, clone_child);
            }
        }
    }

    private Vector3 GetRandomSkew()
    {
        return new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f));
    }

    public void SetupDeathByBleedout(GameObject original_root_bone)
    {
        MatchChildTransforms(original_root_bone.transform, m_RootBone.transform);
    }

    public void SetupDeathByBullet(GameObject original_root_bone, Vector3 firer_position, WeaponScriptableObject.WeaponFireMode weapon_fire_mode)
    {
        MatchChildTransforms(original_root_bone.transform, m_RootBone.transform);

        Vector3 direction = (GetRandomSkew() + (m_CentralMassRigidBody.transform.position - firer_position)).normalized;
        m_CentralMassRigidBody.AddForce(direction * weapon_fire_mode.lethality * BULLET_FORCE_MULTIPLIER, ForceMode.Impulse);
    }

    public void SetupDeathByExplosion(GameObject original_root_bone, Vector3 explosion_position, WeaponScriptableObject.WeaponFireMode weapon_fire_mode)
    {
        MatchChildTransforms(original_root_bone.transform, m_RootBone.transform);

        Vector3 l_SkewedPosition = explosion_position + GetRandomSkew();
        float l_ExplosionForce = weapon_fire_mode.lethality * EXPLOSIVE_FORCE_MULTIPLIER;
        float l_ExplosionRange = weapon_fire_mode.blast_chance_to_hit * EXPLOSIVE_RANGE_MULTIPLIER;

        RecurseExplosionThroughRagdoll(m_RootBone.transform, l_SkewedPosition, l_ExplosionForce, l_ExplosionRange);
    }

    private void RecurseExplosionThroughRagdoll(Transform root_bone, Vector3 explosion_position, float explosion_force, float explosion_range)
    {
        foreach (Transform child_bone in root_bone)
        {
            if (child_bone.TryGetComponent<Rigidbody>(out Rigidbody child_rigidbody))
                child_rigidbody.AddExplosionForce(explosion_force, explosion_position, explosion_range);

            RecurseExplosionThroughRagdoll(child_bone, explosion_position, explosion_force, explosion_range);
        }
    }
}
