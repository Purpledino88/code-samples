using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum eState
    {
        WAITING,
        TAKING_TURN,
        BUSY
    }
    private eState m_CurrentState;

    [SerializeField] Unit.eUnitForce m_ForceType;

    private float m_Timer = 2.5f;

    private void Awake()
    {
        m_CurrentState = eState.WAITING;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChanged;
    }
    
    private void Update()
    {
        if (TurnSystem.Instance.GetActiveForce() == m_ForceType)
        {
            switch (m_CurrentState)
            {
                case eState.WAITING:
                {
                    break;
                }
                case eState.TAKING_TURN:
                {
                    m_Timer -= Time.deltaTime;
                    if (m_Timer <= 0f)
                    {
                        if (TryTakeAIAction(DelayBetweenActions))
                        {
                            m_CurrentState = eState.BUSY;
                        }
                        else
                        {
                            FinishTurn();
                        }
                    }
                    break;
                }
                case eState.BUSY:
                {
                    break;
                }
            }
        }
    }

    private bool TryTakeAIAction(Action on_complete_delegate)
    {
        foreach (Unit unit in UnitManager.Instance.GetAllUnitsOfForce(m_ForceType))
        {
            if (TryTakeAIAction(unit, on_complete_delegate))
                return true;
        }
        return false;
    }

    private bool TryTakeAIAction(Unit ai_unit, Action on_complete_delegate)
    {
        BasicAction l_BestAction = null;
        BasicAction.GridPositionData l_BestActionData = null;

        foreach (BasicAction act in ai_unit.GetAllActions())
        {
            if (ai_unit.CanTakeAction(act))
            {
                BasicAction.GridPositionData l_newActionData = act.GetHighestPriorityAction();
                if ((l_BestActionData == null) || ((l_newActionData != null) && (l_newActionData.action_priority > l_BestActionData.action_priority)))
                {
                    l_BestAction = act;
                    l_BestActionData = l_newActionData;
                }
            }
        }

        if ((l_BestActionData != null) && (ai_unit.TryTakeAction(l_BestAction)))
        {
            l_BestAction.TakeAction(on_complete_delegate, l_BestActionData.position);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DelayBetweenActions()
    {
        UnitActionSystem.Instance.EndAIAction();
        m_Timer = 1f;
        m_CurrentState = eState.TAKING_TURN;
    }

    private void FinishTurn()
    {
        m_CurrentState = eState.WAITING;
        TurnSystem.Instance.EndTurn();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (TurnSystem.Instance.GetActiveForce() == m_ForceType)
        {
            m_Timer = 1f;
            m_CurrentState = eState.TAKING_TURN;
        }
    }
}
