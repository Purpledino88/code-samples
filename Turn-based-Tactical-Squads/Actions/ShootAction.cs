using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BasicAction
{
    private Unit m_TargetUnit;

    private bool m_HasFiredThisTurn = false;

    private void Update()
    {
        if (IsActive())
        {
            UpdateAllImplementations();
            if (AreAllImplementationsComplete())
                ActionFinished();
        }
    }

    public int GetValidTargetsAtPosition(GridPosition position)
    {
        return GetValidGridPositions(position).Count;
    }

    public override void OnNewTurn()
    {
        m_HasFiredThisTurn = false;
    }

    public override bool CanTakeAction()
    {
        return (!m_HasFiredThisTurn);
    }
    
    public override void TakeAction(System.Action on_complete, GridPosition grid_position)
    {
        m_TargetUnit = LevelGrid.Instance.GetUnitListAtGridPosition(grid_position)[0];
        float l_CoverModifier = LevelGrid.Instance.GetCoverModifer(m_Unit.GetUnitHeadHeight().position, m_TargetUnit);

        foreach (ShootImplementation implementation in m_ElementImplementations)
        {
            implementation.SetTargetUnit(m_TargetUnit); 
            implementation.SetCoverModifier(l_CoverModifier);
        }

        m_HasFiredThisTurn = true;

        ActionStarted(on_complete);
    }

    public override List<GridPositionData> GetValidGridPositions()
    {
        return GetValidGridPositions(m_Unit.GetGridPosition());
    }
    
    private List<GridPositionData> GetValidGridPositions(GridPosition firing_position)
    {
        List<GridPositionData> l_PossibleEnemyPositions = new List<GridPositionData>();

        if (Unit.AreOpposed(m_Unit.GetUnitForce(), Unit.eUnitForce.UNIT_PLAYER))
        {
            foreach (Unit l_TargetUnit in UnitManager.Instance.GetAllUnitsOfForce(Unit.eUnitForce.UNIT_PLAYER))
                CheckUnitIsValidTarget(l_PossibleEnemyPositions, firing_position, l_TargetUnit);
        }

        if (Unit.AreOpposed(m_Unit.GetUnitForce(), Unit.eUnitForce.UNIT_ENEMY))
        {
            foreach (Unit l_TargetUnit in UnitManager.Instance.GetAllUnitsOfForce(Unit.eUnitForce.UNIT_ENEMY))
                CheckUnitIsValidTarget(l_PossibleEnemyPositions, firing_position, l_TargetUnit);
        }

        if (Unit.AreOpposed(m_Unit.GetUnitForce(), Unit.eUnitForce.UNIT_ALLY))
        {
            foreach (Unit l_TargetUnit in UnitManager.Instance.GetAllUnitsOfForce(Unit.eUnitForce.UNIT_ALLY))
                CheckUnitIsValidTarget(l_PossibleEnemyPositions, firing_position, l_TargetUnit);
        }

        return l_PossibleEnemyPositions;
    }

    private void CheckUnitIsValidTarget(List<GridPositionData> list_to_add_to, GridPosition firing_position, Unit target_unit)
    {
        Vector3 l_ShooterWorldPosition = LevelGrid.Instance.GetWorldPosition(firing_position);
        Vector3 l_TargetWorldPosition = LevelGrid.Instance.GetWorldPosition(target_unit.GetGridPosition());
        float l_DistanceToTarget = Vector3.Distance(l_ShooterWorldPosition, l_TargetWorldPosition);

        float l_MaxRange = 0;
        foreach (UnitElement element in m_Unit.GetSurvivingElements())
        {
            foreach (WeaponScriptableObject weapon in element.GetAllWeapons())
            {
                foreach (WeaponScriptableObject.WeaponFireMode fire_mode in weapon.all_fire_modes)
                {
                    float l_WeaponRange = fire_mode.effective_range * 1.5f;
                    if (l_WeaponRange > l_MaxRange)
                        l_MaxRange = l_WeaponRange;
                }
            }
        }
        if (l_DistanceToTarget > l_MaxRange)
            return;

        float l_CoverModifier = LevelGrid.Instance.GetCoverModifer(l_ShooterWorldPosition + m_Unit.GetUnitHeadHeight().localPosition, target_unit);
        if (l_CoverModifier >= 1f)
            return;

        int l_MaxChanceToHit = 0;
        foreach (UnitElement element in m_Unit.GetSurvivingElements())
        {
            foreach (WeaponScriptableObject weapon in element.GetAllWeapons())
            {
                foreach (WeaponScriptableObject.WeaponFireMode fire_mode in weapon.all_fire_modes)
                {
                    int l_ElementHitChance = element.CalculateHitChance(target_unit, weapon, fire_mode, l_CoverModifier);
                    if (l_ElementHitChance > l_MaxChanceToHit)
                        l_MaxChanceToHit = l_ElementHitChance;
                }
            }
        }
        if (l_MaxChanceToHit == 0)
            return;

        GridPositionData l_Data = new GridPositionData();
        l_Data.position = target_unit.GetGridPosition();
        l_Data.colour = Color.red;
        l_Data.action_priority = l_MaxChanceToHit;
        list_to_add_to.Add(l_Data);
    }

    private void CalculateExpectedDamage()
    {

    }

    public override string GetActionName() { return "Fire Weapons"; }

    public override void CollectElementImplementations()
    {
        foreach (UnitElement element in m_Unit.GetSurvivingElements())
        {
            if (element.TryGetComponent<ShootImplementation>(out ShootImplementation implementation))
                m_ElementImplementations.Add(implementation);
        }
    }
}
