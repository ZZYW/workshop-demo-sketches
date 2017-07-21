using UnityEngine;
using System.Collections;
using DynamicFogAndMist;

namespace DynamicFogAndMist
{
	public class RotatingSign : MonoBehaviour
	{
	
		// Update is called once per frame
		void Update ()
		{
			transform.Rotate (Vector3.up, Time.deltaTime * 50);

			if (Input.GetMouseButtonDown (0) && Input.touchCount > 0) {
				if (Input.touches [0].position.x < Screen.width * 0.5f && Input.touches [0].position.y < Screen.height * 0.25f) {
					ToggleFogStyle ();
				}
			}
	
		}

		void ToggleFogStyle ()
		{
			DynamicFog fog = Camera.main.GetComponent<DynamicFog> ();
			switch (fog.preset) {
			case FOG_PRESET.Clear:
				fog.preset = FOG_PRESET.Mist;
				break;
			case FOG_PRESET.Mist:
				fog.preset = FOG_PRESET.WindyMist;
				break;
			case FOG_PRESET.WindyMist:
				fog.preset = FOG_PRESET.GroundFog;
				break;
			case FOG_PRESET.GroundFog:
				fog.preset = FOG_PRESET.Fog;
				break;
			case FOG_PRESET.Fog:
				fog.preset = FOG_PRESET.HeavyFog;
				break;
			case FOG_PRESET.HeavyFog:
				fog.preset = FOG_PRESET.SandStorm;
				break;
			case FOG_PRESET.SandStorm:
				fog.preset = FOG_PRESET.Clear;
				break;
			}
		}
	}
}