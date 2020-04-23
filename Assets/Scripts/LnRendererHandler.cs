using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LnRendererHandler : MonoBehaviour
{
    LineRenderer lRend;
    public float deactivateTime;
    // Start is called before the first frame update
    void Start()
    {
        //lRend = this.gameObject.AddComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > deactivateTime)
        {
            this.gameObject.SetActive(false);
        }
        //lRend.SetColors(Color.red, Color.blue);
        //lRend.material = new Material(Shader.Find("Sprites/Default"));
        //lRend.SetWidth(1, 1);
        //lRend.SetPosition(0, Vector3.zero);
        //lRend.SetPosition(1, Vector3.one);
    }

    public void Initiate(Material arg_material)
    {
        lRend = this.gameObject.AddComponent<LineRenderer>();
        lRend.material = arg_material;
        lRend.SetWidth(0.1f, 0.1f);
        lRend.SetPosition(0, Vector3.zero);
        lRend.SetPosition(1, Vector3.one);
        this.gameObject.SetActive(false);
    }
    public void DrawVector( float arg_time, Vector3 arg_pointOne, Vector3 arg_pointTwo)
    {
        this.gameObject.SetActive(true);
        this.deactivateTime = Time.time + arg_time;
        lRend.positionCount = 2;
        lRend.SetPosition(0, arg_pointOne);
        lRend.SetPosition(1, arg_pointTwo);
    }
    public void DrawPath(float arg_time, List<INavPoint> arg_path)
    {
        this.gameObject.SetActive(true);
        this.deactivateTime = Time.time + arg_time;
        lRend.positionCount = arg_path.Count;
        for (int i = 0; i < arg_path.Count; i++)
        {
            lRend.SetPosition(i, arg_path[i].GetPosition());
        }
    }
}
