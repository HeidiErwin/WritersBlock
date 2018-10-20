
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MBComplexPage))]
public class MBComplexPageEditor : Editor
{
	static public void SwapInf(MBComplexPage mod, int t1, int t2)
	{
		if ( t1 >= 0 && t1 < mod.pages.Count && t2 >= 0 && t2 < mod.pages.Count && t1 != t2 )
		{
			GameObject inf = mod.pages[t1];
			mod.pages.RemoveAt(t1);
			mod.pages.Insert(t2, inf);
		}
	}

	public override void OnInspectorGUI()
	{
		MBComplexPage cpage = (MBComplexPage)target;

		if ( GUILayout.Button("Add Page") )
		{
			cpage.pages.Add(null);
		}

		for ( int i = 0; i < cpage.pages.Count; i++ )
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Page" + i, GUILayout.MaxWidth(45));
			cpage.pages[i] = (GameObject)EditorGUILayout.ObjectField("", cpage.pages[i], typeof(GameObject), true, GUILayout.MaxWidth(128));
			//mod.layers[i].Enabled = EditorGUILayout.Toggle("", mod.layers[i].Enabled, GUILayout.MaxWidth(20));

			if ( GUILayout.Button("Delete", GUILayout.MaxWidth(50)) )
				cpage.pages.RemoveAt(i);

			if ( GUILayout.Button("U", GUILayout.MaxWidth(20)) )
			{
				if ( i > 0 )
					SwapInf(cpage, i, i - 1);
			}

			if ( GUILayout.Button("D", GUILayout.MaxWidth(22)) )
			{
				if ( i < cpage.pages.Count - 1 )
					SwapInf(cpage, i, i + 1);
			}

			EditorGUILayout.EndHorizontal();
		}

		cpage.fillbook = EditorGUILayout.Toggle("Fill Book", cpage.fillbook);
	}
}