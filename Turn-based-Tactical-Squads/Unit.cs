using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{        
    public static event EventHandler OnAnyUnitStatusChange;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDestroyed;

    public static bool AreOpposed(eUnitForce a, eUnitForce b)
    {
        return ((a == eUnitForce.UNIT_ENEMY) != (b == eUnitForce.UNIT_ENEMY));
    }

    private const int ACTION_POINTS_MAX = 2;

    [SerializeField] int m_Agility;
    public int GetAgility() { return m_Agility; }
    [SerializeField] int m_Discipline;
    public int GetDiscipline() { return m_Discipline; }
    [SerializeField] int m_Defence;
    public int GetDefence() { return m_Defence; }
    [SerializeField] int m_Marksmanship;
    public int GetMarksmanship() { return m_Marksmanship; }

    [SerializeField] private Transform m_HeadHeightTransform;
    public Transform GetUnitHeadHeight() { return m_HeadHeightTransform; }

    public enum eUnitForce {
        UNIT_PLAYER,
        UNIT_ENEMY,
        UNIT_ALLY
    };
    [SerializeField] private eUnitForce m_UnitForce;
    public eUnitForce GetUnitForce() { return m_UnitForce; }

    [SerializeField] private int m_StartingNumberOfElements;
    [SerializeField] private GameObject m_ElementPrefab;

    [SerializeField] private List<WeaponScriptableObject> m_SpecialWeapons;

    private List<UnitElement> m_StartingElements;
    public List<UnitElement> GetStartingElements() { return m_StartingElements; }
    private List<UnitElement> m_SurvivingElements;
    public List<UnitElement> GetSurvivingElements() { return m_SurvivingElements; }

    private int m_ActionPoints;
    public int GetAvailableActionPoints() { return m_ActionPoints; }

    private GridPosition m_CurrentGridPosition;
    public GridPosition GetGridPosition() { return m_CurrentGridPosition; }

    private BasicAction[] m_PossibleActions;
    public BasicAction[] GetAllActions() { return m_PossibleActions; }
    public T GetActionOfType<T>() where T : BasicAction
    {
        foreach (BasicAction act in m_PossibleActions)
        {
            if (act is T)
                return (T)act;
        }
        return null;
    }
    
    public Vector3 GetWorldPosition() { return transform.position; }

    private void Awake()
    {
        m_PossibleActions = GetComponents<BasicAction>();
        CreateElements();
        m_ActionPoints = ACTION_POINTS_MAX;
    }

    private void CreateElements()
    {
        m_StartingElements = new List<UnitElement>();
        m_SurvivingElements = new List<UnitElement>();
        
        for (int i = 0; i < m_StartingNumberOfElements; i++)
        {
            GameObject l_NewGameObject = Instantiate(m_ElementPrefab, transform);
            l_NewGameObject.transform.SetParent(transform);

            if (m_StartingNumberOfElements > 1)
            {
                l_NewGameObject.transform.localPosition = Quaternion.Euler(0, (360 / m_StartingNumberOfElements) * i, 0) * (Vector3.forward * 0.25f);
            }

            UnitElement l_NewElement = l_NewGameObject.GetComponent<UnitElement>();
            m_StartingElements.Add(l_NewElement);
            m_SurvivingElements.Add(l_NewElement);

            if (i < m_SpecialWeapons.Count)
                l_NewElement.Setup(this, m_Marksmanship, m_SpecialWeapons[i]);
            else
                l_NewElement.Setup(this, m_Marksmanship, null);
        }
    }

    private void Start() 
    {
        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChange;
        UnitActionSystem.Instance.OnActiveChange += UnitActionSystem_OnActiveChange;

        foreach (UnitElement element in m_SurvivingElements)
        {
            element.OnElementStatusChange += UnitElement_OnElementStatusChange;
            element.OnElementDestruction += UnitElement_OnElementDestruction;
            //element.Setup(this, m_Marksmanship);
        }

        m_CurrentGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(m_CurrentGridPosition, this);
        
        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        GridPosition l_NewGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (l_NewGridPosition != m_CurrentGridPosition)
        {
            LevelGrid.Instance.UnitChangedGridPosition(this, m_CurrentGridPosition, l_NewGridPosition);
            m_CurrentGridPosition = l_NewGridPosition;
        }
    }

    public bool CanTakeAction(BasicAction act)
    {
        return ((m_ActionPoints >= act.GetActionPointsCost()) && (act.CanTakeAction()));
    }

    public bool TryTakeAction(BasicAction act)
    {
        int l_Cost = act.GetActionPointsCost();
        if (m_ActionPoints >= l_Cost)
        {
            m_ActionPoints -= l_Cost;
            Unit.OnAnyUnitStatusChange?.Invoke(this, EventArgs.Empty);
            return true;
        }
        return false;
    }

    public UnitElement GetRandomElement()
    {
        if (m_SurvivingElements.Count > 0)
            return m_SurvivingElements[UnityEngine.Random.Range(0, m_SurvivingElements.Count)];
        else
            return null;
    }

    public void ResolveExplosion(WeaponScriptableObject.WeaponFireMode fire_mode)
    {
        List<UnitElement> l_ElementsHit = new List<UnitElement>();

        foreach (UnitElement element in m_SurvivingElements)
        {
            if (UnityEngine.Random.Range(0f, 1f) < fire_mode.blast_chance_to_hit)
                l_ElementsHit.Add(element);
        }
        
        foreach (UnitElement element_hit in l_ElementsHit)
        {
            if (UnityEngine.Random.Range(0f, 1f) < fire_mode.blast_chance_to_hit)
                element_hit.ResolveBlastHit(fire_mode);
        }
    }

    public float GetUnitMobilityModifier()
    {
        float l_ElementBaseMobility = 1f / m_SurvivingElements.Count;
        float l_CumulativeMobility = 0f;

        foreach (UnitElement element in m_SurvivingElements)
        {
            if (element.GetHealth() > 2)
                l_CumulativeMobility += l_ElementBaseMobility;
            else if (element.GetHealth() > 1)
                l_CumulativeMobility += (l_ElementBaseMobility / 2);
        }

        return l_CumulativeMobility;
    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e)
    {
        if (m_UnitForce == TurnSystem.Instance.GetActiveForce())
        {
            foreach (BasicAction act in m_PossibleActions)
                act.OnNewTurn();

            foreach (UnitElement element in m_SurvivingElements)
                element.OnNewTurn();
                
            m_ActionPoints = ACTION_POINTS_MAX;
            Unit.OnAnyUnitStatusChange?.Invoke(this, EventArgs.Empty);
        }
    }

    private void UnitElement_OnElementStatusChange(object sender, EventArgs e)
    {
        OnAnyUnitStatusChange?.Invoke(this, EventArgs.Empty);
    }

    private void UnitElement_OnElementDestruction(object sender, EventArgs e)
    {
        UnitElement sending_element = (UnitElement)sender;
        m_SurvivingElements.Remove(sending_element);

        OnAnyUnitStatusChange?.Invoke(this, EventArgs.Empty);
    }

    private void UnitActionSystem_OnActiveChange(object sender, bool is_active)
    {
        if ((!is_active) && (m_SurvivingElements.Count == 0))
        {
            LevelGrid.Instance.RemoveUnitAtGridPosition(m_CurrentGridPosition, this);
            OnAnyUnitDestroyed?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        TurnSystem.Instance.OnTurnChange -= TurnSystem_OnTurnChange;
        UnitActionSystem.Instance.OnActiveChange -= UnitActionSystem_OnActiveChange;
    }
}
