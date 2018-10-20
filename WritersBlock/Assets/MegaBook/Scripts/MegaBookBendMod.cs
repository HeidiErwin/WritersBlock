
using UnityEngine;

[System.Serializable]
public enum MegaBookAxis
{
	X,
	Y,
	Z
}

[System.Serializable]
public class MegaBookBendMod
{
	public float		angle			= 0.0f;
	public Vector3		gizmo_pos		= Vector3.zero;
	public Vector3		gizmo_rotation	= Vector3.zero;
	public Vector3		center			= Vector3.zero;
	public float		from			= 0.0f;
	public float		to				= 0.0f;
	public bool			fromto			= false;
	public MegaBookAxis	axis			= MegaBookAxis.X;
	public Vector3		gizmo_scale		= Vector3.one;
	public float		dir				= 0.0f;
	Matrix4x4			tm				= Matrix4x4.identity;
	Matrix4x4			invtm			= Matrix4x4.identity;
	Matrix4x4			mat				= new Matrix4x4();
	Matrix4x4			tmAbove			= new Matrix4x4();
	Matrix4x4			tmBelow			= new Matrix4x4();
	float				r				= 0.0f;

	public void SetAxis(Matrix4x4 tmAxis)
	{
		Matrix4x4 itm = tmAxis.inverse;
		tm = tmAxis * tm;
		invtm = invtm * itm;
	}

	public void SetTM()
	{
		tm = Matrix4x4.identity;
		Quaternion rot = Quaternion.Euler(-gizmo_rotation);

		tm.SetTRS(gizmo_pos + center, rot, gizmo_scale);
		invtm = tm.inverse;
	}

	void CalcR(MegaBookPage page, MegaBookAxis axis, float ang)
	{
		float len = 0.0f;

		if ( !fromto )
		{
			switch ( axis )
			{
				case MegaBookAxis.X: len = page.bbox.max.x - page.bbox.min.x; break;
				case MegaBookAxis.Z: len = page.bbox.max.y - page.bbox.min.y; break;
				case MegaBookAxis.Y: len = page.bbox.max.z - page.bbox.min.z; break;
			}
		}
		else
			len = to - from;

		if ( Mathf.Abs(ang) < 0.0001f )
			r = 0.0f;
		else
			r = len / ang;
	}

	public Vector3 Map(int i, Vector3 p)
	{
		if ( r == 0.0f && !fromto )
			return p;

		p = tm.MultiplyPoint3x4(p);	// tm may have an offset gizmo etc

		if ( fromto )
		{
			if ( p.y <= from )
				return invtm.MultiplyPoint3x4(tmBelow.MultiplyPoint3x4(p));
			else
			{
				if ( p.y >= to )
					return invtm.MultiplyPoint3x4(tmAbove.MultiplyPoint3x4(p));
			}
		}

		if ( r == 0.0f )
			return invtm.MultiplyPoint3x4(p);

		float x = p.x;
		float y = p.y;

		float yr = y / r;

		float c = Mathf.Cos(Mathf.PI - yr);
		float s = Mathf.Sin(Mathf.PI - yr);
		float px = r * c + r - x * c;
		p.x = px;
		float pz = r * s - x * s;
		p.y = pz;
		p = invtm.MultiplyPoint3x4(p);
		return p;
	}

	public void Deform(MegaBookPage page, Vector3[] verts, Vector3[] sverts)
	{
		Calc(page);

		for ( int i = 0; i < verts.Length; i++ )
		{
			if ( r == 0.0f && !fromto )
				sverts[i] = verts[i];
			else
			{
				Vector3 p = tm.MultiplyPoint3x4(verts[i]);	// tm may have an offset gizmo etc

				if ( fromto )
				{
					if ( p.y <= from )
					{
						sverts[i] = invtm.MultiplyPoint3x4(tmBelow.MultiplyPoint3x4(p));
						continue;
					}
					else
					{
						if ( p.y >= to )
						{
							sverts[i] = invtm.MultiplyPoint3x4(tmAbove.MultiplyPoint3x4(p));
							continue;
						}
					}
				}

				if ( r == 0.0f )
					sverts[i] = invtm.MultiplyPoint3x4(p);
				else
				{
					float x = p.x;
					float y = p.y;

					float yr = y / r;

					float c = Mathf.Cos(Mathf.PI - yr);
					float s = Mathf.Sin(Mathf.PI - yr);
					float px = r * c + r - x * c;
					p.x = px;
					float pz = r * s - x * s;
					p.y = pz;
					sverts[i] = invtm.MultiplyPoint3x4(p);
				}
			}
		}
	}

	void Calc(MegaBookPage page)
	{
		SetTM();
		if ( from > to ) from = to;
		if ( to < from ) to = from;

		mat = Matrix4x4.identity;

		switch ( axis )
		{
			case MegaBookAxis.X: MegaBookMatrix.RotateZ(ref mat, Mathf.PI * 0.5f); break;
			case MegaBookAxis.Y: MegaBookMatrix.RotateX(ref mat, -Mathf.PI * 0.5f); break;
			case MegaBookAxis.Z: break;
		}

		MegaBookMatrix.RotateY(ref mat, Mathf.Deg2Rad * dir);
		SetAxis(mat);

		CalcR(page, axis, Mathf.Deg2Rad * angle);

		if ( fromto )
		{
			fromto = false;
			float len  = to - from;
			float rat1, rat2;

			if ( len == 0.0f )
				rat1 = rat2 = 1.0f;
			else
			{
				rat1 = to / len;
				rat2 = from / len;
			}

			Vector3 pt;
			tmAbove = Matrix4x4.identity;
			MegaBookMatrix.Translate(ref tmAbove, 0.0f, -to, 0.0f);
			MegaBookMatrix.RotateZ(ref tmAbove, -Mathf.Deg2Rad * angle * rat1);
			MegaBookMatrix.Translate(ref tmAbove, 0.0f, to, 0.0f);
			pt = new Vector3(0.0f, to, 0.0f);
			MegaBookMatrix.Translate(ref tmAbove, tm.MultiplyPoint3x4(Map(0, invtm.MultiplyPoint3x4(pt))) - pt);

			tmBelow = Matrix4x4.identity;
			MegaBookMatrix.Translate(ref tmBelow, 0.0f, -from, 0.0f);
			MegaBookMatrix.RotateZ(ref tmBelow, Mathf.Deg2Rad * angle * rat2);
			MegaBookMatrix.Translate(ref tmBelow, 0.0f, from, 0.0f);
			pt = new Vector3(0.0f, from, 0.0f);
			MegaBookMatrix.Translate(ref tmBelow, tm.MultiplyPoint3x4(Map(0, invtm.MultiplyPoint3x4(pt))) - pt);

			fromto = true;
		}
	}
}
