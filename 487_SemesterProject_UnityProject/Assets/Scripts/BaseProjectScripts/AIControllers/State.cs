using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/State")]
public class State : ScriptableObject
{
    public Action[] actions;
    public Transition[] transitions;
    public Color sceneGizmoColor = Color.grey;

    public void UpdateState(StateController stateController)
    {
        DoActions(stateController);
        CheckTransitions(stateController);
    }

    private void DoActions(StateController stateController)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].Act(stateController);
        }
    }

    private void CheckTransitions(StateController stateController)
    {
        for (int i = 0; i < transitions.Length; i++)
        {
            bool decisionSucceeded = transitions[i].decision.Decide(stateController);

            if (decisionSucceeded)
            {
                stateController.TransitionToState(transitions[i].trueState);
            }
            else
            {
                stateController.TransitionToState(transitions[i].falseState);
            }
        }
    }
}