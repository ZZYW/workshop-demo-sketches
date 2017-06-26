using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playVideo : MonoBehaviour {

	// Use this for initialization
	void Start () {

		Renderer r = GetComponent<Renderer>();
		MovieTexture movie = (MovieTexture)r.material.mainTexture;

		//if (movie.isPlaying)
		//{
		//	movie.Pause();
		//}
		//else
		//{
			movie.Play();
		//}

		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
