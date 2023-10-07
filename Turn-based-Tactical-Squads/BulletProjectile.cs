using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private const float PROJECTILE_SPEED = 25f;

    [SerializeField] private TrailRenderer m_Trail;
    [SerializeField] private GameObject m_BulletMissParticleEffect;
    [SerializeField] private GameObject m_BulletHitParticleEffect;

    private UnitElement m_TargetElement;

    private UnitElement m_FiringElement;

    private Vector3 m_TargetPosition;

    private WeaponScriptableObject.WeaponFireMode m_FireModeUsed;

    public void Setup(UnitElement target_element, UnitElement firing_element, Vector3 target_position, WeaponScriptableObject.WeaponFireMode fire_mode_used)
    {
        m_TargetElement = target_element;
        m_FiringElement = firing_element;
        m_TargetPosition = target_position;
        m_FireModeUsed = fire_mode_used;
    }

    private void Update()
    {
        float l_DistanceBefore = Vector3.Distance(m_TargetPosition, transform.position);

        Vector3 l_Direction = (m_TargetPosition - transform.position).normalized;
        transform.position += (l_Direction * PROJECTILE_SPEED * Time.deltaTime);
        
        float l_DistanceAfter = Vector3.Distance(m_TargetPosition, transform.position);

        if (l_DistanceBefore < l_DistanceAfter)
        { 
            transform.position = m_TargetPosition;
            if (m_TargetElement != null)
            {
                m_TargetElement.ResolveBulletHit(m_FiringElement, m_FireModeUsed); 
                Instantiate(m_BulletHitParticleEffect, m_TargetPosition, Quaternion.identity);
            }
            else
            {
                Instantiate(m_BulletMissParticleEffect, m_TargetPosition, Quaternion.identity);
            }

            m_Trail.transform.parent = null;
            Destroy(gameObject);
        }
    }
}
