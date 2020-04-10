using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public static CameraScript staticInstance;
    [SerializeField] float offsetXScaling = 20;
    [SerializeField] float offsetZScaling = 20;
    [SerializeField] float maxOffsetX = 15;
    [SerializeField] float maxOffsetZ = 15;
    void Start()
    {
        if ( staticInstance == null)
        {
            staticInstance = this;
        }
        else
        {
            Debug.Log("there already is one CameraPosition");
        }
    }

    void Update()
    {
        //calculate camera offset
        float offsetX = Mathf.Clamp(2 * offsetXScaling * (Input.mousePosition.x / Screen.width - 0.5f),
                                    -maxOffsetX,
                                    maxOffsetX);

        
        float offsetZ = Mathf.Clamp(2 * offsetZScaling * (Input.mousePosition.y / Screen.height - 0.5f),
                                    -maxOffsetZ,
                                    maxOffsetZ);
      
        this.transform.position = (new Vector3(
                                                PlayerMovementAndRotation.staticRef.transform.position.x + offsetX,
                                                this.transform.localPosition.y,
                                                PlayerMovementAndRotation.staticRef.transform.position.z + offsetZ)  );
    }
}
