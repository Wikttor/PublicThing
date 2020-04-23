using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *         GUI.skin.box.normal.background = background; ///WHY??????
           GUI.Box(new Rect(10, 10, 500, 500), GUIContent.none);*/
public class DisplayValue : MonoBehaviour
{
    [SerializeField] int width = 50;
    [SerializeField] int length = 500;
    [SerializeField] int separation = 20;
    [SerializeField] int xOffset = 50;
    [SerializeField] int yOffset = 100;
    [SerializeField] Color backgroundColor;
    [SerializeField] Color frontColor;
    [SerializeField]public List<IDisplayableFiniteValue> listOfValuesToDisplay;

    Texture2D background;
    Texture2D front;

    public static DisplayValue staticRef;
    private void Start()
    {
        DoTheSingletonThing();
        listOfValuesToDisplay = new List<IDisplayableFiniteValue>();

        background = new Texture2D(1, 1);
        background.SetPixel(0, 0, Color.red);
        background.Apply();
        front = new Texture2D(1, 1);
        front.SetPixel(0, 0, Color.green);
        front.Apply();
    }

    private void DoTheSingletonThing()
    {
        if (!staticRef)
        {
            staticRef = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
        
    private void OnGUI()
    {
        int elementDisplayedId = 0;
        foreach ( IDisplayableFiniteValue value in listOfValuesToDisplay)
        {
            DrawRect(new Rect(xOffset, yOffset + elementDisplayedId*(width + separation), length, width), background);
            DrawRect(new Rect(xOffset, yOffset + elementDisplayedId * (width + separation), value.GetCurrentValue() / value.GetMaxValue() * length, width), front);
            elementDisplayedId++;
        }

    }

    public static void Add(IDisplayableFiniteValue newValue)
    {
        staticRef.listOfValuesToDisplay.Add(newValue);
    }

    private void DrawRect(Rect rect, Texture2D color)
    {
        GUI.skin.box.normal.background = color;
        GUI.Box(rect, GUIContent.none);
    } 

}

public interface IDisplayableFiniteValue
{
    float GetMaxValue();
    float GetCurrentValue();
}
