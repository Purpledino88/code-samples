using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    //Singleton code
    public static Pathfinder Instance { get; private set; }

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private const float MAX_MOBILITY_MODIFIER = 11.0f;

    [SerializeField] private GameObject m_PathDebugObjectPrefab;
    [SerializeField] private LayerMask m_ObstaclesLayerMask;

    private int m_Height;
    private int m_Width;

    private GridSystem<PathNode> m_GridSystem;

    private void Awake()
    {
         if (Instance != null)
        {
            Debug.LogError("Multiple instances of Pathfinder singleton object: " + Instance + " - " + this);
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void Setup(int width, int height)
    {
        m_Width = width;
        m_Height = height;
        m_GridSystem = new GridSystem<PathNode>(m_Width, m_Height, (GridSystem<PathNode> sys, GridPosition pos) => new PathNode(pos));
        //m_GridSystem.CreateDebugObjects(m_PathDebugObjectPrefab);

        for (int x = 0; x < m_Width; x++)
        {
            for (int z = 0; z < m_Height; z++)
            {
                GridPosition l_GridPosition = new GridPosition(x, z);
                Vector3 l_WorldPosition = LevelGrid.Instance.GetWorldPosition(l_GridPosition);
                if (Physics.Raycast(l_WorldPosition + Vector3.down, Vector3.up, out RaycastHit hit, 5, m_ObstaclesLayerMask))
                {
                    Obstacle l_Obstacle = hit.transform.GetComponent<Obstacle>();
                    GetNode(x, z).SetMobilityModifier(1f + l_Obstacle.GetMobilityModifier());
                }
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition start_position, GridPosition target_position, out int return_path_length)
    {
        List<PathNode> l_OpenList = new List<PathNode>();
        List<PathNode> l_ClosedList = new List<PathNode>();

        PathNode l_StartNode = m_GridSystem.GetGridObject(start_position);
        PathNode l_TargetNode = m_GridSystem.GetGridObject(target_position);
        l_OpenList.Add(l_StartNode);

        for (int x = 0; x < m_Width; x++)
        {
            for (int z = 0; z < m_Height; z++)
            {
                GridPosition l_CurrentPosition = new GridPosition(x, z);
                PathNode l_CurrentNode = m_GridSystem.GetGridObject(l_CurrentPosition);
                l_CurrentNode.Reset();
            }
        }

        l_StartNode.SetWalkingCost(0);
        l_StartNode.SetHeuristicCost(CalculateDistance(start_position, target_position));

        while (l_OpenList.Count > 0)
        {
            PathNode l_CurrentNode = GetNodeWithShortestPath(l_OpenList);

            if (l_CurrentNode == l_TargetNode)
            {
                return_path_length = l_CurrentNode.GetTotalCost();
                return CalculatePath(l_CurrentNode);
            }

            l_OpenList.Remove(l_CurrentNode);
            l_ClosedList.Add(l_CurrentNode);

            foreach (PathNode neighbour in GetNeighbours(l_CurrentNode))
            {
                if (!l_ClosedList.Contains(neighbour))
                {
                    if (neighbour.GetMobilityModifier() < MAX_MOBILITY_MODIFIER)
                    {
                        int l_DistanceToNeighbour = Mathf.RoundToInt(CalculateDistance(l_CurrentNode.GetGridPosition(), neighbour.GetGridPosition()) * neighbour.GetMobilityModifier());
                        int l_TentativeWalkCost = l_CurrentNode.GetWalkingCost() + l_DistanceToNeighbour;

                        if (l_TentativeWalkCost < neighbour.GetWalkingCost())
                        {
                            neighbour.SetPreviousNode(l_CurrentNode);
                            neighbour.SetWalkingCost(l_TentativeWalkCost);
                            neighbour.SetHeuristicCost(CalculateDistance(neighbour.GetGridPosition(), target_position));

                            if (!l_OpenList.Contains(neighbour))
                                l_OpenList.Add(neighbour);
                        }
                    }
                    else
                    {
                        l_ClosedList.Add(neighbour);
                    }
                }
            }
        }

        return_path_length = 0;
        return null;
    }

    private int CalculateDistance(GridPosition pos_a, GridPosition pos_b)
    {
        int l_deltaX = Mathf.Abs(pos_a.x - pos_b.x);
        int l_deltaZ = Mathf.Abs(pos_a.z - pos_b.z);
        int l_DiagonalMovement = Mathf.Min(l_deltaX, l_deltaZ);
        int l_StraightMovement = Mathf.Abs(l_deltaX - l_deltaZ);
        return ((l_DiagonalMovement * MOVE_DIAGONAL_COST) + (l_StraightMovement * MOVE_STRAIGHT_COST));
    }

    private List<GridPosition> CalculatePath(PathNode final_node)
    {
        List<GridPosition> l_Path = new List<GridPosition>();

        PathNode l_CurrentNode = final_node;
        l_Path.Add(final_node.GetGridPosition());

        while (l_CurrentNode.GetPreviousNode() != null)
        {
            l_Path.Add(l_CurrentNode.GetGridPosition());
            l_CurrentNode = l_CurrentNode.GetPreviousNode();
        }

        l_Path.Reverse();

        return l_Path;
    }

    private PathNode GetNode(int x, int z)
    {
        return m_GridSystem.GetGridObject(new GridPosition(x, z));
    }

    private PathNode GetNodeWithShortestPath(List<PathNode> list_of_nodes)
    {
        PathNode l_BestNode = list_of_nodes[0];
        for (int i = 0; i < list_of_nodes.Count; i++)
        {
            if (list_of_nodes[i].GetTotalCost() < l_BestNode.GetTotalCost())
                l_BestNode = list_of_nodes[i];
        }
        return l_BestNode;
    }

    private List<PathNode> GetNeighbours(PathNode central_node)
    {
        List<PathNode> l_NeighbourList = new List<PathNode>();

        GridPosition l_CentralPosition = central_node.GetGridPosition();

        TryAddNeighbourNodeToList(l_NeighbourList, l_CentralPosition.x + 1, l_CentralPosition.z - 1);
        TryAddNeighbourNodeToList(l_NeighbourList, l_CentralPosition.x + 1, l_CentralPosition.z + 0);
        TryAddNeighbourNodeToList(l_NeighbourList, l_CentralPosition.x + 1, l_CentralPosition.z + 1);
        TryAddNeighbourNodeToList(l_NeighbourList, l_CentralPosition.x + 0, l_CentralPosition.z - 1);
        TryAddNeighbourNodeToList(l_NeighbourList, l_CentralPosition.x + 0, l_CentralPosition.z + 1);
        TryAddNeighbourNodeToList(l_NeighbourList, l_CentralPosition.x - 1, l_CentralPosition.z - 1);
        TryAddNeighbourNodeToList(l_NeighbourList, l_CentralPosition.x - 1, l_CentralPosition.z + 0);
        TryAddNeighbourNodeToList(l_NeighbourList, l_CentralPosition.x - 1, l_CentralPosition.z + 1);

        return l_NeighbourList;
    }

    private void TryAddNeighbourNodeToList(List<PathNode> neighbour_list, int try_x, int try_z)
    {
        if ((try_x >= 0) && (try_x < m_Width) && (try_z >= 0) && (try_z < m_Height))
        {
            neighbour_list.Add(m_GridSystem.GetGridObject(new GridPosition(try_x, try_z)));
        }
    }
}
