using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanController : MonoBehaviour
{
	public int size = 10;
	public float UVScale = 2f;
	public OctaveWave[] waves;

	Mesh oceanMesh;
	MeshFilter meshFilter;

	private void Start()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (oceanMesh != null)
		{ return; }

		oceanMesh = NewMesh();

		meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = oceanMesh;
	}

	Mesh NewMesh()
	{
		Mesh m = new Mesh();
		m.name = gameObject.name;

		m.vertices = GenerateVertices(size);
		m.triangles = GenerateTriangles(m.vertices.Length);
		m.uv = GenerateUVs(m.vertices.Length);
		m.RecalculateNormals();
		m.RecalculateBounds();
		return m;
	}

	Vector3[] GenerateVertices(int meshSize)
	{
		Vector3[] verts = new Vector3[(meshSize + 1) * (meshSize + 1)];

		for (int x = 0; x <= meshSize; ++x)
		{
			for (int z = 0; z <= meshSize; ++z)
			{
				//verts[Index(x, z)] = new Vector3(x - (meshSize / 2f), 0, z - (meshSize / 2f));
				verts[Index(x, z)] = new Vector3(x, 0, z);
			}
		}
		return verts;
	}

	int[] GenerateTriangles(int numberOfVertices)
	{
		int[] tries = new int[numberOfVertices * 6];

		//Two triangles are one tile
		for (int x = 0; x < size; ++x)
		{
			for (int z = 0; z < size; ++z)
			{
				tries[Index(x, z) * 6 + 0] = Index(x, z);
				tries[Index(x, z) * 6 + 1] = Index(x + 1, z + 1);
				tries[Index(x, z) * 6 + 2] = Index(x + 1, z);
				tries[Index(x, z) * 6 + 3] = Index(x, z);
				tries[Index(x, z) * 6 + 4] = Index(x, z + 1);
				tries[Index(x, z) * 6 + 5] = Index(x + 1, z + 1);
			}
		}

		return tries;
	}

	Vector2[] GenerateUVs(int numberOfVertices)
	{
		Vector2[] uvs = new Vector2[numberOfVertices];

		//always set one UV over 'n' tiles than flip  the UV and set it again
		for (int x = 0; x <= size; ++x)
		{
			for (int z = 0; z <= size; ++z)
			{
				Vector2 v = new Vector2((x / UVScale) % 2, (z / UVScale) % 2);
				uvs[Index(x, z)] = new Vector2(v.x <= 1 ? v.x : 2 - v.x, v.y <= 1 ? v.y : 2 - v.y);
			}
		}
		return uvs;
	}

	int Index(int x, int z)
	{
		return x * (size + 1) + z;
	}

	/*
	private void Update()
	{
		FrameUpdate();
	}
	*/

	public void FrameUpdate()
	{
		Vector3[] verts = oceanMesh.vertices;

		for (int x = 0; x <= size; ++x)
		{
			for (int z = 0; z <= size; ++z)
			{
				float y = 0;

				for (int ot = 0; ot < waves.Length; ++ot)
				{
					float perlin = Mathf.PerlinNoise((x * waves[ot].scale.x + Time.time * waves[ot].speed.x) / size,
								(z * waves[ot].scale.y + Time.time * waves[ot].speed.y) / size) - 0.5f;
					y += perlin * waves[ot].height;
				}
				verts[Index(x, z)] = new Vector3(x, y, z);
			}
		}
		oceanMesh.vertices = verts;
		oceanMesh.RecalculateNormals();
	}

	public float GetHeight(Vector3 position)
	{
		//scale factor and position in local space
		//Posição dentro do grid
		Vector3 scale = new Vector3(1 / transform.lossyScale.x, 0, 1 / transform.lossyScale.z);
		Vector3 localPosition = Vector3.Scale((position - transform.position), scale);

		//get edge points
		//Posição das bordas do triangulo/quadrado formado pelos triangulos
		Vector3 p1 = new Vector3(Mathf.Floor(localPosition.x), 0, Mathf.Floor(localPosition.z));
		Vector3 p2 = new Vector3(Mathf.Floor(localPosition.x), 0, Mathf.Ceil(localPosition.z));
		Vector3 p3 = new Vector3(Mathf.Ceil(localPosition.x), 0, Mathf.Floor(localPosition.z));
		Vector3 p4 = new Vector3(Mathf.Ceil(localPosition.x), 0, Mathf.Ceil(localPosition.z));

		//Clamp if the position is outside the plane
		p1 = new Vector3(Mathf.Clamp(p1.x, 0, size), 0, Mathf.Clamp(p1.z, 0, size));
		p2 = new Vector3(Mathf.Clamp(p2.x, 0, size), 0, Mathf.Clamp(p2.z, 0, size));
		p3 = new Vector3(Mathf.Clamp(p3.x, 0, size), 0, Mathf.Clamp(p3.z, 0, size));
		p4 = new Vector3(Mathf.Clamp(p4.x, 0, size), 0, Mathf.Clamp(p4.z, 0, size));

		//get the max distance to one of the edges and take that to compute 'max - dist'
		float max = Mathf.Max(Vector3.Distance(p1, localPosition), Vector3.Distance(p2, localPosition),
			Vector3.Distance(p3, localPosition), Vector3.Distance(p4, localPosition) + Mathf.Epsilon);

		float dist = (max - Vector3.Distance(p1, localPosition))
											+ (max - Vector3.Distance(p2, localPosition))
											+ (max - Vector3.Distance(p3, localPosition))
											+ (max - Vector3.Distance(p4, localPosition) + Mathf.Epsilon);

		//weighted sum
		float height = oceanMesh.vertices[Index((int)p1.x, (int)p1.z)].y * (max - Vector3.Distance(p1, localPosition))
											 + oceanMesh.vertices[Index((int)p2.x, (int)p2.z)].y * (max - Vector3.Distance(p2, localPosition))
											 + oceanMesh.vertices[Index((int)p3.x, (int)p3.z)].y * (max - Vector3.Distance(p3, localPosition))
											 + oceanMesh.vertices[Index((int)p4.x, (int)p4.z)].y * (max - Vector3.Distance(p4, localPosition));

		//scale
		return height * transform.lossyScale.y / dist;
	}
}
