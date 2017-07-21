using UnityEngine;
using System.Collections;

namespace VolumetricFogAndMist
{
	public class DemoMountainClouds : MonoBehaviour
	{

		void OnGUI ()
		{
			Rect rect = new Rect (10, 10, Screen.width - 20, 30);
			GUI.Label (rect, "Climb to the top of the mountain to see the clouds (WASD keys to move).");
		}
	}
}