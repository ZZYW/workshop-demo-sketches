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
				public class VolumetricFogPosT : MonoBehaviour, IVolumetricFogRenderComponent {
	
								public VolumetricFog fog { get; set; }

								void OnRenderImage (RenderTexture source, RenderTexture destination) {
												if (fog != null)
																fog.DoOnRenderImage (source, destination);
								}

								public void DestroySelf() {
												DestroyImmediate(this);
								}

				}

}