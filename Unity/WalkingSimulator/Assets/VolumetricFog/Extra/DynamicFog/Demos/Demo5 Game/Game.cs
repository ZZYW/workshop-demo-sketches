using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DynamicFogAndMist {
				public class Game : MonoBehaviour {

								GameObject ball;
								GUIStyle labelStyle;
								int score, impacts, fireCount;
								string scoreText;
								static Game _instance;

								public static Game instance {
												get {
																if (_instance == null) {
																				GameObject go = GameObject.Find("Demo");
																				_instance = go.GetComponent<Game>();
																}
																return _instance;
												}
								}


								void Start () {
												// Init some variables and materials
												score = 0;
												fireCount = 0;
												ball = GameObject.Find("Ball");
												ball.SetActive(false);

												// Create pillars
												GameObject root = new GameObject("Pillars");
												GameObject pillarRef = GameObject.Find("Pillar");
												for (float z=-100;z<100;z+=20f) {
																for (float x=-100;x<100;x+=10f) {
																				GameObject pillar = Instantiate(pillarRef) as GameObject;
																				pillar.transform.SetParent(root.transform);
																				pillar.transform.position = new Vector3(x, -30f, z);
																				pillar.transform.localScale = new Vector3(8f, Random.Range(20f, 40f) * (Mathf.Abs(pillar.transform.position.x) * 0.01f + 1f),  16f);

																				if (Random.value>0.8f) {
																								AddBonusCylinder(pillar.transform);
																				}

																}
												}

												// Initializes scoring text
												UpdateScoreText();
								}

								void AddBonusCylinder(Transform pillarTransform) {
												Vector3 position =  pillarTransform.position + Vector3.up * pillarTransform.localScale.y * 0.5f;
												GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
												Material fogMat = Instantiate(Resources.Load<Material>("Materials/DFMLambertSolidColor")) as Material;
												fogMat.color = new Color(1f, 0.5f, 0.1f);
												cylinder.GetComponent<Renderer>().sharedMaterial = fogMat;
												cylinder.transform.position = position;
												cylinder.transform.localScale = Vector3.one * 3f;
												cylinder.transform.SetParent(pillarTransform, true);
												cylinder.AddComponent<BonusCylinderHit>();
								}
	
								void FixedUpdate () {
												// Make camera advance
												Camera.main.transform.position += Camera.main.transform.forward * Time.deltaTime * 20f;
								}

								void Update() {
												// Launch ball?
												if (Input.GetMouseButtonDown(0)) {
																StartCoroutine(LaunchBall());
												}
								}

								void OnGUI() {
												if (labelStyle==null) {
																labelStyle = new GUIStyle(GUI.skin.label);
																labelStyle.normal.textColor = Color.black;
																labelStyle.fontSize = 40;
												}
												GUI.Label(new Rect(10,10,1000,60), scoreText, labelStyle);
								}


								IEnumerator LaunchBall() {
												fireCount++;
												UpdateScoreText();
												GameObject ballClone = Instantiate(ball) as GameObject;
												ballClone.transform.position = Camera.main.transform.position;
												ballClone.SetActive(true);
												Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
												Rigidbody rb = ballClone.GetComponent<Rigidbody>();
												rb.velocity = (ray.direction * 80f) + Camera.main.transform.forward * 20f;
												ballClone.transform.Find("Sounds/ShootSound").GetComponent<AudioSource>().Play();
												yield return new WaitForSeconds(10f);
												Destroy(ballClone);
								}


								public void AnnotateScore(int points) {
												score += points;
												impacts ++;
												UpdateScoreText();
								}

								void UpdateScoreText() {
												scoreText = "Score: " + score.ToString();
												if (fireCount>0) {
																scoreText += "  Balls Fired: " + fireCount + " (" + (impacts*100f/fireCount).ToString("F1") + "%)";
												}
								}

				}
}