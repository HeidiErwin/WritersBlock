
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

#if !UNITY_FLASH && !UNITY_PS3 && !UNITY_METRO && !UNITY_WP8
using System.Threading;
#endif

[ExecuteInEditMode]
public class MegaBookBuilder : MonoBehaviour
{
	public List<MegaBookPage>		pages				= new List<MegaBookPage>();
	public List<MegaBookPageParams>	pageparams			= new List<MegaBookPageParams>();
	public int				seed				= 0;
	public int				NumPages			= 10;
	public float			BottomAngle			= 0.0f;
	public int				Flex_RandomSeed		= 0;
	public float			Flex_CCenter		= 0.652f;
	public float			Flex_CArea			= 0.7f;
	public float			Flex_MaxAngle		= 190.0f;
	public float			Flex_MinAngle		= 0.0f;
	public bool				Flex_Random			= true;
	public float			Flex_RandomDegree	= 25.0f;
	public float			pageWidth			= 1.0f;
	public float			pageLength			= 0.7f;
	public float			pageHeight			= 0.0025f;
	public float			pageGap				= 0.0025f;
	public int				LengthSegs			= 8;
	public int				WidthSegs			= 30;
	public int				HeightSegs			= 1;
	public float			Turn_CCenter		= 0.0f;
	public float			Turn_CArea			= 0.05f;
	public float			Turn_maxAngle		= 180.0f;
	public float			Turn_minAngle		= 0.0f;
	public float			Turn_CLevel			= 0.0f;
	public float			Turn_Spread			= 5.0f;
	public float			Land_CCenter		= 0.25f;
	public float			Land_CArea			= 0.8f;
	public float			Land_maxAngle		= 0.0f;
	public float			Land_minAngle		= 0.0f;
	public Material			basematerial;
	public Material			basematerial1;
	public Material			basematerial2;
	public Mesh				pagemesh;
	public Mesh				holemesh;
	public GameObject		pageobject;
	public GameObject		holeobject;
	public int				frontmat;
	public int				backmat;
	public Vector3			rotate				= Vector3.zero;
	public bool				rebuildmeshes		= false;
	public bool				rebuild				= false;
	public float			Flip				= 0.0f;
	public bool				enableturner		= true;
	public bool				enablelander		= true;
	public bool				enableflexer		= true;
	public bool				runcontrollers		= true;
	public List<Texture2D>	pagetextures		= new List<Texture2D>();
	public bool				PivotBase			= false;	//true;
	public bool				PivotEdge			= true;
	public bool				animate				= false;
	public float			time				= 0.0f;
	public float			speed				= 1.0f;
	public AnimationCurve	pagesectioncrv		= new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public float			BottomMaxAngle		= 180.0f;
	public float			shuffle				= 14.0f;
	public float			spineradius			= 0.0f;
	//public GameObject		frontcover;
	//public GameObject		backcover;
	public GameObject		spine;
	public bool				usebookthickness	= false;
	public float			bookthickness		= 1.0f;	// Page gap will be calculated 
	public bool				showmaterials		= false;
	public int				shownummaterials	= 10;
	public bool				showmesh			= false;
	public Vector2			matpos				= Vector2.zero;
	public Texture2D		background;
	public Texture2D		background1;
	public Rect				copyarea			= new Rect(0.1f, 0.1f, 0.8f, 0.8f);
	public Rect				copyarea1			= new Rect(0.1f, 0.1f, 0.8f, 0.8f);
	public bool				spineedge			= true;
	public bool				UseThreading		= false;
	public bool				changespineangle	= true;
	public Vector3			pagesizevariation	= Vector3.zero;
	public Texture2D		mask;
    public bool             nobackgrounds		= false;
	public bool				useholepage			= true;
	public int				xhole				= 1;
	public int				yhole				= 1;
	public bool				showobjects			= false;
	public int				editpage			= 0;
	public bool				showmeshoptions		= false;
	public bool				showflipoptions		= false;
	public bool				showbackgrounds		= false;

    // Page Texture streaming, file or web
    public int				streamcount			= 0;
    public string			url					= "www.website.com/pages/";
    public string			filelist			= "pages.txt";
    public bool				usefilelist			= false;
    public bool				usefilearray		= false;
    public List<string>		files				= new List<string>();
    public string			prefix				= "";
    public string			extension			= ".jpg";

	// Control Options
	public float			page				= 0.0f;
	public float			turnspd				= 0.0f;
	public float			turntime			= 1.0f;
	public bool				Snap				= true;

	public bool				updatecollider		= false;

	public Transform		frontcover;
	public Vector3			frontpivot;
	public float			frontang;
	public Transform		backcover;
	public float			backang;
	public Vector3			backpivot;
	public bool				showcover = false;

	public bool					showdynammesh = false;
	public bool					dynammeshenabled = false;
	public MegaBookDynamicMesh	dynamobj;
	public Vector3				dynamoffset;
	public Vector3				dynamscale = Vector3.one;
	public Vector3				dynamrot;
	public Vector3				backdynamoffset;
	public Vector3				backdynamscale = Vector3.one;
	public Vector3				backdynamrot;

	public bool					usecols = false;
	public Color				color = Color.white;

	public bool					linkeditpage = true;

	public bool					castshadows = true;
	public bool					receiveshadows = true;
	public bool					uselightprobes = false;

	[ContextMenu("Help")]
	public void Help()
	{
		Application.OpenURL("http://www.west-racing.com/mf/?page_id=5422");
	}

	public int GetPageCount()
	{
		return pages.Count;
	}

	public int GetCurrentPage()
	{
		return (int)page;
	}

	public float GetCurrentTurn()
	{
		return page % 1.0f;
	}

	public void GetTransform(int page, Vector2 pos, out Vector3 p, out Quaternion r)
	{
		p = Vector3.zero;
		r = Quaternion.identity;
	}

	public IEnumerator DownloadTexture(string url, int p, bool front)
	{
		//Texture2D tex = GetPageTexture(p, front);
		WWW www = new WWW(url);
		yield return www;
		Texture2D t = new Texture2D(www.texture.width, www.texture.height);
		www.LoadImageIntoTexture(t);
		SetPageTexture(t, p, front);
	}

	public IEnumerator FetchTexture(string url,  Texture2D texture)
	{
		if ( usefilelist )
		{
			WWW www = new WWW(url);
			yield return www;
			www.LoadImageIntoTexture(texture);
		}
	}

	public Texture2D GetPageTexture(int p, bool front)
	{
		Material[] mats = pages[p].obj.GetComponent<MeshRenderer>().materials;

		if ( front )
		{
			return (Texture2D)mats[0].mainTexture;
		}
		else
			return (Texture2D)mats[1].mainTexture;
	}

	public void SetPageTexture(Texture2D tex, int p, bool front)
	{
		Material[] mats = pages[p].obj.GetComponent<MeshRenderer>().materials;

		if ( front )
			mats[0].mainTexture = tex;
		else
			mats[1].mainTexture = tex;
	}

	public float MinPageVal()
	{
		if ( frontcover )
			return -1.0f;

		return 0.0f;
	}

	public float MaxPageVal()
	{
		if ( backcover )
			return NumPages + 1;

		return NumPages;
	}

	public void NextPage()
	{
		page += 1.0f;
		if ( page > MaxPageVal() )
			page = MaxPageVal();
	}

	public void PrevPage()
	{
		page -= 1.0f;

		if ( page < MinPageVal() )
			page = MinPageVal();
	}

	public void SetPage(float val, bool force)
	{
		page = val;
		page = Mathf.Clamp(page, MinPageVal(), MaxPageVal());
		if ( force )
		{
			turnspd = 0.0f;
			Flip = page;
		}
	}

	public float GetPage()
	{
		return Flip;
	}

	public void SetSnap(bool snapvalue)
	{
		Snap = snapvalue;
	}

	public bool GetSnap()
	{
		return Snap;
	}

	public void SetTurnTime(float time)
	{
		turntime = time;
	}

	public float GetTurnTime()
	{
		return turntime;
	}

	public void ApplyScaling()
	{
		Vector3 scl = transform.lossyScale;

		pageWidth *= scl.x;
		pageLength *= scl.z;
		pageHeight *= scl.y;
		rebuildmeshes = true;
		UpdateSettings();
	}


	int pageindex = 0;

#if !UNITY_FLASH && !UNITY_PS3 && !UNITY_METRO && !UNITY_WP8
	public class MegaBookTaskInfo
	{
		public string			name;
		public volatile int		start;
		public volatile int		end;
		public AutoResetEvent	pauseevent;
		public Thread			_thread;
		public MegaBookBuilder	book;
		public int				index;
		public int				cores;
	}

	public int Cores = 4;
	static bool isRunning = false;
	MegaBookTaskInfo[] tasks;

	void MakeThreads()
	{
		if ( Cores > 0 )
		{
			isRunning = true;
			tasks = new MegaBookTaskInfo[Cores];

			for ( int i = 0; i < Cores; i++ )
			{
				tasks[i] = new MegaBookTaskInfo();

				tasks[i].book = this;
				tasks[i].name = "ThreadID " + i;
				tasks[i].pauseevent = new AutoResetEvent(false);
				tasks[i]._thread = new Thread(DoWork);
				tasks[i]._thread.Start(tasks[i]);
			}
		}
	}

