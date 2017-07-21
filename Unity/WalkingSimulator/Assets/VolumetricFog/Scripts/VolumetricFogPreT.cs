//------------------------------------------------------------------------------------------------------------------
// Volumetric Fog & Mist
// Copyright (c) Kronnect Games
//------------------------------------------------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace VolumetricFogAndMist {

				[ExecuteInEditMode]
				[RequireComponent (typeof(Camera), typeof(VolumetricFog))]
				public class VolumetricFogPreT : MonoBehaviour, IVolumetricFogRenderComponent {

								public VolumetricFog fog { get; set; }

								// Postprocess the image
								void OnPreRender () {
												if (fog != null)
																fog.DoOnPreRender ();
								}

								[ImageEffectOpaque]
								void OnRenderImage (RenderTexture source, RenderTexture destination) {
												if (fog != null)
																fog.DoOnRenderImage (source, destination);
								}

								IEnumerator OnPostRender () {
												if (fog != null && fog.improveTransparency) {
																// Apply a second pass only over the transparent objects
																yield return new WaitForEndOfFrame ();
																if (fog != null)
																				fog.DoOnPostRender ();
												} 
								}

								public void DestroySelf() {
												DestroyImmediate(this);
								}


				}

}