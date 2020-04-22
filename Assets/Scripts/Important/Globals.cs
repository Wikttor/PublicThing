using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    public Material LineMaterial;
    private float waitBeforeInitializingTime = 1f;
    private bool initialized = false;
}


public enum Layers { Obstacles = 1 << 8};