	void DoWork(object info)
	{
		MegaBookTaskInfo inf = (MegaBookTaskInfo)info;

		while ( isRunning )
		{
			inf.pauseevent.WaitOne(Timeout.Infinite, false);

			if ( inf.end > 0 )
			{
				for ( int i = inf.start; i < inf.end; i++ )
					pages[i].DoWork(this, (Flip * 14.0f) - ((float)i * shuffle));
			}

			inf.end = 0;	// Done the job
		}
	}

	void WaitJobs()
	{
		if ( Cores > 0 )
		{
			int	count = 0;
			bool wait = false;
			do
			{
				wait = false;
				for ( int i = 0; i < tasks.Length; i++ )
				{
					if ( tasks[i].end > 0 )
					{
						wait = true;
						break;
					}
				}

				if ( wait )
				{
					count++;
					Thread.Sleep(0);
				}
			} while ( wait );
		}
	}

	void DoWork1(int start, int end)
	{
		for ( int i = start; i < end; i++ )
			pages[i].DoWork(this, (Flip * 14.0f) - ((float)i * shuffle));
	}

	public void UpdateBookMT()
	{
		if ( Cores == 0 )
			Cores = SystemInfo.processorCount - 1;

		if ( Cores < 1 || !Application.isPlaying )
			return;

		if ( tasks == null )
			MakeThreads();

		for ( int i = 0; i < pages.Count; i++ )
			pages[i].PreMT(this, (Flip * 14.0f) - ((float)i * shuffle));

		int step = 0;
		if ( Cores > 0 )
		{
			step = pages.Count / Cores;
			int index = step;
			for ( int i = 0; i < tasks.Length; i++ )
			{
				tasks[i].index = i + 1;
				tasks[i].cores = tasks.Length;
				tasks[i].start = index;
				tasks[i].end = index + step;
				tasks[i].book = this;
				index += step;
			}

			tasks[Cores - 1].end = pages.Count;

			for ( int i = 0; i < tasks.Length; i++ )
				tasks[i].pauseevent.Set();
		}

		// Do this thread work
		DoWork1(0, step);	// Bias the first job to reduce wait

		// Now need to sit and wait for jobs done, we should be doing work here
		WaitJobs();

		for ( int i = 0; i < pages.Count; i++ )
		{
			if ( pages[i].deform )
			{
				pages[i].deform = false;

				pages[i].mesh.vertices = pages[i].sverts;
				pages[i].mesh.RecalculateBounds();
				pages[i].mesh.RecalculateNormals();
			}
		}
	}

	void OnApplicationQuit()
	{
		if ( Application.isPlaying )
		{
			isRunning = false;

			if ( tasks != null )
			{
				for ( int i = 0; i < tasks.Length; i++ )
				{
					tasks[i].pauseevent.Set();

					while ( tasks[i]._thread.IsAlive )
					{
					}
				}
			}
			tasks = null;
			}
		}
#endif

	public void BuildPageTextures()
	{
		//madepages.Clear();

        if ( nobackgrounds == false )
        {
		    for ( int i = 0; i < pageparams.Count; i++ )
		    {
			    MegaBookPageParams page = pageparams[i];
			    if ( page.pageobj == null )
			    {
				    if ( page.usebackground && (page.background || background ) )	//&& page.usebackground )
				    {
					    Texture2D bg = background;
					    if ( page.background )
						    bg = page.background;

						Rect rect = copyarea;
						if ( page.copyarea.width != 0.0f && page.copyarea.height != 0.0f  )
							rect = page.copyarea;

					    page.madefront = MakePage(bg, page.front, rect, false, page.alphatexturefront);
				    }

				    if ( page.usebackground1 && (page.background1 || background1) )	//&& page.usebackground1 )
				    {
					    Texture2D bg = background1;
					    if ( page.background1 )
						    bg = page.background1;

						Rect rect = copyarea1;
						if ( page.copyarea1.width != 0.0f && page.copyarea1.height != 0.0f )
							rect = page.copyarea1;

					    page.madeback = MakePage(bg, page.back, rect, true, page.alphatextureback);
				    }
			    }
		    }
        }
	}

	Texture2D MakePage(Texture2D src, Texture2D image, Rect area, bool back, bool alpha)
	{
		if ( src )
		{
			Texture2D page = new Texture2D(src.width, src.height);

			page.SetPixels32(src.GetPixels32());

			Rect rect = area;

			if ( back )
			{
				float w = rect.width;
				rect.xMax = 1.0f - rect.xMin;
				rect.xMin = rect.xMax - w;
			}

			rect.xMin = rect.xMin * src.width;
			rect.xMax = rect.xMax * src.width;
			rect.yMin = rect.yMin * src.height;
			rect.yMax = rect.yMax * src.height;

			float width = 1.0f / (float)src.width;
			float height = 1.0f / (float)src.height;

			float h1 = 1.0f / rect.height;
			float w1 = 1.0f / rect.width;

			if ( mask )
			{
				if ( image )
				{
					if ( alpha )
					{
						for ( int y = (int)rect.y; y < rect.y + rect.height; y++ )
						{
							if ( y >= 0 && y < page.height )
							{
								float ya = (y - rect.y) * h1;	/// rect.height;
								float ya1 = (float)y * height;	/// (float)src.height;

								for ( int x = (int)rect.x; x < rect.x + rect.width; x++ )
								{
									if ( x >= 0 && x < page.width )
									{
										float xa = (x - rect.x) * w1;	/// rect.width;
										float xa1 = (float)x * width;	/// (float)src.width;
										Color col = image.GetPixelBilinear(xa, ya);
										if ( col.a != 0.0f )
										{
											Color col1 = page.GetPixelBilinear(xa1, ya1);
											Color mcol = mask.GetPixelBilinear(xa, ya);
											col = Color.Lerp(col1, col, mcol.r);
											page.SetPixel(x, y, col);	//image.GetPixelBilinear(xa, ya));
										}
									}
								}
							}
						}
					}
					else
					{
						for ( int y = (int)rect.y; y < rect.y + rect.height; y++ )
						{
							if ( y >= 0 && y < page.height )
							{
								float ya = (y - rect.y) * h1;	/// rect.height;
								float ya1 = (float)y * height;	/// (float)src.height;

								for ( int x = (int)rect.x; x < rect.x + rect.width; x++ )
								{
									if ( x >= 0 && x < page.width )
									{
										float xa = (x - rect.x) * w1;	/// rect.width;
										float xa1 = (float)x * width;	/// (float)src.width;
										Color col = image.GetPixelBilinear(xa, ya);
										Color col1 = page.GetPixelBilinear(xa1, ya1);
										Color mcol = mask.GetPixelBilinear(xa, ya);
										col = Color.Lerp(col1, col, mcol.r);
										page.SetPixel(x, y, col);	//image.GetPixelBilinear(xa, ya));
									}
								}
							}
						}
					}
				}
			}
			else
			{
				if ( image )
				{
					if ( alpha )
					{
						for ( int y = (int)rect.y; y < rect.y + rect.height; y++ )
						{
							if ( y >= 0 && y < page.height )
							{
								float ya = (y - rect.y) / rect.height;

								for ( int x = (int)rect.x; x < rect.x + rect.width; x++ )
								{
									if ( x >= 0 && x < page.width )
									{
										float xa = (x - rect.x) / rect.width;
										Color c = image.GetPixelBilinear(xa, ya);
										if ( c.a != 0.0f )
											page.SetPixel(x, y, c);	//image.GetPixelBilinear(xa, ya));
									}
								}
							}
						}
					}
					else
					{
						for ( int y = (int)rect.y; y < rect.y + rect.height; y++ )
						{
							if ( y >= 0 && y < page.height )
							{
								float ya = (y - rect.y) / rect.height;

								for ( int x = (int)rect.x; x < rect.x + rect.width; x++ )
								{
									if ( x >= 0 && x < page.width )
									{
										float xa = (x - rect.x) / rect.width;
										page.SetPixel(x, y, image.GetPixelBilinear(xa, ya));
									}
								}
							}
						}
					}
				}
			}

			page.Apply();	//true, true);

			return page;
		}
		else
			return image;
	}

	void Start()
	{
		if ( Application.isPlaying )
		{
			//if ( background && madepages.Count == 0 )
				BuildPageTextures();
		}
		Flip = page;
		rebuild = true;
	}
	
	void RemovePages()
	{
		List<Transform> children = new List<Transform>();

		for ( int i = 0; i < gameObject.transform.childCount; i++ )
		{
			if ( gameObject.transform.GetChild(i).name == "Page" )
				children.Add(gameObject.transform.GetChild(i));
		}

		for ( int i = 0; i < children.Count; i++ )
		{
			MeshFilter mf = children[i].gameObject.GetComponent<MeshFilter>();

			if ( mf )
			{
				//Mesh mesh = mf.sharedMesh;
				//mf.sharedMesh = null;

				//if ( mesh )
				{
					//if ( Application.isEditor )
						//DestroyImmediate(mesh);
					//else
						//Destroy(mesh);
				}
			}

			MeshRenderer mr = children[i].gameObject.GetComponent<MeshRenderer>();

			if ( mr )
			{
				Material[] mats = mr.sharedMaterials;

				//mr.sharedMaterials = null;

				for ( int m = 0; m < mats.Length; m++ )
				{
					//Material mat = mats[i];
					//mats[i] = null;
					//if ( Application.isEditor )
						//DestroyImmediate(mat);
					//else
						//Destroy(mat);
					
				}
			}

			if ( Application.isEditor )
				DestroyImmediate(children[i].gameObject);
			else
				Destroy(children[i].gameObject);
		}

		Resources.UnloadUnusedAssets();
		System.GC.Collect();
	}

