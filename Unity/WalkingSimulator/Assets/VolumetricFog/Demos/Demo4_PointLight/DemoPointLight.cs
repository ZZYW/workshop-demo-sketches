using UnityEngine;
using System.Collections;

namespace VolumetricFogAndMist {
				public class DemoPointLight : MonoBehaviour {

								public Vector3 attractionPos = new Vector3(-58, 3f, -19f);

								Vector3[] fairyDirections = new Vector3[VolumetricFog.MAX_POINT_LIGHTS];

								void Update () {
												const float accel = 0.01f;

												for (int k = 0; k < VolumetricFog.MAX_POINT_LIGHTS; k++) {
																GameObject fairy = VolumetricFog.instance.GetPointLight (k);
																if (fairy != null) {
																				fairy.transform.position += fairyDirections [k];
																				Vector3 fairyPos = fairy.transform.position;
																				if (fairyPos.x > attractionPos.x)
																								fairyDirections [k].x -= accel;
																				else
																								fairyDirections [k].x += accel;
																				if (fairyPos.y > attractionPos.y + 1.0f)
																								fairyDirections [k].y -= accel * 0.1f;
																				else
																								fairyDirections [k].y += accel * 0.1f;
																				if (fairyPos.z > attractionPos.z)
																								fairyDirections [k].z -= accel;
																				else
																								fairyDirections [k].z += accel;
																}
												}

								}
				}
}