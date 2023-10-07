using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDebugObject : GridDebugObject
{
    [SerializeField] TMPro.TextMeshPro m_WalkingText;
    [SerializeField] TMPro.TextMeshPro m_HeuristicText;
    [SerializeField] TMPro.TextMeshPro m_TotalText;
    [SerializeField] TMPro.TextMeshPro m_MobilityModifierText;

    private PathNode m_PathNode;

    public override void SetGridObject(object grid_object)
    {
        base.SetGridObject(grid_object);
        m_PathNode = (PathNode)grid_object;
    }

    protected override void Update()
    {
        if (m_PathNode.GetWalkingCost() < 1000)
            m_WalkingText.text = m_PathNode.GetWalkingCost().ToString();
        else 
            m_WalkingText.text = "***";
            
        if (m_PathNode.GetHeuristicCost() < 1000)
            m_HeuristicText.text = m_PathNode.GetHeuristicCost().ToString();
        else 
            m_HeuristicText.text = "***";
            
        if (m_PathNode.GetTotalCost() < 1000)
            m_TotalText.text = m_PathNode.GetTotalCost().ToString();
        else 
            m_TotalText.text = "***";
            
        if (m_PathNode.GetMobilityModifier() < 11f)
            m_MobilityModifierText.text = m_PathNode.GetMobilityModifier().ToString();
        else 
            m_TotalText.text = "***";
    }
}
