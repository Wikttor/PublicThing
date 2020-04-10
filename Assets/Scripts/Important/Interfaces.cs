using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INavNet
{
    //List<INavPoint> FindPath(INavPoint start, INavPoint destination, float objectWidth);
}

public interface INavPoint
{
    Vector3 GetPosition();
    bool OverlapWithCollider();
}

public interface IMovementAndRotation
{
    bool GetIsMoving();
    float GetRotation();
}