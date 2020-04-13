﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Physics.Raycast(this.transform.position + this.transform.forward, this.transform.forward, out hit, 666);
            LnRendererBuffer.sList[0].DrawVector(this.transform.position, hit.point);
        }
    }
}
