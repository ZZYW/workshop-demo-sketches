using UnityEngine;
using System.Collections;

namespace DynamicFogAndMist {

				public class Demo2 : MonoBehaviour {

								static GUIStyle labelStyle;

								void OnGUI () {
												if (labelStyle == null) {
																labelStyle = new GUIStyle (GUI.skin.label);
																labelStyle.normal.textColor = Color.black;
												}

												Rect rect = new Rect (10, 10, Screen.width - 20, 30);
												GUI.Label (rect, "Demo 2 scene: move with WASD. Advance to find the rotating sign which will make the fog disappear.", labelStyle);
								}
				}
}