using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Indicator : PooledObject
{
    public float destroyTime = 2f;

    public abstract void Indicate();
}
