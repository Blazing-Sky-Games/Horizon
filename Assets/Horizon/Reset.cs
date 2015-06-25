using UnityEngine;
using System.Collections;

public class Reset : MonoBehaviour {

	public void resetGame()
	{
		Application.LoadLevel(0);
	}
}
