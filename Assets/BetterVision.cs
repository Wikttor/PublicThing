using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterVision : MonoBehaviour
{
    public float[] vision;
    public int resolution = 360;

    public static BetterVision sRef;
    public float lastDraw;
    public float drawEvery = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        vision = new float[resolution];
        if (sRef == null)
        {
            sRef = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool draw = false;
        if (Time.time > lastDraw + drawEvery)
        {
            draw = true;
            lastDraw += drawEvery;
        }

        for(int i = 0; i < resolution; i++)
        {
            float angle = i * (Mathf.PI * 2f / resolution);
            Vector3 direction = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
            RaycastHit hit;
            Physics.Raycast(transform.position, direction, out hit, 1000f, (int)Layers.Obstacles);
            vision[i] = hit.distance;
            
            if (draw)
            {
                LnRendererBuffer.sList[0].DrawVector(0.1f, transform.position, transform.position + direction * vision[i]);
            }
        }
    }

    public bool isVisible(Vector3 arg_position)
    {
        arg_position -= transform.position;
        int index = CalculateIndex(arg_position);

        return arg_position.magnitude < vision[index] ? true : false;
    }

    private int CalculateIndex(Vector3 arg_relativePosition)
    {
        float angle = Vector3.Angle(Vector3.forward, arg_relativePosition) / 180 * Mathf.PI;
        angle = arg_relativePosition.x < 0 ? 2f * Mathf.PI - angle : angle;
        float fIndex = angle / 2f / Mathf.PI * resolution;
        int index = (int)fIndex;
        index += fIndex - (float)index >= 0.5f ? 1 : 0;
        return index;
    }

    public float VisionEdgeProximity( Vector3 arg_position)
    {
        int proximity = 0;
        bool finished = false;
        arg_position -= transform.position;
        int index = CalculateIndex(arg_position);

        float distance = arg_position.magnitude;

        while (!finished)
        {
            if (proximity > 20 ||
                proximity > index ||
                proximity >= resolution - index ||
                distance < vision[(index + proximity) % resolution] ||
                distance < vision[(resolution + index - proximity) % resolution])
            {
                finished = true;
            }
            proximity++;
        }

        return (float)((proximity - 1) * (2 * Mathf.PI * distance));
    }
}
