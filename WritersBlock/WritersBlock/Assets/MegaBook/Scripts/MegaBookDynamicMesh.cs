
using UnityEngine;

// Use this class to provide the mesh data for the asked for page
// So for example if you have a text asset in your project which you use to build text meshes, you ask that
// system to build a mesh for the text you want to display an then use the methods below to pass that data
// back to the book builder so it can add the text mesh vertices, materials etc to the page mesh it is building.
public class MegaBookDynamicMesh : MonoBehaviour
{
	public MeshFilter	mf;
	public Mesh			mesh;

	public virtual int GetNumMaterials(int page, bool front)
	{
		Renderer rend = GetComponent<Renderer>();
		if ( rend )
			return rend.sharedMaterials.Length;

		return 0;
	}

	public virtual Material GetMaterial(int page, bool front, int i)
	{
		Renderer rend = GetComponent<Renderer>();
		if ( rend )
			return rend.sharedMaterials[i];

		return null;
	}

	public virtual void BuildMesh(int page, bool front)
	{
		if ( !mesh )
		{
			mf = gameObject.GetComponent<MeshFilter>();
			if ( mf )
				mesh = mf.sharedMesh;
		}
	}

	public virtual Vector3[] GetVertices(int page, bool front)
	{
		if ( !mesh )
		{
			mf = gameObject.GetComponent<MeshFilter>();
			if ( mf )
				mesh = mf.sharedMesh;
		}

		if ( mesh )
			return mesh.vertices;

		return new Vector3[0];
	}

	public virtual Color[] GetColors(int page, bool front)
	{
		if ( !mesh )
		{
			mf = gameObject.GetComponent<MeshFilter>();
			if ( mf )
				mesh = mf.sharedMesh;
		}

		if ( mesh )
			return mesh.colors;

		return new Color[0];
	}

	public virtual Vector2[] GetUVs(int page, bool front)
	{
		if ( !mesh )
		{
			mf = gameObject.GetComponent<MeshFilter>();
			if ( mf )
				mesh = mf.sharedMesh;
		}

		if ( mesh )
			return mesh.uv;

		return new Vector2[0];
	}

	public virtual int[] GetTris(int page, bool front, int m)
	{
		if ( !mesh )
		{
			mf = gameObject.GetComponent<MeshFilter>();
			if ( mf )
				mesh = mf.sharedMesh;
		}

		if ( mesh )
			return mesh.GetTriangles(m);

		return new int[0];
	}
}