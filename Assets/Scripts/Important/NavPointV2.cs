using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavPointV2 : INavPoint
{
    public Vector3 position;
    public NavPointV2[] neighbours;
    public bool overlapWithCollider;
    public float distancetoNearestObject = 0;
    public int obstacleProximity;
    public int distanceFromCentre;
    public int vision;

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
        vision = 5000;
        obstacleProximity = -1;
        position = new Vector3();
        neighbours = new NavPointV2[6];
        overlapWithCollider = false;
        distanceFromCentre = 0;
    }
    public NavPointV2(Vector3 argPosition, int argDistance)
    {
        vision = 5000;
        obstacleProximity = -1;
        position = argPosition;
        neighbours = new NavPointV2[6];
        overlapWithCollider = false;
        distanceFromCentre = argDistance;
    }
    public static bool Exist(NavPointV2 arg_point)
    {
        return (arg_point != null && arg_point.overlapWithCollider != true);
    }
}
