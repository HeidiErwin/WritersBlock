
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MegaBookPageObject
{
	public Vector3		pos;
	public Vector3		rot;
	public float		offset;
	public GameObject	obj;
	public Vector3		BaryCoord = Vector3.zero;
	public int[]		BaryVerts = new int[3];
	public bool			attached = false;
	public Vector3		BaryCoord1 = Vector3.zero;
	public int[]		BaryVerts1 = new int[3];
	public Vector3		attachforward = new Vector3(0.005f, 0.0f, 0.0f);
	public bool			overridevisi = false;
	public float		visilow = -1.0f;
	public float		visihigh = 1.0f;
	public bool			message = false;
}

[System.Serializable]
public class MegaBookPageParams
{
	public Texture2D		front;
	public Texture2D		back;
	public Rect				copyarea;
	public Texture2D		background;
	public Rect				copyarea1;
	public Texture2D		background1;
	public Texture2D		madefront;
	public Texture2D		madeback;
	public Material			frontmat;
	public Material			backmat;
	public Material			edgemat;
	public Mesh				pagemesh;
	public Mesh				holemesh;
	public GameObject		pageobj;
	public GameObject		holeobj;
	public int				frontmatindex = -1;
	public int				backmatindex = -1;
	public bool				usebackground = true;
	public bool				usebackground1 = true;
	public Vector3			rotate = Vector3.zero;
	public bool				swapsides = false;
	public bool				stiff = false;
	public Mesh				extramesh;
	public List<MegaBookPageObject>	objects = new List<MegaBookPageObject>();
	public float			visobjlow = -1.0f;
	public float			visobjhigh = 1.0f;
	public bool				alphatexturefront = false;
	public bool				alphatextureback = false;
	//public string			text;
}

[System.Serializable]
public class MegaBookPage
{
	public MegaBookBendMod	flexer;
	public MegaBookBendMod	turner;
	public MegaBookBendMod	lander;
	public Vector3			pivot;
	public float			width;
	public float			length;
	public Vector3[]		verts;
	public Vector3[]		sverts;
	public GameObject		obj;
	public Mesh				mesh;
	public Bounds			bbox;
	public AnimationCurve	landerangcon	= new AnimationCurve();
	public AnimationCurve	flexangcon		= new AnimationCurve();
	public AnimationCurve	turnerangcon	= new AnimationCurve();
	public AnimationCurve	turnerfromcon	= new AnimationCurve();
	public bool				deform			= false;
	public float			turnangle		= 0.0f;
	public bool				stiff			= false;
	public Mesh				holemesh;
	public Vector3[]		verts1;
	public Vector3[]		sverts1;
	public MeshFilter		mf;
	public bool				showinghole;
	public bool				showobjects;
	public List<MegaBookPageObject>	objects = new List<MegaBookPageObject>();
	public float			visobjlow = -1.0f;
	public float			visobjhigh = 1.0f;

	public MeshCollider		collider;
	//public string			text;
	public int				pnum;

	bool Equals(float v1, float v2)
	{
		if ( Mathf.Abs(v1 - v2) > 0.00001f )
			return true;

		return false;
	}

	public void Update(MegaBookBuilder book, float flip, bool dohole)
	{
		if ( book.runcontrollers )
		{
			float angle = -landerangcon.Evaluate(flip);
			if ( Equals(angle, lander.angle) )
			{
				lander.angle = angle;
				deform = true;
			}

			angle = -flexangcon.Evaluate(flip);
			if ( Equals(angle, flexer.angle) )
			{
				flexer.angle = angle;
				deform = true;
			}

			angle = -turnerangcon.Evaluate(flip);
			angle += book.BottomAngle;
			if ( Equals(angle, turner.angle) )
			{
				turner.angle = angle;
				deform = true;
			}

			float from = turnerfromcon.Evaluate(flip);
			if ( Equals(from, turner.from) )
			{
				turner.from = from;
				deform = true;
			}
		}

		if ( flip <= (visobjlow * 14.0f) || flip >= (visobjhigh * 14.0f) )
			showobjects = false;
		else
			showobjects = true;

		if ( (flip < -14.0f || flip >= 28.0f) && holemesh && dohole )
		{
			if ( showinghole == false )
			{
				deform = true;
				showinghole = true;
			}
		}
		else
		{
			if ( showinghole )
			{
				deform = true;
				showinghole = false;
			}
		}

		if ( deform )
		{
			deform = false;

			if ( stiff )
			{
				//if ( book.enableturner )
				//	turner.Deform(this, verts, sverts);

			}
			else
			{
				if ( showinghole )
				{
					if ( book.enableflexer )
						flexer.Deform(this, verts1, sverts1);
					else
					{
						for ( int i = 0; i < verts1.Length; i++ )
							sverts1[i] = verts1[i];
					}

					if ( book.enablelander )
						lander.Deform(this, sverts1, sverts1);

					if ( book.enableturner )
						turner.Deform(this, sverts1, sverts1);

					holemesh.vertices = sverts1;
					holemesh.RecalculateBounds();
					holemesh.RecalculateNormals();

					if ( mf.sharedMesh != holemesh )
						mf.sharedMesh = holemesh;

					if ( book.updatecollider && collider )
					{
						collider.sharedMesh = null;
						//collider.sharedMesh = holemesh;
						collider.enabled = false;
					}
				}
				else
				{
					if ( book.enableflexer )
						flexer.Deform(this, verts, sverts);
					else
					{
						for ( int i = 0; i < verts.Length; i++ )
							sverts[i] = verts[i];
					}

					if ( book.enablelander )
						lander.Deform(this, sverts, sverts);

					if ( book.enableturner )
						turner.Deform(this, sverts, sverts);

					mesh.vertices = sverts;
					mesh.RecalculateBounds();
					mesh.RecalculateNormals();

					if ( mf.sharedMesh != mesh )
						mf.sharedMesh = mesh;

					// Could do this only when show objects is on
					if ( book.updatecollider && collider )	//&& showobjects )
					{
						//Debug.Log("update collider " + flip);
						collider.enabled = true;
						//if ( collider.sharedMesh != null )
						{
							collider.sharedMesh = null;
							collider.sharedMesh = mesh;
						}
						//else
							//collider.sharedMesh = mesh;
					}
				}
			}
		}

		// Update attached objects
		for ( int i = 0; i < objects.Count; i++ )
		{
			UpdateAttached(objects[i], flip);
		}
	}

