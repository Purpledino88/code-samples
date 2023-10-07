using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float m_MobilityModifier;
    public float GetMobilityModifier() { return m_MobilityModifier; }
    
    [SerializeField] private float m_VisibilityModifier;
    public float GetVisibilityModifier() { return m_VisibilityModifier; }
    
    [SerializeField] private float m_CoverModifier;
    public float GetCoverModifier() { return m_CoverModifier; }
}
