using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFindAndShowPath : MonoBehaviour
{
    public static TestFindAndShowPath staticRef;
    public bool keepFindingPath = false;
    public bool keepFindingPathWithBetterNavnet = false;

    public float i_pathWidth = 3;

    [SerializeField] INavPoint start;
    //private NavPointV2 nearestNavPoint = null;


    void Start()
    {
        if (!staticRef)
        {
            staticRef = this;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && BetterNavNet.allNavPoints.Count == 0)
        {
            BetterNavNet.CreateNavNet();
            //TestFindAndShowPath.staticRef.keepFindingPathWithBetterNavnet = true;
        }

        //if (keepFindingPathWithBetterNavnet)
        //{

            //NavPointV2 newNearestNavPoint = BetterNavNet.FindNearestNavpoint(nearestNavPoint, this.transform.position);
            //if (nearestNavPoint == null || nearestNavPoint != newNearestNavPoint)
            //{
            //    nearestNavPoint = newNearestNavPoint;
            //}
        //}
    }
}
