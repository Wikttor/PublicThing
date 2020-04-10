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
    [Tooltip("radius; unit is equals to distance between adjecent navpoints")] public int i_size;
    public float i_pursuingDistance = 1f;


    public List<INavPoint> path;
    public int pathElementIndex;
    public Vector3 velocity;

    public bool isAIEnabled = false;
    public float nextAItime;
    void Start()
    {
        //if (staticRef == null)
        //{
            //staticRef = this;
            rBody = this.transform.GetComponent<Rigidbody>();
            behaviourState = EnemyBehaviourState.Default;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            isAIEnabled = true;
            nextAItime = Time.time;
        }
        nearestNavPoint = BetterNavNet.FindNearestNavpoint(nearestNavPoint, this.transform.position);
        if (isAIEnabled)
        {
            AIScript();
        }
    }

    private void AIScript()
    {
        switch (behaviourState)
        {
            case EnemyBehaviourState.Default:
                AIDefault();
                break;
            case EnemyBehaviourState.RunAway:
                AIRunAway();
                break;
            case EnemyBehaviourState.Pursue:
                AIPursue();
                break;
        }
    }
    private void AIDefault()
    {
        velocity = new Vector3(0f, 0f, 0f);
        velocity.Set(velocity.x, 0f, velocity.z);
        rBody.velocity = velocity;
        //if (WithinSightOfPlayer())
        if (isAIEnabled)
        {
            path = BetterNavNet.FindPath(nearestNavPoint, PlayerMovementAndRotation.nearestNavPoint, i_size);
            pathElementIndex = 0;
            if (path != null)
            {
                LineRenderingThing.staticRef.DrawPath(path);
                behaviourState = EnemyBehaviourState.Pursue;
            }
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
            Debug.Log("nolongerinpursuit");
        }
    }

    private INavPoint FindLastVisibleNavPoint()
    {
        bool nothingBlockingWayToNextWaypoint = true;
        while (nothingBlockingWayToNextWaypoint && pathElementIndex + 1 < path.Count)
        {
            if (pathElementIndex + 1 < path.Count)
            {
                nothingBlockingWayToNextWaypoint = !Physics.Raycast
                    (
                        this.transform.position, // origin
                        path[pathElementIndex + 1].GetPosition() - this.transform.position, // direction
                        (path[pathElementIndex + 1].GetPosition() - this.transform.position).magnitude, //range
                        (int)Layers.Obstacles// layer
                    );
                if (nothingBlockingWayToNextWaypoint)
                {
                    pathElementIndex++;
                }
            }
        }
        return path[pathElementIndex];
    }
    private INavPoint FindLastVisibleNavPoint(int arg_size)
    {
        bool nothingBlockingWayToTheNextWaypoint = true;
        while (nothingBlockingWayToTheNextWaypoint && pathElementIndex + 1 < path.Count)
        {
            if (pathElementIndex + 1 < path.Count)
            {
                Vector3 rayStart = this.transform.position;
                Vector3 rayDirection = path[pathElementIndex + 1].GetPosition() - this.transform.position;
                float rayDistance = (path[pathElementIndex + 1].GetPosition() - this.transform.position).magnitude;
                Vector3 offset = (Quaternion.Euler(0f, 90f, 0f) * rayDirection.normalized) * arg_size * BetterNavNet.staticRef.distanceBetweenNavPoints;

                if (Physics.Raycast(rayStart, rayDirection, rayDistance, (int)Layers.Obstacles) ||
                    Physics.Raycast(rayStart + offset, rayDirection, rayDistance, (int)Layers.Obstacles) ||
                    Physics.Raycast(rayStart - offset, rayDirection, rayDistance, (int)Layers.Obstacles)
                    )
                {
                    nothingBlockingWayToTheNextWaypoint = false;
                }              
                if (nothingBlockingWayToTheNextWaypoint)
                {
                    pathElementIndex++;
                }
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
        /*
        RaycastHit hit;
        Vector3 hitOrigin = PlayerMovementAndRotation.staticRef.transform.position + PlayerMovementAndRotation.staticRef.transform.forward;
        Physics.Raycast(hitOrigin, this.transform.position - hitOrigin, out hit, 500f);
        if (hit.transform == null)
        {
            Debug.Log("hit jest null");
        }
        Enemy l_enemy = hit.transform.GetComponent<Enemy>();
        if (l_enemy != null)
        {
            return true;
        }    
        return false;
        */
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





public enum EnemyBehaviourState { Default, RunAway, Hide, Attack, Pursue}