using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    public Material LineMaterial;
    void Start()
    {
        if(LnRendererBuffer.sList == null)
        {
            LnRendererBuffer.sList = new List<LnRendererBuffer>();
            LnRendererBuffer.InitiateNew(LineMaterial);
        }

        //TestCreateLine();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TestCreateLine()
    {
       // LnRendererBuffer.sList[0].DrawVector();
    }
}


public enum Layers { Obstacles = 1 << 8};