using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private GridPosition m_Position;
    public GridPosition GetGridPosition() { return m_Position; }

    private int m_WalkingCost;
    public int GetWalkingCost() { return m_WalkingCost; }

    private int m_HeuristicCost;
    public int GetHeuristicCost() { return m_HeuristicCost; }

    private int m_TotalCost;
    public int GetTotalCost() { return m_TotalCost; }

    private float m_MobilityModifier = 1.0f;
    public float GetMobilityModifier() { return m_MobilityModifier; }
    public void SetMobilityModifier(float modifier) {m_MobilityModifier = modifier; }

    private PathNode m_PreviousNode;
    public PathNode GetPreviousNode() { return m_PreviousNode; }

    public PathNode(GridPosition position)
    {
        m_Position = position;
    }

    public void SetPreviousNode(PathNode previous_node)
    {
        m_PreviousNode = previous_node;
    }

    public void SetWalkingCost(int cost) 
    { 
        m_WalkingCost = cost; 
        CalculateTotalCost();
    }

    public void SetHeuristicCost(int cost)
    { 
        m_HeuristicCost = cost; 
        CalculateTotalCost();
    }
    private void CalculateTotalCost() 
    { 
        m_TotalCost = m_WalkingCost + m_HeuristicCost; 
    }

    public void Reset()
    {
        m_WalkingCost = int.MaxValue;
        m_HeuristicCost = 0;
        CalculateTotalCost();
        m_PreviousNode = null;
    }

    public override string ToString()
    {
        return m_Position.ToString();
    }
}
