using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace DynamicFogAndMist {
				public class MyFPSCounter : MonoBehaviour {

								const float fpsMeasurePeriod = 0.5f;
								private int m_FpsAccumulator = 0;
								private float m_FpsNextPeriod = 0;
								private int m_CurrentFps;
								const string display = "{0} FPS";
								private Text m_GuiText;

	
								private void Start () {
												m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
												m_GuiText = GetComponent<Text> ();

//		Application.targetFrameRate = 60;
//		QualitySettings.vSyncCount = 1;
								}

	
								private void Update () {
												// measure average frames per second
												m_FpsAccumulator++;
												if (Time.realtimeSinceStartup > m_FpsNextPeriod) {
																m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
																m_FpsAccumulator = 0;
																m_FpsNextPeriod += fpsMeasurePeriod;
																m_GuiText.text = string.Format (display, m_CurrentFps);
												}
								}
				}
}
