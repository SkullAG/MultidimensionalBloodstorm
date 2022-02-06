using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

[CreateAssetMenu(menuName = "FSM/Decisions/GroundedDecision")]
public class GroundedDecision : FSM.Decision
{
    public override bool Decide(Controller controller)
    {
        return controller.GetComponent<EntityControls>().isGrounded;
    }
}
