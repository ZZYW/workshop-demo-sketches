using UnityEngine;
using System.Collections;

namespace VolumetricFogAndMist {
				public class DemoMultipleFogAreas : MonoBehaviour {

								void OnGUI () {
												Rect rect = new Rect (10, 30, Screen.width - 20, 30);
												GUI.Label (rect, "Press C key to create a cloud-shape fog area, B for box-shape fog area, X to remove all.");
								}

								void Update() {
												if (Input.GetKeyDown(KeyCode.C)) {
																CreateCloud();
												} else	if (Input.GetKeyDown(KeyCode.B)) {
																CreateBoxFog();
												} else if (Input.GetKeyDown(KeyCode.X)) {
																VolumetricFog.RemoveAllFogAreas();
												}

								}

								/// <summary>
								/// Create a random cloud shape fog area near camera position.
								/// </summary>
								void CreateCloud() {
												Vector3 position = Camera.main.transform.position + Camera.main.transform.forward * 100 +  Random.insideUnitSphere * 50f;
												if (position.y<10) position.y = 10f;
												float radius = Random.value * 50f + 85f;
												VolumetricFog fog = VolumetricFog.CreateFogArea(position, radius);
												fog.color = new Color(0.6f,0.57f,0.5f,1f);
								}

								/// <summary>
								/// Create a random box-shape fog area near camera position.
								/// </summary>
								void CreateBoxFog() {
												Vector3 position = Camera.main.transform.position + Camera.main.transform.forward * 100 +  Random.insideUnitSphere * 50f;
												if (position.y<10) position.y = 10f;
												Vector3 size = new Vector3(Random.value * 50f + 35f, Random.value * 10f + 15f, Random.value * 50f + 35f);
												VolumetricFog fog = VolumetricFog.CreateFogArea(position, size);
												fog.color = new Color(0.6f,0.57f,0.5f,1f);
												fog.noiseScale = 2f;
								}
				}
}