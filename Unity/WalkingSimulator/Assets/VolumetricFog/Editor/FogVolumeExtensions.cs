using UnityEngine;
using UnityEditor;
using System.Collections;

namespace VolumetricFogAndMist {
	public class FogVolumeExtensions : MonoBehaviour {

		[MenuItem("GameObject/Create Other/Volumetric Fog Volume")]
		static void CreateFogVolume (MenuCommand menuCommand) {
			GameObject fogVolume = Resources.Load<GameObject> ("Prefabs/FogVolume");
			if (fogVolume == null) {
				Debug.LogError ("Could not load FogVolume from Resources/Prefabs folder!");
				return;
			}
			GameObject newFogVolume = Instantiate (fogVolume);
			newFogVolume.name = "Volumetric Fog Volume";

			// Ensure it gets reparented if this was a context click (otherwise does nothing)
			GameObjectUtility.SetParentAndAlign (newFogVolume, menuCommand.context as GameObject);

			// Register root object for undo.
			Undo.RegisterCreatedObjectUndo (newFogVolume, "Create Volumetric Fog Volume");
			Selection.activeObject = newFogVolume;

			// Enables fog volumes in fog component
			VolumetricFog fog = Camera.main.GetComponent<VolumetricFog> ();
			if (fog != null)
				fog.useFogVolumes = true;
		}
	}

}
