using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedActionTest : MonoBehaviour, IDisplayableFiniteValue
{

    public float i_maxValue = 100;
    private float currentValue = 100;
    public float i_valueChangePeriod = 0.4f;
    private float lastUpdateTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if( Time.time > lastUpdateTime + i_valueChangePeriod)
        {
            currentValue = Random.Range(0, i_maxValue);
            lastUpdateTime += i_valueChangePeriod;
        }
    }

    public float GetMaxValue()
    {
        return i_maxValue;
    }
    public float GetCurrentValue()
    {
        return currentValue;
    }
}