	public void ClearMadeTextures()
	{
		for ( int i = 0; i < pageparams.Count; i++ )
		{
			pageparams[i].madefront = null;
			pageparams[i].madeback = null;
		}
	}

	GameObject MakePageObject(MegaBookPage page, int pnum)	//, Material mat)
	{
		GameObject cobj = new GameObject();
		cobj.name = "Page";
		cobj.layer = gameObject.layer;
		MeshFilter mf = cobj.AddComponent<MeshFilter>();
		MeshRenderer cmr = cobj.AddComponent<MeshRenderer>();

		cmr.castShadows = castshadows;
		cmr.receiveShadows = receiveshadows;
		cmr.useLightProbes = uselightprobes;

		page.pnum = pnum;
		page.mf = mf;

		if ( updatecollider )
		{
			page.collider = cobj.AddComponent<MeshCollider>();
		}

		float width = pageWidth;
		float length = pageLength;
		float height = pageHeight;

		Vector3 lscl = transform.lossyScale;
		//Debug.Log("scl " + lscl);
		width *= lscl.x;
		length *= lscl.z;
		height *= lscl.y;

		if ( pnum > 0 && pnum < NumPages - 1 )
		{
			width += Random.Range(-1.0f, 1.0f) * pagesizevariation.x;
			length += Random.Range(-1.0f, 1.0f) * pagesizevariation.y;
			height += Random.Range(-1.0f, 1.0f) * pagesizevariation.z;
		}

		MegaBookPageParams pp = pageparams[pageindex];

		page.stiff = pp.stiff;

		if ( pageobject || pp.pageobj )
		{
			GameObject obj = pageobject;
			Vector3 rot = rotate;
			
			if ( pp.pageobj )
			{
				obj = pp.pageobj;
				rot = pp.rotate;
			}

			Mesh mesh = CopyMeshObject(page, obj, width, length, height, rot);	//ate);
			mf.sharedMesh = mesh;
			
			if ( page.collider )
			{
				page.collider.sharedMesh = null;
				page.collider.sharedMesh = mesh;
			}

			MeshRenderer mr = obj.GetComponent<MeshRenderer>();

			Material[] mats = new Material[mr.sharedMaterials.Length];

			for ( int i = 0; i < mats.Length; i++ )
			{
				mats[i] = new Material(mr.sharedMaterials[i]);
			}

			int fm = frontmat;
			if ( pp.frontmatindex != -1 )
				fm = pp.frontmatindex;

			if ( fm >= 0 && fm < mats.Length )
			{
				if ( pp.madefront )
					mats[fm].mainTexture = pp.madefront;
				else
					mats[fm].mainTexture = pp.front;
			}

			int bm = backmat;
			if ( pp.backmatindex != -1 )
				bm = pp.backmatindex;

			if ( bm >= 0 && bm < mats.Length )
			{
				if ( pp.madeback )
					mats[bm].mainTexture = pp.madeback;
				else
					mats[bm].mainTexture = pp.back;
			}

			cmr.sharedMaterials = mats;	//mr.sharedMaterials;

			obj = holeobject;
			if ( pp.holeobj )
				obj = pp.holeobj;

			if ( holeobject )
				page.holemesh = CopyMeshObjectHole(page, holeobject, width, length, height, rot);
		}
		else
		{
			Vector3 rot = rotate;
			Mesh pm = pagemesh;
			if ( pp.pagemesh )
			{
				pm = pp.pagemesh;
				rot = pp.rotate;
			}

			if ( pm )
			{
				// make a copy of pagemesh
				Mesh mesh = CopyMesh(page, pm, width, length, height, rot);
				mf.sharedMesh = mesh;

				Material[] mats = new Material[3];

				int m1 = frontmat;
				int m2 = backmat;

				if ( pp.frontmatindex != -1 )
					m1 = pp.frontmatindex;

				if ( pp.backmatindex != -1 )
					m2 = pp.backmatindex;

				m1 = Mathf.Clamp(m1, 0, 2);
				m2 = Mathf.Clamp(m2, 0, 2);

				if ( pp.swapsides )
				{
					int m = m1;
					m1 = m2;
					m2 = m;
				}

				if ( pp.frontmat )
					mats[m1] = new Material(pp.frontmat);
				else
					mats[m1] = new Material(basematerial);

				if ( pp.backmat )
					mats[m2] = new Material(pp.backmat);
				else
					mats[m2] = new Material(basematerial1);

				if ( pp.edgemat )
					mats[2] = pp.edgemat;
				else
					mats[2] = basematerial2;

				if ( pp.madefront )
					mats[m1].mainTexture = pp.madefront;
				else
					mats[m1].mainTexture = pp.front;

				if ( pp.madeback )
					mats[m2].mainTexture = pp.madeback;
				else
					mats[m2].mainTexture = pp.back;

				cmr.sharedMaterials = mats;

				if ( page.collider )
				{
					page.collider.sharedMesh = null;
					page.collider.sharedMesh = mesh;
				}

				Mesh hmesh = holemesh;
				if ( pp.holemesh != null )
					hmesh = pp.holemesh;

				if ( hmesh )
					page.holemesh = CopyMeshHole(page, hmesh, width, length, height, rot);
			}
			else
			{
				// Make a procedural page mesh
				//Mesh mesh = CreatePageMesh(page, pageWidth, pageLength, pageHeight, WidthSegs, LengthSegs, HeightSegs);
				Mesh mesh = CreatePageMesh(page, width, length, height, WidthSegs, LengthSegs, HeightSegs);

				if ( useholepage )
					page.holemesh = CreatePageMeshHole(page, width, length, height, WidthSegs, LengthSegs, HeightSegs, xhole, yhole);

				mf.sharedMesh = mesh;	//new Mesh();

				Material[] mats;

				if ( dynamobj && dynammeshenabled )
				{
					int nmb = dynamobj.GetNumMaterials(page.pnum, false);

					int nmf = dynamobj.GetNumMaterials(page.pnum, true);
					int tnum = nmf + nmb;	//dynamobj.GetNumMaterials(page.pnum, false);

					mats = new Material[3 + tnum];

					for ( int i = 0; i < nmf; i++ )
					{
						//mats[3 + i] = new Material(dynamobj.GetMaterial(page.pnum, true, i));
						mats[3 + i] = dynamobj.GetMaterial(page.pnum, true, i);
					}

					nmb = dynamobj.GetNumMaterials(page.pnum, false);
					for ( int i = 0; i < nmb; i++ )
					{
						//mats[3 + nmf + i] = new Material(dynamobj.GetMaterial(page.pnum, false, i));
						mats[3 + nmf + i] = dynamobj.GetMaterial(page.pnum, false, i);
					}
				}
				else
					mats = new Material[3];

				if ( pp.frontmat )
					mats[0] = new Material(pp.frontmat);
				else
				{
					if ( basematerial )
						mats[0] = new Material(basematerial);
				}

				if ( pp.backmat )
					mats[1] = new Material(pp.backmat);
				else
				{
					if ( basematerial1 )
						mats[1] = new Material(basematerial1);
				}

				if ( pp.edgemat )
					mats[2] = pp.edgemat;
				else
					mats[2] = basematerial2;

				if ( pp.madefront )
					mats[0].mainTexture = pp.madefront;
				else
					mats[0].mainTexture = pp.front;

				if ( pp.madeback )
					mats[1].mainTexture = pp.madeback;
				else
					mats[1].mainTexture = pp.back;

				cmr.sharedMaterials = mats;

				if ( page.collider )
				{
					page.collider.sharedMesh = null;
					page.collider.sharedMesh = mesh;
				}
			}
		}

		page.mesh.name = "PageMesh " + pnum;
		// Attach objects
		page.visobjlow = pp.visobjlow;
		page.visobjhigh = pp.visobjhigh;

		for ( int i = 0; i < pp.objects.Count; i++ )
		{
			AttachObject(page, pp.objects[i]);
		}

		pageindex++;
		pageindex = pageindex % pageparams.Count;
		cobj.transform.parent = transform;

		return cobj;
	}

	static void MakeQuad1(List<int> f, int a, int b, int c, int d)
	{
		f.Add(a);
		f.Add(b);
		f.Add(c);

		f.Add(c);
		f.Add(d);
		f.Add(a);
	}

	int MaxComponent(Vector3 v)
	{
		if ( Mathf.Abs(v.x) > Mathf.Abs(v.y) )
		{
			if ( Mathf.Abs(v.x) > Mathf.Abs(v.z) )
				return 0;
			else
				return 2;
		}
		else
		{
			if ( Mathf.Abs(v.y) > Mathf.Abs(v.z) )
				return 1;
			else
				return 2;
		}
	}

