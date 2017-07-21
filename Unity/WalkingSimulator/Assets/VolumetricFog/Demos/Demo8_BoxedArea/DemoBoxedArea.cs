using UnityEngine;
using System.Collections;

namespace VolumetricFogAndMist
{
	public class DemoBoxedArea : MonoBehaviour
	{
		void Update ()
		{
			VolumetricFog fog = VolumetricFog.instance;
			if (Input.GetKeyDown (KeyCode.T)) {
				fog.enabled = !fog.enabled;
			}
		}

		void OnGUI ()
		{
			Rect rect = new Rect (10, 10, Screen.width - 20, 30);
			GUI.Label (rect, "Move around with WASD or cursor keys, space to jump, T to toggle fog on/off.");
			rect = new Rect (10, 30, Screen.width - 20, 30);
			GUI.Label (rect, "Current fog preset: " + VolumetricFog.instance.GetCurrentPresetName());
		}
	}
}