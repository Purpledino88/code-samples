using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootImplementation : BasicImplementation
{
    private const float ROTATE_SPEED = 10f;
    private const float AIMING_STATE_TIME = 1.0f;
    private const float FIRING_STATE_TIME_RANDOM_DELAY = 1.5f;
    private const float FIRING_STATE_TIME = 0.1f;
    private const float COOLDOWN_STATE_TIME = 0.5f;

    private enum eFiringState 
    {
        AIMING,
        FIRING,
        COOLDOWN
    }
    private eFiringState m_FiringState;
    private float m_FiringStateTimer;

    private bool m_CanFireBullet;

    private int m_ShotsToFire;

    private Unit m_TargetUnit;
    public void SetTargetUnit(Unit target_unit) { m_TargetUnit = target_unit; }

    private float m_CoverModifier;
    public void SetCoverModifier(float cover_modifier) { m_CoverModifier = cover_modifier; }

    public override void StartElement()
    {
        if (m_Element.IsAbleToAct())
        {
            m_FiringState = eFiringState.AIMING;
            m_FiringStateTimer = AIMING_STATE_TIME;
            m_ShotsToFire = 0;
            m_CanFireBullet = false;
            m_Completable = false;
            m_Element.SelectTargets(m_TargetUnit);
        }
        else
        {
            m_Completable = true;
        }
    }
    
    public override void UpdateElement()
    {
        if (!m_Completable)
        {
            float dt = Time.deltaTime;
            m_FiringStateTimer -= dt;

            switch (m_FiringState)
            {
                case eFiringState.AIMING:
                {
                    Vector3 l_AimDirection = (m_TargetUnit.GetWorldPosition() - transform.position).normalized;
                    transform.forward = Vector3.Lerp(transform.forward, l_AimDirection, Time.deltaTime * ROTATE_SPEED);
                    break;
                }
                case eFiringState.FIRING:
                {
                    if (m_CanFireBullet)
                    {
                        m_Element.FireWeapon(m_TargetUnit, m_Element.GetSelectedFireMode().shots_per_action -  m_ShotsToFire, m_CoverModifier);
                        m_ShotsToFire--;
                        m_CanFireBullet = false;
                    }
                    break;
                }
                case eFiringState.COOLDOWN:
                {
                    break;
                }
            }

            if (m_FiringStateTimer < 0f)
            {
                if (m_ShotsToFire > 0)
                {
                    m_FiringStateTimer = FIRING_STATE_TIME;
                    m_CanFireBullet = true;
                }
                else
                {
                    EndState();
                }
            }
        }
    }
    
    public override void EndElement()
    {
        m_Element.FinishFiring();
    }

    private void EndState()
    {
        switch (m_FiringState)
        {
            case eFiringState.AIMING:
            {
                if (m_FiringStateTimer < 0f)
                {
                    m_FiringState = eFiringState.FIRING;
                    m_FiringStateTimer = FIRING_STATE_TIME + (Random.Range(0f, FIRING_STATE_TIME_RANDOM_DELAY));
                    m_ShotsToFire = m_Element.GetSelectedFireMode().shots_per_action;
                }
                break;
            }
            case eFiringState.FIRING:
            {
                if (m_FiringStateTimer < 0f)
                {
                    m_FiringState = eFiringState.COOLDOWN;
                    m_FiringStateTimer = COOLDOWN_STATE_TIME;
                }
                break;
            }
            case eFiringState.COOLDOWN:
            {
                if (m_FiringStateTimer < 0f)
                {
                    m_Completable = true;
                }
                break;
            }
        }
    }
}
