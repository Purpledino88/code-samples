using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceSelectionManager : MonoBehaviour
{
    //Singleton code
    public static ForceSelectionManager Instance { get; private set; }

    [SerializeField] private UnitSelectionPanel m_UnitSelectionPanel;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of ForceSelectionManager singleton object: " + Instance + " - " + this);
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void ShowAllCommandSquads() { m_UnitSelectionPanel.ShowCommandSquads(); }
    public void ShowAllRegularSquads() { m_UnitSelectionPanel.ShowRegularSquads(); }
    public void ShowAllSpecialistSquads() { m_UnitSelectionPanel.ShowSpecialistSquads(); }

}
