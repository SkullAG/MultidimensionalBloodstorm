using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FSM
{
    public class Controller : MonoBehaviour
    {
        public State currentState;
        public State remainState;

        private int health;

        public bool ActiveAI { get; set; }

        private Animator _animatorController;

        public void Start()
        {
            ActiveAI = true;
            _animatorController = GetComponent<Animator>();
        }

        public void Update()
        {
            if (!ActiveAI) return;

            //is required for the circumstances
            //health = GetComponent<HealthSistem>().health;

            currentState.UpdateState(this);
        }

        public void Transition(State nextState)
        {
            if (nextState != remainState)
            {
                currentState = nextState;
            }
        }

        public void SetAnimation(string animation, bool value)
        {
            _animatorController.SetBool(animation, value);
        }
    }
}
