using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BasicAction
{
    private const float MOVE_SPEED = 2f;
    private const int BASE_MOVEMENT_POINTS = 60;

    private List<Vector3> m_MovementList;    
    private int m_ListIndex;
    private int m_MovementPointsRemaining;
    private int m_SprintPointsRemaining;
    private float m_MobilityModifier = 1f;

    private void Start()
    {
        m_MovementPointsRemaining = BASE_MOVEMENT_POINTS;
        m_SprintPointsRemaining = m_Unit.GetAgility() * 10;
    }

    private void Update() 
    {
        if (IsActive())
        {
            float dt = Time.deltaTime;
            Vector3 l_MovementTargetPosition = m_MovementList[m_ListIndex];

            if (Vector3.Distance(transform.position, l_MovementTargetPosition) > dt)
            {
                Vector3 move_direction = (l_MovementTargetPosition - transform.position).normalized;
                transform.position += MOVE_SPEED * dt * move_direction;
                UpdateAllImplementations();
            }
            else
            {
                m_ListIndex++;
                
                if (m_ListIndex >= m_MovementList.Count)
                {
                    transform.position = l_MovementTargetPosition;
                    ActionFinished();
                }
                else
                {
                    foreach (MoveImplementation implementation in m_ElementImplementations)
                        implementation.SetMovementTarget(m_MovementList[m_ListIndex]);
                }
            }
        }   
    }

    public override void OnNewTurn()
    {
        m_MobilityModifier = m_Unit.GetUnitMobilityModifier();
        m_MovementPointsRemaining = Mathf.RoundToInt(m_MobilityModifier * BASE_MOVEMENT_POINTS);
        m_SprintPointsRemaining = Mathf.RoundToInt((m_Unit.GetAgility() * 10) * m_MobilityModifier);
    }

    public override bool CanTakeAction()
    {
        return (m_MovementPointsRemaining > 0);
    }

    public override void TakeAction(System.Action on_complete, GridPosition grid_position)
    {
        List<GridPosition> l_Path = Pathfinder.Instance.FindPath(m_Unit.GetGridPosition(), grid_position, out int l_MovementPointsUsed);
        m_SprintPointsRemaining -= l_MovementPointsUsed;
        m_MovementPointsRemaining += m_SprintPointsRemaining;

        if (m_SprintPointsRemaining < 0)    //If sprinting then call m_Unit.TryTakeAction again as a hacky workaround to spend the additional action point
            m_Unit.TryTakeAction(this);

        m_ListIndex = 0;
        m_MovementList = new List<Vector3>();        
        
        foreach (GridPosition pos in l_Path)
            m_MovementList.Add(LevelGrid.Instance.GetWorldPosition(pos));

        foreach (MoveImplementation implementation in m_ElementImplementations)
            implementation.SetMovementTarget(m_MovementList[0]);

        ActionStarted(on_complete);
    }

    public override List<GridPositionData> GetValidGridPositions()
    {
        int l_MovementPointsThisAction;
        if (m_Unit.GetAvailableActionPoints() == 2)
            l_MovementPointsThisAction = m_MovementPointsRemaining + m_SprintPointsRemaining;
        else
            l_MovementPointsThisAction = m_MovementPointsRemaining;

        GridPosition l_UnitGridPosition = m_Unit.GetGridPosition();

        List<GridPositionData> l_PossibleMovementPositions = new List<GridPositionData>();

        int l_PossibleMoves = Mathf.CeilToInt(l_MovementPointsThisAction / 10f);
        for (int x = -l_PossibleMoves; x <= l_PossibleMoves; x++)
        {
            for (int z = -l_PossibleMoves; z <= l_PossibleMoves; z++)
            {
                GridPosition l_OffsetPosition = new GridPosition(x, z) + l_UnitGridPosition;

                if (!LevelGrid.Instance.IsValid(l_OffsetPosition))
                    continue;

                if (l_UnitGridPosition == l_OffsetPosition)
                    continue;

                if (LevelGrid.Instance.IsOccupied(l_OffsetPosition))
                    continue;

                if (Pathfinder.Instance.FindPath(l_UnitGridPosition, l_OffsetPosition, out int l_MovementPointsRequired) == null)
                    continue;

                if (l_MovementPointsRequired > l_MovementPointsThisAction)
                    continue;

                GridPositionData l_Data = new GridPositionData();
                l_Data.position = l_OffsetPosition;
                l_Data.action_priority = m_Unit.GetActionOfType<ShootAction>().GetValidTargetsAtPosition(l_OffsetPosition) * 10;

                if (m_Unit.GetAvailableActionPoints() == 2)
                {
                    if (l_MovementPointsRequired > (BASE_MOVEMENT_POINTS * m_MobilityModifier))
                        l_Data.colour = Color.red;
                    else if (l_MovementPointsRequired < (m_Unit.GetAgility() * 10 * m_MobilityModifier))
                        l_Data.colour = Color.green;
                    else
                        l_Data.colour = Color.yellow;
                }
                else
                {
                    l_Data.colour = Color.red;
                }

                l_PossibleMovementPositions.Add(l_Data);
            }
        }

        return l_PossibleMovementPositions;
    }

    public override string GetActionName() { return "Move"; }

    public override void CollectElementImplementations()
    {
        foreach (UnitElement element in m_Unit.GetSurvivingElements())
        {
            if (element.TryGetComponent<MoveImplementation>(out MoveImplementation implementation))
                m_ElementImplementations.Add(implementation);
        }
    }
}