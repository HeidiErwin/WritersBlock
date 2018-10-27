
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MegaBookControl : MonoBehaviour
{
	public float			page		= 0.0f;
	public float			turnspd		= 0.0f;
	public float			turntime	= 1.0f;
	public Vector2			pos			= Vector2.zero;
	public Vector3			guisize		= Vector2.one;
	public Color			col			= Color.white;
	public float			detail		= 1.0f;
	public float			dsize		= 300.0f;
	public float			svd			= 0.9f;
	public Vector2			spos		= new Vector2();
	public float			sx			= 232.0f;	//190.0f;
	public Rect				windowRect	= new Rect(1600 - 410, 5, 250, 300);
	public float			topspace	= 30.0f;
	public GUISkin			skin;
	public MegaBookBuilder	book;
	public Vector2			guipos		= Vector2.zero;
	public Vector2			guisize1	= Vector2.zero;
	static public float		mousesensi	= 1.0f;
	public bool				ShowGui		= false;
	static public bool		mouseover	= false;
	float					lastscreenheight = 0.0f;

	void Start()
	{
	}

	void Update()
	{
		if ( Input.GetKeyDown(KeyCode.Escape) )
			ShowGui = !ShowGui;

		if ( Input.GetKeyDown(KeyCode.RightArrow) )
			book.NextPage();

		if ( Input.GetKeyDown(KeyCode.LeftArrow) )
			book.PrevPage();

		//if ( Input.GetKeyDown(KeyCode.D) )
		//{
			//StartCoroutine(book.DownloadTexture("file:///d:/fraps/morph.jpg", (int)page, true));
			//Debug.Log("downloading");
		//}
	}

	void DoWindow(int windowID)
	{
		ShowGUI();
		GUI.DragWindow();
	}

	void SizeChange()
	{
		lastscreenheight = Screen.height;
	}

	public void NextPage()
	{
		book.NextPage();
	}

	public void PrevPage()
	{
		book.PrevPage();
	}

	void OnGUI()
	{
		if ( ShowGui )
		{
			GUI.skin = skin;
			if ( Screen.height != lastscreenheight )
			{
				SizeChange();
			}

			windowRect = GUILayout.Window(0, windowRect, DoWindow, "Book Options", GUILayout.MinWidth(250));	//, GUILayout.MinWidth(150), GUILayout.MaxWidth(400));	//BeginArea(new Rect(10, 5, 175, 700));
		}

		GUI.skin = skin;
		GUI.color = col;

		GUI.skin = skin;
		GUILayout.BeginArea(new Rect(Screen.width - guipos.x, Screen.height - guipos.y, guisize1.x, guisize1.y));

		GUILayout.BeginHorizontal();

		GUILayout.BeginVertical();
		if ( book )
		{
			GUILayout.Label("Page " + book.page);
			GUI.color = col;
			book.page = GUILayout.HorizontalSlider(book.page, book.MinPageVal(), book.MaxPageVal(), GUILayout.Width(400), GUILayout.Height(30));

			GUILayout.EndVertical();

			if ( GUILayout.Button("Prev", GUILayout.Width(100)) )
				book.PrevPage();

			if ( GUILayout.Button("Next", GUILayout.Width(100)) )
				book.NextPage();
		}

		ShowGui = GUILayout.Toggle(ShowGui, "Options");

		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	bool Slider(string name, ref float val, float min, float max)
	{
		GUILayout.Label(name + " - " + val.ToString("0.00"));
		float nval = GUILayout.HorizontalSlider(val, min, max);

		if ( nval != val )
		{
			val = nval;
			return true;
		}

		return false;
	}

	void ShowGUI()
	{
		spos = GUILayout.BeginScrollView(spos);
		GUI.color = col;

		if ( book )
		{
			GUILayout.Label("Turn Speed - " + book.GetTurnTime().ToString("0.00"));
			book.SetTurnTime(GUILayout.HorizontalSlider(book.GetTurnTime(), 0.0f, 3.0f));

			book.SetSnap(GUILayout.Toggle(book.GetSnap(), "Snap"));

			// Book params
			if ( Slider("Turn Radius", ref book.Turn_CArea, 0.01f, 2.0f) )
				book.Turn_CAreaChange(book.Turn_CArea);

			if ( Slider("Turn Position", ref book.Turn_CCenter, 0.0f, 1.0f) )
				book.Turn_CCenterChange(book.Turn_CCenter);

			if ( Slider("Max Angle", ref book.Turn_maxAngle, -180.0f, 180.0f) )
				book.Turn_maxAngleChange(book.Turn_maxAngle);

			if ( Slider("Min Angle", ref book.Turn_minAngle, -270.0f, 90.0f) )
				book.Turn_minAngleChange(book.Turn_minAngle);

			book.changespineangle = GUILayout.Toggle(book.changespineangle, "Change Spine Ang");

			if ( Slider("Spine Angle", ref book.BottomAngle, -book.BottomMaxAngle, 0.0f) )
			{
			}
		}
		GUILayout.EndScrollView();

		if ( GUILayout.Button("Close") )
			ShowGui = false;
	}
}