﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class Pathfinder : MonoBehaviour
{
    public Transform seeker, target;

    Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
        
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            FindPath(seeker.position, target.position);
        }
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        //List<Node> openSet = new List<Node>(); // Set of nodes to be evaluated
        HashSet<Node> closedSet = new HashSet<Node>(); // Set of nodes already evaluated
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            //UnityEngine.Debug.Log(openSet.Count);
            //Node currentNode = openSet[0];
            Node currentNode = openSet.RemoveFirst();
            /*
            for (int i = 1; i < openSet.Count; i++) // Find the node with the min fCost
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }
              openSet.Remove(currentNode);
            */
            
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                sw.Stop();
                print("Path found: " + sw.ElapsedMilliseconds + " ms");
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }  
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path;
        grid.DrawPath();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX); 
    }
	
}
