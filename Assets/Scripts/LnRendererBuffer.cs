using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LnRendererBuffer
{
    public static List<LnRendererBuffer> sList;
    private List<LnRendererHandler> buffer;


    public static int InitiateNew(Material arg_material)
    {
        if(sList == null)
        {
            sList = new List<LnRendererBuffer>();
        }
        sList.Add(new LnRendererBuffer());
        sList[0].buffer = new List<LnRendererHandler>();
        for (int i = 0; i < 10; i++)
        {
            LnRendererHandler newHandler = (new GameObject("LnRenderer")).AddComponent<LnRendererHandler>();
            newHandler.Initiate(arg_material);
            sList[0].buffer.Add(newHandler);
            }
        return sList.Count - 1;
    }

    public void DrawVector(Vector3 arg_pointOne, Vector3 arg_pointTwo) 
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
        buffer[i].DrawVector(1f, arg_pointOne, arg_pointTwo);
    }
}
