using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : PooledObject
{
    public virtual bool InteractWithPlayer(ControllerMultiPlayer player)
    {
        return true;
    }
}
