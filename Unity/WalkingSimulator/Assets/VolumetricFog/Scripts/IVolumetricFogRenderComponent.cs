//------------------------------------------------------------------------------------------------------------------
// Volumetric Fog & Mist
// Copyright (c) Kronnect Games
//------------------------------------------------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace VolumetricFogAndMist {

			 interface IVolumetricFogRenderComponent {
								VolumetricFog fog { get; set; }
								void DestroySelf();
				}

}