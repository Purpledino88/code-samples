using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGrenadeAction : BasicAction
{
    private const int MAX_THROWN_RANGE = 9;

    private void Update()
    {
        if (IsActive())
        {
            UpdateAllImplementations();
            if (AreAllImplementationsComplete())
                ActionFinished();
        }
    }

    public override List<GridPositionData> GetValidGridPositions()
    {
        List<GridPositionData> l_PossibleSmokePositions = new List<GridPositionData>();
        for (int x = -MAX_THROWN_RANGE; x <= MAX_THROWN_RANGE; x++)
        {
            for (int z = -MAX_THROWN_RANGE; z <= MAX_THROWN_RANGE; z++)
            {
                GridPosition l_OffsetPosition = new GridPosition(x, z) + m_Unit.GetGridPosition();

                if (!LevelGrid.Instance.IsValid(l_OffsetPosition))
                    continue;

                if (Vector3.Distance(LevelGrid.Instance.GetWorldPosition(m_Unit.GetGridPosition()), LevelGrid.Instance.GetWorldPosition(l_OffsetPosition)) > MAX_THROWN_RANGE)
                    continue;

                GridPositionData l_Data = new GridPositionData();
                l_Data.position = l_OffsetPosition;
                l_Data.action_priority = m_Unit.GetActionOfType<ShootAction>().GetValidTargetsAtPosition(l_OffsetPosition) * 10;
                l_Data.colour = Color.blue;
                l_PossibleSmokePositions.Add(l_Data);
            }
        }

        return l_PossibleSmokePositions;
    }

    public override string GetActionName() { return "Smoke Grenade"; }

    public override void TakeAction(System.Action on_complete, GridPosition grid_position)
    {
        Debug.Log("Throwing Grenade");
        ActionStarted(on_complete);
    }

    public override void CollectElementImplementations()
    {
        
    }
}
