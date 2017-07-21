using UnityEngine;
using System.Collections;

namespace VolumetricFogAndMist
{
	public class FogAreaCullingManager : MonoBehaviour
	{

		public VolumetricFog fog;
		bool isSpherical;

		void OnEnable ()
		{
			MeshFilter mf = GetComponent<MeshFilter> ();
			isSpherical = mf.sharedMesh.vertexCount > 24;
			if (!Application.isPlaying) {
				Debug.Log ("Fog Areas should be created on runtime.");
			}
		}

		void OnBecameVisible ()
		{
			if (fog != null)
				fog.enabled = true;
		}

		void OnBecameInvisible ()
		{
			if (fog != null)
				fog.enabled = false;
		}

		void OnDestroy ()
		{
			if (fog != null)
				fog.DestroySelf ();
		}

		void Update ()
		{
			UpdateFogAreaExtents ();
		}

		public void UpdateFogAreaExtents ()
		{
			if (fog == null) {
				Destroy (gameObject);
				return;
			}
			fog.fogAreaPosition = transform.position;
			fog.fogAreaRadius = transform.localScale.x * 0.5f;
			if (isSpherical) {
				transform.localScale = Vector3.one * transform.localScale.x;
				fog.fogAreaHeight = 0;
				fog.fogAreaDepth = 0;
			} else {
				fog.fogAreaHeight = transform.localScale.y * 0.5f;
				fog.fogAreaDepth = transform.localScale.z * 0.5f;
			}
		}
	
	}
}
