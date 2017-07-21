using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DynamicFogAndMist {
				public enum FOG_TYPE {
								DesktopFogWithSkyHaze = 0,
								MobileFogWithSkyHaze = 1,
								MobileFogOnlyGround = 2,
								DesktopFogPlusWithSkyHaze = 3,
								MobileFogSimple = 4
				}

				static class FOG_TYPE_Ext {
								public static bool isPlus (this FOG_TYPE fogType) {
												return fogType == FOG_TYPE.DesktopFogPlusWithSkyHaze || fogType == FOG_TYPE.MobileFogSimple;
								}
				}

				public enum FOG_PRESET {
								Clear,
								Mist,
								WindyMist,
								GroundFog,
								Fog,
								HeavyFog,
								SandStorm,
								Custom
				}

				[ExecuteInEditMode]
				[RequireComponent (typeof(Camera))]
				[HelpURL ("http://kronnect.com/taptapgo")]
				[ImageEffectAllowedInSceneView]
				public class DynamicFog : MonoBehaviour {

								public FOG_TYPE effectType = FOG_TYPE.DesktopFogPlusWithSkyHaze;
								public FOG_PRESET preset = FOG_PRESET.Mist;
								public bool useFogVolumes = false;
								public bool enableDithering = false;
								[Range (0, 1)]
								public float
												alpha = 1.0f;
								[Range (0, 1)]
								public float
												noiseStrength = 0.5f;
								[Range (0, 0.999f)]
								public float
												distance = 0.1f;
								[Range (0.0001f, 2f)]
								public float
												distanceFallOff = 0.01f;
								[Range (0, 1.2f)]
								public float
												maxDistance = 0.999f;
								[Range (0.0001f, 0.5f)]
								public float
												maxDistanceFallOff = 0f;
								[Range (0, 500)]
								public float
												height = 1f;
								[Range (0, 1)]
								public float
												heightFallOff = 0.1f;
								public float
												baselineHeight = 0;
								public bool clipUnderBaseline = false;
								[Range (0, 15)]
								public float
												turbulence = 0.1f;
								[Range (0, 0.2f)]
								public float
												speed = 0.1f;
								public Color color = Color.white;
								public Color color2 = Color.gray;
								[Range (0, 500)]
								public float
												skyHaze = 50f;
								[Range (0, 1)]
								public float
												skySpeed = 0.3f;
								[Range (0, 1)]
								public float
												skyNoiseStrength = 0.1f;
								[Range (0, 1)]
								public float
												skyAlpha = 1.0f;
								public GameObject sun;
								public bool fogOfWarEnabled = false;
								public Vector3 fogOfWarCenter;
								public Vector3 fogOfWarSize = new Vector3 (1024, 0, 1024);
								public int fogOfWarTextureSize = 256;
								Material fogMatAdv, fogMatFogSky, fogMatOnlyFog, fogMatVol, fogMatSimple;
								[SerializeField]
								Material
												fogMat;
								float initialFogAlpha, targetFogAlpha;
								float initialSkyHazeAlpha, targetSkyHazeAlpha;
								float transitionDuration;
								float transitionStartTime;
								float currentFogAlpha, currentSkyHazeAlpha;
								Camera currentCamera;
								Texture2D fogOfWarTexture;
								Color32[] fogOfWarColorBuffer;
								Light sunLight;
								Vector3 sunDirection = Vector3.zero;
								Color sunColor = Color.white;
								float sunIntensity = 1f;
								static DynamicFog _fog;
								List<string> shaderKeywords;

								public static DynamicFog instance { 
												get { 
																if (_fog == null) {
																				foreach (Camera camera in Camera.allCameras) {
																								_fog = camera.GetComponent<DynamicFog> ();
																								if (_fog != null)
																												break;
																				}
																}
																return _fog;
												} 
								}

								public string GetCurrentPresetName () {
												return Enum.GetName (typeof(FOG_PRESET), preset);
								}

								public Camera fogCamera {
												get {
																return currentCamera;
												}
								}

								// Creates a private material used to the effect
								void OnEnable () {
												Init ();
												UpdateMaterialProperties ();
								}

								void Reset () {
												UpdateMaterialProperties ();
								}

								void OnDestroy () {
												fogMat = null;
												if (fogMatVol != null) {
																DestroyImmediate (fogMatVol);
																fogMatVol = null;
												}
												if (fogMatAdv != null) {
																DestroyImmediate (fogMatAdv);
																fogMatAdv = null;
												}
												if (fogMatFogSky != null) {
																DestroyImmediate (fogMatFogSky);
																fogMatFogSky = null;
												}
												if (fogMatOnlyFog != null) {
																DestroyImmediate (fogMatOnlyFog);
																fogMatOnlyFog = null;
												}
												if (fogMatSimple != null) {
																DestroyImmediate (fogMatSimple);
																fogMatSimple = null;
												}
												if (fogOfWarTexture != null) {
																DestroyImmediate (fogOfWarTexture);
																fogOfWarTexture = null;
												}
								}

								void Init () {
												targetFogAlpha = -1;
												targetSkyHazeAlpha = -1;
												currentFogAlpha = alpha;
												currentSkyHazeAlpha = skyAlpha;
												currentCamera = GetComponent<Camera> ();
												if (currentCamera.depthTextureMode == DepthTextureMode.None) {
																currentCamera.depthTextureMode = DepthTextureMode.Depth;
												}
												UpdateFogOfWarTexture ();
								}

								// Check possible alpha transition
								void Update () {
												if (fogMat == null)
																return;
												// Check transitions
												if (targetFogAlpha >= 0) {
																if (targetFogAlpha != currentFogAlpha || targetSkyHazeAlpha != currentSkyHazeAlpha) {
																				if (transitionDuration > 0) {
																								currentFogAlpha = Mathf.Lerp (initialFogAlpha, targetFogAlpha, (Time.time - transitionStartTime) / transitionDuration);
																								currentSkyHazeAlpha = Mathf.Lerp (initialSkyHazeAlpha, targetSkyHazeAlpha, (Time.time - transitionStartTime) / transitionDuration);
																				} else {
																								currentFogAlpha = targetFogAlpha;
																								currentSkyHazeAlpha = targetSkyHazeAlpha;
																				}
																				fogMat.SetFloat ("_FogAlpha", currentFogAlpha);
																				SetSkyData ();
																}
												} else if (currentFogAlpha != alpha || targetSkyHazeAlpha != currentSkyHazeAlpha) {
																if (transitionDuration > 0) {
																				currentFogAlpha = Mathf.Lerp (initialFogAlpha, alpha, (Time.time - transitionStartTime) / transitionDuration);
																				currentSkyHazeAlpha = Mathf.Lerp (initialSkyHazeAlpha, alpha, (Time.time - transitionStartTime) / transitionDuration);
																} else {
																				currentFogAlpha = alpha;
																				currentSkyHazeAlpha = skyAlpha;
																}
																fogMat.SetFloat ("_FogAlpha", currentFogAlpha);
																SetSkyData ();
												}

												// Updates sun illumination
												if (sun != null) {
																bool needFogColorUpdate = false;
																if (sun.transform.forward != sunDirection) {
																				needFogColorUpdate = true;
																}
																if (sunLight != null) {
																				if (sunLight.color != sunColor || sunLight.intensity != sunIntensity) {
																								needFogColorUpdate = true;
																				}
																}
																if (needFogColorUpdate)
																				UpdateFogColor ();
												}

								}

								public void CheckPreset () {
												switch (preset) {
												case FOG_PRESET.Clear:
																alpha = 0;
																break;
												case FOG_PRESET.Mist:
																alpha = 0.75f;
																skySpeed = 0.11f;
																skyHaze = 15;
																skyNoiseStrength = 1;
																skyAlpha = 0.33f;
																distance = 0;
																distanceFallOff = 0.07f;
																height = 4.4f;
																heightFallOff = 1;
																turbulence = 0;
																noiseStrength = 0.6f;
																speed = 0.01f;
																color = new Color (0.89f, 0.89f, 0.89f, 1);
																color2 = color;
																maxDistance = 0.999f;
																maxDistanceFallOff = 0f;
																break;
												case FOG_PRESET.WindyMist:
																alpha = 0.75f;
																skySpeed = 0.3f;
																skyHaze = 35;
																skyNoiseStrength = 0.32f;
																skyAlpha = 0.33f;
																distance = 0;
																distanceFallOff = 0.07f;
																height = 2f;
																heightFallOff = 1;
																turbulence = 2;
																noiseStrength = 0.6f;
																speed = 0.06f;
																color = new Color (0.89f, 0.89f, 0.89f, 1);
																color2 = color;
																maxDistance = 0.999f;
																maxDistanceFallOff = 0f;
																break;
												case FOG_PRESET.GroundFog:
																alpha = 1;
																skySpeed = 0.3f;
																skyHaze = 35;
																skyNoiseStrength = 0.32f;
																skyAlpha = 0.33f;
																distance = 0;
																distanceFallOff = 0;
																height = 1f;
																heightFallOff = 1;
																turbulence = 0.4f;
																noiseStrength = 0.7f;
																speed = 0.005f;
																color = new Color (0.89f, 0.89f, 0.89f, 1);
																color2 = color;
																maxDistance = 0.999f;
																maxDistanceFallOff = 0f;
																break;
												case FOG_PRESET.Fog:
																alpha = 0.96f;
																skySpeed = 0.3f;
																skyHaze = 155;
																skyNoiseStrength = 0.6f;
																skyAlpha = 0.93f;
																distance = effectType.isPlus () ? 0.2f : 0.01f;
																distanceFallOff = 0.04f;
																height = 20f;
																heightFallOff = 1;
																turbulence = 0.4f;
																noiseStrength = 0.4f;
																speed = 0.005f;
																color = new Color (0.89f, 0.89f, 0.89f, 1);
																color2 = color;
																maxDistance = 0.999f;
																maxDistanceFallOff = 0f;
																break;
												case FOG_PRESET.HeavyFog:
																alpha = 1;
																skySpeed = 0.05f;
																skyHaze = 350;
																skyNoiseStrength = 0.8f;
																skyAlpha = 0.97f;
																distance = effectType.isPlus () ? 0.1f : 0f;
																distanceFallOff = 0.045f;
																height = 35f;
																heightFallOff = 0.88f;
																turbulence = 0.4f;
																noiseStrength = 0.24f;
																speed = 0.003f;
																color = new Color (0.86f, 0.847f, 0.847f, 1);
																color2 = color;
																maxDistance = 0.999f;
																maxDistanceFallOff = 0f;
																break;
												case FOG_PRESET.SandStorm:
																alpha = 1;
																skySpeed = 0.49f;
																skyHaze = 333;
																skyNoiseStrength = 0.72f;
																skyAlpha = 0.97f;
																distance = effectType.isPlus () ? 0.15f : 0f;
																distanceFallOff = 0.028f;
																height = 83f;
																heightFallOff = 0;
																turbulence = 15;
																noiseStrength = 0.45f;
																speed = 0.2f;
																color = new Color (0.364f, 0.36f, 0.36f, 1);
																color2 = color;
																maxDistance = 0.999f;
																maxDistanceFallOff = 0f;
																break;
												}
								}

								// Postprocess the image
								void OnRenderImage (RenderTexture source, RenderTexture destination) {
												if (fogMat == null || alpha == 0 || currentCamera == null) {
																Graphics.Blit (source, destination);
																return;
												}
												fogMat.SetMatrix ("_ClipToWorld", currentCamera.cameraToWorldMatrix * currentCamera.projectionMatrix.inverse);
												Graphics.Blit (source, destination, fogMat);
								}

								public void UpdateMaterialProperties () {
												CheckPreset ();
			
												switch (effectType) {
												case FOG_TYPE.MobileFogOnlyGround:
																if (fogMatOnlyFog == null) {
																				fogMatOnlyFog = Instantiate (Resources.Load<Material> ("Materials/DFGOnlyFog")) as Material;
																				fogMatOnlyFog.hideFlags = HideFlags.DontSave;
																}
																fogMat = fogMatOnlyFog;
																break;
												case FOG_TYPE.MobileFogWithSkyHaze:
																if (fogMatFogSky == null) {
																				fogMatFogSky = Instantiate (Resources.Load<Material> ("Materials/DFGWithSky")) as Material;
																				fogMatFogSky.hideFlags = HideFlags.DontSave;
																}
																fogMat = fogMatFogSky;
																break;
												case FOG_TYPE.DesktopFogPlusWithSkyHaze:
																if (fogMatVol == null) {
																				fogMatVol = Instantiate (Resources.Load<Material> ("Materials/DFGDesktopPlus")) as Material;
																				fogMatVol.hideFlags = HideFlags.DontSave;
																}
																fogMat = fogMatVol;
																break;
												case FOG_TYPE.MobileFogSimple:
																if (fogMatSimple == null) {
																				fogMatSimple = Instantiate (Resources.Load<Material> ("Materials/DFGSimple")) as Material;
																				fogMatSimple.hideFlags = HideFlags.DontSave;
																}
																fogMat = fogMatSimple;
																break;
												default:
																if (fogMatAdv == null) {
																				fogMatAdv = Instantiate (Resources.Load<Material> ("Materials/DFGDesktop")) as Material;
																				fogMatAdv.hideFlags = HideFlags.DontSave;
																}
																fogMat = fogMatAdv;
																break;
												}

												if (fogMat == null)
																return;

												if (currentCamera == null)
																currentCamera = GetComponent<Camera> ();

												fogMat.SetFloat ("_FogSpeed", speed);

												Vector3 noiseData = new Vector3 (noiseStrength, turbulence, currentCamera.farClipPlane * 15.0f / 1000f);
												fogMat.SetVector ("_FogNoiseData", noiseData);

												Vector4 heightData = new Vector4 (height + 0.001f, baselineHeight, clipUnderBaseline ? -0.01f : -10000, heightFallOff);
												fogMat.SetVector ("_FogHeightData", heightData);

												fogMat.SetFloat ("_FogAlpha", currentFogAlpha);
												Vector4 distanceData = new Vector4 (distance, distanceFallOff, maxDistance, maxDistanceFallOff);
												if (effectType.isPlus ()) {
																distanceData.x = currentCamera.farClipPlane * distance;
																distanceData.y = distanceFallOff * distanceData.x + 0.0001f;
																distanceData.z *= currentCamera.farClipPlane;
												}
												fogMat.SetVector ("_FogDistance", distanceData);

												UpdateFogColor ();
												SetSkyData ();


												if (shaderKeywords == null) {
																shaderKeywords = new List<string> ();
												} else {
																shaderKeywords.Clear ();
												}

												if (fogOfWarEnabled) {
																if (fogOfWarTexture == null) {
																				UpdateFogOfWarTexture ();
																}
																fogMat.SetTexture ("_FogOfWar", fogOfWarTexture);
																fogMat.SetVector ("_FogOfWarCenter", fogOfWarCenter);
																fogMat.SetVector ("_FogOfWarSize", fogOfWarSize);
																Vector3 ca = fogOfWarCenter - 0.5f * fogOfWarSize;
																fogMat.SetVector ("_FogOfWarCenterAdjusted", new Vector3 (ca.x / fogOfWarSize.x, 1f, ca.z / fogOfWarSize.z));
																shaderKeywords.Add ("FOG_OF_WAR_ON");
												} 
												if (enableDithering) {
																shaderKeywords.Add ("DITHER_ON");
												}
												fogMat.shaderKeywords = shaderKeywords.ToArray ();
								}

								void SetSkyData () {
												// x = haze, y = speed, z = noise, w = alpha
												Vector4 skyData = new Vector4 (skyHaze, skySpeed, skyNoiseStrength, currentSkyHazeAlpha);
												fogMat.SetVector ("_FogSkyData", skyData);
								}

								void UpdateFogColor () {
												if (fogMat == null)
																return;

												if (sun != null) {
																if (sunLight == null)
																				sunLight = sun.GetComponent<Light> ();
																if (sunLight != null && sunLight.transform != sun.transform) {
																				sunLight = sun.GetComponent<Light> ();
																}
																sunDirection = sun.transform.forward;
																if (sunLight != null) {
																				sunColor = sunLight.color;
																				sunIntensity = sunLight.intensity;
																}
												}
			
												float fogIntensity = sunIntensity * Mathf.Clamp01 (1.0f - sunDirection.y);
												fogMat.SetColor ("_FogColor", fogIntensity * color * sunColor);
												fogMat.SetColor ("_FogColor2", fogIntensity * color2 * sunColor);
								}

								public void SetTargetAlpha (float newFogAlpha, float newSkyHazeAlpha, float duration) {
												if (!useFogVolumes)
																return;
												this.initialFogAlpha = currentFogAlpha;
												this.initialSkyHazeAlpha = currentSkyHazeAlpha;
												this.targetFogAlpha = newFogAlpha;
												this.targetSkyHazeAlpha = newSkyHazeAlpha;
												this.transitionDuration = duration;
												this.transitionStartTime = Time.time;
								}

								public void ClearTargetAlpha (float duration) {
												SetTargetAlpha (-1, -1, duration);
								}

								#region Fog of War stuff

								void UpdateFogOfWarTexture () {
												if (!fogOfWarEnabled)
																return;
												int size = GetScaledSize (fogOfWarTextureSize, 1.0f);
												//			fogOfWarTexture = new Texture2D(size, size, TextureFormat.Alpha8, false);
												fogOfWarTexture = new Texture2D (size, size, TextureFormat.ARGB32, false);
												fogOfWarTexture.hideFlags = HideFlags.DontSave;
												fogOfWarTexture.filterMode = FilterMode.Bilinear;
												fogOfWarTexture.wrapMode = TextureWrapMode.Clamp;
												ResetFogOfWar ();
								}

								/// <summary>
								/// Changes the alpha value of the fog of war at world position. It takes into account FogOfWarCenter and FogOfWarSize.
								/// Note that only x and z coordinates are used. Y (vertical) coordinate is ignored.
								/// </summary>
								/// <param name="worldPosition">in world space coordinates.</param>
								/// <param name="radius">radius of application in world units.</param>
								public void SetFogOfWarAlpha (Vector3 worldPosition, float radius, float fogNewAlpha) {
												if (fogOfWarTexture == null)
																return;
			
												float tx = (worldPosition.x - fogOfWarCenter.x) / fogOfWarSize.x + 0.5f;
												if (tx < 0 || tx > 1f)
																return;
												float tz = (worldPosition.z - fogOfWarCenter.z) / fogOfWarSize.z + 0.5f;
												if (tz < 0 || tz > 1f)
																return;
			
												int tw = fogOfWarTexture.width;
												int th = fogOfWarTexture.height;
												int px = (int)(tx * tw);
												int pz = (int)(tz * th);
												int colorBufferPos = pz * tw + px;
												byte newAlpha8 = (byte)(fogNewAlpha * 255);
												Color32 existingColor = fogOfWarColorBuffer [colorBufferPos];
												if (newAlpha8 != existingColor.a) { // just to avoid over setting the texture in an Update() loop
																float tr = radius / fogOfWarSize.z;
																int delta = Mathf.FloorToInt (th * tr);
																for (int r = pz - delta; r <= pz + delta; r++) {
																				if (r > 0 && r < th - 1) {
																								for (int c = px - delta; c <= px + delta; c++) {
																												if (c > 0 && c < tw - 1) {
																																int distance = Mathf.FloorToInt (Mathf.Sqrt ((pz - r) * (pz - r) + (px - c) * (px - c)));
																																if (distance <= delta) {
																																				colorBufferPos = r * tw + c;
																																				Color32 colorBuffer = fogOfWarColorBuffer [colorBufferPos];
																																				colorBuffer.a = (byte)Mathf.Lerp (newAlpha8, colorBuffer.a, (float)distance / delta);
																																				fogOfWarColorBuffer [colorBufferPos] = colorBuffer;
																																				fogOfWarTexture.SetPixel (c, r, colorBuffer);
																																}
																												}
																								}
																				}
																}
																fogOfWarTexture.Apply ();
												}
								}

								public void ResetFogOfWarAlpha (Vector3 worldPosition, float radius) {
												if (fogOfWarTexture == null)
																return;
			
												float tx = (worldPosition.x - fogOfWarCenter.x) / fogOfWarSize.x + 0.5f;
												if (tx < 0 || tx > 1f)
																return;
												float tz = (worldPosition.z - fogOfWarCenter.z) / fogOfWarSize.z + 0.5f;
												if (tz < 0 || tz > 1f)
																return;
			
												int tw = fogOfWarTexture.width;
												int th = fogOfWarTexture.height;
												int px = (int)(tx * tw);
												int pz = (int)(tz * th);
												int colorBufferPos = pz * tw + px;
												float tr = radius / fogOfWarSize.z;
												int delta = Mathf.FloorToInt (th * tr);
												for (int r = pz - delta; r <= pz + delta; r++) {
																if (r > 0 && r < th - 1) {
																				for (int c = px - delta; c <= px + delta; c++) {
																								if (c > 0 && c < tw - 1) {
																												int distance = Mathf.FloorToInt (Mathf.Sqrt ((pz - r) * (pz - r) + (px - c) * (px - c)));
																												if (distance <= delta) {
																																colorBufferPos = r * tw + c;
																																Color32 colorBuffer = fogOfWarColorBuffer [colorBufferPos];
																																colorBuffer.a = 255;
																																fogOfWarColorBuffer [colorBufferPos] = colorBuffer;
																																fogOfWarTexture.SetPixel (c, r, colorBuffer);
																												}
																								}
																				}
																}
																fogOfWarTexture.Apply ();
												}
								}

								public void ResetFogOfWar () {
												if (fogOfWarTexture == null)
																return;
												int h = fogOfWarTexture.height;
												int w = fogOfWarTexture.width;
												int newLength = h * w;
												if (fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length != newLength) {
																fogOfWarColorBuffer = new Color32[newLength];
												}
												Color32 opaque = new Color32 (255, 255, 255, 255);
												for (int k = 0; k < newLength; k++)
																fogOfWarColorBuffer [k] = opaque;
												fogOfWarTexture.SetPixels32 (fogOfWarColorBuffer);
												fogOfWarTexture.Apply ();
								}

								int GetScaledSize (int size, float factor) {
												size = (int)(size / factor);
												size /= 4;
												if (size < 1)
																size = 1;
												return size * 4;
								}

								#endregion

				}

}