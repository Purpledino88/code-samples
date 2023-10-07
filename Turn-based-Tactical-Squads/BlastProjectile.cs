using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastProjectile : MonoBehaviour
{
    [SerializeField] private TrailRenderer m_Trail;
    [SerializeField] private GameObject m_ExplosionParticleEffect;
    [SerializeField] private float m_ProjectileSpeed;
    [SerializeField] private AnimationCurve m_ArcYCurve;

    private GridPosition m_TargetGridPosition;
    private Vector3 m_TargetWorldPosition;

    private WeaponScriptableObject.WeaponFireMode m_FireModeUsed;

    private float m_TotalFlatDistance;
    private float m_DirectFireYModifier;
    private float m_MaxArcHeight;
    private float m_StartingYHeight;

    private Vector3 m_TargetFlatPosition;
    private Vector3 m_StartingFlatPosition;

    public void Setup(GridPosition target_grid_position, WeaponScriptableObject.WeaponFireMode fire_mode)
    {
        m_TargetGridPosition = target_grid_position;
        m_TargetWorldPosition = LevelGrid.Instance.GetWorldPosition(m_TargetGridPosition);
        m_FireModeUsed = fire_mode;
        
        m_StartingYHeight = transform.position.y;
        m_DirectFireYModifier = (m_TargetWorldPosition - transform.position).normalized.y;
        m_TargetFlatPosition = Flatten(m_TargetWorldPosition);
        m_StartingFlatPosition = Flatten(transform.position);
        m_TotalFlatDistance = Vector3.Distance(m_StartingFlatPosition, m_TargetFlatPosition);
        float l_PercentageOfMaxRange = m_TotalFlatDistance / (fire_mode.effective_range * 1.5f);
        Debug.Log(l_PercentageOfMaxRange + " " + (fire_mode.effective_range * 0.75f));
        m_MaxArcHeight = l_PercentageOfMaxRange * (fire_mode.effective_range * 0.75f);
    }

    private void Update()
    {
        Vector3 l_Direction = (m_TargetWorldPosition - transform.position).normalized;
        transform.position += (l_Direction * m_ProjectileSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, GetArcedYValue(), transform.position.z);

        //Debug.Log(Vector3.Distance(Flatten(transform.position), m_StartingFlatPosition) + " < " + m_TotalFlatDistance);
        if (Vector3.Distance(Flatten(transform.position), m_StartingFlatPosition) > m_TotalFlatDistance)
        {
            transform.position = m_TargetWorldPosition;
            Instantiate(m_ExplosionParticleEffect, m_TargetWorldPosition, Quaternion.identity);

            foreach (Unit unit in LevelGrid.Instance.GetUnitListAtGridPosition(m_TargetGridPosition))
                unit.ResolveExplosion(m_FireModeUsed);

            m_Trail.transform.parent = null;
            Destroy(gameObject);
        }
    }

    private Vector3 Flatten(Vector3 world_position)
    {
        return new Vector3(world_position.x, 0, world_position.z);
    }

    private float GetArcedYValue()
    {
        float l_FlatDistanceCovered = m_TotalFlatDistance - (Vector3.Distance(Flatten(transform.position), m_TargetFlatPosition));
        float l_FlatDistanceNormalised = l_FlatDistanceCovered / m_TotalFlatDistance;
        float l_ArcYModifier = m_ArcYCurve.Evaluate(l_FlatDistanceNormalised) * m_MaxArcHeight;
        float l_DirectYModifier = l_FlatDistanceCovered * m_DirectFireYModifier;
        return ((l_ArcYModifier) + l_DirectYModifier + m_StartingYHeight);
    }
}
