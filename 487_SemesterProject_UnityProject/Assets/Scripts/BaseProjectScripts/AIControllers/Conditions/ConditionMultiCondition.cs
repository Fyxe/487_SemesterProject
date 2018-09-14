using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Conditions/MultiCondition", order = -1)]
public class ConditionMultiCondition : Condition
{
    public enum MultiConditionType { and, or }

    public MultiConditionType type;
    public List<Condition> conditions = new List<Condition>();

    public override bool CheckCondition(AI ai)
    {
        switch (type)
        {
            case MultiConditionType.and:
                foreach (var i in conditions)
                {
                    if (!i.CheckCondition(ai))
                    {
                        return false;
                    }
                }
                return true;
            case MultiConditionType.or:
                foreach (var i in conditions)
                {
                    if (i.CheckCondition(ai))
                    {
                        return true;
                    }
                }
                return false;
            default:
                return false;
        }
    }

}
