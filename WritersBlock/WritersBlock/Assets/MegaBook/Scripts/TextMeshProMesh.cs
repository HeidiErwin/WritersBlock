
using UnityEngine;
using System.Collections.Generic;

// If you have the Typogenic asset change false to true below to use this example
#if false
using TMPro;

public class TextMeshProMesh : MegaBookDynamicMesh
{
	TextMeshPro		tmpro;

	public List<string>	text = new List<string>();

	public override int GetNumMaterials(int page, bool front)
	{
		return renderer.sharedMaterials.Length;
	}

	public override Material GetMaterial(int page, bool front, int i)
	{
		return renderer.sharedMaterials[i];
	}

	public override void BuildMesh(int page, bool front)
	{
		if ( !tmpro )
		{
			tmpro = gameObject.GetComponent<TextMeshPro>();
			mesh = tmpro.mesh;
		}

		int index = page * 2;
		if ( !front )
			index++;

		if ( index < text.Count )
		{
			tmpro.text = text[index];
			tmpro.ForceMeshUpdate();
			mesh = tmpro.mesh;
		}
	}

	public override Vector3[] GetVertices(int page, bool front)
	{
		if ( mesh )
			return mesh.vertices;

		return new Vector3[0];
	}

	public override Color[] GetColors(int page, bool front)
	{
		if ( mesh )
			return mesh.colors;

		return new Color[0];
	}

	public override Vector2[] GetUVs(int page, bool front)
	{
		if ( mesh )
			return mesh.uv;

		return new Vector2[0];
	}

	public override int[] GetTris(int page, bool front, int m)
	{
		if ( mesh )
			return mesh.GetTriangles(m);

		return new int[0];
	}
}
#endif