using UnityEngine;
using System.Collections;

namespace VolumetricFogAndMist {
	public class FogVolume : MonoBehaviour {

		[Tooltip("Target fog alpha when camera enters this fog volume")]
		[Range(0,1)]
		public float targetFogAlpha = 0.5f;

		[Tooltip("Target sky haze alpha when camera enters this fog volume")]
		[Range(0,1)]
		public float targetSkyHazeAlpha = 0.5f;

		[Tooltip("Set this to zero for changing fog alpha immediately upon enter/exit fog volume.")]
		public float transitionDuration = 3.0f;

		[Tooltip("Set collider that will trigger this fog volume. If not set, this fog volume will react to any collider which has the main camera. If you use a third person controller, assign the character collider here.")]
		public Collider targetCollider;

		bool cameraInside;
		VolumetricFog fog;

		void Start () {
			fog = VolumetricFog.instance;
		}

		void OnTriggerEnter (Collider other) {
			if (cameraInside) return;
			// Check if other collider has the main camera attached
			if (other==targetCollider || other.gameObject.transform.GetComponentInChildren<Camera>() == fog.fogCamera) {
				cameraInside = true;
				fog.SetTargetAlpha(targetFogAlpha, targetSkyHazeAlpha, transitionDuration);
			}
		}

		void OnTriggerExit(Collider other) {
			if (!cameraInside) return;
			if (other==targetCollider || other.gameObject.transform.GetComponentInChildren<Camera>() == fog.fogCamera) {
				cameraInside = false;
				fog.ClearTargetAlpha(transitionDuration);
			}
		}

	}

}