	Mesh CreatePageMesh(MegaBookPage page, float width, float length, float height, int widthsegs, int lengthsegs, int heightsegs)
	{
		Mesh mesh = new Mesh();

		Vector3 vb = new Vector3(width, height, length) / 2.0f;
		Vector3 va = -vb;

		if ( PivotBase )
		{
			va.y = 0.0f;
			vb.y = height;
		}

		if ( PivotEdge )
		{
			va.x = 0.0f;
			vb.x = width;
		}

		float dx = width / (float)widthsegs;
		float dy = height / (float)heightsegs;
		float dz = length / (float)lengthsegs;

		Vector3 p = va;

		// Lists should be static, clear out to reuse
		List<Vector3>	verts = new List<Vector3>();
		List<Vector2>	uvs = new List<Vector2>();
		List<int>		tris = new List<int>();
		List<int>		tris1 = new List<int>();
		List<int>		tris2 = new List<int>();

		Vector2 uv = Vector2.zero;

		// Do we have top and bottom
		if ( width > 0.0f && length > 0.0f )
		{
			p.y = vb.y;
			for ( int iz = 0; iz <= lengthsegs; iz++ )
			{
				p.x = va.x;
				for ( int ix = 0; ix <= widthsegs; ix++ )
				{
					float alpha = (float)ix / (float)widthsegs;
					p.x = va.x + (pagesectioncrv.Evaluate(alpha) * width);

					verts.Add(p);

					uv.x = p.x / width;
					uv.y = (p.z + vb.z) / length;

					uvs.Add(uv);
					p.x += dx;
				}
				p.z += dz;
			}

			for ( int iz = 0; iz < lengthsegs; iz++ )
			{
				int kv = iz * (widthsegs + 1);
				for ( int ix = 0; ix < widthsegs; ix++ )
				{
					MakeQuad1(tris, kv, kv + widthsegs + 1, kv + widthsegs + 2, kv + 1);
					kv++;
				}
			}

			int index = verts.Count;

			p.y = va.y;
			p.z = va.z;

			for ( int iy = 0; iy <= lengthsegs; iy++ )
			{
				p.x = va.x;
				for ( int ix = 0; ix <= widthsegs; ix++ )
				{
					float alpha = (float)ix / (float)widthsegs;
					p.x = va.x + (pagesectioncrv.Evaluate(alpha) * width);

					verts.Add(p);
					uv.x = 1.0f - (p.x / width);
					uv.y = ((p.z + vb.z) / length);

					uvs.Add(uv);
					p.x += dx;
				}
				p.z += dz;
			}

			for ( int iy = 0; iy < lengthsegs; iy++ )
			{
				int kv = iy * (widthsegs + 1) + index;
				for ( int ix = 0; ix < widthsegs; ix++ )
				{
					MakeQuad1(tris1, kv, kv + 1, kv + widthsegs + 2, kv + widthsegs + 1);
					kv++;
				}
			}
		}

		// Front back
		if ( width > 0.0f && height > 0.0f )
		{
			int index = verts.Count;

			p.z = va.z;
			p.y = va.y;
			for ( int iz = 0; iz <= heightsegs; iz++ )
			{
				p.x = va.x;
				for ( int ix = 0; ix <= widthsegs; ix++ )
				{
					float alpha = (float)ix / (float)widthsegs;
					p.x = va.x + (pagesectioncrv.Evaluate(alpha) * width);

					verts.Add(p);
					uv.x = (p.x + vb.x) / width;
					uv.y = (p.y + vb.y) / height;
					uvs.Add(uv);
					p.x += dx;
				}
				p.y += dy;
			}

			for ( int iz = 0; iz < heightsegs; iz++ )
			{
				int kv = iz * (widthsegs + 1) + index;
				for ( int ix = 0; ix < widthsegs; ix++ )
				{
					MakeQuad1(tris2, kv, kv + widthsegs + 1, kv + widthsegs + 2, kv + 1);
					kv++;
				}
			}

			index = verts.Count;

			p.z = vb.z;
			p.y = va.y;
			for ( int iy = 0; iy <= heightsegs; iy++ )
			{
				p.x = va.x;
				for ( int ix = 0; ix <= widthsegs; ix++ )
				{
					float alpha = (float)ix / (float)widthsegs;
					p.x = va.x + (pagesectioncrv.Evaluate(alpha) * width);

					verts.Add(p);
					uv.x = (p.x + vb.x) / width;
					uv.y = (p.y + vb.y) / height;
					uvs.Add(uv);
					p.x += dx;
				}
				p.y += dy;
			}

			for ( int iy = 0; iy < heightsegs; iy++ )
			{
				int kv = iy * (widthsegs + 1) + index;
				for ( int ix = 0; ix < widthsegs; ix++ )
				{
					MakeQuad1(tris2, kv, kv + 1, kv + widthsegs + 2, kv + widthsegs + 1);
					kv++;
				}
			}
		}

		// Left Right
		if ( length > 0.0f && height > 0.0f )
		{
			int index = verts.Count;

			p.x = vb.x;
			p.y = va.y;
			for ( int iz = 0; iz <= heightsegs; iz++ )
			{
				p.z = va.z;
				for ( int ix = 0; ix <= lengthsegs; ix++ )
				{
					verts.Add(p);
					uv.x = (p.z + vb.z) / length;
					uv.y = (p.y + vb.y) / height;
					uvs.Add(uv);
					p.z += dz;
				}
				p.y += dy;
			}

			for ( int iz = 0; iz < heightsegs; iz++ )
			{
				int kv = iz * (lengthsegs + 1) + index;
				for ( int ix = 0; ix < lengthsegs; ix++ )
				{
					MakeQuad1(tris2, kv, kv + lengthsegs + 1, kv + lengthsegs + 2, kv + 1);
					kv++;
				}
			}

			if ( spineedge )
			{
				index = verts.Count;

				p.x = va.x;
				p.y = va.y;
				for ( int iy = 0; iy <= heightsegs; iy++ )
				{
					p.z = va.z;
					for ( int ix = 0; ix <= lengthsegs; ix++ )
					{
						verts.Add(p);
						uv.x = (p.z + vb.z) / length;
						uv.y = (p.y + vb.y) / height;
						uvs.Add(uv);

						p.z += dz;
					}
					p.y += dy;
				}

				for ( int iy = 0; iy < heightsegs; iy++ )
				{
					int kv = iy * (lengthsegs + 1) + index;
					for ( int ix = 0; ix < lengthsegs; ix++ )
					{
						MakeQuad1(tris2, kv, kv + 1, kv + lengthsegs + 2, kv + lengthsegs + 1);
						kv++;
					}
				}
			}
		}

		int ti = verts.Count;
		int tib = verts.Count;

		List<int[]> dtris = new List<int[]>();

		// Set colors
		List<Color>		cols = new List<Color>();

		if ( usecols )
		{
			for ( int i = 0; i < verts.Count; i++ )
				cols.Add(color);
		}

		// Textbuilder
		if ( dynamobj && dynammeshenabled )
		{
			Vector3 losscl = transform.lossyScale;

			Vector3 lscl = Vector3.Scale(losscl, dynamscale);
			Matrix4x4 tm = Matrix4x4.TRS(Vector3.Scale(dynamoffset, losscl), Quaternion.Euler(dynamrot), lscl);	//dynamscale);
			dynamobj.BuildMesh(page.pnum, true);	//, ref textverts, ref texttris);

			Vector2[] textuvs = dynamobj.GetUVs(page.pnum, true);
			Vector3[] textverts = dynamobj.GetVertices(page.pnum, true);
			for ( int i = 0; i < textverts.Length; i++ )
				verts.Add(tm.MultiplyPoint3x4(textverts[i]));

			for ( int i = 0; i < textuvs.Length; i++ )
				uvs.Add(textuvs[i]);

			if ( usecols )
			{
				Color[] colverts = dynamobj.GetColors(page.pnum, true);

				if ( colverts == null || colverts.Length == 0 )
				{
					for ( int i = 0; i < textverts.Length; i++ )
						cols.Add(color);
				}
				else
				{
					for ( int i = 0; i < colverts.Length; i++ )
						cols.Add(colverts[i]);
				}
			}

			for ( int m = 0; m < dynamobj.GetNumMaterials(page.pnum, true); m++ )
				dtris.Add(dynamobj.GetTris(page.pnum, true, m));

			tib = verts.Count;
			lscl = Vector3.Scale(losscl, backdynamscale);
			tm = Matrix4x4.TRS(Vector3.Scale(losscl, backdynamoffset), Quaternion.Euler(backdynamrot), lscl);	//backdynamscale);
			dynamobj.BuildMesh(page.pnum, false);	//, ref textverts, ref texttris);

			textuvs = dynamobj.GetUVs(page.pnum, false);
			textverts = dynamobj.GetVertices(page.pnum, false);
			for ( int i = 0; i < textverts.Length; i++ )
				verts.Add(tm.MultiplyPoint3x4(textverts[i]));

			for ( int i = 0; i < textuvs.Length; i++ )
				uvs.Add(textuvs[i]);

			if ( usecols )
			{
				Color[] colverts = dynamobj.GetColors(page.pnum, false);

				if ( colverts == null || colverts.Length == 0 )
				{
					for ( int i = 0; i < textverts.Length; i++ )
						cols.Add(color);
				}
				else
				{
					for ( int i = 0; i < colverts.Length; i++ )
						cols.Add(colverts[i]);
				}
			}

			for ( int m = 0; m < dynamobj.GetNumMaterials(page.pnum, false); m++ )
				dtris.Add(dynamobj.GetTris(page.pnum, false, m));
		}

		page.verts = verts.ToArray();
		page.sverts = verts.ToArray();	//new Vector3[page.verts.Length];

		mesh.Clear();

		if ( dynamobj && dynammeshenabled )
			mesh.subMeshCount = 3 + dynamobj.GetNumMaterials(page.pnum, true) + dynamobj.GetNumMaterials(page.pnum, false);
		else
			mesh.subMeshCount = 3;

		mesh.vertices = page.verts;	//verts.ToArray();
		mesh.uv = uvs.ToArray();
		if ( usecols )
			mesh.colors = cols.ToArray();

		mesh.SetTriangles(tris.ToArray(), 0);
		mesh.SetTriangles(tris1.ToArray(), 1);
		mesh.SetTriangles(tris2.ToArray(), 2);

		if ( dynamobj && dynammeshenabled )
		{
			int mo = dynamobj.GetNumMaterials(page.pnum, true);
			int ix = 0;

			for ( int m = 0; m < dynamobj.GetNumMaterials(page.pnum, true); m++ )
			{
				int[] texttris = dtris[ix++];	//dynamobj.GetTris(page.pnum, true, m);

				for ( int i = 0; i < texttris.Length; i++ )
					texttris[i] += ti;

				mesh.SetTriangles(texttris, 3 + m);
			}

			for ( int m = 0; m < dynamobj.GetNumMaterials(page.pnum, false); m++ )
			{
				//int[] texttris = dynamobj.GetTris(page.pnum, false, m);
				int[] texttris = dtris[ix++];	//dynamobj.GetTris(page.pnum, true, m);

				for ( int i = 0; i < texttris.Length; i++ )
					texttris[i] += tib;

				mesh.SetTriangles(texttris, 3 + m + mo);
			}
		}

		mesh.RecalculateNormals();

		BuildTangents(mesh, page.verts, mesh.normals, mesh.triangles, mesh.uv);

		mesh.RecalculateBounds();

		page.bbox = mesh.bounds;
		page.mesh = mesh;
		return mesh;
	}

