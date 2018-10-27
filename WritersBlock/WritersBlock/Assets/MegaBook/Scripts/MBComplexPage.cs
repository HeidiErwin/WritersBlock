
using UnityEngine;
using System.Collections.Generic;

public class MBComplexPage : MegaBookDynamicMesh
{
	public List<GameObject>	pages		= new List<GameObject>();
	List<Vector3>			verts		= new List<Vector3>();
	List<Vector2>			uvs			= new List<Vector2>();
	List<Color>				cols		= new List<Color>();
	List<int[]>				subtris		= new List<int[]>();
	List<Material>			materials	= new List<Material>();
	public bool				fillbook = false;

	[ContextMenu("Help")]
	public void Help()
	{
		Application.OpenURL("http://www.west-racing.com/mf/?page_id=5881");
	}

	int GetIndex(int page, bool front)
	{
		int index = page * 2;
		if ( !front )
			index++;

		if ( fillbook )
		{
			index = index % (pages.Count);	//(int)Mathf.Repeat((float)index, (float)(pages.Count - 1));
		}

		return index;
	}

	public override int GetNumMaterials(int page, bool front)
	{
		materials.Clear();

		int index = GetIndex(page, front);

		if ( index < pages.Count )
		{
			GameObject obj = pages[index];
			if ( obj )
			{
				Renderer[] rends = obj.GetComponentsInChildren<Renderer>();

				for ( int i = 0; i < rends.Length; i++ )
				{
					for ( int m = 0; m < rends[i].sharedMaterials.Length; m++ )
						materials.Add(rends[i].sharedMaterials[m]);
				}
			}
		}

		return materials.Count;
	}

	public override Material GetMaterial(int page, bool front, int i)
	{
		return materials[i];
	}

	public override void BuildMesh(int page, bool front)
	{
		int index = GetIndex(page, front);

		if ( index < pages.Count )
		{
			GetMeshData(pages[index]);
		}
		else
		{
			verts.Clear();
			uvs.Clear();
			cols.Clear();
			subtris.Clear();
		}
	}

	public void GetMeshData(GameObject obj)
	{
		verts.Clear();
		uvs.Clear();
		cols.Clear();

		subtris.Clear();

		if ( obj )
		{
			Matrix4x4 ptm = obj.transform.worldToLocalMatrix;

			MeshFilter[] filters = obj.GetComponentsInChildren<MeshFilter>();

			int vindex = 0;
			for ( int i = 0; i < filters.Length; i++ )
			{
				MeshFilter mf = filters[i];

				Matrix4x4 tm = ptm * mf.transform.localToWorldMatrix;	// * ptm;

				Mesh mesh = mf.sharedMesh;

				Vector3[] vertices = mesh.vertices;
				Vector2[] uv1 = mesh.uv;
				Color[] colors = mesh.colors;

				for ( int v = 0; v < mesh.vertexCount; v++ )
				{
					verts.Add(tm.MultiplyPoint3x4(vertices[v]));
					uvs.Add(uv1[v]);
				}

				if ( mesh.colors != null && mesh.colors.Length > 0 )
				{
					for ( int v = 0; v < mesh.vertexCount; v++ )
						cols.Add(colors[v]);
				}
				else
					for ( int v = 0; v < mesh.vertexCount; v++ )
						cols.Add(Color.white);

				for ( int m = 0; m < mesh.subMeshCount; m++ )
				{
					int[] tris = mesh.GetTriangles(m);

					for ( int t = 0; t < tris.Length; t++ )
						tris[t] += vindex;

					subtris.Add(tris);
				}

				vindex += mesh.vertexCount;
			}
		}
	}

	public override Vector3[] GetVertices(int page, bool front)
	{
		return verts.ToArray();
	}

	public override Color[] GetColors(int page, bool front)
	{
		return cols.ToArray();
	}

	public override Vector2[] GetUVs(int page, bool front)
	{
		return uvs.ToArray();
	}

	public override int[] GetTris(int page, bool front, int m)
	{
		return subtris[m];
	}
}
