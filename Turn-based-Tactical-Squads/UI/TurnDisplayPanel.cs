using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnDisplayPanel : MonoBehaviour
{
    [SerializeField] Button m_EndTurnButton;
    [SerializeField] TextMeshProUGUI m_EnemyTurnText;
    [SerializeField] TextMeshProUGUI m_AllyTurnText;

    private void Start()
    {
        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChange;     
        m_EndTurnButton.gameObject.SetActive(true);
        m_EnemyTurnText.gameObject.SetActive(false);
        m_AllyTurnText.gameObject.SetActive(false);   
    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e)
    {
        switch (TurnSystem.Instance.GetActiveForce())
        {
            case Unit.eUnitForce.UNIT_PLAYER:
            {
                m_EndTurnButton.gameObject.SetActive(true);
                m_EnemyTurnText.gameObject.SetActive(false);
                m_AllyTurnText.gameObject.SetActive(false);
                break;
            }
            case Unit.eUnitForce.UNIT_ENEMY:
            {
                m_EndTurnButton.gameObject.SetActive(false);
                m_EnemyTurnText.gameObject.SetActive(true);
                m_AllyTurnText.gameObject.SetActive(false);
                break;
            }
            case Unit.eUnitForce.UNIT_ALLY:
            {
                m_EndTurnButton.gameObject.SetActive(false);
                m_EnemyTurnText.gameObject.SetActive(false);
                m_AllyTurnText.gameObject.SetActive(true);
                break;
            }
        }
    }
}
