using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRenderingThing : MonoBehaviour
{
    public LineRenderer lnRenderer;
    Vector3[] positions;
    public static LineRenderingThing staticRef;
    void Start()
    {
        if (!staticRef)
        {
            staticRef = this;
        }
        lnRenderer = GetComponent<LineRenderer>();
        lnRenderer.SetWidth(0.1f, 0.1f);
    }
    public void DrawPath(List<INavPoint> path)
    {
        lnRenderer.positionCount = path.Count;
        int i = 0;
        foreach(INavPoint npoint in path)
        {
            lnRenderer.SetPosition(i, npoint.GetPosition());
            i++;
        }
    }
}
