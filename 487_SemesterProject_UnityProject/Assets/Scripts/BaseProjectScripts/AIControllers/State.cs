using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/State",order = -1)]
public class State : ScriptableObject
{
    public Action[] actions;
    public Transition[] transitions;

    public void UpdateState(AI ai)
    {
        DoActions(ai);
        CheckTransitions(ai);
    }

    private void DoActions(AI ai)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].Act(ai);
        }
    }

    private void CheckTransitions(AI ai)
    {
        for (int i = 0; i < transitions.Length; i++)
        {
            bool decisionSucceeded = transitions[i].condition.CheckCondition(ai);

            if (decisionSucceeded)
            {
                ai.ChangeState(transitions[i].stateTrue);
            }
            else
            {
                ai.ChangeState(transitions[i].stateFalse);
            }
        }
    }
}