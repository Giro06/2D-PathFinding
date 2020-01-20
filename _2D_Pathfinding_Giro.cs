using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector2 position=new Vector2();
    public bool isActive;
    public float distanceToTarget;
    public List<Node> connectedNodes;

    public Node cameFromNode;
    public float gCost;
    public float hCost;
    public float fCost;
    public Node(Vector2 position,bool isActive,float distanceToTarget)
    {
        this.position = position;
        this.isActive = isActive;
        this.distanceToTarget = distanceToTarget;
        connectedNodes=new List<Node>();
        
    }

    public Node()
    {
        connectedNodes=new List<Node>();
    }
  

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
}
 
public class _2D_Pathfinding_Giro : MonoBehaviour
{

    public float widht=1;
    public float height=1;

    public int rowCount=1;
    public int columnCount=1;

    private float distanceBetweenNodes_Widht;
    private float distanceBetweenNodes_Height;

    private Node[,] grids;
    private Vector2[,] drawVector;
    public Vector2 startPoint;
   
    private List<Node> openList;
    private List<Node> closedList;
   public void GenerateGrids(float widht,float height,int rowCount,int columnCount,Vector2 startPoint)
    {
        distanceBetweenNodes_Widht = CalculateDistance(widht, columnCount);
        distanceBetweenNodes_Height = CalculateDistance(height, rowCount);
        grids = new Node[columnCount,rowCount];
        
        for (int x = 0; x < columnCount; x++)
        {
            for (int y = 0; y < rowCount; y++)
            {
                grids[x,y]=new Node(startPoint + new Vector2(x*distanceBetweenNodes_Widht,-y*distanceBetweenNodes_Height),false,999999);
               
            }
        }
        for (int x = 0; x < columnCount; x++)
        {
            for (int y = 0; y < rowCount; y++)
            {
                if(x>0)
                    grids[x,y].connectedNodes.Add(grids[x-1,y]);
                if(x<columnCount-1)
                    grids[x,y].connectedNodes.Add(grids[x+1,y]);
                if(y>0)
                    grids[x,y].connectedNodes.Add(grids[x,y-1]);
                if(y<rowCount-1)
                    grids[x,y].connectedNodes.Add(grids[x,y+1]);
                
                if(x>0 && y>0)
                    grids[x,y].connectedNodes.Add(grids[x-1,y-1]);
                if(x<columnCount-1 && y<rowCount-1)
                    grids[x,y].connectedNodes.Add(grids[x+1,y+1]);
                if(y>0 && x<columnCount-1)
                    grids[x,y].connectedNodes.Add(grids[x+1,y-1]);
                if(x<0 && y<rowCount-1)
                    grids[x,y].connectedNodes.Add(grids[x+1,y-1]);
            }
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BakeMap();    
        }

     
    }

   public void BakeMap()
    {
        GenerateGrids(widht,height,rowCount,columnCount,startPoint);
        for (int x = 0; x < columnCount; x++)
        {
            for (int y = 0; y < rowCount; y++)
            {
                if (CheckForCollision(grids[x, y]))
                {
                    grids[x, y].isActive = false;

                }
                else
                {
                  
                    grids[x, y].isActive = true;
                }
             
            }
        }
        
    }