	Mesh CreatePageMeshHole(MegaBookPage page, float width, float length, float height, int widthsegs, int lengthsegs, int heightsegs, int xhole, int yhole)
	{
		Mesh mesh = new Mesh();

		Vector3 vb = new Vector3(width, height, length) / 2.0f;
		Vector3 va = -vb;

		if ( PivotBase )
		{
			va.y = 0.0f;
			vb.y = height;
		}

		if ( PivotEdge )
		{
			va.x = 0.0f;
			vb.x = width;
		}

		float dx = width / (float)widthsegs;
		float dy = height / (float)heightsegs;
		float dz = length / (float)lengthsegs;

		Vector3 p = va;

		// Lists should be static, clear out to reuse
		List<Vector3>	verts = new List<Vector3>();
		List<Vector2>	uvs = new List<Vector2>();
		List<int>		tris = new List<int>();
		List<int>		tris1 = new List<int>();
		List<int>		tris2 = new List<int>();
		List<Color>		cols = new List<Color>();

		Vector2 uv = Vector2.zero;

		// Do we have top and bottom
		if ( width > 0.0f && length > 0.0f )
		{
			p.y = vb.y;

			int[,] grid = new int[lengthsegs + 1, widthsegs + 1];

			for ( int iz = 0; iz <= lengthsegs; iz++ )
			{
				p.x = va.x;
				for ( int ix = 0; ix <= widthsegs; ix++ )
				{
					if ( ix <= xhole || ix >= widthsegs - xhole || iz <= yhole || iz >= lengthsegs - yhole )
					{
						grid[iz, ix] = verts.Count;

						float alpha = (float)ix / (float)widthsegs;
						p.x = va.x + (pagesectioncrv.Evaluate(alpha) * width);

						verts.Add(p);

						uv.x = p.x / width;
						uv.y = (p.z + vb.z) / length;

						uvs.Add(uv);
					}
					p.x += dx;
				}
				p.z += dz;
			}

			int kv = 0;
			for ( int iz = 0; iz < lengthsegs; iz++ )
			{
				for ( int ix = 0; ix < widthsegs; ix++ )
				{
					if ( ix < xhole || ix >= widthsegs - xhole || iz < yhole || iz >= lengthsegs - yhole )
					{
						kv = grid[iz, ix];
						MakeQuad1(tris, kv, grid[iz + 1, ix], grid[iz + 1, ix + 1], kv + 1);
					}
				}
			}

			p.y = va.y;
			p.z = va.z;

			for ( int iy = 0; iy <= lengthsegs; iy++ )
			{
				p.x = va.x;
				for ( int ix = 0; ix <= widthsegs; ix++ )
				{
					if ( ix <= xhole || ix >= widthsegs - xhole || iy <= yhole || iy >= lengthsegs - yhole )
					{
						grid[iy, ix] = verts.Count;

						float alpha = (float)ix / (float)widthsegs;
						p.x = va.x + (pagesectioncrv.Evaluate(alpha) * width);

						verts.Add(p);
						uv.x = 1.0f - (p.x / width);
						//uv.x = p.x / width;
						uv.y = ((p.z + vb.z) / length);

						uvs.Add(uv);
					}
					p.x += dx;
				}
				p.z += dz;
			}

			for ( int iy = 0; iy < lengthsegs; iy++ )
			{
				for ( int ix = 0; ix < widthsegs; ix++ )
				{
					if ( ix < xhole || ix >= widthsegs - xhole || iy < yhole || iy >= lengthsegs - yhole )
					{
						kv = grid[iy, ix];
						MakeQuad1(tris1, kv, kv + 1, grid[iy + 1, ix + 1], grid[iy + 1, ix]);
					}
				}
			}
		}

		// Front back
		if ( width > 0.0f && height > 0.0f )
		{
			int index = verts.Count;

			p.z = va.z;
			p.y = va.y;
			for ( int iz = 0; iz <= heightsegs; iz++ )
			{
				p.x = va.x;
				for ( int ix = 0; ix <= widthsegs; ix++ )
				{
					float alpha = (float)ix / (float)widthsegs;
					p.x = va.x + (pagesectioncrv.Evaluate(alpha) * width);

					verts.Add(p);
					uv.x = (p.x + vb.x) / width;
					uv.y = (p.y + vb.y) / height;
					uvs.Add(uv);
					p.x += dx;
				}
				p.y += dy;
			}

			for ( int iz = 0; iz < heightsegs; iz++ )
			{
				int kv = iz * (widthsegs + 1) + index;
				for ( int ix = 0; ix < widthsegs; ix++ )
				{
					MakeQuad1(tris2, kv, kv + widthsegs + 1, kv + widthsegs + 2, kv + 1);
					kv++;
				}
			}

			index = verts.Count;

			p.z = vb.z;
			p.y = va.y;
			for ( int iy = 0; iy <= heightsegs; iy++ )
			{
				p.x = va.x;
				for ( int ix = 0; ix <= widthsegs; ix++ )
				{
					float alpha = (float)ix / (float)widthsegs;
					p.x = va.x + (pagesectioncrv.Evaluate(alpha) * width);

					verts.Add(p);
					uv.x = (p.x + vb.x) / width;
					uv.y = (p.y + vb.y) / height;
					uvs.Add(uv);
					p.x += dx;
				}
				p.y += dy;
			}

			for ( int iy = 0; iy < heightsegs; iy++ )
			{
				int kv = iy * (widthsegs + 1) + index;
				for ( int ix = 0; ix < widthsegs; ix++ )
				{
					MakeQuad1(tris2, kv, kv + 1, kv + widthsegs + 2, kv + widthsegs + 1);
					kv++;
				}
			}
		}

		// Left Right
		if ( length > 0.0f && height > 0.0f )
		{
			int index = verts.Count;

			p.x = vb.x;
			p.y = va.y;
			for ( int iz = 0; iz <= heightsegs; iz++ )
			{
				p.z = va.z;
				for ( int ix = 0; ix <= lengthsegs; ix++ )
				{
					verts.Add(p);
					uv.x = (p.z + vb.z) / length;
					uv.y = (p.y + vb.y) / height;
					uvs.Add(uv);
					p.z += dz;
				}
				p.y += dy;
			}

			for ( int iz = 0; iz < heightsegs; iz++ )
			{
				int kv = iz * (lengthsegs + 1) + index;
				for ( int ix = 0; ix < lengthsegs; ix++ )
				{
					MakeQuad1(tris2, kv, kv + lengthsegs + 1, kv + lengthsegs + 2, kv + 1);
					kv++;
				}
			}

			if ( spineedge )
			{
				index = verts.Count;

				p.x = va.x;
				p.y = va.y;
				for ( int iy = 0; iy <= heightsegs; iy++ )
				{
					p.z = va.z;
					for ( int ix = 0; ix <= lengthsegs; ix++ )
					{
						verts.Add(p);
						uv.x = (p.z + vb.z) / length;
						uv.y = (p.y + vb.y) / height;
						uvs.Add(uv);

						p.z += dz;
					}
					p.y += dy;
				}

				for ( int iy = 0; iy < heightsegs; iy++ )
				{
					int kv = iy * (lengthsegs + 1) + index;
					for ( int ix = 0; ix < lengthsegs; ix++ )
					{
						MakeQuad1(tris2, kv, kv + 1, kv + lengthsegs + 2, kv + lengthsegs + 1);
						kv++;
					}
				}
			}
		}

		if ( usecols )
		{
			for ( int i = 0; i < verts.Count; i++ )
				cols.Add(color);
		}

		page.verts1 = verts.ToArray();
		page.sverts1 = new Vector3[page.verts1.Length];

		mesh.Clear();
		mesh.subMeshCount = 3;
		mesh.vertices = page.verts1;	//verts.ToArray();
		mesh.uv = uvs.ToArray();
		if ( usecols )
		{
			mesh.colors = cols.ToArray();
		}
		mesh.SetTriangles(tris.ToArray(), 0);
		mesh.SetTriangles(tris1.ToArray(), 1);
		mesh.SetTriangles(tris2.ToArray(), 2);

		mesh.RecalculateNormals();

		BuildTangents(mesh, page.verts1, mesh.normals, mesh.triangles, mesh.uv);

		mesh.RecalculateBounds();

		page.bbox = mesh.bounds;
		page.holemesh = mesh;
		return mesh;
	}

