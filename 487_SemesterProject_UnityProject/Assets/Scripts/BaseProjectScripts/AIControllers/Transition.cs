using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Transition
{
    public Condition condition;
    public State stateTrue;
    public State stateFalse;
}