using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BasicAction
{
    private void Update()
    {
        if (IsActive())
        {
            UpdateAllImplementations();
            if (AreAllImplementationsComplete())
                ActionFinished();
        }
    }
    
    public override void TakeAction(System.Action on_complete, GridPosition grid_position)
    {
        ActionStarted(on_complete);
    }

    public override List<GridPositionData> GetValidGridPositions() 
    { 
        GridPositionData data = new GridPositionData();
        data.position = m_Unit.GetGridPosition();
        data.colour = Color.blue;
        data.action_priority = 0;
        return new List<GridPositionData> {data}; 
    }


    public override string GetActionName() { return "Spin"; }

    public override int GetActionPointsCost()
    {
        return 2;
    }

    public override void CollectElementImplementations()
    {
        foreach (UnitElement element in m_Unit.GetSurvivingElements())
        {
            if (element.TryGetComponent<SpinImplementation>(out SpinImplementation implementation))
                m_ElementImplementations.Add(implementation);
        }
    }
}
