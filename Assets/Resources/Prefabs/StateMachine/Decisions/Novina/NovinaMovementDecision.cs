using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

[CreateAssetMenu(menuName = "FSM/Decisions/NovinaMovementDecision")]

public class NovinaMovementDecision : FSM.Decision
{
    public override bool Decide(Controller controller)
    {
        return controller.GetComponent<NovinaFollow>().IsMoving;
    }
}
