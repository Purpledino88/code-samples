using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject 
{
    private GridSystem<GridObject> m_GridSystem;
    private GridPosition m_Position;

    private List<Unit> m_OccupyingUnitList;
    public List<Unit> GetUnitList() { return m_OccupyingUnitList; }
    public void AddUnit(Unit unit) { m_OccupyingUnitList.Add(unit); }
    public void RemoveUnit(Unit unit) { m_OccupyingUnitList.Remove(unit); }
    public bool IsOccupied() { return m_OccupyingUnitList.Count > 0; }

    public GridObject(GridSystem<GridObject> parent_gridsystem, GridPosition position)
    {
        m_GridSystem = parent_gridsystem;
        m_Position = position;
        m_OccupyingUnitList = new List<Unit>();
    }

    public override string ToString()
    {
        string l_UnitString = "";
        foreach (Unit u in m_OccupyingUnitList)
        {
            l_UnitString += (u + "\n");
        }
        return m_Position.ToString() + "\n" + l_UnitString;
    }
}
