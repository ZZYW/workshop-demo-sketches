using UnityEngine;
using System.Collections;

namespace DynamicFogAndMist {
				public class DemoFogOfWar : MonoBehaviour {
								bool fogCuttingOn = true;
								static GUIStyle labelStyle;

								void Update () {
												DynamicFog fog = DynamicFog.instance;
												if (Input.GetKeyDown (KeyCode.F)) {
																switch (fog.preset) {
																case FOG_PRESET.Custom:
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
																				fog.preset = FOG_PRESET.Mist;
																				break;
																}
												} else if (Input.GetKeyDown (KeyCode.T)) {
																fog.enabled = !fog.enabled;
												} else if (Input.GetKeyDown (KeyCode.C)) {
																fogCuttingOn = !fogCuttingOn;
																fog.fogOfWarEnabled = fogCuttingOn;
																fog.ResetFogOfWar ();
												} else if (Input.GetKeyDown (KeyCode.R)) {
																fog.ResetFogOfWar ();
												}
			
												if (fogCuttingOn) {
																fog.SetFogOfWarAlpha (Camera.main.transform.position, 4, 0);
												}
								}

								void OnGUI () {
												if (labelStyle == null) {
																labelStyle = new GUIStyle (GUI.skin.label);
																labelStyle.normal.textColor = Color.black;
												}

												Rect rect = new Rect (10, 10, Screen.width - 20, 30);
												GUI.Label (rect, "Move around with WASD or cursor keys, space to jump, F key to change fog style, T to toggle fog on/off, C to toggle fog cutting, R to reset fog.", labelStyle);
												rect = new Rect (10, 30, Screen.width - 20, 30);
												GUI.Label (rect, "Current fog preset: " + DynamicFog.instance.GetCurrentPresetName (), labelStyle);
												if (fogCuttingOn) {
																rect = new Rect (10, 50, Screen.width - 20, 30);
																GUI.Label (rect, "FOG CUTTING ON", labelStyle);
												}
								}
				}
}