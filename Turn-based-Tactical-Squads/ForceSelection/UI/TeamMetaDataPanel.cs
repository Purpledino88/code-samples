using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamMetaDataPanel : MonoBehaviour
{
    [SerializeField] private Transform m_SpecialEquipmentContainer;

    private TeamMetaData m_TeamMetaData;

    public void ShowDetails(TeamMetaData team_meta_data)
    {
        m_TeamMetaData = team_meta_data;
        UpdateSpecialWeapons();
        UpdateEquipment();
        UpdateElements();
    }

    public void UpdateSpecialWeapons()
    {

    }

    public void UpdateEquipment()
    {

    }

    private void UpdateElements()
    {

    }
}
