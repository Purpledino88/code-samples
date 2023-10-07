using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    //Singleton code
    public static TurnSystem Instance { get; private set; }

    public EventHandler OnTurnChange;

    private Unit.eUnitForce m_CurrentActiveForce = Unit.eUnitForce.UNIT_PLAYER;
    public Unit.eUnitForce GetActiveForce() { return m_CurrentActiveForce; }

    private int m_TurnNumber = 1;
    public int GetTurnNumber() { return m_TurnNumber; }

    private void Awake() 
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of TurnSystem singleton object: " + Instance + " - " + this);
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void EndTurn()
    {
        switch (m_CurrentActiveForce)
        {
            case Unit.eUnitForce.UNIT_PLAYER:
            {
                m_CurrentActiveForce = Unit.eUnitForce.UNIT_ENEMY;
                break;
            }
            case Unit.eUnitForce.UNIT_ENEMY:
            {
                m_CurrentActiveForce = Unit.eUnitForce.UNIT_ALLY;
                break;
            }
            case Unit.eUnitForce.UNIT_ALLY:
            {
                m_CurrentActiveForce = Unit.eUnitForce.UNIT_PLAYER;
                m_TurnNumber++;
                break;
            }
        }

        Debug.Log($"CURRENT {m_CurrentActiveForce} TURN {m_TurnNumber}");
        OnTurnChange?.Invoke(this, EventArgs.Empty);
    }
}
