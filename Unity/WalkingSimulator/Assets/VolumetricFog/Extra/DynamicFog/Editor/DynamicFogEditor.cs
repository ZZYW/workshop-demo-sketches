using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace DynamicFogAndMist
{
	[CustomEditor (typeof(DynamicFog))]
	public class DynamicFogEditor : Editor
	{

		static GUIStyle titleLabelStyle, sectionHeaderStyle;
		static Color titleColor;
		static bool[] expandSection = new bool[5];
		const string SECTION_PREFS = "DynamicFogExpandSection";
		static GUIContent[] FOG_TYPE_OPTIONS = new GUIContent[] { 
												new GUIContent ("Desktop Fog + Sky Haze"),
												new GUIContent ("Desktop Fog Plus + Sky Haze"), 
												new GUIContent ("Mobile Fog + Sky Haze"), 
												new GUIContent ("Mobile Fog (No Sky Haze)"),
												new GUIContent ("Mobile Fog (Simplified)")
								};
		static int[] FOG_TYPE_VALUES = new int[] { 0, 3, 1, 2, 4 };
		static string[] sectionNames = new string[] {
												"Fog Properties",
												"Sky Properties",
												"Fog of War"
								};
		const int FOG_PROPERTIES = 0;
		const int SKY_PROPERTIES = 1;
		const int FOG_OF_WAR = 2;
		SerializedProperty effectType, preset, useFogVolumes, enableDithering;
		SerializedProperty alpha, noiseStrength, distance, distanceFallOff, maxDistance, maxDistanceFallOff, height, heightFallOff, baselineHeight, clipUnderBaseline, turbulence, speed, color, color2;
		SerializedProperty skyHaze, skySpeed, skyNoiseStrength, skyAlpha, sun;
		SerializedProperty fogOfWarEnabled, fogOfWarCenter, fogOfWarSize, fogOfWarTextureSize;

		void OnEnable ()
		{
			titleColor = EditorGUIUtility.isProSkin ? new Color (0.52f, 0.66f, 0.9f) : new Color (0.12f, 0.16f, 0.4f);
			for (int k = 0; k < expandSection.Length; k++) {
				expandSection [k] = EditorPrefs.GetBool (SECTION_PREFS + k, false);
			}
			effectType = serializedObject.FindProperty ("effectType");
			preset = serializedObject.FindProperty ("preset");
			useFogVolumes = serializedObject.FindProperty ("useFogVolumes");
			enableDithering = serializedObject.FindProperty ("enableDithering");

			alpha = serializedObject.FindProperty ("alpha");
			noiseStrength = serializedObject.FindProperty ("noiseStrength");
			distance = serializedObject.FindProperty ("distance");
			distanceFallOff = serializedObject.FindProperty ("distanceFallOff");
			maxDistance = serializedObject.FindProperty ("maxDistance");
			maxDistanceFallOff = serializedObject.FindProperty ("maxDistanceFallOff");
			height = serializedObject.FindProperty ("height");
			heightFallOff = serializedObject.FindProperty ("heightFallOff");
			baselineHeight = serializedObject.FindProperty ("baselineHeight");
			clipUnderBaseline = serializedObject.FindProperty ("clipUnderBaseline");
			turbulence = serializedObject.FindProperty ("turbulence");
			speed = serializedObject.FindProperty ("speed");

			color = serializedObject.FindProperty ("color");
			color2 = serializedObject.FindProperty ("color2");
			skyHaze = serializedObject.FindProperty ("skyHaze");
			skySpeed = serializedObject.FindProperty ("skySpeed");
			skyNoiseStrength = serializedObject.FindProperty ("skyNoiseStrength");
			skyAlpha = serializedObject.FindProperty ("skyAlpha");
			sun = serializedObject.FindProperty ("sun");

			fogOfWarEnabled = serializedObject.FindProperty ("fogOfWarEnabled");
			fogOfWarCenter = serializedObject.FindProperty ("fogOfWarCenter");
			fogOfWarSize = serializedObject.FindProperty ("fogOfWarSize");
			fogOfWarTextureSize = serializedObject.FindProperty ("fogOfWarTextureSize");
		}

		void OnDestroy ()
		{
			// Save folding sections state
			for (int k = 0; k < expandSection.Length; k++) {
				EditorPrefs.SetBool (SECTION_PREFS + k, expandSection [k]);
			}
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.UpdateIfDirtyOrScript ();

			if (sectionHeaderStyle == null) {
				sectionHeaderStyle = new GUIStyle (EditorStyles.foldout);
			}
			sectionHeaderStyle.normal.textColor = titleColor;
			sectionHeaderStyle.margin = new RectOffset (12, 0, 0, 0);
			sectionHeaderStyle.fontStyle = FontStyle.Bold;

			if (titleLabelStyle == null) {
				titleLabelStyle = new GUIStyle (EditorStyles.label);
			}
			titleLabelStyle.normal.textColor = titleColor;
			titleLabelStyle.fontStyle = FontStyle.Bold;


			EditorGUILayout.Separator ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("General Settings", titleLabelStyle);
			if (GUILayout.Button ("Help", GUILayout.Width (40))) {
				if (!EditorUtility.DisplayDialog ("Dynamic Fog & Mist", "To learn more about a property in this inspector move the mouse over the label for a quick description (tooltip).\n\nPlease check README file in the root of the asset for details and contact support.\n\nIf you like Dynamic Fog & Mist, please rate it on the Asset Store. For feedback and suggestions visit our support forum on kronnect.com.", "Close", "Visit Support Forum")) {
					Application.OpenURL ("http://kronnect.com/taptapgo");
				}
			}
			EditorGUILayout.EndHorizontal ();


			int prevEffectType = effectType.intValue;
			int prevPreset = preset.intValue;
			bool prevFogVolumes = useFogVolumes.boolValue;
			int prevDither = enableDithering.intValue;

			EditorGUILayout.IntPopup (effectType, FOG_TYPE_OPTIONS, FOG_TYPE_VALUES, new GUIContent ("Effect Type", "Choose a shader variant. Each variant provides different capabilities. Read documentation for explanation."));

			EditorGUILayout.PropertyField (preset, new GUIContent ("Preset", "Amount of detail of the liquid effect. The 'Simple' setting does not use 3D textures which makes it compatible with mobile."));
			EditorGUILayout.PropertyField (useFogVolumes, new GUIContent ("Use Fog Volumes", "Enables fog volumes. These are zones which changes the transparency of the fog automatically, either making it disappear or appear."));

			int effect = effectType.intValue;

			if (effect >= 3) {
				EditorGUILayout.PropertyField (enableDithering, new GUIContent ("Enable Dithering", "Reduces banding artifacts."));
			}


			EditorGUILayout.Separator ();
			expandSection [FOG_PROPERTIES] = EditorGUILayout.Foldout (expandSection [FOG_PROPERTIES], sectionNames [FOG_PROPERTIES], sectionHeaderStyle);

			if (expandSection [FOG_PROPERTIES]) {
				EditorGUILayout.PropertyField (alpha, new GUIContent ("Alpha", "Global fog transparency. You can also change the transparency at color level."));
				if (effect != 4)
					EditorGUILayout.PropertyField (noiseStrength, new GUIContent ("Noise Strength", "Set this value to zero to use solid colors."));
				EditorGUILayout.PropertyField (distance, new GUIContent ("Distance", "The starting distance of the fog measure in linear 0-1 values (0=camera near clip, 1=camera far clip)."));
				EditorGUILayout.PropertyField (distanceFallOff, new GUIContent ("Distance Fall Off", "Makes the fog appear smoothly on the near distance."));
				if (effect < 4)
					EditorGUILayout.PropertyField (maxDistance, new GUIContent ("Max Distance", "The end distance of the fog measure in linear 0-1 values (0=camera near clip, 1=camera far clip)."));
				if (effect < 3) {
					EditorGUILayout.PropertyField (maxDistanceFallOff, new GUIContent ("Distance Fall Off", "Makes the fog disappear smoothly on the far distance."));
				}

				EditorGUILayout.PropertyField (height, new GUIContent ("Height", "Height of the fog in meters."));
				EditorGUILayout.PropertyField (heightFallOff, new GUIContent ("Height Fall Off", "Increase to make the fog change gradually its density based on height."));
				EditorGUILayout.PropertyField (baselineHeight, new GUIContent ("Baseline Height", "Vertical position of the fog in meters. Height is counted above this baseline height."));

				if (effect < 3) {
					EditorGUILayout.PropertyField (clipUnderBaseline, new GUIContent ("Clip Under Baseline", "Enable this property to only render fog above baseline height."));
					EditorGUILayout.PropertyField (turbulence, new GUIContent ("Turbulence", "Amount of fog turbulence."));
				}

				EditorGUILayout.PropertyField (speed, new GUIContent ("Speed", "Speed of fog animation if noise strength or turbulence > 0 (turbulence not available in Desktop Fog Plus mode)."));

				EditorGUILayout.PropertyField (color);
				if (effect != 4)
					EditorGUILayout.PropertyField (color2);
			}

			if (effect != 2 && effect != 4) {
				EditorGUILayout.Separator ();
				expandSection [SKY_PROPERTIES] = EditorGUILayout.Foldout (expandSection [SKY_PROPERTIES], sectionNames [SKY_PROPERTIES], sectionHeaderStyle);

				if (expandSection [SKY_PROPERTIES]) {
					EditorGUILayout.PropertyField (skyHaze, new GUIContent ("Haze", "Vertical range for the sky haze."));
					EditorGUILayout.PropertyField (skySpeed, new GUIContent ("Speed", "Speed of sky haze animation."));
					EditorGUILayout.PropertyField (skyNoiseStrength, new GUIContent ("Noise Strength", "Amount of noise for the sky haze effect."));
					EditorGUILayout.PropertyField (skyAlpha, new GUIContent ("Alpha", "Transparency of sky haze."));
					EditorGUILayout.PropertyField (sun, new GUIContent ("Sun", "Assign a game object (a directional light acting as Sun for example) to make the fog color sync automatically with the Sun orientation and light intensity."));
				}
			}

			EditorGUILayout.Separator ();
			expandSection [FOG_OF_WAR] = EditorGUILayout.Foldout (expandSection [FOG_OF_WAR], sectionNames [FOG_OF_WAR], sectionHeaderStyle);

			if (expandSection [FOG_OF_WAR]) {
				EditorGUILayout.PropertyField (fogOfWarEnabled, new GUIContent ("Enabled", "Enables fog of war feature. This requires that you assign a fog of war mask texture at runtime. Read documentation or demo scene for details."));
				EditorGUILayout.PropertyField (fogOfWarCenter, new GUIContent ("Center", "World space position of the center of the fog of war mask texture."));
				EditorGUILayout.PropertyField (fogOfWarSize, new GUIContent ("Area Size", "Size of the fog of war area in world space units."));
				EditorGUILayout.PropertyField (fogOfWarTextureSize, new GUIContent ("Texture Size", "Size of the fog of war mask texture."));
			}

			EditorGUILayout.Separator ();

			if (serializedObject.ApplyModifiedProperties () || (Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "UndoRedoPerformed")) {
				DynamicFog fog = (DynamicFog)target;
				if (prevPreset == preset.intValue && prevEffectType == effectType.intValue && prevFogVolumes == useFogVolumes.boolValue && prevDither == enableDithering.intValue)
					fog.preset = FOG_PRESET.Custom;
				fog.UpdateMaterialProperties ();
			}
		}

	}

}
