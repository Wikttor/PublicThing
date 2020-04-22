using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public float i_visualisationTime = 1f;
    public Material i_shootingMaterial;
    LnRendererBuffer lnRenderer;

    void Start()
    {
        lnRenderer = new LnRendererBuffer(i_shootingMaterial); 
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Physics.Raycast(this.transform.position + this.transform.forward, this.transform.forward, out hit, 666);
            lnRenderer.DrawVector(i_visualisationTime, this.transform.position, hit.point);
        }
    }
}
