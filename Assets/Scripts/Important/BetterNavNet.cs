﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterNavNet : MonoBehaviour, INavNet
{
    [SerializeField] Vector3 rootPosition;
    [SerializeField] public float distanceBetweenNavPoints = 1f;
    [SerializeField] public static BetterNavNet staticRef;

    [SerializeField] int radius = 20;

    public static List<NavPointV2> allNavPoints = new List<NavPointV2>();
    private static Vector3[] relativeNavPointPositions = new Vector3[6];

    static Dictionary<NavPointV2, NavPointV2> pathDictionaryValueIsOrigin;
    //static List<NavPointV2> processedNavPoints;

    void Start()
    {
        if (!staticRef)
        {
            staticRef = this;
        }
        SetRelativePositionVectors();
    }
    private void SetRelativePositionVectors()
    {
        float sqrtThree = Mathf.Sqrt(3f);
        relativeNavPointPositions[0] = new Vector3(-distanceBetweenNavPoints, 0, 0);
        relativeNavPointPositions[1] = new Vector3(-distanceBetweenNavPoints / 2, 0, sqrtThree * distanceBetweenNavPoints / 2);
        relativeNavPointPositions[2] = new Vector3(distanceBetweenNavPoints / 2, 0, sqrtThree * distanceBetweenNavPoints / 2);
        relativeNavPointPositions[3] = new Vector3(distanceBetweenNavPoints, 0, 0);
        relativeNavPointPositions[4] = new Vector3(distanceBetweenNavPoints / 2, 0, -sqrtThree * distanceBetweenNavPoints / 2);
        relativeNavPointPositions[5] = new Vector3(-distanceBetweenNavPoints / 2, 0, -sqrtThree * distanceBetweenNavPoints / 2);
    }
    public static List<INavPoint> FindPath(NavPointV2 arg_start, NavPointV2 arg_destination, int arg_pathWidth)
    {
        arg_start = FindNearestLegitimateNavPoint(arg_start, arg_pathWidth);
        arg_destination = FindNearestLegitimateNavPoint(arg_destination, arg_pathWidth);
        int processedNavPointID = 0;
        NavPointV2 nPoint;
        bool isPathFound = false;

        if (arg_start == null)
        {
            Debug.Log("Start is null");
        }
        if (arg_destination == null)
        {
            Debug.Log("Destination is null");
        }

        pathDictionaryValueIsOrigin = new Dictionary<NavPointV2, NavPointV2>();
        List<NavPointV2> spottedNavPoints = new List<NavPointV2>();
        //List<INavPoint> path = new List<INavPoint>();
        if (arg_start != null)
        {
            pathDictionaryValueIsOrigin.Add(arg_start as NavPointV2, arg_start as NavPointV2);

        }
        else
        {
            Debug.Log("PathFind: argument Start is null");
        }
        spottedNavPoints.Add(arg_start as NavPointV2);
        while (!isPathFound && processedNavPointID < spottedNavPoints.Count)
        {
            nPoint = spottedNavPoints[processedNavPointID];
            for (int direction = 0; direction < 6; direction++)
            {
                NavPointV2 neighbour = nPoint.neighbours[direction];
                if (neighbour != null &&
                    !neighbour.OverlapWithCollider() && 
                    neighbour.obstacleProximity >= arg_pathWidth)
                {
                    if (neighbour == arg_destination as NavPointV2 )
                    {
                        isPathFound = true;
                    }
                    if (pathDictionaryValueIsOrigin.TryGetValue(neighbour, out NavPointV2 forgetAboutIt) == false)
                    {
                        spottedNavPoints.Add(neighbour);
                        pathDictionaryValueIsOrigin.Add(neighbour, nPoint);
                    }
                }
            }
            processedNavPointID++;
        }
        if (isPathFound)
        {
            return PreparePathToReturn(arg_start as NavPointV2, arg_destination as NavPointV2);
        }
        else
        {
            //Debug.Log("no path found; Start is " + arg_start.GetPosition() + " Destination is " + arg_destination.GetPosition() );
            Debug.Log("tutaj będzie buba " + ((NavPointV2)arg_destination).distancetoNearestObject);
        }
        return null;
    }

    private static NavPointV2 FindNearestLegitimateNavPoint(NavPointV2 arg_npoint, int arg_width)
    {
        bool foundLegitimateNavPoint = false;
        List<NavPointV2> nPointList = new List<NavPointV2>();
        nPointList.Add(arg_npoint);
        int listElementIndex = 0;
        while (!foundLegitimateNavPoint)
        {
            foreach ( NavPointV2 neighbour in nPointList[listElementIndex].neighbours)
            {
                if( neighbour!= null && neighbour.overlapWithCollider != true)
                {
                    if (neighbour.obstacleProximity >= arg_width)
                    {
                        arg_npoint = neighbour;
                        foundLegitimateNavPoint = true;
                    }
                    else
                    {
                        nPointList.Add(neighbour);
                    }
                }
            }
            listElementIndex++;
        }
        return arg_npoint;
    }

    private static List<INavPoint> PreparePathToReturn(NavPointV2 arg_start, NavPointV2 arg_destination)
    {
        bool pathReadyToReturn = false;

        List<INavPoint> r_path = new List<INavPoint>();
        r_path.Add(arg_destination);
        NavPointV2 currentlyProcessedNavPoint = null;
        pathDictionaryValueIsOrigin.TryGetValue(arg_destination as NavPointV2, out currentlyProcessedNavPoint);
        while (!pathReadyToReturn)
        {
            r_path.Add(currentlyProcessedNavPoint);
            if (currentlyProcessedNavPoint == arg_start as NavPointV2)
            {
                pathReadyToReturn = true;
            }
            else
            {
                pathDictionaryValueIsOrigin.TryGetValue(currentlyProcessedNavPoint, out currentlyProcessedNavPoint);
            }
        }
        r_path.Reverse();
        return r_path;
    }

    private bool EnoughRoomToGoThrough(NavPointV2 navPoint, float pathseekerSize)
    {
        if (pathseekerSize == 0)
        {
            return true;
        }
        bool result = true;
        if (pathseekerSize <= 1)
        {
            foreach (NavPointV2 neighbour in navPoint.neighbours)
            {
                if (neighbour != null && neighbour.overlapWithCollider == true)
                {
                    result = false;
                }
            }
        }
        else
        {
            foreach (NavPointV2 neighbour in navPoint.neighbours)
            {
                if (!EnoughRoomToGoThrough(neighbour, pathseekerSize - 2))
                {
                    result = false;
                }
            }
        }
        return result;

    }
    public static void CreateNavNet()
    {
        NavPointV2 processedNavPoint = new NavPointV2();
        processedNavPoint.position = staticRef.rootPosition;
        //processedNavPoint.neighbours = new NavPointV2[6];
        //processedNavPoint.overlapWithCollider = false;
        processedNavPoint.distanceFromCentre = 0;
        allNavPoints.Add(processedNavPoint);
        int proccessedNavPointId = 0;

        while (proccessedNavPointId < allNavPoints.Count)
        {
            processedNavPoint = allNavPoints[proccessedNavPointId];
            CheckForExistingNeighbours(processedNavPoint);
            CreateNavPointsAround(processedNavPoint);
            proccessedNavPointId++;
        }
        CheckIfNavpointsOverlapWithColliders();
        ImprovedCheckingForOpenSpaces();
    }

    private static void ImprovedCheckingForOpenSpaces()
    {
        List<NavPointV2> queuedNavPoints = new List<NavPointV2>();
        MarkNavPointsNextToObstacleOrNull(queuedNavPoints);
        int processedNavPointIndex = 0;
        while ( processedNavPointIndex < queuedNavPoints.Count)
        {
            SetObstacleProximityInNeighboursAndAddToTheQueue(queuedNavPoints[processedNavPointIndex], queuedNavPoints);
            processedNavPointIndex++;
        }
        Debug.Log("counf of queuedNavPoints is " + queuedNavPoints.Count);
    }

    private static void SetObstacleProximityInNeighboursAndAddToTheQueue( NavPointV2 arg_navPoint, List<NavPointV2> arg_queuedNavPoints)
    {
        foreach(NavPointV2 neighbour in arg_navPoint.neighbours)
        {
            if (neighbour != null && neighbour.obstacleProximity == -1 && neighbour.overlapWithCollider != true)
            {
                neighbour.obstacleProximity = arg_navPoint.obstacleProximity + 1;
                arg_queuedNavPoints.Add(neighbour);
            }
        }
    }
    private static void MarkNavPointsNextToObstacleOrNull( List<NavPointV2> arg_queuedNavPoints)
    {
        foreach (NavPointV2 processedNavPoint in allNavPoints)
        {
            bool nullNeighbours = false;
            if (processedNavPoint.overlapWithCollider)
            {
                SetObstacleProximityInNeighboursAndAddToTheQueue(processedNavPoint, arg_queuedNavPoints);
            }
            else
            {

                foreach (NavPointV2 neighbour in processedNavPoint.neighbours)
                {
                    if (neighbour == null)
                    {
                        nullNeighbours = true;
                    }
                }
            }
            if (nullNeighbours)
            {
                arg_queuedNavPoints.Add(processedNavPoint);
                processedNavPoint.obstacleProximity = 0;
            }
        }
    }

    private static void CheckForOpenSpacesInNavNet()
    {
        foreach (NavPointV2 navPoint in allNavPoints)
        {
            if (navPoint.overlapWithCollider == false)
            {
                CheckForOpenSpacesAroundArg(navPoint);
            }
        }
    }

    private static void CheckForExistingNeighbours(NavPointV2 nPoint)
    {
        for (int direction = 0; direction < 6; direction++)
        {
            if (nPoint.neighbours[direction] != null)
            {
                CheckForCommonNeighbours(direction, nPoint, nPoint.neighbours[direction]);
            }
        }
    }
    private static void CheckForOpenSpacesAroundArg(NavPointV2 arg)
    {
        float distanceFromNavPoint = 0;
        bool foundNearestCollision = false;
        List<NavPointV2> navPointsSomewhatProcessed = new List<NavPointV2>();
        navPointsSomewhatProcessed.Add(arg);
        int processedNavPointId = -1;

        while (!foundNearestCollision)
        {
            processedNavPointId++;
            NavPointV2 processedNavPoint = navPointsSomewhatProcessed[processedNavPointId];

            if (processedNavPoint.overlapWithCollider)
            {
                foundNearestCollision = true;
                distanceFromNavPoint = (arg.position - processedNavPoint.position).magnitude;
            }
            else
            {
                foreach (NavPointV2 neighbour in processedNavPoint.neighbours)
                {
                    if (neighbour != null)
                    {
                        if (!navPointsSomewhatProcessed.Contains(neighbour))
                        {
                            navPointsSomewhatProcessed.Add(neighbour);
                        }
                    }
                    else
                    {
                        foundNearestCollision = true;
                        distanceFromNavPoint = (arg.position - processedNavPoint.position).magnitude;
                    }
                }
            }
        }
        arg.distancetoNearestObject = distanceFromNavPoint - BetterNavNet.staticRef.distanceBetweenNavPoints;
        if (arg.distancetoNearestObject < 0f) // most likely not the best solution in the world, but should do the trick
        {
            //Debug.LogError("t");
            arg.distancetoNearestObject = 0.0001f;
        }
    }

    public static void CreateNavPointsAround(NavPointV2 nPoint)
    {
        nPoint.distanceFromCentre++;
        if (nPoint.distanceFromCentre <= staticRef.radius)
        {
            for (int direction = 0; direction < 6; direction++)
            {
                if (nPoint.neighbours[direction] == null)
                {
                    NavPointV2 newNavPoint = new NavPointV2(nPoint.position + relativeNavPointPositions[direction], nPoint.distanceFromCentre);
                    newNavPoint.neighbours = new NavPointV2[6];
                    nPoint.neighbours[direction] = newNavPoint;
                    newNavPoint.neighbours[(direction + 3) % 6] = nPoint;
                    allNavPoints.Add(nPoint.neighbours[direction]);

                }
            }
        }
    }

    private static void CheckForCommonNeighbours(int direction, NavPointV2 callerNavPoint, NavPointV2 knownNeighbour)
    {
        NavPointV2 commonNeighbour = null;
        if (knownNeighbour.neighbours[(direction + 2) % 6] != null)
        {
            commonNeighbour = knownNeighbour.neighbours[(direction + 2) % 6];
            commonNeighbour.neighbours[(direction + 4) % 6] = callerNavPoint;
            callerNavPoint.neighbours[(direction + 1) % 6] = commonNeighbour;
        }
        if (knownNeighbour.neighbours[(direction + 4) % 6] != null)
        {
            commonNeighbour = knownNeighbour.neighbours[(direction + 4) % 6];
            commonNeighbour.neighbours[(direction + 2) % 6] = callerNavPoint;
            callerNavPoint.neighbours[(direction + 5) % 6] = commonNeighbour;
        }
    }

    public static void CheckIfNavpointsOverlapWithColliders()
    {
        foreach (NavPointV2 npoint in allNavPoints)
        {
            if (!npoint.OverlapWithCollider())
            {
                for (int direction = 0; direction < 6; direction++)
                {
                    if (npoint.neighbours[direction] != null &&
                        Physics.Raycast( npoint.GetPosition(), relativeNavPointPositions[direction], staticRef.distanceBetweenNavPoints, (int)Layers.Obstacles)
                        )
                    {
                        npoint.neighbours[direction].overlapWithCollider = true;
                    }
                }
            }
        }
    }
    public static NavPointV2 FindNearestNavpoint(NavPointV2 arg_oldNearestNavPoint, Vector3 arg_position)
    {

        float distanceFromNearestNavPoint = 6666;
        if (arg_oldNearestNavPoint == null)
        {
            foreach (NavPointV2 processedNavPoint in BetterNavNet.allNavPoints)
            {
                float distanceFromProcessedNavPoint = (processedNavPoint.GetPosition() - arg_position).magnitude;
                if (processedNavPoint.overlapWithCollider == false && (
                    arg_oldNearestNavPoint == null || distanceFromNearestNavPoint > distanceFromProcessedNavPoint) )
                {
                    arg_oldNearestNavPoint = processedNavPoint;
                    distanceFromNearestNavPoint = distanceFromProcessedNavPoint;
                }
            }
        }
        else
        {
            for (int dir = 0; dir < 6; dir++)
            {
                NavPointV2 processedNavPoint = arg_oldNearestNavPoint.neighbours[dir];
                if (processedNavPoint !=null && 
                    processedNavPoint.overlapWithCollider == false &&  
                    distanceFromNearestNavPoint > (processedNavPoint.position - arg_position).magnitude)
                {
                    arg_oldNearestNavPoint = processedNavPoint;
                    distanceFromNearestNavPoint = (processedNavPoint.position - arg_position).magnitude;
                }
            }
        }
        return arg_oldNearestNavPoint;
    }
    public static NavPointV2 FindNearestWayPointOutOfSight(NavPointV2 arg_location, float arg_size, float arg_distance)
    {
        NavPointV2 result = null;

        return result;
    }
}


public class NavPointV2 : INavPoint
{
    public Vector3 position;
    public NavPointV2[] neighbours;
    public bool overlapWithCollider;
    public float distancetoNearestObject = 0;
    public int obstacleProximity;
    public int distanceFromCentre;

    public Vector3 GetPosition()
    {
        return position;
    }
    public bool OverlapWithCollider()
    {
        return overlapWithCollider;
    }

    public NavPointV2()
    {
        obstacleProximity = -1;
        position = new Vector3();
        neighbours = new NavPointV2[6];
        overlapWithCollider = false;
        distanceFromCentre = 0;
    }
    public NavPointV2(Vector3 argPosition, int argDistance)
    {
        obstacleProximity = -1;
        position = argPosition;
        neighbours = new NavPointV2[6];
        overlapWithCollider = false;
        distanceFromCentre = argDistance;
        ;
    }

}
