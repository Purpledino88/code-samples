using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamMetaData
{
    private SquadScriptableObject.FireTeam m_FireTeamDetails;
    private int m_TroopQuality;
    private WeaponScriptableObject m_PrimaryWeapon;

    public TeamMetaData(SquadScriptableObject.FireTeam fire_team, bool is_player)
    {
        m_FireTeamDetails = fire_team;
    }

    public void SetSquadLevelDetails(int troop_quality, WeaponScriptableObject primary_weapon)
    {
        m_TroopQuality = troop_quality;
        m_PrimaryWeapon = primary_weapon;
    }
}
