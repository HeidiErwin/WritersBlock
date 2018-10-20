
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaBookBuilder))]
public class MegaBookBuilderEditor : Editor
{
	//SerializedProperty _prop_page;
	//SerializedProperty _prop_linkeditpage;
	//SerializedProperty _prop_snap;
	//SerializedProperty _prop_turntime;
	//SerializedProperty _prop_numpages;

	[MenuItem("GameObject/Create Other/MegaBook")]
	static void CreateBook()
	{
		Vector3 pos = Vector3.zero;

		if ( UnityEditor.SceneView.lastActiveSceneView )
			pos = UnityEditor.SceneView.lastActiveSceneView.pivot;

		GameObject go = new GameObject("Book");

		MegaBookBuilder book = go.AddComponent<MegaBookBuilder>();

		book.pagesectioncrv = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
		book.basematerial = (Material)Resources.Load("Materials/MegaBook Front", typeof(Material));
		book.basematerial1 = (Material)Resources.Load("Materials/MegaBook Back", typeof(Material));
		book.basematerial2 = (Material)Resources.Load("Materials/MegaBook Edge", typeof(Material));

		for ( int i = 0; i < book.NumPages; i++ )
		{
			MegaBookPageParams page = new MegaBookPageParams();

			if ( i == 0 )
			{
				page.front = (Texture2D)Resources.Load("Textures/MegaBook Cover", typeof(Texture2D));
				page.back = (Texture2D)Resources.Load("Textures/MegaBook InsideFrontCover", typeof(Texture2D));
			}
			else
			{
				if ( i == book.NumPages - 1 )
				{
					page.front = (Texture2D)Resources.Load("Textures/MegaBook InsideBackCover", typeof(Texture2D));
					page.back = (Texture2D)Resources.Load("Textures/MegaBook BackCover", typeof(Texture2D));
				}
				else
				{
					page.front = (Texture2D)Resources.Load("Textures/MegaBook Front", typeof(Texture2D));
					page.back = (Texture2D)Resources.Load("Textures/MegaBook Back", typeof(Texture2D));
				}
			}

			book.pageparams.Add(page);
		}

		go.transform.position = pos;
		Selection.activeObject = go;
		book.rebuild = true;
	}

	private MegaBookBuilder		src;
	private MegaBookUndo		undoManager;

	private void OnEnable()
	{
		src = target as MegaBookBuilder;

		// Instantiate undoManager
		undoManager = new MegaBookUndo(src, "MegaBook Param Change");

		//_prop_page = serializedObject.FindProperty("page");
		//_prop_linkeditpage = serializedObject.FindProperty("linkeditpage");
		//_prop_snap = serializedObject.FindProperty("Snap");
		//_prop_turntime = serializedObject.FindProperty("turntime");
		//_prop_numpages = serializedObject.FindProperty("NumPages");
	}

	void SwapTextures(MegaBookBuilder mod, int t1, int t2)
	{
		if ( t1 >= 0 && t1 < mod.pagetextures.Count && t2 >= 0 && t2 < mod.pagetextures.Count && t1 != t2 )
		{
			Texture2D mt1 = mod.pagetextures[t1];
			mod.pagetextures.RemoveAt(t1);
			mod.pagetextures.Insert(t2, mt1);
			EditorUtility.SetDirty(target);
		}
	}

	void SwapPages(MegaBookBuilder mod, int t1, int t2)
	{
		if ( t1 >= 0 && t1 < mod.pages.Count && t2 >= 0 && t2 < mod.pages.Count && t1 != t2 )
		{
			MegaBookPageParams mt1 = mod.pageparams[t1];
			mod.pageparams.RemoveAt(t1);
			mod.pageparams.Insert(t2, mt1);
			EditorUtility.SetDirty(target);
		}
	}

	bool CheckTextureReadable(Texture2D texture)
	{
		if ( texture != null )
		{
			string texturePath = AssetDatabase.GetAssetPath(texture);
			TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(texturePath);
			if ( textureImporter.isReadable )
				return true;
			else
			{
				EditorUtility.DisplayDialog("Texture Select", "Texture is not set to Readable", "OK");
				return false;
			}
		}

		return true;
	}

