using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour 
{
	float scale = 10.0f;
	float speed = 1.0f;
	Vector3[] baseHeight;


	void Update () 
	{
		var mesh = GetComponent<MeshFilter> ().mesh;

		if (baseHeight == null)
			baseHeight = mesh.vertices;

		var vertices = new Vector3[baseHeight.Length];

		for (int i = 0; i < vertices.Length; i++) 
		{
			var vertex = baseHeight [i];
			vertex.y += Mathf.Sin (Time.time * speed + baseHeight [i].x + baseHeight [i].y + baseHeight [i].z) * scale;
			vertices [i] = vertex;
		}

		mesh.vertices = vertices;
		mesh.RecalculateNormals();
	}
}
