using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Horizon.Core.WeakSubscription;

[ExecuteInEditMode]
public class Initilizer : MonoBehaviour
{
	public static void CallOnInit(Action action)
	{
		actions.Add(action);
	}

	public static void StartConstruction()
	{
		constructorsInProgress++;
	}

	public static void EndConstruction()
	{
		if(constructorsInProgress == 1 && MainThreadStarted && !initilizing)
		{
			init();
		}

		constructorsInProgress--;
		constructorsInProgress = constructorsInProgress < 0 ? 0 : constructorsInProgress;
	}

	static Initilizer()
	{
		actions = new List<Action>();
	}

	static void init ()
	{
		initilizing = true;

		for(int i = 0; i < actions.Count; i++)
		{
			try
			{
				actions[i]();
			}
			catch(Exception e)
			{
				Debug.LogError(e);
			}
		}

		actions.Clear();
		initilizing = false;
	}

	public void OnEnable()
	{
		MainThreadStarted = true;

		if(constructorsInProgress == 0) init();
	}

	//void Update()
	//{
		//constructorsInProgress = 0;
	//}

	public void OnDestroy()
	{
		actions.Clear();
		MainThreadStarted = false;
		constructorsInProgress = 0;
	}

	private static int constructorsInProgress = 0;

	private static List<Action> actions;
	private static bool MainThreadStarted = false;
	private static bool initilizing = false;
}

