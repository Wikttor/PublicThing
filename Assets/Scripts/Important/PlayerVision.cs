using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVision : MonoBehaviour
{
    public Dictionary<NavPointV2, int> nPointVisionDict;
    public List<NavPointV2> firstOutOfSightPoints;
    public List<NavPointV2> waitingForProcessing;
    public List<NavPointV2> processedThisFrame;
    public List<NavPointV2> visiblePoints;

    void Update()
    {
        if (BetterNavNet.navNetCreated && nPointVisionDict == null)
        {
            nPointVisionDict = new Dictionary<NavPointV2, int>();
            waitingForProcessing = new List<NavPointV2>();
            processedThisFrame = new List<NavPointV2>();
            firstOutOfSightPoints = new List<NavPointV2>();
            visiblePoints = new List<NavPointV2>();
            FirstRun();
        }

    }
    private void FirstRun()
    {
        foreach (NavPointV2 point in BetterNavNet.allNavPoints)
        {
            if (!point.overlapWithCollider && Physics.Raycast(transform.position, point.position - transform.position, (int)Layers.Obstacles))
            {
                nPointVisionDict.Add(point, 0);
                waitingForProcessing.Add(point);
            }
            else
            {
                nPointVisionDict.Add(point, -1);
                visiblePoints.Add(point);
            }
        }
        Debug.Log("dictCount: " + nPointVisionDict.Count + "OutOfSightCount: " + waitingForProcessing.Count);

        foreach ( NavPointV2 point in waitingForProcessing)
        {
            bool hasVisibleNeighbours = false;
            int i = 0;
            while( !hasVisibleNeighbours && i < point.neighbours.Length)
            {
                NavPointV2 neighbour = point.neighbours[i];
                if (neighbour != null && visiblePoints.Contains(neighbour) )
                {
                    firstOutOfSightPoints.Add(point);
                }
                i++;
            }

        }
        waitingForProcessing = firstOutOfSightPoints;
        firstOutOfSightPoints = new List<NavPointV2>();
    }
}
