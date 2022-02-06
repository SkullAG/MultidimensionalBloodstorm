using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


#if __DEBUG_AVAILABLE__

using UnityEditor;

#endif

public class GameManager : MonoBehaviour
{
	public Transform gameCamera;
	public Transform[] NovinaPositions;

	public NovinaFollow Novina;

	public enum CinematicCommandId
	{
		enterCinematicMode,
		exitCinematicMode,
		wait,
		log,
		showDialog,
		setNovinaPosition,
		startShakingCamera,
		stopShakingCamera,
		setNovinaFacing,
	}

	[System.Serializable]
	public struct CinematicCommand
	{
		public CinematicCommandId id;
		public string param1;
		public string param2;
		public string param3;
		public string param4;

	}

	[System.Serializable]
	public struct CinematicSequence
	{
		public CinematicCommand[] commands;

	}

	[Header("Cinematic system")]
	public CinematicSequence[] sequences;

	[Header("Dialog system")]
	public TextMeshProUGUI dialogText;
	public string[] dialogStrings;

	bool isCinematicMode;

	int sequenceIndex;
	int commandIndex;

	bool showingDialog;

	float dialogTime;

	bool waiting;
	float waitTimer;

	int dialogIndex;

	CameraManager gameCameraC;

	KeyCode[] debugKey = { KeyCode.S, KeyCode.T, KeyCode.A, KeyCode.R };
	int debugKeyProgress = 0;

	// Start is called before the first frame update
	void Start()
	{
		isCinematicMode = false;
		waiting = false;

		showingDialog = false;
		dialogIndex = 0;

		gameCameraC = gameCamera.GetComponent<CameraManager>();
	}

	// Update is called once per frame
	void Update()
	{
		if (isCinematicMode)
		{
			if (showingDialog)
			{
				string text = dialogStrings[dialogIndex];

				dialogText.gameObject.SetActive(true);
				dialogText.text = text;

				if (dialogTime <= 0)
				{
					dialogText.gameObject.SetActive(false);
					showingDialog = false;
					dialogText.text = "";
					commandIndex++;
				}
				dialogTime -= Time.deltaTime;
			}
			else if (waiting)
			{
				if (waitTimer <= 0)
				{
					waiting = false;
					commandIndex++;
				}
				else
				{
					waitTimer -= Time.deltaTime;
				}
			}
			else if (commandIndex < sequences[sequenceIndex].commands.Length)
			{
				CinematicCommand command = sequences[sequenceIndex].commands[commandIndex];

				if (command.id == CinematicCommandId.enterCinematicMode)
				{
					Novina.CinematicMode = true;
				}
				else if (command.id == CinematicCommandId.exitCinematicMode)
				{
					isCinematicMode = false;
					Novina.CinematicMode = false;
				}
				else if (command.id == CinematicCommandId.log)
				{
					string message = command.param1;

					Debug.Log("Cinematic Log:\n" + message);
				}
				else if (command.id == CinematicCommandId.showDialog)
				{
					int Index = Int32.Parse(command.param1);

					dialogTime = Single.Parse(command.param2);

					showingDialog = true;
					dialogIndex = Index;
				}
				else if (command.id == CinematicCommandId.wait)
				{
					float time = Single.Parse(command.param1);

					waiting = true;
					waitTimer = time;
				}
				else if (command.id == CinematicCommandId.setNovinaPosition)
				{
					Novina.CinematicPosition = NovinaPositions[Int32.Parse(command.param1)].position;
				}
				else if (command.id == CinematicCommandId.startShakingCamera)
				{
					gameCameraC.StartCameraShake(Single.Parse(command.param1), Single.Parse(command.param2), Single.Parse(command.param3));
				}
				else if (command.id == CinematicCommandId.stopShakingCamera)
				{
					gameCameraC.StartCameraShake(0, 0, 0);
				}
				else if (command.id == CinematicCommandId.setNovinaFacing)
				{
					Novina.CinematicFacing = Int32.Parse(command.param1);
				}
				else
				{
					Debug.LogError("Commando de cinematica no implementado");
				}


				if (!waiting && !showingDialog)
				{
					commandIndex++;
				}
			}
			else {
				isCinematicMode = false;
			}
		}
	}

	public void OnTriggerCinematic(int index)
	{
		if(!isCinematicMode)
		{
			isCinematicMode = true;
			sequenceIndex = index;
			commandIndex = 0;
		}
		else
		{
			isCinematicMode = true;
			Novina.CinematicMode = false;

			waiting = false;
			showingDialog = false;
			waitTimer = 0;
			dialogTime = 0;

			sequenceIndex = index;
			commandIndex = 0;

			Debug.LogError("Cinematica saltada");
		}

		//showingDialog = true;
		//dialogIndex = index;
	}

	public bool IsCinematicMode ()
	{
		return isCinematicMode;
	}
}
