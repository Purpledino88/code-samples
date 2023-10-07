using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadSelectionButton : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI m_Text;

    private SquadScriptableObject m_SquadType;

    public void Setup(SquadScriptableObject squad_type)
    {
        m_SquadType = squad_type;
        m_Text.text = m_SquadType.squad_name;
    }

    public void SelectSquad()
    {
        Debug.Log("Selecting squad of type " + m_SquadType);
    }
}
