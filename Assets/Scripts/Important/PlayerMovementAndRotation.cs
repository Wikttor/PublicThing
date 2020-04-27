using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementAndRotation : MonoBehaviour, IMovementAndRotation, ITrackableNearestNavPoint
{
    [SerializeField] bool isMovementRelativeToRotation = false;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float moveBackwardsModifier = 0.5f;
    [SerializeField] float moveSidewaysModifier = 0.8f;
    [SerializeField] float moveForwardAngle = 30f;

    public static PlayerMovementAndRotation staticRef = null;

    private Camera myCamera;
    private Rigidbody thisRigidbody;

    bool isMoving = false;

    private Vector3 m_movementVector;

    public static NavPointV2 nearestNavPoint;


    void Start()
    {
        if(staticRef == null)
        {
            staticRef = this;
        }
        if (thisRigidbody == null)
        {
            thisRigidbody = this.GetComponent<Rigidbody>();
        }

    }

    void Update()
    {
        if (myCamera == null  &&  CameraScript.staticInstance != null)
        {
            myCamera = CameraScript.staticInstance.GetComponent<Camera>();
        }
        if(BetterNavNet.navNetCreated) 
        {
            nearestNavPoint = BetterNavNet.FindNearestNavpoint(nearestNavPoint, transform.position);
        }
        ProcessRotation();
        ProcessMovement();
    }
    void ProcessMovement()
    {
        if (Input.anyKey)
        {
            EstablishMovementDirection();
            m_movementVector *= moveSpeed * CalculateMovementSpeedModificators();

            if (thisRigidbody != null)
            {
                this.thisRigidbody.velocity = m_movementVector;
            }
            isMoving = (m_movementVector.magnitude > 0);
        } else
        {
            isMoving = false;
        }
  
    }

    private void EstablishMovementDirection()
    {
        m_movementVector = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {

            m_movementVector += isMovementRelativeToRotation ? this.transform.forward : Vector3.forward;
        }
        
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            m_movementVector -= isMovementRelativeToRotation ? this.transform.right : Vector3.right;
        }
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            m_movementVector += isMovementRelativeToRotation ? this.transform.right : Vector3.right;
        }

        m_movementVector.Normalize();

        if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            m_movementVector -= isMovementRelativeToRotation ? this.transform.forward : Vector3.forward;
            m_movementVector.Normalize();
        }
    }

    private float CalculateMovementSpeedModificators()
    {
        if (Mathf.Abs(Vector3.Angle(transform.forward, m_movementVector)) > 90)
        {
            return moveBackwardsModifier;
        }
        else if (Mathf.Abs(Vector3.Angle(transform.forward, m_movementVector)) > moveForwardAngle)
        {
            return moveSidewaysModifier;
        }else
        {
            return 1;
        }
    }

    void ProcessRotation()
    {
        if (myCamera != null) 
        {
            Vector3 pointToLookAt = myCamera.ScreenToWorldPoint(new Vector3
                (
                    Input.mousePosition.x,
                    Input.mousePosition.y,
                    CameraScript.staticInstance.transform.position.y - this.transform.position.y
                )
            );
            this.transform.LookAt(pointToLookAt);
        }
    }
    public bool GetIsMoving()
    {
        return isMoving;
    }
    public float GetRotation()
    {
        return transform.rotation.y;
    }

    public Vector3 NearestNavPointPositionGet()
    {
        if( nearestNavPoint != null)
        {
            return nearestNavPoint.GetPosition();
        } else
        {
            return Vector3.zero;
        }
            
    }
}