	void BuildTangents(Mesh mesh, Vector3[] verts, Vector3[] norms, int[] tris, Vector2[] uvs)
	{
		int vertexCount = mesh.vertices.Length;

		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];
		Vector4[] tangents = new Vector4[vertexCount];

		for ( int a = 0; a < tris.Length; a += 3 )
		{
			long i1 = tris[a];
			long i2 = tris[a + 1];
			long i3 = tris[a + 2];

			Vector3 v1 = verts[i1];
			Vector3 v2 = verts[i2];
			Vector3 v3 = verts[i3];

			Vector2 w1 = uvs[i1];
			Vector2 w2 = uvs[i2];
			Vector2 w3 = uvs[i3];

			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;

			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;

			float r = 1.0f / (s1 * t2 - s2 * t1);

			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;

			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
		}

		for ( int a = 0; a < vertexCount; a++ )
		{
			Vector3 n = norms[a];
			Vector3 t = tan1[a];

			Vector3.OrthoNormalize(ref n, ref t);
			tangents[a].x = t.x;
			tangents[a].y = t.y;
			tangents[a].z = t.z;
			tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
		}

		mesh.tangents = tangents;
	}

	Mesh CopyMeshObject(MegaBookPage page, GameObject srcobj, float width, float length, float height, Vector3 rot)
	{
		Mesh srcmesh = srcobj.GetComponent<MeshFilter>().sharedMesh;
		return CopyMesh(page, srcmesh, width, length, height, rot);
	}

	Mesh CopyMesh(MegaBookPage page, Mesh srcmesh, float width, float length, float height, Vector3 rot)
	{
		Mesh mesh = new Mesh();

		Vector3 scl = srcmesh.bounds.size;
		scl.x = 1.0f / scl.x;
		scl.y = 1.0f / scl.y;
		scl.z = 1.0f / scl.z;

		scl.x *= width;
		scl.y *= length;
		scl.z *= height;

		Matrix4x4 tm = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rot), scl);

		Vector3[] v = srcmesh.vertices;
		Vector3 min = Vector3.zero;
		Vector3 max = Vector3.zero;

		for ( int i = 0; i < v.Length; i++ )
		{
			v[i] = tm.MultiplyPoint3x4(v[i]);

			if ( i == 0 )
			{
				min = v[i];
				max = v[i];
			}

			if ( v[i].x < min.x ) min.x = v[i].x;
			if ( v[i].y < min.y ) min.y = v[i].y;
			if ( v[i].z < min.z ) min.z = v[i].z;

			if ( v[i].x > max.x ) max.x = v[i].x;
			if ( v[i].y > max.y ) max.y = v[i].y;
			if ( v[i].z > max.z ) max.z = v[i].z;
		}

		page.verts = v;
		page.sverts = new Vector3[page.verts.Length];

		mesh.subMeshCount = srcmesh.subMeshCount;

		float y = (max.y - min.y) * 0.5f;
		for ( int i = 0; i < v.Length; i++ )
		{
			v[i].x -= min.x;
			v[i].y -= y;
		}

		mesh.vertices = v;
		mesh.uv = srcmesh.uv;
		mesh.normals = srcmesh.normals;
		mesh.colors = srcmesh.colors;
		mesh.tangents = srcmesh.tangents;

		for ( int i = 0; i < srcmesh.subMeshCount; i++ )
			mesh.SetTriangles(srcmesh.GetTriangles(i), i);

		mesh.RecalculateBounds();
		page.bbox = mesh.bounds;
		page.mesh = mesh;
		return mesh;
	}

	Mesh CopyMeshObjectHole(MegaBookPage page, GameObject srcobj, float width, float length, float height, Vector3 rot)
	{
		Mesh srcmesh = srcobj.GetComponent<MeshFilter>().sharedMesh;
		return CopyMeshHole(page, srcmesh, width, length, height, rot);
	}

	Mesh CopyMeshHole(MegaBookPage page, Mesh srcmesh, float width, float length, float height, Vector3 rot)
	{
		Mesh mesh = new Mesh();

		Vector3 scl = srcmesh.bounds.size;
		scl.x = 1.0f / scl.x;
		scl.y = 1.0f / scl.y;
		scl.z = 1.0f / scl.z;

		scl.x *= width;
		scl.y *= length;
		scl.z *= height;

		Matrix4x4 tm = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rot), scl);

		Vector3[] v = srcmesh.vertices;
		Vector3 min = Vector3.zero;
		Vector3 max = Vector3.zero;

		for ( int i = 0; i < v.Length; i++ )
		{
			v[i] = tm.MultiplyPoint3x4(v[i]);

			if ( i == 0 )
			{
				min = v[i];
				max = v[i];
			}

			if ( v[i].x < min.x ) min.x = v[i].x;
			if ( v[i].y < min.y ) min.y = v[i].y;
			if ( v[i].z < min.z ) min.z = v[i].z;

			if ( v[i].x > max.x ) max.x = v[i].x;
			if ( v[i].y > max.y ) max.y = v[i].y;
			if ( v[i].z > max.z ) max.z = v[i].z;
		}

		page.verts1 = v;
		page.sverts1 = new Vector3[page.verts.Length];

		mesh.subMeshCount = srcmesh.subMeshCount;

		float y = (max.y - min.y) * 0.5f;
		for ( int i = 0; i < v.Length; i++ )
		{
			v[i].x -= min.x;
			v[i].y -= y;
		}

		mesh.vertices = v;
		mesh.uv = srcmesh.uv;
		mesh.normals = srcmesh.normals;
		mesh.colors = srcmesh.colors;
		mesh.tangents = srcmesh.tangents;

		for ( int i = 0; i < srcmesh.subMeshCount; i++ )
			mesh.SetTriangles(srcmesh.GetTriangles(i), i);

		mesh.RecalculateBounds();
		page.bbox = mesh.bounds;
		page.mesh = mesh;
		return mesh;
	}

	List<MegaBookPageAttach>	attached = new List<MegaBookPageAttach>();

	public void Detach()
	{
		MegaBookPageAttach[] attach = (MegaBookPageAttach[])FindObjectsOfType(typeof(MegaBookPageAttach));

		attached.Clear();
		for ( int i = 0; i < attach.Length; i++ )
		{
			if ( attach[i].book == this && attach[i].attached )
			{
				attached.Add(attach[i]);
				attach[i].DetachIt();
			}
		}
	}

	public void Attach()
	{
		for ( int i = 0; i < attached.Count; i++ )
		{
			attached[i].AttachItUV();
		}
	}

	void RebuildMeshes()
	{
		UpdateSettings();
		Detach();
		BuildPageMeshes();
		Attach();
		UpdateSettings();
	}

	void Update()
	{
		if ( rebuildmeshes )
		{
			rebuildmeshes = false;
			RebuildMeshes();
		}

		if ( Snap )
			page = (int)page;

		if ( Flip != page )
		{
			Flip = Mathf.SmoothDamp(Flip, page, ref turnspd, turntime);
			if ( Mathf.Abs(Flip - page) < 0.0001f )
			{
				Flip = page;
				turnspd = 0.0f;
			}
		}

		bool doattach = false;
		if ( rebuild )	//meshes )
		{
			rebuild = false;
			// Detach objects
			Detach();
			RemovePages();
			BuildPages();
			doattach = true;
		}
		
		if ( changespineangle )
		{
			float alpha = Flip / (float)NumPages;

			BottomAngle = alpha * -BottomMaxAngle;	//.0f;
			BottomAngle = Mathf.Clamp(BottomAngle, -BottomMaxAngle, 0.0f);
		}
		else
			BottomAngle = 0.0f;

		Vector3 rot = transform.localRotation.eulerAngles;
		rot.z = -BottomAngle;
		transform.localEulerAngles = rot;	// = Quaternion.Euler(rot);

		if ( frontcover )
		{
			float ca = 0.0f;
			if ( Flip < 0.0f )
				ca = -Flip;

			float ang = Mathf.Lerp(frontang, 0.0f, ca);

			frontcover.position = transform.TransformPoint(frontpivot);
			frontcover.localEulerAngles = new Vector3(0.0f, 0.0f, ang);	// - BottomAngle);
		}

		if ( backcover )
		{
			float ca = 0.0f;
			if ( Flip > NumPages )
				ca = Flip - NumPages;

			float ang = Mathf.Lerp(0.0f, backang, ca);

			if ( !changespineangle )
			{
				ang = 0.0f;
			}

			backcover.position = transform.TransformPoint(backpivot);
			backcover.localEulerAngles = new Vector3(0.0f, 0.0f, ang);
		}

		// Do page turning
		if ( UseThreading && Application.isPlaying )
		{
#if !UNITY_FLASH && !UNITY_PS3 && !UNITY_METRO && !UNITY_WP8
			UpdateBookMT();
#endif
		}
		else
		{
			bool dohole = true;
			for ( int i = 0; i < pages.Count; i++ )
			{
				if ( i == 0 || i == pages.Count - 1 )
					dohole = false;
				else
					dohole = true;

				pages[i].Update(this, (Flip * 14.0f) - ((float)i * shuffle), dohole);
			}
		}

		if ( doattach )
		{
			Attach();
		}
	}

	MegaBookPage CreatePage(int i)
	{
		MegaBookPage page = new MegaBookPage();

		page.turner = new MegaBookBendMod();
		page.flexer = new MegaBookBendMod();
		page.lander = new MegaBookBendMod();

		page.flexer.axis = MegaBookAxis.X;
		page.flexer.fromto = true;
		page.flexer.from = -Flex_CArea * pageWidth;
		page.flexer.to = 0.0f;
		page.flexer.center = new Vector3(-Flex_CCenter * pageWidth, 0.0f, 0.0f);

		page.turner.axis = MegaBookAxis.X;
		page.turner.fromto = true;
		page.turner.from = -3.0f;
		page.turner.to = 0.0f;
		page.turner.center = new Vector3(-Turn_CCenter * pageWidth, 0.0f, 0.0f);	
		page.turner.gizmo_rotation = Vector3.zero;

		page.lander.axis = MegaBookAxis.X;
		page.lander.fromto = true;
		page.lander.from = -(Land_CArea * pageWidth);
		page.lander.to = 0.0f;
		page.lander.center = new Vector3(-(pageWidth / 2.0f * Land_CCenter), 0.0f, 0.0f);

		// Anim keys
		page.turnerfromcon.AddKey(0.0f, -3.0f);
		page.turnerfromcon.AddKey(7.0f, -25.0f);
		page.turnerfromcon.AddKey(14.0f, -3.0f);

		page.turnerangcon.AddKey(0.0f, -Turn_minAngle);
		page.turnerangcon.AddKey(2.0f, 0.0f);
		page.turnerangcon.AddKey(12.0f, 0.0f);
		Keyframe key = page.turnerangcon.keys[2];
		key.value = -Turn_maxAngle;	//180.0f;
		page.turnerangcon.MoveKey(2, key);

		page.flexangcon.AddKey(0.0f, 0.0f);
		page.flexangcon.AddKey(7.0f, -Flex_MaxAngle);
		//page.flexangcon.AddKey(7.0f, 0.0f);	//-Flex_MaxAngle);
		page.flexangcon.AddKey(12.0f, Flex_MaxAngle - ((Flex_MaxAngle / 100.0f) * 25.0f));
		page.flexangcon.AddKey(14.0f, 0.0f);

		page.landerangcon.AddKey(0.0f, Land_minAngle);
		page.landerangcon.AddKey(10.0f, 0.0f);
		page.landerangcon.AddKey(14.0f, Land_maxAngle);

		page.obj = MakePageObject(page, i);

		pages.Add(page);

		return page;
	}

	public void BuildPageMeshes()
	{
		RemovePages();
		BuildPages();
	}

	public void UpdateAttached()
	{
		int pnum = 0;
		for ( int i = 0; i < pages.Count; i++ )
		{
			MegaBookPage page = pages[i];

			pnum = i % pageparams.Count;
			MegaBookPageParams parms = pageparams[pnum];

			page.visobjlow = parms.visobjlow;
			page.visobjhigh = parms.visobjhigh;

			//Debug.Log("pobjs " + page.objects.Count + " param " + parms.objects.Count);
			//int ix = 0;
			for ( int j = 0; j < parms.objects.Count; j++ )
			{
				//if ( parms.objects[j].obj )
				{
					page.objects[j].pos = parms.objects[j].pos;
					page.objects[j].rot = parms.objects[j].rot;
					page.objects[j].offset = parms.objects[j].offset;
					page.objects[j].attachforward = parms.objects[j].attachforward;
					page.objects[j].message = parms.objects[j].message;

					UpdateAttachObject(page, page.objects[j]);
				}
			}
		}
	}

	public void BuildPages()
	{
		Quaternion rot = transform.rotation;
		transform.rotation = Quaternion.identity;
		pageindex = 0;
		Random.seed = seed;

		pages.Clear();

		float py = 0.0f;
		if ( NumPages > 1 )
			py = ((NumPages - 1) * pageGap) * 0.5f;

		for ( int i = 0; i < NumPages; i++ )
		{
			MegaBookPage page = CreatePage(i);

			page.obj.transform.localPosition = new Vector3(0.0f, py, 0.0f);

			py -= pageGap;

			// Need to have controls add to start value
			if ( Flex_Random )
				page.flexer.gizmo_rotation = new Vector3(0.0f, Random.Range(-Flex_RandomDegree, Flex_RandomDegree), 0.0f);
			else
				page.flexer.gizmo_rotation = new Vector3(0.0f, Flex_RandomDegree, 0.0f);

			Keyframe key = page.turnerfromcon.keys[0];
			key.value = -((Turn_CArea * pageWidth) + ((float)(NumPages - (float)i) * (pageGap * Turn_Spread)));
			page.turnerfromcon.MoveKey(0, key);

			key = page.turnerfromcon.keys[1];
			key.value = -pageWidth;
			page.turnerfromcon.MoveKey(1, key);

			key = page.turnerfromcon.keys[2];
			key.value = -((Turn_CArea * pageWidth) + ((float)i * (pageGap * Turn_Spread)));
			page.turnerfromcon.MoveKey(2, key);
		}

		UpdateSettings();
		transform.rotation = rot;
	}

	public void UpdateSettings()
	{
		ChangeWidthValue(pageWidth);
		ChangeBookThickness(bookthickness, usebookthickness);
		gapChange(pageGap);
		Flex_CCenterChange(Flex_CCenter);
		Flex_CAreaChange(Flex_CArea);
		Flex_MaxAngleChange(Flex_MaxAngle);
		Flex_RandomChange(Flex_RandomDegree, Flex_RandomSeed, Flex_Random);
		Turn_CCenterChange(Turn_CCenter);
		Turn_CAreaChange(Turn_CArea);
		Turn_maxAngleChange(Turn_maxAngle);
		Turn_minAngleChange(Turn_minAngle);
		Land_CCenterChange(Land_CCenter);
		Land_CAreaChange(Land_CArea);
		Land_MaxAngleChange(Land_maxAngle);
		Land_MinAngleChange(Land_minAngle);
	}

	public void ChangeBookThickness(float thick, bool use)
	{
		usebookthickness = use;
		if ( thick < 0.0f )
			thick = 0.0f;

		bookthickness = thick;
		if ( use )
		{
			if ( NumPages > 1 )
				pageGap = (bookthickness - pageHeight) / (NumPages - 1);
			else
				pageGap = 0.0f;

			gapChange(pageGap);
		}
	}

	void ChangeWidthValue(float widthVal)
	{
		pageWidth = widthVal;
		for ( int i = 0; i < pages.Count; i++ )
			pages[i].width = widthVal;
	}

	void pageLengthChange(float lengthVal)
	{
		for ( int i = 0; i < pages.Count;i++ )
			pages[i].length = lengthVal;
	}

	void gapChange(float gapVal)
	{
		pageGap = gapVal;

		float py = 0.0f;
		if ( NumPages > 1 )
			py = ((NumPages - 1) * pageGap) * 0.5f;

		float radius = 0.0f;
		
		if ( spineradius != 0.0f )
			radius = py / spineradius;

		float px = 0.0f;
		float off = 0.0f;

		for ( int i = 0; i < pages.Count; i++ )
		{
			if ( radius != 0.0f )
			{
				float v = Mathf.Clamp(py / radius, -1.0f, 1.0f);
				float theta = Mathf.Asin(v);
				px = radius - (Mathf.Cos(theta) * radius);

				if ( i == 0 && radius < 0.0f )
					off = px;
				px -= off;
			}

			pages[i].obj.transform.localPosition = new Vector3(px, py, 0.0f);
			py -= pageGap;
			pages[i].deform = true;
		}
	}
	
	public void Flex_CCenterChange(float newVal)
	{
		Flex_CCenter = newVal;
		for ( int i = 0; i < pages.Count; i++ )
		{
			pages[i].flexer.center = new Vector3(-Flex_CCenter * pageWidth, 0.0f, 0.0f);
			pages[i].deform = true;
		}
	}
	
	public void Flex_CAreaChange(float newVal)
	{
		Flex_CArea = newVal;
		for ( int i = 0; i < pages.Count; i++ )
		{
			pages[i].flexer.from = -Flex_CArea * pageWidth;
			pages[i].deform = true;
		}
	}
	
	public void Flex_MaxAngleChange(float newVal)
	{
		Flex_MaxAngle = newVal;
		for ( int i = 0; i < pages.Count; i++ )
		{
			Keyframe key = pages[i].flexangcon.keys[1];
			key.value = -Flex_MaxAngle;
			pages[i].flexangcon.MoveKey(1, key);

			key = pages[i].flexangcon.keys[2];
			key.value = Flex_MaxAngle - ((Flex_MaxAngle / 100.0f) * 25.0f);
			pages[i].flexangcon.MoveKey(2, key);

			pages[i].deform = true;
			pages[i].flexangcon.SmoothTangents(0, 0.0f);
			pages[i].flexangcon.SmoothTangents(1, 0.0f);
			pages[i].flexangcon.SmoothTangents(2, 0.0f);
			pages[i].flexangcon.SmoothTangents(3, 0.0f);
		}
	}

	public void Flex_RandomChange(float newVal, int seed, bool userand)
	{
		Flex_Random = userand;
		Flex_RandomSeed = seed;
		Flex_RandomDegree = newVal;

		Random.seed = Flex_RandomSeed;
		for ( int i = 0; i < pages.Count; i++ )
		{
			if ( Flex_Random )
				pages[i].flexer.gizmo_rotation = new Vector3(0.0f, Random.Range(-Flex_RandomDegree, Flex_RandomDegree), 0.0f);
			else
				pages[i].flexer.gizmo_rotation = new Vector3(0.0f, Flex_RandomDegree, 0.0f);

			pages[i].deform = true;
		}
	}
	
	public void Turn_CCenterChange(float newVal)
	{
		Turn_CCenter = newVal;
		for ( int i = 0; i < pages.Count; i++ )
		{
			pages[i].turner.center = new Vector3(-newVal * pageWidth, 0.0f, 0.0f);
			pages[i].deform = true;
		}
	}

	public void Turn_SpreadChange(float newVal)
	{
		Turn_Spread = newVal;
		Turn_CAreaChange(Turn_CArea);
	}

	public void Turn_CAreaChange(float newVal)
	{
		//Debug.Log("p");
		Turn_CArea = newVal;
		if ( changespineangle )
		{
			for ( int i = 0; i < pages.Count; i++ )
			{
				MegaBookPage page = pages[i];

				Keyframe key = page.turnerfromcon.keys[0];
				if ( i == 0 )
					key.value = -((Turn_CArea * pageWidth) + pageWidth);
				else
					key.value = -((Turn_CArea * pageWidth) + ((float)(NumPages - i) * (pageGap * Turn_Spread)));
				page.turnerfromcon.MoveKey(0, key);

				key = page.turnerfromcon.keys[1];
				key.value = -pageWidth;
				page.turnerfromcon.MoveKey(1, key);

				key = page.turnerfromcon.keys[2];
				key.value = -((Turn_CArea * pageWidth) + ((float)i * (pageGap * Turn_Spread)));
				page.turnerfromcon.MoveKey(2, key);

				page.turnerfromcon.SmoothTangents(0, 0.0f);
				page.turnerfromcon.SmoothTangents(1, 0.0f);
				page.turnerfromcon.SmoothTangents(2, 0.0f);
				page.deform = true;
			}
		}
		else
		{
			//Debug.Log("p");
			for ( int i = 0; i < pages.Count; i++ )
			{
				MegaBookPage page = pages[i];

				Keyframe key = page.turnerfromcon.keys[0];
				//if ( i == 0 )
					key.value = -((Turn_CArea * pageWidth) + (pageWidth * 4.0f));
				//else
					//key.value = -((Turn_CArea * pageWidth) + ((float)(NumPages - i) * (pageGap * Turn_Spread) * 18.0f));
				page.turnerfromcon.MoveKey(0, key);

				key = page.turnerfromcon.keys[1];
				key.value = -pageWidth * 4.0f;
				page.turnerfromcon.MoveKey(1, key);

				key = page.turnerfromcon.keys[2];
				key.value = -((Turn_CArea * pageWidth) + ((float)i * (pageGap * Turn_Spread)));
				page.turnerfromcon.MoveKey(2, key);

				page.turnerfromcon.SmoothTangents(0, 0.0f);
				page.turnerfromcon.SmoothTangents(1, 0.0f);
				page.turnerfromcon.SmoothTangents(2, 0.0f);
				page.deform = true;
			}
		}
	}
	
	public void Turn_maxAngleChange(float newValue)
	{
		Turn_maxAngle = newValue;
		for ( int i = 0; i < pages.Count; i++ )
		{
			Keyframe key = pages[i].turnerangcon.keys[2];
			key.value = -Turn_maxAngle;
			pages[i].turnerangcon.MoveKey(2, key);
			pages[i].deform = true;

			pages[i].turnerangcon.SmoothTangents(0, 0.0f);
			pages[i].turnerangcon.SmoothTangents(1, 0.0f);
			pages[i].turnerangcon.SmoothTangents(2, 0.0f);
		}
	}

	public void Turn_minAngleChange(float newValue)
	{
		Turn_minAngle = newValue;
		for ( int i = 0; i < pages.Count; i++ )
		{
			Keyframe key = pages[i].turnerangcon.keys[0];
			key.value = -Turn_minAngle;
			pages[i].turnerangcon.MoveKey(0, key);
			pages[i].deform = true;

			pages[i].turnerangcon.SmoothTangents(0, 0.0f);
			pages[i].turnerangcon.SmoothTangents(1, 0.0f);
			pages[i].turnerangcon.SmoothTangents(2, 0.0f);
		}
	}

	void Turn_CLevelChange(float newVal)
	{
		Turn_CLevel = newVal;
		for ( int i = 0; i < pages.Count; i++ )
		{
			pages[i].turner.gizmo_rotation = new Vector3(0.0f, Turn_CLevel, 0.0f);
			pages[i].turner.gizmo_pos = new Vector3((-(pageWidth / 2.0f - (Mathf.Cos(Mathf.Deg2Rad * Turn_CLevel) * (pageWidth / 2.0f)))),
				0.0f,
				(((Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * Turn_CLevel))) * (pageWidth / 2.0f)) + ((Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * 0.0f))) * (pageWidth / 2.0f))));
		}
	}
	
	public void Land_CCenterChange(float newVal)
	{
		Land_CCenter = newVal;
		for ( int i = 0; i < pages.Count; i++ )
		{
			pages[i].lander.center = new Vector3(-(pageWidth / 2.0f * Land_CCenter), 0.0f, 0.0f);
			pages[i].deform = true;
		}
	}

	public void Land_CAreaChange(float newVal)
	{
		Land_CArea = newVal;
		for ( int i = 0; i < pages.Count; i++ )
		{
			pages[i].lander.from = -(Land_CArea * pageWidth);
			pages[i].deform = true;
		}
	}

	public void Land_MaxAngleChange(float newVal)
	{
		Land_maxAngle = newVal;
		for ( int i = 0; i < pages.Count; i++ )
		{
			Keyframe key = pages[i].landerangcon.keys[2];
			key.value = Land_maxAngle;
			pages[i].landerangcon.MoveKey(2, key);
			pages[i].deform = true;

			pages[i].landerangcon.SmoothTangents(0, 0.0f);
			pages[i].landerangcon.SmoothTangents(1, 0.0f);
			pages[i].landerangcon.SmoothTangents(2, 0.0f);
		}
	}

	public void Land_MinAngleChange(float newVal)
	{
		Land_minAngle = newVal;
		for ( int i = 0; i < pages.Count; i++ )
		{
			Keyframe key = pages[i].landerangcon.keys[0];
			key.value = Land_minAngle;
			pages[i].landerangcon.MoveKey(0, key);
			pages[i].deform = true;

			pages[i].landerangcon.SmoothTangents(0, 0.0f);
			pages[i].landerangcon.SmoothTangents(1, 0.0f);
			pages[i].landerangcon.SmoothTangents(2, 0.0f);
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

	public void AttachObject(MegaBookPage pobj, MegaBookPageObject obj)
	{
		if ( obj != null )	//&& obj.obj != null )
		{
			Bounds bounds = pobj.bbox;

			// Calc local pos from pos in bounds
			Vector3 pp = obj.pos * 0.01f;

			pp.x = Mathf.Clamp01(pp.x);
			pp.y = Mathf.Clamp01(pp.y);
			pp.z = Mathf.Clamp01(pp.z);

			Vector3 lpos = bounds.min + (Vector3.Scale(pp, bounds.size));

			Vector3 objSpacePt = lpos;

			Vector3[] verts = pobj.verts;
			int[] tris = pobj.mesh.triangles;
			int index = -1;
			MegaBookNearest.NearestPointOnMesh1(objSpacePt, verts, tris, ref index, ref obj.BaryCoord);

			if ( index >= 0 )
			{
				obj.BaryVerts[0] = tris[index];
				obj.BaryVerts[1] = tris[index + 1];
				obj.BaryVerts[2] = tris[index + 2];
			}

			MegaBookNearest.NearestPointOnMesh1(objSpacePt + obj.attachforward, verts, tris, ref index, ref obj.BaryCoord1);

			if ( index >= 0 )
			{
				obj.BaryVerts1[0] = tris[index];
				obj.BaryVerts1[1] = tris[index + 1];
				obj.BaryVerts1[2] = tris[index + 2];
			}

			pobj.objects.Add(obj);
		}
	}

	public void UpdateAttachObject(MegaBookPage pobj, MegaBookPageObject obj)
	{
		if ( obj != null && obj.obj != null )
		{
			Bounds bounds = pobj.bbox;

			// Calc local pos from pos in bounds
			Vector3 pp = obj.pos * 0.01f;

			pp.x = Mathf.Clamp01(pp.x);
			pp.y = Mathf.Clamp01(pp.y);
			pp.z = Mathf.Clamp01(pp.z);

			Vector3 lpos = bounds.min + (Vector3.Scale(pp, bounds.size));

			Vector3 objSpacePt = lpos;

			Vector3[] verts = pobj.verts;
			int[] tris = pobj.mesh.triangles;
			int index = -1;
			MegaBookNearest.NearestPointOnMesh1(objSpacePt, verts, tris, ref index, ref obj.BaryCoord);

			if ( index >= 0 )
			{
				obj.BaryVerts[0] = tris[index];
				obj.BaryVerts[1] = tris[index + 1];
				obj.BaryVerts[2] = tris[index + 2];
			}

			MegaBookNearest.NearestPointOnMesh1(objSpacePt + obj.attachforward, verts, tris, ref index, ref obj.BaryCoord1);

			if ( index >= 0 )
			{
				obj.BaryVerts1[0] = tris[index];
				obj.BaryVerts1[1] = tris[index + 1];
				obj.BaryVerts1[2] = tris[index + 2];
			}

			//pobj.objects.Add(obj);
		}
	}

	public Vector3 CalcPos(GameObject obj, MegaBookPageParams page, Vector3 oldpos)
	{
		return oldpos;
	}
}