	public override void OnInspectorGUI()
	{
		MegaBookBuilder mod = (MegaBookBuilder)target;

		//serializedObject.Update();

		undoManager.CheckUndo();

#if !UNITY_5
		EditorGUIUtility.LookLikeControls();
#endif

		bool update = false;
		bool updatemesh = false;

		if ( Event.current.type == EventType.ExecuteCommand && Event.current.commandName != null )
		{
			if ( Event.current.commandName == "UndoRedoPerformed")
				mod.rebuild = true;
		}

		//if ( GUILayout.Button("Apply Scaling") )
		//{
			//mod.ApplyScaling();
		//}

		//EditorGUILayout.Slider(_prop_page, mod.MinPageVal(), mod.MaxPageVal(), new GUIContent("Page"));
		mod.page = EditorGUILayout.Slider("Page", mod.page, mod.MinPageVal(), mod.MaxPageVal());
		if ( GUI.changed )
		{
			serializedObject.ApplyModifiedProperties();

			if ( mod.linkeditpage )
			{
				mod.editpage = (int)(mod.page + 0.5f);
				mod.editpage = Mathf.Clamp(mod.editpage, 0, mod.pageparams.Count - 1);
			}

			mod.Flip = mod.page;
			mod.turnspd = 0.0f;
			EditorUtility.SetDirty(target);
			GUI.changed = false;
		}

		//mod.shuffle = EditorGUILayout.FloatField("Shuffle", mod.shuffle);

		//EditorGUILayout.PropertyField(_prop_linkeditpage, new GUIContent("Link Edit Page"));
		//EditorGUILayout.PropertyField(_prop_snap, new GUIContent("Snap"));
		//EditorGUILayout.PropertyField(_prop_turntime, new GUIContent("Turn Time"));
		mod.linkeditpage = EditorGUILayout.Toggle("Link Edit Page", mod.linkeditpage);
		mod.Snap = EditorGUILayout.Toggle("Snap", mod.Snap);
		mod.turntime = EditorGUILayout.FloatField("Turn Time", mod.turntime);

		//int ival = mod.NumPages;
		//EditorGUILayout.PropertyField(_prop_numpages, new GUIContent("Num Pages"));

		int ival = EditorGUILayout.IntField("Num Pages", mod.NumPages);
		if ( ival < 1 )
			ival = 1;
		//if ( mod.NumPages < 1 )
			//mod.NumPages = 1;

		if ( ival != mod.NumPages )
		{
			mod.NumPages = ival;
			updatemesh = true;
		}

		float val = 0.0f;
		bool bval = false;

		mod.showmeshoptions = EditorGUILayout.Foldout(mod.showmeshoptions, "Mesh Options");

		if ( mod.showmeshoptions )
		{
			EditorGUILayout.BeginVertical("box");
			val = EditorGUILayout.FloatField("Page Length", mod.pageLength);
			if ( val < 0.0f )
				val = 0.0f;

			if ( val != mod.pageLength )
			{
				mod.pageLength = val;
				updatemesh = true;
			}

			val = EditorGUILayout.FloatField("Page Width", mod.pageWidth);
			if ( val < 0.0f )
				val = 0.0f;
			if ( val != mod.pageWidth )
			{
				mod.pageWidth = val;
				updatemesh = true;
			}

			val = EditorGUILayout.FloatField("Page Height", mod.pageHeight);
			if ( val < 0.0f )
				val = 0.0f;
			if ( val != mod.pageHeight )
			{
				mod.pageHeight = val;
				updatemesh = true;
			}

			ival = EditorGUILayout.IntField("Width Segs", mod.WidthSegs);
			if ( ival != mod.WidthSegs )
			{
				mod.WidthSegs = ival;
				updatemesh = true;
			}

			ival = EditorGUILayout.IntField("Length Segs", mod.LengthSegs);
			if ( ival != mod.LengthSegs )
			{
				mod.LengthSegs = ival;
				updatemesh = true;
			}

			ival = EditorGUILayout.IntField("Height Segs", mod.HeightSegs);
			if ( ival != mod.HeightSegs )
			{
				mod.HeightSegs = ival;
				updatemesh = true;
			}

			bval = EditorGUILayout.Toggle("Spine Edge", mod.spineedge);
			if ( bval != mod.spineedge )
			{
				mod.spineedge = bval;
				updatemesh = true;
			}

			bval = EditorGUILayout.Toggle("Page Hole Mesh", mod.useholepage);
			if ( bval != mod.useholepage )
			{
				mod.useholepage = bval;
				updatemesh = true;
			}

			ival = EditorGUILayout.IntField("X Hole", mod.xhole);
			if ( ival != mod.xhole )
			{
				mod.xhole = ival;
				updatemesh = true;
			}

			ival = EditorGUILayout.IntField("Y Hole", mod.yhole);
			if ( ival != mod.yhole )
			{
				mod.yhole = ival;
				updatemesh = true;
			}

			mod.pagesizevariation = EditorGUILayout.Vector3Field("Variation", mod.pagesizevariation);
			if ( GUI.changed )
				updatemesh = true;

			mod.pagesectioncrv = EditorGUILayout.CurveField("Vertex Dist", mod.pagesectioncrv);
			if ( GUI.changed )
				updatemesh = true;

			mod.pagemesh = (Mesh)EditorGUILayout.ObjectField("Page Mesh", mod.pagemesh, typeof(Mesh), true);
			if ( GUI.changed )
				updatemesh = true;

			mod.holemesh = (Mesh)EditorGUILayout.ObjectField("Hole Mesh", mod.holemesh, typeof(Mesh), true);
			if ( GUI.changed )
				updatemesh = true;

			mod.pageobject = (GameObject)EditorGUILayout.ObjectField("Page Object", mod.pageobject, typeof(GameObject), false);
			if ( GUI.changed )
				updatemesh = true;

			mod.holeobject = (GameObject)EditorGUILayout.ObjectField("Hole Object", mod.holeobject, typeof(GameObject), false);
			if ( GUI.changed )
				updatemesh = true;

			mod.frontmat = EditorGUILayout.IntField("Front Mat ID", mod.frontmat);
			if ( GUI.changed )
				updatemesh = true;

			mod.backmat = EditorGUILayout.IntField("Back Mat ID", mod.backmat);
			if ( GUI.changed )
				updatemesh = true;

			mod.rotate = EditorGUILayout.Vector3Field("Rotate", mod.rotate);
			if ( GUI.changed )
				updatemesh = true;

			mod.basematerial = (Material)EditorGUILayout.ObjectField("Front Material", mod.basematerial, typeof(Material), true);
			if ( GUI.changed )
				updatemesh = true;

			mod.basematerial1 = (Material)EditorGUILayout.ObjectField("Back Material", mod.basematerial1, typeof(Material), true);
			if ( GUI.changed )
				updatemesh = true;

			mod.basematerial2 = (Material)EditorGUILayout.ObjectField("Edge Material", mod.basematerial2, typeof(Material), true);
			if ( GUI.changed )
				updatemesh = true;

			val = EditorGUILayout.FloatField("Page Gap", mod.pageGap);
			if ( val != mod.pageGap )
			{
				mod.pageGap = val;
				update = true;
			}

			val = EditorGUILayout.Slider("Spine Radius", mod.spineradius, -1.0f, 1.0f);
			if ( val != mod.spineradius )
			{
				mod.spineradius = val;
				update = true;
			}

			bval = EditorGUILayout.Toggle("Use Book Thickness", mod.usebookthickness);
			if ( bval != mod.usebookthickness )
			{
				mod.usebookthickness = bval;
				update = true;
			}

			val = EditorGUILayout.FloatField("Book Thickness", mod.bookthickness);
			if ( val != mod.bookthickness )
			{
				mod.bookthickness = val;
				update = true;
			}

			bval = EditorGUILayout.Toggle("Page Colliders", mod.updatecollider);
			if ( bval != mod.updatecollider )
			{
				mod.updatecollider = bval;
				updatemesh = true;
			}

			bval = EditorGUILayout.BeginToggleGroup("Allow Color", mod.usecols);
			if ( bval != mod.usecols )
			{
				mod.usecols = bval;
				updatemesh = true;
			}

			Color col = EditorGUILayout.ColorField("Color", mod.color);
			if ( col != mod.color )
			{
				mod.color = col;
				updatemesh = true;
			}

			bval = EditorGUILayout.Toggle("Cast Shadows", mod.castshadows);
			if ( bval != mod.castshadows )
			{
				mod.castshadows = bval;
				updatemesh = true;
			}

			bval = EditorGUILayout.Toggle("Receive Shadows", mod.receiveshadows);
			if ( bval != mod.receiveshadows )
			{
				mod.receiveshadows = bval;
				updatemesh = true;
			}

			bval = EditorGUILayout.Toggle("Use Light Probes", mod.uselightprobes);
			if ( bval != mod.uselightprobes )
			{
				mod.uselightprobes = bval;
				updatemesh = true;
			}

			EditorGUILayout.EndToggleGroup();

			EditorGUILayout.EndVertical();
		}

		mod.showflipoptions = EditorGUILayout.Foldout(mod.showflipoptions, "Page Turn Options");

		if ( mod.showflipoptions )
		{
			EditorGUILayout.BeginVertical("box");
			mod.BottomAngle = EditorGUILayout.FloatField("Spine Angle", mod.BottomAngle);
			mod.BottomMaxAngle = EditorGUILayout.FloatField("Max Spine Angle", mod.BottomMaxAngle);
			mod.changespineangle = EditorGUILayout.Toggle("Change Spine Angle", mod.changespineangle);

			float center = EditorGUILayout.Slider("Turn Center", mod.Turn_CCenter, 0.0f, 1.0f);
			if ( center != mod.Turn_CCenter )
			{
				mod.Turn_CCenter = center;
				update = true;
			}

			val = EditorGUILayout.Slider("Turn Size", mod.Turn_CArea, 0.01f, 1.0f);
			if ( val != mod.Turn_CArea )
				mod.Turn_CAreaChange(val);

			val = EditorGUILayout.FloatField("Turn Max Angle", mod.Turn_maxAngle);
			if ( val != mod.Turn_maxAngle )
				mod.Turn_maxAngleChange(val);

			val = EditorGUILayout.FloatField("Turn Min Angle", mod.Turn_minAngle);
			if ( val != mod.Turn_minAngle )
				mod.Turn_minAngleChange(val);

			val = EditorGUILayout.FloatField("Turn Spread", mod.Turn_Spread);
			if ( val != mod.Turn_Spread )
				mod.Turn_SpreadChange(val);

			val = EditorGUILayout.Slider("Open Origin", mod.Land_CCenter, 0.0f, 1.0f);
			if ( val != mod.Land_CCenter )
				mod.Land_CCenterChange(val);

			val = EditorGUILayout.Slider("Open Size", mod.Land_CArea, 0.01f, 1.0f);
			if ( val != mod.Land_CArea )
				mod.Land_CAreaChange(val);

			val = EditorGUILayout.FloatField("Open Max Angle", mod.Land_maxAngle);
			if ( val != mod.Land_maxAngle )
				mod.Land_MaxAngleChange(val);

			val = EditorGUILayout.FloatField("Open Min Angle", mod.Land_minAngle);
			if ( val != mod.Land_minAngle )
				mod.Land_MinAngleChange(val);

			val = EditorGUILayout.Slider("Flip Origin", mod.Flex_CCenter, 0.0f, 1.0f);
			if ( val != mod.Flex_CCenter )
				mod.Flex_CCenterChange(val);

			val = EditorGUILayout.Slider("Flip Size", mod.Flex_CArea, 0.01f, 1.0f);
			if ( val != mod.Flex_CArea )
				mod.Flex_CAreaChange(val);

			val = EditorGUILayout.FloatField("Flip Max Angle", mod.Flex_MaxAngle);
			if ( val != mod.Flex_MaxAngle )
				mod.Flex_MaxAngleChange(val);

			bval = EditorGUILayout.Toggle("Flip Random", mod.Flex_Random);
			if ( bval != mod.Flex_Random )
				mod.Flex_RandomChange(mod.Flex_RandomDegree, mod.Flex_RandomSeed, bval);

			ival = EditorGUILayout.IntField("Flip Random Seed", mod.Flex_RandomSeed);
			if ( ival != mod.Flex_RandomSeed )
				mod.Flex_RandomChange(mod.Flex_RandomDegree, ival, bval);

			val = EditorGUILayout.FloatField("Flip Random Ang", mod.Flex_RandomDegree);
			if ( val != mod.Flex_RandomDegree )
				mod.Flex_RandomChange(val, mod.Flex_RandomSeed, mod.Flex_Random);

			EditorGUILayout.EndVertical();
		}

		mod.showmaterials = EditorGUILayout.Foldout(mod.showmaterials, "Pages");

		if ( mod.showmaterials && mod.pageparams.Count > 0 )
		{
			if ( mod.pageparams.Count > 1 )
			{
				mod.editpage = EditorGUILayout.IntSlider("Edit Page", mod.editpage, 0, mod.pageparams.Count - 1);
				//mod.editpage = EditorGUILayout.IntSlider("Edit Page", mod.editpage, 0, mod.NumPages - 1);	//mod.MinPageVal(), mod.MaxPageVal());
			}

			if ( GUILayout.Button("Add Page") )
				mod.pageparams.Add(NewPage(mod.pageparams.Count - 1));

			EditorGUILayout.BeginVertical("box");
			if ( DisplayPage(mod.pageparams[mod.editpage], mod.editpage) )
				updatemesh = true;

			EditorGUILayout.EndVertical();

			if ( GUILayout.Button("Add Page") )
				mod.pageparams.Add(NewPage(mod.pageparams.Count - 1));
		}
		else
			if ( GUILayout.Button("Add Page") )
				mod.pageparams.Add(NewPage(mod.pageparams.Count - 1));

		mod.showbackgrounds = EditorGUILayout.Foldout(mod.showbackgrounds, "Texture Backgrounds");

		if ( mod.showbackgrounds )
		{
			EditorGUILayout.BeginVertical("box");
			bool noback = EditorGUILayout.Toggle("No Backgrounds", mod.nobackgrounds);
			if ( noback != mod.nobackgrounds )
			{
				mod.nobackgrounds = noback;
				update = true;
			}

			Texture2D tex = (Texture2D)EditorGUILayout.ObjectField("Background", mod.background, typeof(Texture2D), false);

			if ( tex != mod.background && CheckTextureReadable(tex) )
			{
				if ( tex == null )
				{
					mod.ClearMadeTextures();
					update = true;
				}
				mod.background = tex;
			}

			tex = (Texture2D)EditorGUILayout.ObjectField("Background Back", mod.background1, typeof(Texture2D), false);

			if ( tex != mod.background1 && CheckTextureReadable(tex) )
			{
				if ( tex == null )
				{
					mod.ClearMadeTextures();
					update = true;
				}
				mod.background1 = tex;
			}

			tex = (Texture2D)EditorGUILayout.ObjectField("Mask", mod.mask, typeof(Texture2D), false);

			if ( tex != mod.mask && CheckTextureReadable(tex) )
			{
				if ( tex == null )
				{
					mod.ClearMadeTextures();
					update = true;
				}
				mod.mask = tex;
			}

			Rect copyarea = EditorGUILayout.RectField("Copy Area", mod.copyarea);
			if ( copyarea != mod.copyarea )
			{
				mod.copyarea = copyarea;
				mod.ClearMadeTextures();
				update = true;
			}

			copyarea = EditorGUILayout.RectField("Copy Area Back", mod.copyarea1);
			if ( copyarea != mod.copyarea1 )
			{
				mod.copyarea1 = copyarea;
				mod.ClearMadeTextures();
				update = true;
			}

			EditorGUILayout.BeginHorizontal();

			if ( GUILayout.Button("Clear") )
			{
				mod.ClearMadeTextures();
				updatemesh = true;
			}

			if ( GUILayout.Button("Make Pages") )
			{
				mod.BuildPageTextures();
				updatemesh = true;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndVertical();
		}

		mod.showcover = EditorGUILayout.Foldout(mod.showcover, "Cover Params");

		if ( mod.showcover )
		{
			EditorGUILayout.BeginVertical("box");
			mod.frontcover = (Transform)EditorGUILayout.ObjectField("Front Cover", mod.frontcover, typeof(Transform), true);
			mod.frontang = EditorGUILayout.FloatField("Front Ang", mod.frontang);
			mod.frontpivot = EditorGUILayout.Vector3Field("Front Pivot", mod.frontpivot);
			mod.backcover = (Transform)EditorGUILayout.ObjectField("Back Cover", mod.backcover, typeof(Transform), true);
			mod.backang = EditorGUILayout.FloatField("Back Ang", mod.backang);
			mod.backpivot = EditorGUILayout.Vector3Field("Back Pivot", mod.backpivot);
			EditorGUILayout.EndVertical();
		}

		mod.showdynammesh = EditorGUILayout.Foldout(mod.showdynammesh, "Dynam Mesh");

		if ( mod.showdynammesh )
		{
			EditorGUILayout.BeginVertical("box");
			bool texton = EditorGUILayout.BeginToggleGroup("Enabled", mod.dynammeshenabled);
			if ( texton != mod.dynammeshenabled )
			{
				mod.dynammeshenabled = texton;
				updatemesh = true;
			}
			MegaBookDynamicMesh textobj = (MegaBookDynamicMesh)EditorGUILayout.ObjectField("Text Object", mod.dynamobj, typeof(MegaBookDynamicMesh), true);
			if ( textobj != mod.dynamobj )
			{
				mod.dynamobj = textobj;
				updatemesh = true;
			}

			EditorGUILayout.BeginVertical("box");
			Vector3 textoffset = EditorGUILayout.Vector3Field("Front Offset", mod.dynamoffset);
			if ( textoffset != mod.dynamoffset )
			{
				mod.dynamoffset = textoffset;
				updatemesh = true;
			}

			Vector3 textrot = EditorGUILayout.Vector3Field("Front Rotate", mod.dynamrot);
			if ( textrot != mod.dynamrot )
			{
				mod.dynamrot = textrot;
				updatemesh = true;
			}

			Vector3 textscale = EditorGUILayout.Vector3Field("Front Scale", mod.dynamscale);
			if ( textscale != mod.dynamscale )
			{
				mod.dynamscale = textscale;
				updatemesh = true;
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical("box");
			textoffset = EditorGUILayout.Vector3Field("Back Offset", mod.backdynamoffset);
			if ( textoffset != mod.backdynamoffset )
			{
				mod.backdynamoffset = textoffset;
				updatemesh = true;
			}

			textrot = EditorGUILayout.Vector3Field("Back Rotate", mod.backdynamrot);
			if ( textrot != mod.backdynamrot )
			{
				mod.backdynamrot = textrot;
				updatemesh = true;
			}

			textscale = EditorGUILayout.Vector3Field("Back Scale", mod.backdynamscale);
			if ( textscale != mod.backdynamscale )
			{
				mod.backdynamscale = textscale;
				updatemesh = true;
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.EndToggleGroup();
			EditorGUILayout.EndVertical();
		}

		if ( GUILayout.Button("Rebuild") )
		{
			updatemesh = true;
			mod.rebuild = true;
			EditorUtility.SetDirty(target);
		}

		if ( updatemesh )
		{
			mod.rebuildmeshes = true;
			//mod.UpdateSettings();
			//mod.Detach();
			//mod.BuildPageMeshes();
			//mod.Attach();
			//mod.UpdateSettings();
		}

		// debug
		for ( int i = 0; i < mod.pages.Count; i++ )
		{
			mod.pages[i].turnerfromcon = EditorGUILayout.CurveField("Turn", mod.pages[i].turnerfromcon);
		}

		//for ( int i = 0; i < mod.pages.Count; i++ )
		//{
			//mod.pages[i].turnerangcon = EditorGUILayout.CurveField("Ang", mod.pages[i].turnerangcon);
		//}

		if ( update )
		{
			mod.UpdateSettings();
		}

		if ( GUI.changed )
		{
			//serializedObject.ApplyModifiedProperties();

			if ( mod.pages.Count > 0 )
				mod.pages[0].deform = true;
			EditorUtility.SetDirty(target);
		}

		undoManager.CheckDirty();
	}

	MegaBookPageParams NewPage(int prev)
	{
		MegaBookBuilder book = (MegaBookBuilder)target;

		MegaBookPageParams page = new MegaBookPageParams();

		if ( prev >= 0 && prev < book.pageparams.Count )
		{
			MegaBookPageParams pp = book.pageparams[prev];

			page.back = pp.back;
			page.front = pp.front;
			page.background = pp.background;
			page.background1 = pp.background1;
			page.backmat = pp.backmat;
			page.frontmat = pp.frontmat;
			page.edgemat = pp.edgemat;

			page.copyarea = pp.copyarea;
			page.copyarea1 = pp.copyarea1;

			page.usebackground = pp.usebackground;
			page.usebackground1 = pp.usebackground1;

			page.pagemesh = pp.pagemesh;
			page.pageobj = pp.pageobj;

			page.rotate = pp.rotate;
		}

		return page;
	}

	bool DisplayPage(MegaBookPageParams page, int i)
	{
		MegaBookBuilder mod = (MegaBookBuilder)target;
		bool retval = false;

		Mesh mesh = (Mesh)EditorGUILayout.ObjectField("Page Mesh", page.pagemesh, typeof(Mesh), false);
		if ( mesh != page.pagemesh )
		{
			page.pagemesh = mesh;
			retval = true;
		}

		mesh = (Mesh)EditorGUILayout.ObjectField("Hole Mesh", page.holemesh, typeof(Mesh), false);
		if ( mesh != page.holemesh )
		{
			page.holemesh = mesh;
			retval = true;
		}

		GameObject obj = (GameObject)EditorGUILayout.ObjectField("Page Object", page.pageobj, typeof(GameObject), false);
		if ( obj != page.pageobj )
		{
			page.pageobj = obj;
			retval = true;
		}

		obj = (GameObject)EditorGUILayout.ObjectField("Hole Object", page.holeobj, typeof(GameObject), false);
		if ( obj != page.holeobj )
		{
			page.holeobj = obj;
			retval = true;
		}

		int mi = EditorGUILayout.IntField("Front Mat Index", page.frontmatindex);
		if ( mi != page.frontmatindex )
		{
			page.frontmatindex = mi;
			retval = true;
		}

		mi = EditorGUILayout.IntField("Back Mat Index", page.backmatindex);
		if ( mi != page.backmatindex )
		{
			page.backmatindex = mi;
			retval = true;
		}

		Vector3 rotate = EditorGUILayout.Vector3Field("Rotate", page.rotate);
		if ( rotate != page.rotate )
		{
			page.rotate = rotate;
			retval = true;
		}

		Material mat = (Material)EditorGUILayout.ObjectField("Front Material", page.frontmat, typeof(Material), false);
		if ( mat != page.frontmat )
		{
			page.frontmat = mat;
			retval = true;
		}

		mat = (Material)EditorGUILayout.ObjectField("Back Material", page.backmat, typeof(Material), false);
		if ( mat != page.backmat )
		{
			page.backmat = mat;
			retval = true;
		}

		EditorGUILayout.BeginHorizontal("box");
		EditorGUILayout.LabelField("Front " + i, GUILayout.Width(80));
		Texture2D ptex = (Texture2D)EditorGUILayout.ObjectField("", page.front, typeof(Texture2D), false, GUILayout.Width(64));
		if ( ptex != page.front )
		{
			page.front = ptex;
			retval = true;
		}

		ptex = (Texture2D)EditorGUILayout.ObjectField("", page.background, typeof(Texture2D), false, GUILayout.Width(64));

		if ( ptex != page.background && CheckTextureReadable(ptex) )
		{
			if ( ptex == null )
				mod.ClearMadeTextures();
			page.background = ptex;
		}

		EditorGUILayout.EndHorizontal();
		bool alpha = EditorGUILayout.Toggle("Use Alpha as Mask", page.alphatexturefront);
		if ( alpha != page.alphatexturefront )
		{
			page.alphatexturefront = alpha;
			retval = true;
		}

		EditorGUILayout.BeginHorizontal("box");
		EditorGUILayout.LabelField("Back", GUILayout.Width(80));

		ptex = (Texture2D)EditorGUILayout.ObjectField("", page.back, typeof(Texture2D), false, GUILayout.Width(64));
		if ( ptex != page.back )
		{
			page.back = ptex;
			retval = true;
		}

		ptex = (Texture2D)EditorGUILayout.ObjectField("", page.background1, typeof(Texture2D), false, GUILayout.Width(64));

		if ( ptex != page.background1 && CheckTextureReadable(ptex) )
		{
			if ( ptex == null )
				mod.ClearMadeTextures();
			page.background1 = ptex;
		}

		EditorGUILayout.EndHorizontal();

		alpha = EditorGUILayout.Toggle("Use Alpha as Mask", page.alphatextureback);
		if ( alpha != page.alphatextureback )
		{
			page.alphatextureback = alpha;
			retval = true;
		}

		page.swapsides = EditorGUILayout.Toggle("Swap Sides", page.swapsides);

		// Should just update the page not the whole book
		page.usebackground = EditorGUILayout.Toggle("Front Background", page.usebackground);
		page.copyarea = EditorGUILayout.RectField("Copy Area", page.copyarea);

		page.usebackground1 = EditorGUILayout.Toggle("Back Background", page.usebackground1);
		page.copyarea1 = EditorGUILayout.RectField("Copy Area Back", page.copyarea1);

		//bool bval = EditorGUILayout.Toggle("Stiff", page.stiff);
		//if ( bval != page.stiff )
		//{
			//page.stiff = bval;
			//retval = true;
		//}

		// Attached objects
		mod.showobjects = EditorGUILayout.Foldout(mod.showobjects, "Page Objects");

		if ( mod.showobjects )
		{
			page.visobjlow = EditorGUILayout.FloatField("Visible Low", page.visobjlow);
			page.visobjhigh = EditorGUILayout.FloatField("Visible High", page.visobjhigh);

			if ( GUILayout.Button("Add Object") )
			{
				MegaBookPageObject pobj = new MegaBookPageObject();
				if ( page.objects.Count > 0 )
				{
					MegaBookPageObject prevobj = page.objects[page.objects.Count - 1];
					pobj.pos = prevobj.pos;
					pobj.rot = prevobj.rot;
					pobj.offset = prevobj.offset;
				}
				page.objects.Add(pobj);
				retval = true;
				mod.rebuild = true;
				GUI.changed = false;
				EditorUtility.SetDirty(target);
			}

			for ( int j = 0; j < page.objects.Count; j++ )
			{
				MegaBookPageObject pobj = page.objects[j];
				GameObject newobj = (GameObject)EditorGUILayout.ObjectField("Object", pobj.obj, typeof(GameObject), true);
				
				if ( newobj != pobj.obj )
				{
					if ( pobj.obj == null )
						pobj.pos = mod.CalcPos(newobj, page, pobj.pos);

					pobj.obj = newobj;
					retval = true;
					GUI.changed = false;
				}
				pobj.pos = EditorGUILayout.Vector3Field("Pos", pobj.pos);
				pobj.rot = EditorGUILayout.Vector3Field("Rot", pobj.rot);
				pobj.offset = EditorGUILayout.FloatField("Offset", pobj.offset);

				pobj.attachforward = EditorGUILayout.Vector3Field("Fwd", pobj.attachforward);

				pobj.overridevisi = EditorGUILayout.BeginToggleGroup("Use Obj Visi", pobj.overridevisi);

				pobj.visilow = EditorGUILayout.FloatField("Visibility Low", pobj.visilow);
				pobj.visihigh = EditorGUILayout.FloatField("Visibility High", pobj.visihigh);

				EditorGUILayout.EndToggleGroup();

				pobj.message = EditorGUILayout.Toggle("Message", pobj.message);

				if ( GUILayout.Button("Delete Page Object") )
				{
					page.objects.RemoveAt(j);
					retval = true;
					mod.rebuild = true;
					GUI.changed = false;
					EditorUtility.SetDirty(target);
				}
			}

			if ( GUI.changed )
			{
				mod.UpdateAttached();

				EditorUtility.SetDirty(target);
			}
		}

		EditorGUILayout.BeginHorizontal();
		if ( GUILayout.Button("Insert") )
		{
			mod.pageparams.Insert(i, NewPage(i));
			retval = true;
		}

		if ( GUILayout.Button("Delete") )
		{
			mod.pageparams.RemoveAt(i);
			retval = true;
		}

		if ( GUILayout.Button("Up") )
		{
			if ( i > 0 )
				SwapPages(mod, i, i - 1);
			retval = true;
		}

		if ( GUILayout.Button("Down") )
		{
			if ( i < mod.pagetextures.Count - 1 )
				SwapPages(mod, i, i + 1);
			retval = true;
		}

		EditorGUILayout.EndHorizontal();

		return retval;
	}

	void OnSceneGUI()
	{
		MegaBookBuilder mod = (MegaBookBuilder)target;
		MegaBookPage pg = mod.pages[mod.editpage];

		switch ( Tools.current )
		{
			case Tool.Move:
				for ( int i = 0; i < pg.objects.Count; i++ )
				{
#if UNITY_3_5
					if ( pg.objects[i].obj && pg.objects[i].obj.active )
#else
					if ( pg.objects[i].obj && pg.objects[i].obj.activeInHierarchy )
#endif
					{
						Transform trans = pg.objects[i].obj.transform;
						Vector3 pos = Handles.PositionHandle(trans.position, trans.rotation);	//Quaternion.identity);
						if ( pos != trans.position )
						{
							Vector3 delta = trans.position - pos;

							//float off = delta.y;
							delta.y = 0.0f;
							MegaBookPageParams parms = mod.pageparams[mod.editpage];	//.objects[i]];

							MegaBookPageObject pobj = parms.objects[i];

							pobj.pos -= delta;

							pobj.pos.x = Mathf.Clamp(pobj.pos.x, 0.0f, 99.99f);
							pobj.pos.z = Mathf.Clamp(pobj.pos.z, 0.0f, 99.99f);

							//if ( Mathf.Abs(delta.x) < 0.01f && Mathf.Abs(delta.z) < 0.01f )
								//pobj.offset += off;

							mod.UpdateAttached();
							EditorUtility.SetDirty(target);
						}
					}
				}
				break;
		}
	}
}