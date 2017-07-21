using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DynamicFogAndMist {
				public class BonusCylinderHit : MonoBehaviour {

								void OnCollisionEnter(Collision collision) {
												Game.instance.AnnotateScore(100);
												// Initiate cylinder bonus effect
												GameObject ball = collision.collider.gameObject;
												StartCoroutine(DestroyCylinder(ball));
												// Play sound
												Destroy(ball.GetComponent<Rigidbody>());
												ball.transform.Find("Sounds/Hit").GetComponent<AudioSource>().Play();
								}

								IEnumerator DestroyCylinder(GameObject ball) {
												GetComponent<Renderer>().sharedMaterial.color = Color.cyan;
												for (int k=10;k>0;k++) {
																transform.localScale *= 0.8f;
																yield return new WaitForSeconds(0.1f);
																if (ball!=null) ball.GetComponent<Renderer>().enabled = !ball.GetComponent<Renderer>().enabled;
												}
												Destroy(ball); 	// enough time for the sound to finish
												Destroy(gameObject);
								}
				}
}