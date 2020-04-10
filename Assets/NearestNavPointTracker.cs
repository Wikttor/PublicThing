using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearestNavPointTracker : MonoBehaviour
{
    public ITrackableNearestNavPoint trackedObject = null;
    public Enemy myEnemy;
    public bool trackPlayer = false;
    public bool trackEnemy = false;

    private void Start()
    {

        if (trackEnemy)
        {
            trackedObject = myEnemy;
        }
    }
    void Update()
    {
        if (trackedObject == null && trackPlayer)
        {
            trackedObject = PlayerMovementAndRotation.staticRef;
        }
        if ( trackedObject != null)
        {
            this.transform.position = trackedObject.NearestNavPointPositionGet();
        }
    }
}


public interface ITrackableNearestNavPoint
{
    Vector3 NearestNavPointPositionGet();
}