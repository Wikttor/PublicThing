using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVision : MonoBehaviour
{
    public GameObject myprefab;
    public static PlayerVision sRef;
    public static bool initialized = false;

    public NavPointV2[] lastVisiblePoints, nextList;
    public int pointCount, nextCount;

    //public int i_visionRange = 100;
    //public int i_maxTrackedDistance = 15;

    private void Start()
    {
        if (sRef == null)
        {
            sRef = this;
        }
    }

    void Update()
    {
        if (initialized)
        {
            UpdateVision();
        }else if (BetterNavNet.navNetCreated && /*/nPointVisionDict == null && */PlayerMovementAndRotation.nearestNavPoint != null)
        {
            initialized = true;
            lastVisiblePoints = new NavPointV2[100000];
            nextList = new NavPointV2[100000];
            PlayerMovementAndRotation.nearestNavPoint.vision = 0;
            lastVisiblePoints[pointCount] = PlayerMovementAndRotation.nearestNavPoint;
            pointCount++;
        }
    }

    public void UpdateVision()
    {
        Debug.Log("tralala");
        List<NavPointV2> visionOne = new List<NavPointV2>();
        List<NavPointV2> visionZero = new List<NavPointV2>();
        nextCount = 0;
        for ( int i = 0; i < pointCount; i++)
        {
            NavPointV2 point = lastVisiblePoints[i];
            if (CheckLOS(point))
            {
                bool stillLastVisible = false;
                foreach (NavPointV2 neighbour in point.neighbours)
                {
                    bool neighbourInLOS = false;
                    if (!NavPointV2.Exist(neighbour) || (neighbour.vision > 0 && !(neighbourInLOS = CheckLOS(neighbour)) ) )
                    {
                        stillLastVisible = true;
                    }
                    if (NavPointV2.Exist(neighbour) && neighbourInLOS && neighbour.vision > 0 && !NavPointOnList(neighbour, nextList, nextCount) && !visionOne.Contains(neighbour) )
                    {
                        nextList[nextCount] = neighbour;
                        nextCount++;
                        //neighbour.vision = 0;
                        visionZero.Add(neighbour);
                    }
                }
                if (  (stillLastVisible || point.obstacleProximity == 0) && !NavPointOnList(point, nextList, nextCount) )
                {
                    nextList[nextCount] = point;
                    nextCount++;
                }
            }
            else
            {
                visionOne.Add(point);
                //point.vision = 1;
                foreach (NavPointV2 neighbour in point.neighbours)
                {
                    if (NavPointV2.Exist(neighbour) && 
                        neighbour.vision == 0 && 
                        !NavPointOnList(neighbour, nextList, nextCount) && 
                        !visionOne.Contains(neighbour) &&
                        !NavPointOnList(neighbour, lastVisiblePoints, pointCount) )
                    {
                        nextList[nextCount] = neighbour;
                        nextCount++;
                    }
                }
            }
        }
       
        foreach (NavPointV2 ss in visionOne)
        {
            ss.vision = 1;
        }

        foreach (NavPointV2 zz in visionZero)
        {
            zz.vision = 0;
        }
        for (int i = 0; i < nextCount; i++)
        {
            nextList[i].vision = 0;
        }
        //VISUALISATION
        if (Input.GetKeyDown(KeyCode.V))
        {
            for (int i = 0; i < nextCount; i++)
            {
                GameObject marker = GameObject.Instantiate(myprefab, nextList[i].position, Quaternion.identity);
                Destroy(marker, 0.2f);
            }
            Debug.Log(nextCount);
        }
        for (int i = 0; i < nextCount; i++)
        {
            lastVisiblePoints[i] = nextList[i];
        }
        pointCount = nextCount;
    }

    private bool CheckLOS(NavPointV2 point)
    {
        float rayRange = (transform.position - point.position).magnitude;
        Ray ray = new Ray(transform.position, point.position - transform.position);
        return !Physics.Raycast(ray, rayRange, (int)Layers.Obstacles);
    }

    private bool NavPointOnList( NavPointV2 arg_point, NavPointV2[] arg_array, int arg_elementCount)
    {
        for ( int i = 0; i < arg_elementCount; i++)
        {
            if (arg_array[i] == arg_point)
            {
                return true;
            }
        }
        return false;
    }
}
