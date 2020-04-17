using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayValueCircle : MonoBehaviour
{

    public Material i_frontMaterial, i_backMaterial;
    private LineRenderer frontRenderer, backRenderer;
    public float i_circleWidth = 0.1f;
    public int i_verticleCount = 100;
    public float i_innerCircleSize = 1.3f;
    public float i_cicrleseparation = 0;
    public Component i_valueHolder;
    public IDisplayableValue value;
    private Vector3[] positions;
    public float i_backYPosition = 0f;
    public float i_frontYPosition = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        value = i_valueHolder as IDisplayableValue;
        frontRenderer = Initiate(i_frontMaterial, i_circleWidth);
        backRenderer = Initiate(i_backMaterial, i_circleWidth);
        backRenderer.positionCount = i_verticleCount;
        positions = new Vector3[i_verticleCount];
        for (int i = 0; i < i_verticleCount; i++)
        {
            float angle = 2 * i * Mathf.PI / i_verticleCount;
            positions[i] = new Vector3(Mathf.Sin(angle) * i_innerCircleSize, i_backYPosition, Mathf.Cos(angle) * i_innerCircleSize);
            backRenderer.SetPositions(positions);
        }
    }

    // Update is called once per frame
    void Update()
    {
            DisplayValue(value);
    }

    private void DisplayValue( IDisplayableValue arg_value)
    {
        frontRenderer.gameObject.transform.position = this.transform.position;
        backRenderer.gameObject.transform.position = this.transform.position;
        int barLength = (int)(arg_value.GetCurrentValue() / arg_value.GetMaxValue()  * (float)i_verticleCount);
        frontRenderer.positionCount = barLength;
        for (int i = 0; i < barLength ; i++)
        {
            float angle = 2 * i * Mathf.PI / i_verticleCount;
            frontRenderer.SetPosition(i, new Vector3(Mathf.Sin(angle) * i_innerCircleSize, i_frontYPosition, Mathf.Cos(angle) * i_innerCircleSize));            
        }
    }

    private LineRenderer Initiate(Material arg_material, float width)
    {
        GameObject temp = new GameObject();
        LineRenderer lRend = temp.gameObject.AddComponent<LineRenderer>();
        lRend.material = arg_material;
        lRend.SetWidth(width, width);
        lRend.useWorldSpace = false;

        return lRend;
    }
}
