using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearestNavPointTracker : MonoBehaviour
{
    public ITrackableNearestNavPoint i_trackedObject = null;
    public Enemy i_myEnemy;
    public bool i_trackPlayer = false;
    public bool i_trackEnemy = false;

    private void Start()
    {

        if (i_trackEnemy)
        {
            i_trackedObject = i_myEnemy;
        }
    }
    void Update()
    {
        if (i_trackedObject == null && i_trackPlayer)
        {
            i_trackedObject = PlayerMovementAndRotation.staticRef;
        }
        if ( i_trackedObject != null)
        {
            this.transform.position = i_trackedObject.NearestNavPointPositionGet();
        }
    }
}


public interface ITrackableNearestNavPoint
{
    Vector3 NearestNavPointPositionGet();
}