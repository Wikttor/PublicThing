using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour, IDisplayableFiniteValue
{
    [SerializeField] float MaxValue = 100;
    [SerializeField] float CurrentValue = 100;
    [SerializeField] float regenerationPerSecond = 5f;
    [SerializeField] float movementCostPerSecond = 5f;
    [SerializeField] bool playerRelated = true;
    [SerializeField] IMovementAndRotation movementAndRotation;
    float lastReductionTime = 0;
    [SerializeField] float timeBeforeRegeneretionStarts = 1f;

    [SerializeField] bool displayThisValue = true;
    [SerializeField] bool addedToDisplayer = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!addedToDisplayer && displayThisValue && DisplayValue.staticRef)
        {
            DisplayValue.Add(this);
            addedToDisplayer = true;
        }
        if (playerRelated && movementAndRotation == null)
        {
            movementAndRotation = PlayerMovementAndRotation.staticRef.GetComponent<PlayerMovementAndRotation>();
        }

        if (movementAndRotation.GetIsMoving())
        {
            ReduceStamina(movementCostPerSecond * Time.deltaTime);
        }

        if(Time.time - lastReductionTime > timeBeforeRegeneretionStarts)
        {
            AddStamina(regenerationPerSecond * Time.deltaTime);
        }


    }

    public float GetMaxValue()
    {
        return MaxValue;
    }
    public float GetCurrentValue()
    {
        return CurrentValue;
    }

    public bool ReduceStamina( float amount)
    {
        lastReductionTime = Time.time;
        if (CurrentValue > amount)
        {
            CurrentValue -= amount;   
            return true;
        }
        else
        {
            return false;
        }
        
    }
    public void AddStamina(float amount)
    {
        CurrentValue = Mathf.Clamp(CurrentValue + amount, 0, MaxValue);
    }
}

