using UnityEngine;
using System.Collections;
using UnityStandardAssets.Cameras;

namespace VolumetricFogAndMist
{
	public class DemoSurroundingFog : MonoBehaviour
	{

		FreeLookCam cam;

		void Start() {
			cam = GetComponent<FreeLookCam>();
		}

		void OnGUI ()
		{
			Rect rect = new Rect (10, 10, Screen.width - 20, 30);
			if (cam.enabled) {
				GUI.Label (rect, "Move around with WASD keys. Press C to disable free look camera");
			} else {
				GUI.Label (rect, "Move around with WASD keys. Press C to enable free look camera");
			}
		}

		void Update() {
			if (Input.GetKeyDown(KeyCode.C)) {
				cam.enabled = !cam.enabled;
			}
		}
	}
}