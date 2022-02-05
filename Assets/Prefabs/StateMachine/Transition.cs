using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FSM
{
	[System.Serializable]
	public struct SuperDecision
	{
		public Decision decision;
		public bool Invert;
	}

	[System.Serializable]
	public class Transition
	{
		public SuperDecision[] decisions;
		public State trueState;
		public State falseState;
	}

}
