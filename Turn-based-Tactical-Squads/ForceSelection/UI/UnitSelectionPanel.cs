using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionPanel : MonoBehaviour
{
    [SerializeField] private List<SquadScriptableObject> m_CommandSquads;
    [SerializeField] private List<SquadScriptableObject> m_RegularSquads;
    [SerializeField] private List<SquadScriptableObject> m_SpecialistSquads;

    [SerializeField] private GameObject m_SquadSelectionButtonPrefab;

    public void ShowCommandSquads()
    {
        ShowSquads(m_CommandSquads);
    }

    public void ShowRegularSquads()
    {
        ShowSquads(m_RegularSquads);
    }

    public void ShowSpecialistSquads()
    {
        ShowSquads(m_SpecialistSquads);
    }

    private void ShowSquads(List<SquadScriptableObject> list_of_squads)
    {
        foreach (Transform child in this.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (SquadScriptableObject squad in list_of_squads)
        {
            GameObject l_SquadButton = Instantiate(m_SquadSelectionButtonPrefab, this.transform);
            SquadSelectionButton l_SquadSelectionButton = l_SquadButton.GetComponent<SquadSelectionButton>();
            l_SquadSelectionButton.Setup(squad);
        }
    }

    public void SelectSquad(SquadScriptableObject desired_squad)
    {

    }
}
