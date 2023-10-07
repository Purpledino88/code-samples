using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAction : BasicAction
{
    public override bool CanTakeAction() { return true; }

    public override List<GridPositionData> GetValidGridPositions()
    {
        List<GridPositionData> l_PossibleEnemyPositions = new List<GridPositionData>();

        if (Unit.AreOpposed(m_Unit.GetUnitForce(), Unit.eUnitForce.UNIT_PLAYER))
        {
            foreach (Unit l_TargetUnit in UnitManager.Instance.GetAllUnitsOfForce(Unit.eUnitForce.UNIT_PLAYER))
                CheckUnitIsValidTarget(l_PossibleEnemyPositions, l_TargetUnit);
        }

        if (Unit.AreOpposed(m_Unit.GetUnitForce(), Unit.eUnitForce.UNIT_ENEMY))
        {
            foreach (Unit l_TargetUnit in UnitManager.Instance.GetAllUnitsOfForce(Unit.eUnitForce.UNIT_ENEMY))
                CheckUnitIsValidTarget(l_PossibleEnemyPositions, l_TargetUnit);
        }

        if (Unit.AreOpposed(m_Unit.GetUnitForce(), Unit.eUnitForce.UNIT_ALLY))
        {
            foreach (Unit l_TargetUnit in UnitManager.Instance.GetAllUnitsOfForce(Unit.eUnitForce.UNIT_ALLY))
                CheckUnitIsValidTarget(l_PossibleEnemyPositions, l_TargetUnit);
        }

        return l_PossibleEnemyPositions;
    }

    private void CheckUnitIsValidTarget(List<GridPositionData> list_to_add_to, Unit target_unit)
    {
        Vector3 l_ShooterWorldPosition = m_Unit.GetWorldPosition();
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

        GridPositionData l_Data = new GridPositionData();
        l_Data.position = target_unit.GetGridPosition();
        l_Data.colour = Color.red;
        l_Data.action_priority = 0;
        list_to_add_to.Add(l_Data);
    }

    public override string GetActionName() { return "Aim"; }

    public override void TakeAction(System.Action on_complete, GridPosition grid_position)
    {

    }

    public override void CollectElementImplementations()
    {

    }
}
