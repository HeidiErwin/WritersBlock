using UnityEngine;

[ExecuteInEditMode]
public class MegaBookPageAttach : MonoBehaviour
{
	public Vector3			pos;
	public MegaBookBuilder	book;
	public int				page = 0;
	public Vector3			BaryCoord = Vector3.zero;
	public int[]			BaryVerts = new int[3];
	public bool				attached = false;
	public Vector3			BaryCoord1 = Vector3.zero;
	public int[]			BaryVerts1 = new int[3];
	public Vector3			attachforward = new Vector3(1.0f, 0.0f, 0.0f);
	public Vector3			AxisRot = Vector3.zero;
	public GameObject		visibleObj;
	public float			offset = 0.0f;

	void LateUpdate()
	{
		if ( book )
		{
			if ( !attached )
				AttachItUV();

			MegaBookPage pg = book.pages[page];

			if ( visibleObj )
			{
#if UNITY_3_5
				if ( pg.showobjects )
				{
					if ( visibleObj.active )
						visibleObj.SetActiveRecursively(false);

					return;
				}
				else
				{
					if ( !visibleObj.active )
						visibleObj.SetActiveRecursively(true);
				}
#else
				if ( pg.showobjects )
				{
					if ( visibleObj.activeInHierarchy )
						visibleObj.SetActive(false);

					return;
				}
				else
				{
					if ( !visibleObj.activeInHierarchy )
						visibleObj.SetActive(true);
				}
#endif
			}

			GameObject target = pg.obj;

			Vector3 v0 = pg.sverts[BaryVerts[0]];
			Vector3 v1 = pg.sverts[BaryVerts[1]];
			Vector3 v2 = pg.sverts[BaryVerts[2]];

			Vector3 pos = target.transform.localToWorldMatrix.MultiplyPoint(GetCoordMine(v0, v1, v2, BaryCoord));

			// Rotation
			Vector3 va = v1 - v0;
			Vector3 vb = v2 - v1;

			Vector3 norm = Vector3.Cross(va, vb);

			v0 = pg.sverts[BaryVerts1[0]];
			v1 = pg.sverts[BaryVerts1[1]];
			v2 = pg.sverts[BaryVerts1[2]];

			Vector3 fwd = target.transform.localToWorldMatrix.MultiplyPoint(GetCoordMine(v0, v1, v2, BaryCoord1)) - pos;

			Quaternion erot = Quaternion.Euler(AxisRot);
			Quaternion rot = Quaternion.LookRotation(fwd, norm) * erot;

			transform.position = pos + (offset * norm.normalized);
			transform.rotation = rot;
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

	public void DetachIt()
	{
	}

	public void AttachItUV()
	{
		if ( book )
		{
			attached = true;

			MegaBookPage pobj = book.pages[page];

			Bounds bounds = pobj.bbox;

			// Calc local pos from pos in bounds
			Vector3 pp = pos * 0.01f;

			pp.x = Mathf.Clamp01(pp.x);
			pp.y = Mathf.Clamp01(pp.y);
			pp.z = Mathf.Clamp01(pp.z);

			Vector3 lpos = bounds.min + (Vector3.Scale(pp, bounds.size));

			Vector3 objSpacePt = lpos;

			Vector3[] verts = pobj.verts;
			int[] tris = pobj.obj.GetComponent<MeshFilter>().sharedMesh.triangles;
			int index = -1;
			MegaBookNearest.NearestPointOnMesh1(objSpacePt, verts, tris, ref index, ref BaryCoord);

			if ( index >= 0 )
			{
				BaryVerts[0] = tris[index];
				BaryVerts[1] = tris[index + 1];
				BaryVerts[2] = tris[index + 2];
			}

			MegaBookNearest.NearestPointOnMesh1(objSpacePt + attachforward, verts, tris, ref index, ref BaryCoord1);

			if ( index >= 0 )
			{
				BaryVerts1[0] = tris[index];
				BaryVerts1[1] = tris[index + 1];
				BaryVerts1[2] = tris[index + 2];
			}
		}
	}
}
public class MegaBookNearest
{
	public static Vector3 NearestPointOnMesh1(Vector3 pt, Vector3[] verts, int[] tri, ref int index, ref Vector3 bary)
	{
		float nearestSqDist = float.MaxValue;

		Vector3 nearestPt = Vector3.zero;
		nearestSqDist = float.MaxValue;

		for ( int i = 0; i < tri.Length; i += 3 )
		{
			Vector3 a = verts[tri[i]];
			Vector3 b = verts[tri[i + 1]];
			Vector3 c = verts[tri[i + 2]];

			float dist = DistPoint3Triangle3Dbl(pt, a, b, c);

			float possNearestSqDist = dist;

			if ( possNearestSqDist < nearestSqDist )
			{
				index = i;
				bary = mTriangleBary;
				nearestPt = mClosestPoint1;
				nearestSqDist = possNearestSqDist;
			}
		}

		return nearestPt;
	}

	public static float DistPoint3Triangle3Dbl(Vector3 mPoint, Vector3 v0, Vector3 v1, Vector3 v2)
	{
		Vector3 diff = v0 - mPoint;
		Vector3 edge0 = v1 - v0;
		Vector3 edge1 = v2 - v0;
		double a00 = edge0.sqrMagnitude;	//.SquaredLength();
		double a01 = Vector3.Dot(edge1, edge0);
		double a11 = edge1.sqrMagnitude;
		double b0 = Vector3.Dot(edge0, diff);
		double b1 = Vector3.Dot(edge1, diff);
		double c = diff.sqrMagnitude;
		double det = Mathf.Abs((float)a00 * (float)a11 - (float)a01 * (float)a01);
		double s = a01 * b1 - a11 * b0;
		double t = a01 * b0 - a00 * b1;
		double sqrDistance;

		if ( s + t <= det )
		{
			if ( s < (double)0.0 )
			{
				if ( t < (double)0 )  // region 4
				{
					if ( b0 < (double)0 )
					{
						t = (double)0;
						if ( -b0 >= a00 )
						{
							s = (double)1;
							sqrDistance = a00 + ((double)2) * b0 + c;
						}
						else
						{
							s = -b0 / a00;
							sqrDistance = b0 * s + c;
						}
					}
					else
					{
						s = (double)0;
						if ( b1 >= (double)0 )
						{
							t = (double)0;
							sqrDistance = c;
						}
						else if ( -b1 >= a11 )
						{
							t = (double)1;
							sqrDistance = a11 + ((double)2) * b1 + c;
						}
						else
						{
							t = -b1 / a11;
							sqrDistance = b1 * t + c;
						}
					}
				}
				else  // region 3
				{
					s = (double)0;
					if ( b1 >= (double)0 )
					{
						t = (double)0;
						sqrDistance = c;
					}
					else if ( -b1 >= a11 )
					{
						t = (double)1;
						sqrDistance = a11 + ((double)2) * b1 + c;
					}
					else
					{
						t = -b1 / a11;
						sqrDistance = b1 * t + c;
					}
				}
			}
			else if ( t < (double)0 )  // region 5
			{
				t = (double)0;
				if ( b0 >= (double)0 )
				{
					s = (double)0;
					sqrDistance = c;
				}
				else if ( -b0 >= a00 )
				{
					s = (double)1;
					sqrDistance = a00 + ((double)2) * b0 + c;
				}
				else
				{
					s = -b0 / a00;
					sqrDistance = b0 * s + c;
				}
			}
			else  // region 0
			{
				// minimum at interior point
				double invDet = ((double)1) / det;
				s *= invDet;
				t *= invDet;
				sqrDistance = s * (a00 * s + a01 * t + ((double)2) * b0) +
					t * (a01 * s + a11 * t + ((double)2) * b1) + c;
			}
		}
		else
		{
			double tmp0, tmp1, numer, denom;

			if ( s < (double)0 )  // region 2
			{
				tmp0 = a01 + b0;
				tmp1 = a11 + b1;
				if ( tmp1 > tmp0 )
				{
					numer = tmp1 - tmp0;
					denom = a00 - ((double)2) * a01 + a11;
					if ( numer >= denom )
					{
						s = (double)1;
						t = (double)0;
						sqrDistance = a00 + ((double)2) * b0 + c;
					}
					else
					{
						s = numer / denom;
						t = (double)1 - s;
						sqrDistance = s * (a00 * s + a01 * t + ((double)2) * b0) +
							t * (a01 * s + a11 * t + ((double)2) * b1) + c;
					}
				}
				else
				{
					s = (double)0;
					if ( tmp1 <= (double)0 )
					{
						t = (double)1;
						sqrDistance = a11 + ((double)2) * b1 + c;
					}
					else if ( b1 >= (double)0 )
					{
						t = (double)0;
						sqrDistance = c;
					}
					else
					{
						t = -b1 / a11;
						sqrDistance = b1 * t + c;
					}
				}
			}
			else if ( t < (double)0 )  // region 6
			{
				tmp0 = a01 + b1;
				tmp1 = a00 + b0;
				if ( tmp1 > tmp0 )
				{
					numer = tmp1 - tmp0;
					denom = a00 - ((double)2) * a01 + a11;
					if ( numer >= denom )
					{
						t = (double)1;
						s = (double)0;
						sqrDistance = a11 + ((double)2) * b1 + c;
					}
					else
					{
						t = numer / denom;
						s = (double)1 - t;
						sqrDistance = s * (a00 * s + a01 * t + ((double)2) * b0) +
							t * (a01 * s + a11 * t + ((double)2) * b1) + c;
					}
				}
				else
				{
					t = (double)0;
					if ( tmp1 <= (double)0 )
					{
						s = (double)1;
						sqrDistance = a00 + ((double)2) * b0 + c;
					}
					else if ( b0 >= (double)0 )
					{
						s = (double)0;
						sqrDistance = c;
					}
					else
					{
						s = -b0 / a00;
						sqrDistance = b0 * s + c;
					}
				}
			}
			else  // region 1
			{
				numer = a11 + b1 - a01 - b0;
				if ( numer <= (double)0 )
				{
					s = (double)0;
					t = (double)1;
					sqrDistance = a11 + ((double)2) * b1 + c;
				}
				else
				{
					denom = a00 - ((double)2) * a01 + a11;
					if ( numer >= denom )
					{
						s = (double)1;
						t = (double)0;
						sqrDistance = a00 + ((double)2) * b0 + c;
					}
					else
					{
						s = numer / denom;
						t = (double)1 - s;
						sqrDistance = s * (a00 * s + a01 * t + ((double)2) * b0) +
							t * (a01 * s + a11 * t + ((double)2) * b1) + c;
					}
				}
			}
		}

		// Account for numerical round-off error.
		if ( sqrDistance < (double)0 )
			sqrDistance = (double)0;

		//mClosestPoint0 = mPoint;
		mClosestPoint1.x = v0.x + (float)(s * edge0.x + t * edge1.x);
		mClosestPoint1.y = v0.y + (float)(s * edge0.y + t * edge1.y);
		mClosestPoint1.z = v0.z + (float)(s * edge0.z + t * edge1.z);
		mTriangleBary[1] = (float)s;
		mTriangleBary[2] = (float)t;
		mTriangleBary[0] = (float)((double)1 - s - t);
		return (float)sqrDistance;
	}

	static Vector3 mTriangleBary = Vector3.zero;
	static Vector3 mClosestPoint1 = Vector3.zero;
}