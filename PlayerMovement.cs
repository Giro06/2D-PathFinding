using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public _2D_Pathfinding_Giro pathfinder;
    private List<Node> myPath=new List<Node>();
    private LineRenderer pathRender;
    void Start()
    {
        pathRender = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {     
           pathfinder.GenerateDistanceMap(pathfinder.FindClosestNodePosition(Camera.main.ScreenToWorldPoint(Input.mousePosition)),pathfinder.FindClosestNodePosition(transform.position));
           
           myPath= pathfinder.FindPath(pathfinder.FindClosestNodePosition(transform.position), 
                   pathfinder.FindClosestNodePosition(Camera.main.ScreenToWorldPoint(Input.mousePosition))); 
           DrawPath(myPath);
        }
    }

    void DrawPath(List<Node> path)
    {
        pathRender.numPositions = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            pathRender.SetPosition(i,path[i].position);
        }
    }
}
