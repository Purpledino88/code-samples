using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicAction : MonoBehaviour
{
    public class GridPositionData
    {
        public GridPosition position;
        public Color colour;
        public int action_priority;
    }

    protected Unit m_Unit;

    private bool m_IsActive = false;
    public bool IsActive() { return m_IsActive; }

    private System.Action m_OnCompleteDelegate;

    protected List<BasicImplementation> m_ElementImplementations;

    protected virtual void Awake()
    {
        m_Unit = GetComponent<Unit>();

        m_ElementImplementations = new List<BasicImplementation>();
        CollectElementImplementations();

        foreach (BasicImplementation implementation in m_ElementImplementations)
            implementation.OnImplementationDestruction += OnDestructionOfImplementation;
    }

    public virtual int GetActionPointsCost()
    {
        return 1;
    }

    public bool IsValidGridPosition(GridPosition grid_position)
    {
        foreach (GridPositionData data in GetValidGridPositions())
        {
            if (data.position == grid_position)
                return true;
        }
        return false;
    }

    protected void ActionStarted(System.Action on_complete)
    {
        m_IsActive = true;
        m_OnCompleteDelegate = on_complete;

        foreach (BasicImplementation implementation in m_ElementImplementations)
            implementation.StartElement();
    }

    protected void ActionFinished()
    {
        foreach (BasicImplementation implementation in m_ElementImplementations)
            implementation.EndElement();

        m_IsActive = false;
        m_OnCompleteDelegate();
    }

    protected void UpdateAllImplementations()
    {
        foreach (BasicImplementation implementation in m_ElementImplementations)
            implementation.UpdateElement();
    }

    protected bool AreAllImplementationsComplete()
    {
        foreach (BasicImplementation implementation in m_ElementImplementations)
        {
            if (!implementation.CanComplete())
                return false;
        }
        return true;
    }

    private void OnDestructionOfImplementation(object sender, EventArgs e)
    {
        m_ElementImplementations.Remove((BasicImplementation)sender);
    }

    public GridPositionData GetHighestPriorityAction()
    {
        List<GridPositionData> l_PossibleActions = GetValidGridPositions();
        if (l_PossibleActions.Count > 0)
        {
            l_PossibleActions.Sort((GridPositionData a, GridPositionData b) => b.action_priority - a.action_priority);
            return l_PossibleActions[0];
        }
        else
        {
            return null;
        }
    }

    public virtual void OnNewTurn() { }

    public virtual bool CanTakeAction() { return true; }

    public abstract List<GridPositionData> GetValidGridPositions();

    public abstract string GetActionName();

    public abstract void TakeAction(System.Action on_complete, GridPosition grid_position);

    public abstract void CollectElementImplementations();
}
