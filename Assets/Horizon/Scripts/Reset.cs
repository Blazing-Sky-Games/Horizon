using UnityEngine;
using System.Collections;

public class Reset : MonoBehaviour {

	public void resetGame()
	{
		//reload the combat scene
		// this function is just for some ui buttons to use as a onclick event handler
		Application.LoadLevel(0);
	}
}
