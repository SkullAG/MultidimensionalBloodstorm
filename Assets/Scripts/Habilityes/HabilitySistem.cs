using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class HabilitySistem : MonoBehaviour
{
    //public Transform HabilityUser;

    public abstract void Activate(InputAction.CallbackContext context);
}
