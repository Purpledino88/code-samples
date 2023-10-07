using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeCloud : MonoBehaviour
{
    private const float DISPERSAL_CHANCE = 0.25f;

    private int m_TurnsActive = 0;
    private Unit.eUnitForce m_Force;

    private void Awake()
    {
        m_Force = TurnSystem.Instance.GetActiveForce();
        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if ((UnityEngine.Random.Range(0f, 1f)) < (DISPERSAL_CHANCE * m_TurnsActive))
            GameObject.Destroy(this); 
    }

    private void OnDestroy() 
    {
        TurnSystem.Instance.OnTurnChange -= TurnSystem_OnTurnChanged;
    }
}
