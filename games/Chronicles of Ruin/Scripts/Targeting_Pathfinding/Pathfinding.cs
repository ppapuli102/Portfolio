using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour {

    PathRequestManager requestManager;
    static Grid grid;
    Vector3[] waypoints;


    private void Awake() {
        grid = GetComponent<Grid>();
        requestManager = GetComponent<PathRequestManager>();
        //Debug.Log(GetPathDistance(new Vector3(0,1,0), new [] {new Vector3(0,-1,0), new Vector3(-1,-2,0), new Vector3(-4,1,0)}));
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos) {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    private IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {
        waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable) {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0) { 
                Node currentNode = openSet.RemoveFirst();            
                closedSet.Add(currentNode);

                if (currentNode == targetNode) {
                    // then we have reached our goal, exit the loop
                    pathSuccess = true;
                    break;
                }
                // Now that the lowest F cost node is selected, we can search its neighbours to find the next lowest F cost to become the current node
                foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;  // The last neighbour creates the chain of parents to the start node

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }

        //yield return null;
        if (pathSuccess) {
            waypoints = RetracePath(startNode, targetNode);
            pathSuccess = waypoints.Length > 0;
        }
        yield return null;
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    public Vector3[] FindValidPaths(Vector3 startPos, Vector3 targetPos) {
        waypoints = new Vector3[0];

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable) {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0) { 
                Node currentNode = openSet.RemoveFirst();            
                closedSet.Add(currentNode);

                if (currentNode == targetNode) {
                    // then we have reached our goal, exit the loop
                    break;
                }
                // Now that the lowest F cost node is selected, we can search its neighbours to find the next lowest F cost to become the current node
                foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;  // The last neighbour creates the chain of parents to the start node

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }
        waypoints = RetracePath(startNode, targetNode);

        return waypoints;
    }

    private Vector3[] RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);

        return waypoints;
    }

    private Vector3[] SimplifyPath(List<Node> path) {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        if (path.Count != 1) {
            for (int i = 0; i < path.Count - 1; i++) {
                Vector2 directionNew = new Vector2(path[i].gridX - path[i + 1].gridX, path[i].gridY - path[i + 1].gridY);
                if (directionNew != directionOld) {
                    waypoints.Add(path[i].worldPosition);
                }
                directionOld = directionNew;
            }
        } else {
            waypoints.Add(path[0].worldPosition);
        }
        return waypoints.ToArray();
    }

    public bool HasPath(int gridX, int gridY, int targetX, int targetY, int distance) { 
        bool hasPath = false;
        int totalDistance = 0;

        //Debug.Log(grid.NodeFromWorldPoint(new Vector3(targetX, targetY, 0)).worldPosition); // Debugging

        Vector3 startPos = new Vector3(gridX, gridY, 0);
        Vector3 targetPos = new Vector3(targetX, targetY, 0);
        // Store the waypoints from starting point to target point
        waypoints = FindValidPaths(startPos, targetPos);

        // Add up the distance from starting point to the last waypoint
        totalDistance = GetPathDistance(startPos, waypoints);

        //Debug.Log(totalDistance + GetPathDistance(waypoints)); //Debugging the distance to each node
        if (totalDistance <= distance) {
            hasPath = true;
        }
        //Debug.Log(totalDistance);
        //Debug.Log(hasPath); //Debugging hasPath boolean
        return hasPath;
    }

    int GetDistance(Node nodeA, Node nodeB) {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 20*dstY + 10 * (dstX-dstY);
        return 20*dstX + 10 * (dstY-dstX);
    }

    public int GetPathDistance(Vector3 startPos, Vector3[] waypoints) {
        int totalDistance = 0;
        // Add up the distance from starting point to the last waypoint
        if (waypoints.Length > 0)
            totalDistance += GetDistance(grid.NodeFromWorldPoint(startPos), grid.NodeFromWorldPoint(waypoints[0]));
        
        for (int i = 0; i < waypoints.Length - 1; i ++) {
            totalDistance += GetDistance(grid.NodeFromWorldPoint(waypoints[i]), grid.NodeFromWorldPoint(waypoints[i+1]));
        }
        return totalDistance / 10;
    }

}

