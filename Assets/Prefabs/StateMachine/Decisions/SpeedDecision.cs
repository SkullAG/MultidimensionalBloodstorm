using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

[CreateAssetMenu(menuName = "FSM/Decisions/SpeedDecision")]

public class SpeedDecision : FSM.Decision
{
    public Vector2 MinSpeed, MaxSpeed;
    public bool separatedAxis;

    public override bool Decide(Controller controller)
    {
        Vector2 vel = controller.GetComponent<Rigidbody2D>().velocity;

        bool xTrue = (Mathf.Abs(vel.x) > MinSpeed.x && Mathf.Abs(vel.x) < MaxSpeed.x) ;
        bool yTrue = (Mathf.Abs(vel.y) > MinSpeed.y && Mathf.Abs(vel.y) < MaxSpeed.y);

        if(separatedAxis)
        {
            return (xTrue || yTrue);
        }

        return (xTrue && yTrue);
    }
}
