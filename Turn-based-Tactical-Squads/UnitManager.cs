using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    //Singleton code
    public static UnitManager Instance { get; private set; }

    private List<Unit> m_AllPlayerUnits;
    private List<Unit> m_AllEnemyUnits;
    private List<Unit> m_AllAllyUnits;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of UnitManager singleton object: " + Instance + " - " + this);
            Destroy(this);
            return;
        }
        Instance = this;

        m_AllPlayerUnits = new List<Unit>();
        m_AllEnemyUnits = new List<Unit>();
        m_AllAllyUnits = new List<Unit>();
    }

    private void Start()
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDestroyed += Unit_OnAnyUnitDestroyed;
    }

    public List<Unit> GetAllUnitsOfForce(Unit.eUnitForce force)
    {
        switch (force)
        {
            case Unit.eUnitForce.UNIT_PLAYER:
            {
                return m_AllPlayerUnits;
            }
            case Unit.eUnitForce.UNIT_ENEMY:
            {
                return m_AllEnemyUnits;
            }
            case Unit.eUnitForce.UNIT_ALLY:
            {
                return m_AllAllyUnits;
            }
            default:
            {
                Debug.Log("Force is not implemented in UnitManager: " + force);
                return null;
            }
        }
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        Unit l_Unit = sender as Unit;

        switch (l_Unit.GetUnitForce())
        {
            case Unit.eUnitForce.UNIT_PLAYER:
            {
                m_AllPlayerUnits.Add(l_Unit);
                break;
            }
            case Unit.eUnitForce.UNIT_ENEMY:
            {
                m_AllEnemyUnits.Add(l_Unit);
                break;
            }
            case Unit.eUnitForce.UNIT_ALLY:
            {
                m_AllAllyUnits.Add(l_Unit);
                break;
            }
        }
    }

    private void Unit_OnAnyUnitDestroyed(object sender, EventArgs e)
    {
        Unit l_Unit = sender as Unit;

        switch (l_Unit.GetUnitForce())
        {
            case Unit.eUnitForce.UNIT_PLAYER:
            {
                m_AllPlayerUnits.Remove(l_Unit);
                break;
            }
            case Unit.eUnitForce.UNIT_ENEMY:
            {
                m_AllEnemyUnits.Remove(l_Unit);
                break;
            }
            case Unit.eUnitForce.UNIT_ALLY:
            {
                m_AllAllyUnits.Remove(l_Unit);
                break;
            }
        }
    }
}
