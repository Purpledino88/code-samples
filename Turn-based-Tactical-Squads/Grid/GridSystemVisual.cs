using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    //Singleton code
    //public static GridSystemVisual Instance { get; private set; }

    [SerializeField] private GameObject _GridVisualPrefab;

    private GridSystemVisualSingular[,] m_VisualArray;
/*
    private void Awake() 
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of GridSystemVisual singleton object: " + Instance + " - " + this);
            Destroy(this);
            return;
        }
        Instance = this;
    }*/

    private void UpdateGridVisuals()
    {
        HideAll();

        BasicAction l_SelectedAction = UnitActionSystem.Instance.GetSelectedAction();
        if (l_SelectedAction != null && !l_SelectedAction.IsActive())
            ShowSelected(l_SelectedAction.GetValidGridPositions());
    }

    private void Start()
    {
        m_VisualArray = new GridSystemVisualSingular[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition l_GridPosition = new GridPosition(x, z);
                GameObject visual = Instantiate(_GridVisualPrefab, LevelGrid.Instance.GetWorldPosition(l_GridPosition), Quaternion.identity);
                m_VisualArray[x, z] = visual.GetComponent<GridSystemVisualSingular>();
            }
        }
        UnitActionSystem.Instance.OnSelectedActionChange += UnitActionSystem_OnSelectedActionChange;
        UnitActionSystem.Instance.OnActiveChange += UnitActionSystem_OnActiveChange;
        UpdateGridVisuals();
    }

    public void HideAll()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                m_VisualArray[x, z].HideVisual();
            }
        }
    }

    /*public void ShowSelected(List<GridPosition> list_of_positions)
    {
        foreach (GridPosition pos in list_of_positions)
            m_VisualArray[pos.x, pos.z].ShowVisual(Color.white);
    }*/
    public void ShowSelected(List<BasicAction.GridPositionData> list_of_positions)
    {
        foreach (BasicAction.GridPositionData data in list_of_positions)
            m_VisualArray[data.position.x, data.position.z].ShowVisual(data.colour);
    }

    private void UnitActionSystem_OnSelectedActionChange(object sender, EventArgs e)
    {       
        UpdateGridVisuals(); 
    }

    private void UnitActionSystem_OnActiveChange(object sender, bool is_active)
    {       
        if (is_active)
            UpdateGridVisuals(); 
    }
}
