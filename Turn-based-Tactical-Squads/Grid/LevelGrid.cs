using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    //Singleton code
    public static LevelGrid Instance { get; private set; }

    [SerializeField] private GameObject _GridDebugObjectPrefab;
    [SerializeField] private int m_Width;
    public int GetWidth() { return m_Width; }
    [SerializeField] private int m_Height;
    public int GetHeight() { return m_Height; }

    private GridSystem<GridObject> m_GridSystem;
    public Vector3 GetWorldPosition(GridPosition grid_position) => m_GridSystem.GetWorldPosition(grid_position);
    public GridPosition GetGridPosition(Vector3 world_position) => m_GridSystem.GetGridPosition(world_position);
    public bool IsValid(GridPosition grid_position) => m_GridSystem.IsValid(grid_position);

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of LevelGrid singleton object: " + Instance + " - " + this);
            Destroy(this);
            return;
        }
        Instance = this;

        m_GridSystem = new GridSystem<GridObject>(m_Width, m_Height, (GridSystem<GridObject> sys, GridPosition pos) => new GridObject(sys, pos));
        Pathfinder.Instance.Setup(m_Width, m_Height);
    }

    public float GetCoverModifer(Vector3 firing_world_position, Unit target_unit)
    {
        Vector3 l_TargetWorldPosition = LevelGrid.Instance.GetWorldPosition(target_unit.GetGridPosition());
        float l_DistanceToTarget = Vector3.Distance(firing_world_position, l_TargetWorldPosition);

        Vector3 l_ShootDirection = (l_TargetWorldPosition - firing_world_position).normalized;

        float l_Modifier = 0f;

        RaycastHit[] l_ObstaclesArray = Physics.RaycastAll(firing_world_position, l_ShootDirection, l_DistanceToTarget, LayerMask.GetMask("Obstacles"));
        //Debug.Log("Testing " + l_ObstaclesArray.Length);
        foreach (RaycastHit hit in l_ObstaclesArray)
        {
            //Debug.Log("     " + hit.transform.parent);
            Obstacle l_Obstacle = hit.transform.GetComponent<Obstacle>();
            if (l_Obstacle != null)
                l_Modifier += l_Obstacle.GetCoverModifier();
        }
        return l_Modifier;
    }

    public void UnitChangedGridPosition(Unit unit, GridPosition previous_position, GridPosition new_position)
    {
        AddUnitAtGridPosition(new_position, unit);
        RemoveUnitAtGridPosition(previous_position, unit);
    }

    public void AddUnitAtGridPosition(GridPosition position, Unit unit)
    {
        m_GridSystem.GetGridObject(position).AddUnit(unit);
    }

    public void RemoveUnitAtGridPosition(GridPosition position, Unit unit)
    {
        m_GridSystem.GetGridObject(position).RemoveUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition position)
    {
        return m_GridSystem.GetGridObject(position).GetUnitList();
    }

    public bool IsOccupied(GridPosition position)
    {
        return m_GridSystem.GetGridObject(position).IsOccupied();
    }
}
