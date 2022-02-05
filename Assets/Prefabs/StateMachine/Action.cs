using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using UnityEngine.Events;

namespace FSM
{
	[System.Serializable]
	public struct booleanAction
    {
		public string Animation;
		public bool IsActive;
    }
	[CreateAssetMenu(menuName = "FSM/Action")]
	public class Action : ScriptableObject
	{
		public booleanAction[] actionList;

		public virtual void Act(Controller controller)
        {
			foreach(booleanAction a in actionList)
            {
				controller.SetAnimation(a.Animation, a.IsActive);

			}
		}
	}


}