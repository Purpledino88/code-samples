using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStatusBar : MonoBehaviour
{
    [SerializeField] private Unit m_Unit;
    [SerializeField] private ActionPointsDisplay m_ActionPointsDisplay;
    [SerializeField] private Transform m_HealthIndicatorCollection;
    [SerializeField] private GameObject m_HealthIndicatorPrefab;

    private List<ElementHealthIndicator> m_HealthIndicatorList;

    private void Start()
    {
        m_HealthIndicatorList = new List<ElementHealthIndicator>();
        foreach (UnitElement element in m_Unit.GetStartingElements())
        {
            GameObject l_NewGameObject = Instantiate(m_HealthIndicatorPrefab, m_HealthIndicatorCollection);
            m_HealthIndicatorList.Add(l_NewGameObject.GetComponent<ElementHealthIndicator>());
        }

        UpdateDetails();

        Unit.OnAnyUnitStatusChange += Unit_OnAnyUnitStatusChange;
    }

    private void UpdateDetails()
    {
        m_ActionPointsDisplay.UpdateDisplay(m_Unit.GetAvailableActionPoints());
        
        for (int count = 0; count < m_HealthIndicatorList.Count; count++)
        {
            if (count < m_Unit.GetStartingElements().Count)
                m_HealthIndicatorList[count].ShowHealth(m_Unit.GetStartingElements()[count]);   
        } 
    }
    
    private void Unit_OnAnyUnitStatusChange(object sender, System.EventArgs e)
    {
        UpdateDetails();
    }

    private void OnDestroy() 
    {
        Unit.OnAnyUnitStatusChange -= Unit_OnAnyUnitStatusChange;
    }
}