	void UpdateAttached(MegaBookPageObject pobj, float flip)
	{
		GameObject target = pobj.obj;

		if ( target )
		{
			bool show = showobjects;

			if ( pobj.overridevisi )
			{
				if ( flip <= (pobj.visilow * 14.0f) || flip >= (pobj.visihigh * 14.0f) )
					show = false;
				else
					show = true;
			}

#if UNITY_3_5
			if ( show )	//objects )
			{
				if ( !target.active )
					target.SetActiveRecursively(true);
			}
			else
			{
				if ( target.active )
					target.SetActiveRecursively(false);
				return;
			}
#else
			if ( show )	//objects )
			{
				if ( !target.activeInHierarchy )
					target.SetActive(true);
			}
			else
			{
				if ( target.activeInHierarchy )
					target.SetActive(false);
				return;
			}
#endif
			if ( pobj.message )
			{
				pobj.obj.SendMessage("BookVisibility", ((flip / 14.0f) - pobj.visilow) / (pobj.visihigh - pobj.visilow), SendMessageOptions.DontRequireReceiver);
			}
			Vector3 v0 = sverts[pobj.BaryVerts[0]];
			Vector3 v1 = sverts[pobj.BaryVerts[1]];
			Vector3 v2 = sverts[pobj.BaryVerts[2]];

			Vector3 pos = obj.transform.localToWorldMatrix.MultiplyPoint(GetCoordMine(v0, v1, v2, pobj.BaryCoord));

			// Rotation
			Vector3 va = v1 - v0;
			Vector3 vb = v2 - v1;

			Vector3 norm = obj.transform.TransformDirection(Vector3.Cross(va, vb).normalized);

			v0 = sverts[pobj.BaryVerts1[0]];
			v1 = sverts[pobj.BaryVerts1[1]];
			v2 = sverts[pobj.BaryVerts1[2]];

			Vector3 fwd = obj.transform.localToWorldMatrix.MultiplyPoint(GetCoordMine(v0, v1, v2, pobj.BaryCoord1)) - pos;
			//Debug.DrawRay(pos, fwd.normalized);

			//norm = obj.transform.TransformDirection(norm);
			Quaternion erot = Quaternion.Euler(pobj.rot);

			Quaternion rot = Quaternion.identity;
			if ( fwd == Vector3.zero )
				rot = erot;
			else
				rot = Quaternion.LookRotation(fwd, norm) * erot;

			//Quaternion rot = Quaternion.LookRotation(fwd, norm) * erot;
			//Quaternion rot = Quaternion.FromToRotation(obj.transform.up, norm) * erot;
			target.transform.position = pos + (pobj.offset * norm);	//.normalized);
			target.transform.rotation = rot;
		}
	}

	Vector3 GetCoordMine(Vector3 A, Vector3 B, Vector3 C, Vector3 bary)
	{
		Vector3 p = Vector3.zero;
		p.x = (bary.x * A.x) + (bary.y * B.x) + (bary.z * C.x);
		p.y = (bary.x * A.y) + (bary.y * B.y) + (bary.z * C.y);
		p.z = (bary.x * A.z) + (bary.y * B.z) + (bary.z * C.z);

		return p;
	}

	public void DoWork(MegaBookBuilder book, float flip)
	{
		if ( deform )
		{
			if ( book.enableflexer )
				flexer.Deform(this, verts, sverts);
			else
			{
				for ( int i = 0; i < verts.Length; i++ )
					sverts[i] = verts[i];
			}

			if ( book.enablelander )
				lander.Deform(this, sverts, sverts);

			if ( book.enableturner )
				turner.Deform(this, sverts, sverts);
		}
	}

	public void PreMT(MegaBookBuilder book, float flip)
	{
		if ( book.runcontrollers )
		{
			float angle = -landerangcon.Evaluate(flip);
			if ( Equals(angle, lander.angle) )
			{
				lander.angle = angle;
				deform = true;
			}

			angle = -flexangcon.Evaluate(flip);
			if ( Equals(angle, flexer.angle) )
			{
				flexer.angle = angle;
				deform = true;
			}

			angle = -turnerangcon.Evaluate(flip);
			angle += book.BottomAngle;
			if ( Equals(angle, turner.angle) )
			{
				turner.angle = angle;
				deform = true;
			}

			float from = turnerfromcon.Evaluate(flip);
			if ( Equals(from, turner.from) )
			{
				turner.from = from;
				deform = true;
			}
		}
	}
}
