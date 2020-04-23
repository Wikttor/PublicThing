using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LnRendererBuffer
{
    public static List<LnRendererBuffer> sList;
    private List<LnRendererHandler> buffer;
    private static GameObject commonParent;
    private Material material;


    public LnRendererBuffer(Material arg_material)
    {
        if (commonParent == null)
        {
            commonParent = new GameObject("BufferedLineRenderers");
        }
        if(sList == null)
        {
            sList = new List<LnRendererBuffer>();
        }
        sList.Add(this);
        sList[sList.Count - 1].material = arg_material;
        sList[sList.Count - 1].buffer = new List<LnRendererHandler>();
        sList[sList.Count - 1].AddNewRenderer();
    }

    private int AddNewRenderer()
    {
        LnRendererHandler newHandler = (new GameObject("LnRenderer")).AddComponent<LnRendererHandler>();
        newHandler.transform.parent = commonParent.transform;
        newHandler.Initiate(material);
        this.buffer.Add(newHandler);
        return this.buffer.Count;
    }

    public void DrawVector(float arg_time , Vector3 arg_pointOne, Vector3 arg_pointTwo) 
    {
        GetBufferedRenderer().DrawVector(arg_time, arg_pointOne, arg_pointTwo);
    }

    public void DrawPath(float arg_time, List<INavPoint> arg_path)
    {
        GetBufferedRenderer().DrawPath(arg_time, arg_path);
    }

    private LnRendererHandler GetBufferedRenderer()
    {
        int i = 0;
        while (i < buffer.Count)
        {
            if (!buffer[i].gameObject.active)
            {
                break;
            }
            i++;
        }
        if (i >= buffer.Count)
        {
            AddNewRenderer();
        }
        return buffer[i];
    }
}
