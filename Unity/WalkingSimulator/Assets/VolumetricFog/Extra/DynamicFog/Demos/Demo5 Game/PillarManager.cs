using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DynamicFogAndMist {
				public class PillarManager : MonoBehaviour {

								float initialX;

								void Start () {
												initialX = transform.position.x;
												RepositionPillar();
								}
	
								void Update () {
												if (transform.position.z<Camera.main.transform.position.z - transform.localScale.z) RepositionPillar();
								}

								void RepositionPillar() {
												Vector3 pos = transform.position + Vector3.forward * 200f; // move back
												transform.position = new Vector3(initialX + Random.value * 6 - 3f, pos.y, pos.z);
								}

								void OnCollisionEnter(Collision collision) {
												GetComponent<AudioSource>().Play();
								}

				}
}