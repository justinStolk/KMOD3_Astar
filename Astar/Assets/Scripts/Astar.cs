using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
    /// Note that you will probably need to add some helper functions
    /// from the startPos to the endPos
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    /// 

    private Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node>();
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        List<Node> openNodes = new List<Node>();
        List<Node> closedNodes = new List<Node>();
        
        Node startNode = new Node(startPos, null, 0, Mathf.Abs((startPos.x - endPos.x)) + Mathf.Abs((startPos.y - endPos.y)));
        openNodes.Add(startNode);
        while (openNodes.Count > 0)
        {
            Node currentNode = openNodes.OrderBy((x) => x.FScore).First();
            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);
            if (currentNode.position == endPos)
            {
                List<Vector2Int> path = new List<Vector2Int>();
                path.Add(currentNode.position);
                while(currentNode != startNode)
                {
                    path.Add(currentNode.parent.position);
                    currentNode = currentNode.parent;
                }
                Debug.Log("I got the path!");
                path.Reverse();
                return path;
            }

            List<Node> neighbours = GetNeighbours(currentNode, grid.GetLength(0), grid.GetLength(1));
            foreach(Node neighbour in neighbours)
            {
                Cell currentCell = grid[currentNode.position.x, currentNode.position.y];
                Cell neighbourCell = grid[neighbour.position.x, neighbour.position.y];
                if (closedNodes.Contains(neighbour))
                {
                    continue;
                }
                float tentativeGScore = currentNode.GScore + GetDistance(currentNode, neighbour);
                if (tentativeGScore < neighbour.GScore || !openNodes.Contains(neighbour))
                {
                    neighbour.GScore = tentativeGScore;
                    neighbour.HScore = Vector2.Distance(neighbour.position, endPos);
                    neighbour.parent = currentNode;
                    if (IsTraversable(currentCell, neighbourCell) && !openNodes.Contains(neighbour))
                    {
                        openNodes.Add(neighbour);
                    }
                }
            }
        }
        return null;
    }

    private List<Node> GetNeighbours(Node startNode, int width, int height)
    {
        List<Node> result = new List<Node>();
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                int nodeX = startNode.position.x + x;
                int nodeY = startNode.position.y + y;
                if (nodeX < 0 || nodeX >= width || nodeY < 0 || nodeY >= height || Mathf.Abs(x) == Mathf.Abs(y))
                {
                    continue;
                }
                Vector2Int neighbourPosition = new Vector2Int(nodeX, nodeY);
                if (!nodes.ContainsKey(neighbourPosition))
                {
                    Node neighbour = new Node(neighbourPosition, startNode, Mathf.RoundToInt(startNode.GScore + Vector2Int.Distance(startNode.position, neighbourPosition)), 0);
                    nodes.Add(neighbourPosition, neighbour);
                }
                result.Add(nodes[neighbourPosition]);
            }
        }
        return result;
    }
    private bool IsTraversable(Cell startCell, Cell neighbour)
    {
        if (neighbour.gridPosition.x > startCell.gridPosition.x && neighbour.HasWall(Wall.LEFT) )
        {
            return false;
        }
        else if (neighbour.gridPosition.x < startCell.gridPosition.x && neighbour.HasWall(Wall.RIGHT))
        {
            return false;
        }
        else if (neighbour.gridPosition.y < startCell.gridPosition.y && neighbour.HasWall(Wall.UP))
        {
            return false;
        }
        else if (neighbour.gridPosition.y > startCell.gridPosition.y && neighbour.HasWall(Wall.DOWN))
        {
            return false;
        }
        return true;
    }
    private int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.position.x - nodeB.position.x);
        int distanceY = Mathf.Abs(nodeA.position.y - nodeB.position.y);
        return 1 * distanceX + 1 * distanceY;
    }
    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