   public List<Node> FindPath(Node start, Node end)
   {
       Node startNode = start;
       Node endNode = end;

       if (startNode == null || endNode == null)
           return null;

       openList=new List<Node>{startNode};
       closedList=new List<Node>();

       for (int x = 0; x < columnCount; x++)
       {
           for (int y = 0; y < rowCount; y++)
           {
               Node pathNode = grids[x, y];
               pathNode.gCost = 9999999;
               pathNode.CalculateFCost();
               pathNode.cameFromNode = null;
           }
       }

       startNode.gCost = 0;
       startNode.hCost = CalculateDistanceCost(startNode, endNode);
       startNode.CalculateFCost();
       while (openList.Count > 0)
       {
           Node currentNode = GetLowestFCostNode(openList);
           if (currentNode == endNode)
           {
               return CalculatePath(endNode);
           }
           openList.Remove(currentNode);
           closedList.Add(currentNode);
           foreach (Node neighbourNode in currentNode.connectedNodes)
           {
               if(closedList.Contains(neighbourNode)) continue;
               if (!neighbourNode.isActive)
               {
                   closedList.Add(neighbourNode);
                   continue;
               }

               float tentativeGcost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
               if (tentativeGcost < neighbourNode.gCost)
               {
                   neighbourNode.cameFromNode = currentNode;
                   neighbourNode.gCost = tentativeGcost;
                   neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                   neighbourNode.CalculateFCost();

                   if (!openList.Contains(neighbourNode))
                   {
                       openList.Add(neighbourNode);
                   }
               }

           }
       }

       return null;

   }
    
  
   public void GenerateDistanceMap(Node targetNode,Node current)
    {
        foreach (Node x in grids)
        {
            if (x.isActive)
            {
                x.distanceToTarget = (x.position - targetNode.position).magnitude+(x.position-current.position).magnitude;
            }
        }
    }
   public Node FindClosestNodePosition(Vector2 position)
    {
        Node temp=new Node();
        float distance=100;

        for (int x = 0; x < columnCount; x++)
        {
            for (int y = 0; y < rowCount; y++)
            {
                if ((grids[x, y].position - position).magnitude < distance)
                {
                    temp=grids[x,y];
                    distance = (grids[x, y].position - position).magnitude;
                }
            }
        }

        return temp;
    }
   public float CalculateDistance(float length,int gridCount)
    {
        return  length/(gridCount - 1);
    }

 
   public bool CheckForCollision(Node point)
    {

        RaycastHit2D hit = Physics2D.Raycast(point.position, Vector2.zero);

        if (hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
   // DRAWİNGGG*******************************************************
   void DrawVectors()
   {
       distanceBetweenNodes_Widht = CalculateDistance(widht, columnCount);
       distanceBetweenNodes_Height = CalculateDistance(height, rowCount);
      drawVector=new Vector2[columnCount,rowCount];
        
       for (int x = 0; x < columnCount; x++)
       {
           for (int y = 0; y < rowCount; y++)
           {
               drawVector[x,y]=startPoint + new Vector2(x*distanceBetweenNodes_Widht,-y*distanceBetweenNodes_Height);
               
           }
       }
      
   }
   public bool CheckForCollisionForDrawing(Vector2 point)
   {

       RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);

       if (hit.collider != null)
       {
           return true;
       }
       else
       {
           return false;
       }
   }
    private void OnDrawGizmos()
    {   DrawVectors();
        if (drawVector.Length != null)
        {
            for (int x = 0; x < columnCount; x++)
            {
                for (int y = 0; y < rowCount; y++)
                {
                    if (CheckForCollisionForDrawing(drawVector[x, y]))
                    {
                        Gizmos.color = Color.red;
                        if(x<columnCount-1)
                            Gizmos.DrawLine(drawVector[x,y],drawVector[x+1,y]);
                
                        if(y<rowCount-1)
                            Gizmos.DrawLine(drawVector[x,y],drawVector[x,y+1]);
                    }
                    else
                    {   
                        Gizmos.color=Color.white;
                        if(x<columnCount-1)
                            Gizmos.DrawLine(drawVector[x,y],drawVector[x+1,y]);
                
                        if(y<rowCount-1)
                            Gizmos.DrawLine(drawVector[x,y],drawVector[x,y+1]);
                    }
             
                }
            }
        }
       
    }
    // UTİLSSS
    public float CalculateDistanceCost(Node start, Node end)
    {   
        return (start.position - end.position).magnitude;
    }

    Node GetLowestFCostNode(List<Node> openList)
    {
        Node lowestFCostNode = openList[0];
        for (int i = 1; i < openList.Count; i++) {
            if (openList[i].fCost < lowestFCostNode.fCost) {
                lowestFCostNode = openList[i];
            }
        }
        return lowestFCostNode;
    }
    private List<Node> CalculatePath(Node endNode) {
        List<Node> path = new List<Node>();
        path.Add(endNode);
        Node currentNode = endNode;
        while (currentNode.cameFromNode != null) {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }
}
