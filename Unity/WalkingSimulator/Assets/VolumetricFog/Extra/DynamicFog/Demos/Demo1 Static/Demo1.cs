using UnityEngine;
using System.Collections;

namespace DynamicFogAndMist {

				public class Demo1 : MonoBehaviour {

								static GUIStyle labelStyle;

								void OnGUI () {
												if (labelStyle == null) {
																labelStyle = new GUIStyle (GUI.skin.label);
																labelStyle.normal.textColor = Color.black;
												}

												Rect rect = new Rect (10, 10, Screen.width - 20, 30);
												GUI.Label (rect, "Demo 1 scene: windy mist fog style. Notice the subtle fog animation. To change look, select Main Camera and check image effect settings in inspector.", labelStyle);
								}
				}
}