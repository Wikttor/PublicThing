using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, ITrackableNearestNavPoint
{
    //public static Enemy staticRef;
    private static Rigidbody rBody;

    public NavPointV2 nearestNavPoint, destination;
    public EnemyBehaviourState behaviourState;
    public float i_movementSpeed = 1000;
    [Tooltip("radius; unit is equal to distance between adjecent navpoints")] public int i_size;
    public float i_pursuingDistance = 1f;
    public int i_searchDistance = 80;

    public List<INavPoint> path;
    public int pathElementIndex;
    public Vector3 velocity;

    public bool isAIEnabled = false;
    float nextAItime;
    public float i_AICheckDelay = 1f;
    public float i_AICheckDelayDeviationAmount = 0.3f;

    LnRendererBuffer pathvisualisation;
    public  Material i_pathMaterial;
    void Start()
    {
        rBody = this.transform.GetComponent<Rigidbody>();
        behaviourState = EnemyBehaviourState.Default;
        pathvisualisation = new LnRendererBuffer(i_pathMaterial);    
    }
    void Update()
    {
        if (!isAIEnabled && BetterNavNet.navNetCreated)
        {
            isAIEnabled = true;
            nextAItime = Time.time;
        }
        if (isAIEnabled)
        {
            AIScript();
        }
    }
    private void AIScript()
    {
        nearestNavPoint = BetterNavNet.FindNearestNavpoint(nearestNavPoint, this.transform.position);

        switch (behaviourState)
        {
            case EnemyBehaviourState.Default:
                AIDefault();
                break;
            case EnemyBehaviourState.RunAway:
                AIRunAway();
                break;
            case EnemyBehaviourState.Move:
                AIPursue();
                break;
        }
    }
    private void AIDefault()
    { 
        rBody.velocity = new Vector3(0f, 0f, 0f);
        if (Time.time < nextAItime)
        {
            return;
        }

        if (WithinSightOfPlayer())
        {
            //AIStartPursuing();
            AIStartHiding();
        }
        nextAItime += Random.Range( 1f - i_AICheckDelayDeviationAmount, 1 + i_AICheckDelayDeviationAmount) * i_AICheckDelay;
    }

    private void AIStartHiding()
    {
        NavPointV2 hideoutLocation = BetterNavNet.FindNearestWayPointOutOfSight(nearestNavPoint, i_size, i_searchDistance, PlayerMovementAndRotation.staticRef.transform.position);
        if (hideoutLocation != null)
        {
            path = BetterNavNet.FindPath(nearestNavPoint, hideoutLocation, i_size);
        }
        pathElementIndex = 0;
        if (path != null)
        {
            pathvisualisation.DrawPath(2f, path);
            behaviourState = EnemyBehaviourState.Move;
        }
    }
    private void AIStartPursuing()
    {
        path = BetterNavNet.FindPath(nearestNavPoint, PlayerMovementAndRotation.nearestNavPoint, i_size);
        pathElementIndex = 0;
        if (path != null)
        {
            LineRenderingThing.staticRef.DrawPath(path);
            behaviourState = EnemyBehaviourState.Move;
        }
    }
    private void AIPursue()
    {
        float distanceFromDestination = (path[path.Count - 1].GetPosition() - this.transform.position).magnitude;
        if (distanceFromDestination > i_pursuingDistance)
        {
            Vector3 movementDirection = (FindLastVisibleNavPoint(i_size).GetPosition() - transform.position).normalized;
            velocity =  movementDirection * i_movementSpeed;
            velocity.Set(velocity.x, 0f, velocity.z);
            rBody.velocity = velocity;
        }
        if (distanceFromDestination <= i_pursuingDistance)
        {
            behaviourState = EnemyBehaviourState.Default;
        }
    }
    private INavPoint FindLastVisibleNavPoint()
    {
        return FindLastVisibleNavPoint(0);
    }
    private INavPoint FindLastVisibleNavPoint(int arg_size)
    {
        bool nothingBlockingWayToTheNextWaypoint = true;
        while (nothingBlockingWayToTheNextWaypoint && pathElementIndex + 1 < path.Count)
        {
            Vector3 rayStart = this.transform.position;
            Vector3 rayDirection = path[pathElementIndex + 1].GetPosition() - this.transform.position;
            float rayDistance = (path[pathElementIndex + 1].GetPosition() - this.transform.position).magnitude;
            Vector3 offset = (Quaternion.Euler(0f, 90f, 0f) * rayDirection.normalized) * arg_size * BetterNavNet.staticRef.distanceBetweenNavPoints;

            if (
                Physics.Raycast(rayStart, rayDirection, rayDistance, (int)Layers.Obstacles) ||
                    (
                    arg_size != 0 &&
                        (
                        Physics.Raycast(rayStart + offset, rayDirection, rayDistance, (int)Layers.Obstacles) ||
                        Physics.Raycast(rayStart - offset, rayDirection, rayDistance, (int)Layers.Obstacles)
                        )
                    )
                )
            {
                nothingBlockingWayToTheNextWaypoint = false;
            }              
            if (nothingBlockingWayToTheNextWaypoint)
            {
                pathElementIndex++;
            }
        }
        return path[pathElementIndex];
    }
    private void AIHide()
    {

    }
    private void AIRunAway()
    {
        Debug.Log("runaway");
        rBody.velocity = new Vector3(0, 0, i_movementSpeed);
    }
    public bool WithinSightOfPlayer()
    {
        RaycastHit hit;
        Vector3 hitOrigin = PlayerMovementAndRotation.staticRef.transform.position + PlayerMovementAndRotation.staticRef.transform.forward;
        if (Physics.Raycast(hitOrigin, this.transform.position - hitOrigin, out hit, (this.transform.position - hitOrigin).magnitude, (int)Layers.Obstacles) )
        {
            return false;
        } else
        {
            return true;
        }
    }

    public Vector3 NearestNavPointPositionGet()
    {
        if(nearestNavPoint != null && nearestNavPoint.position != null)
        {
            return this.nearestNavPoint.position;
        }
        else
        {
            return Vector3.zero;
        }    
    }
}


public enum EnemyBehaviourState { Default, RunAway, Hide, Attack, Move}