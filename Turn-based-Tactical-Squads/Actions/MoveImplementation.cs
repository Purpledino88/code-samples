using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveImplementation : BasicImplementation
{
    private const float ROTATE_SPEED = 10f; 

    private Vector3 m_MovementTargetPosition;   
    public void SetMovementTarget(Vector3 target_position) { m_MovementTargetPosition = (target_position + transform.localPosition); }

    public override void StartElement()
    {
        m_Element.SetAnimationBool("UnitIsMoving", true);
    }
    
    public override void UpdateElement()
    {
        transform.forward = Vector3.Slerp(transform.forward, (m_MovementTargetPosition - transform.position).normalized, Time.deltaTime * ROTATE_SPEED);
    }
    
    public override void EndElement()
    {
        m_Element.SetAnimationBool("UnitIsMoving", false);
    }
}
