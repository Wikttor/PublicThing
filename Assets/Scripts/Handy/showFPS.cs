using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showFPS : MonoBehaviour
{
    float frameCount = 0f;
    [SerializeField] float fpsCountUpdateFrequency = 2;
    float timeSinceLastUpdate = 0f;

    void Update()
    {
        frameCount++;
        timeSinceLastUpdate += Time.deltaTime;
        if(timeSinceLastUpdate > fpsCountUpdateFrequency)
        {
            Debug.Log(" FPS: " + (frameCount / fpsCountUpdateFrequency).ToString());
            timeSinceLastUpdate -= fpsCountUpdateFrequency;
            frameCount = 0;
        }
    }
}
