#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static partial class P3D_Helper
{
	public static bool BaseRectSet;
	
	public static Rect BaseRect;
	
	private static GUIStyle none;
	
	private static GUIStyle error;
	
	private static GUIStyle noError;
	
	private static Texture2D pointCursor;
	
	private static GUIStyle miniActiveButtonLeft;
	
	private static GUIStyle miniActiveButtonMid;
	
	private static GUIStyle miniActiveButtonRight;
	
	private static GUIStyle smallTopText;
	
	private static GUIStyle smallLeftText;
	
	private static GUIStyle smallEntryText;
	
	private static GUIStyle boldFoldout;
	
	public static GUIStyle MiniActiveButtonLeft
	{
		get
		{
			if (miniActiveButtonLeft == null)
			{
				miniActiveButtonLeft = new GUIStyle(EditorStyles.miniButtonLeft);
				miniActiveButtonLeft.normal = miniActiveButtonLeft.active;
			}
			
			return miniActiveButtonLeft;
		}
	}
	
	public static GUIStyle MiniActiveButtonMid
	{
		get
		{
			if (miniActiveButtonMid == null)
			{
				miniActiveButtonMid = new GUIStyle(EditorStyles.miniButtonMid);
				miniActiveButtonMid.normal = miniActiveButtonMid.active;
			}
			
			return miniActiveButtonMid;
		}
	}
	
	public static GUIStyle MiniActiveButtonRight
	{
		get
		{
			if (miniActiveButtonRight == null)
			{
				miniActiveButtonRight = new GUIStyle(EditorStyles.miniButtonRight);
				miniActiveButtonRight.normal = miniActiveButtonRight.active;
			}
			
			return miniActiveButtonRight;
		}
	}
	
	public static GUIStyle SmallTopText
	{
		get
		{
			if (smallTopText == null)
			{
				smallTopText = new GUIStyle(EditorStyles.label);
				smallTopText.fontSize  = 9;
				smallTopText.alignment = TextAnchor.LowerLeft;
			}
			
			return smallTopText;
		}
	}
	
	public static GUIStyle SmallLeftText
	{
		get
		{
			if (smallLeftText == null)
			{
				smallLeftText = new GUIStyle(EditorStyles.label);
				smallLeftText.fontSize  = 10;
				smallLeftText.alignment = TextAnchor.LowerRight;
			}
			
			return smallLeftText;
		}
	}
	
	public static GUIStyle SmallEntryText
	{
		get
		{
			if (smallEntryText == null)
			{
				smallEntryText = new GUIStyle(EditorStyles.numberField);
				smallEntryText.fontSize = 10;
			}
			
			return smallEntryText;
		}
	}
	
	public static GUIStyle BoldFoldout
	{
		get
		{
			if (boldFoldout == null)
			{
				boldFoldout = new GUIStyle(EditorStyles.foldout);
				boldFoldout.fontStyle = FontStyle.Bold;
			}
			
			return boldFoldout;
		}
	}
	
	public static GUIStyle None
	{
		get
		{
			if (none == null)
			{
				none = new GUIStyle();
			}
			
			return none;
		}
	}
	
	public static GUIStyle Error
	{
		get
		{
			if (error == null)
			{
				error                   = new GUIStyle();
				error.border            = new RectOffset(3, 3, 3, 3);
				error.normal            = new GUIStyleState();
				error.normal.background = CreateTempTexture(12, 12, "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAALElEQVQIHWP4z8CgC8SHgfg/lNZlQBIACYIlGEEMBjTABOQfQRM7AlKGYSYAoOwcvDRV9/MAAAAASUVORK5CYII=");
			}
			
			return error;
		}
	}
	
	public static GUIStyle NoError
	{
		get
		{
			if (noError == null)
			{
				noError        = new GUIStyle();
				noError.border = new RectOffset(3, 3, 3, 3);
				noError.normal = new GUIStyleState();
			}
			
			return noError;
		}
	}
	
	public static Texture2D PointCursor
	{
		get
		{
			if (pointCursor == null)
			{
				pointCursor = CreateTempTexture(21, 21, "iVBORw0KGgoAAAANSUhEUgAAABUAAAAVCAYAAACpF6WWAAAAWElEQVQ4EWP8//8/A7UBEwkGNhCrlhRDiTWTYdRQooOKaIU0CVPGgU6nRHufBaqygYAOZHlkNjZtDUPH+6TEPiFvw4OCFEPhmggxRg0lFEKky9MkTGmS+AGWSQ8dYRvKHwAAAABJRU5ErkJggg==");
			}
			
			return pointCursor;
		}
	}
	
	public static void SetDirty(Object target)
	{
		UnityEditor.EditorUtility.SetDirty(target);

#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		UnityEditor.EditorApplication.MarkSceneDirty();
#else
		UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
#endif
	}

	public static GameObject CreatePrefab(string path, GameObject source, bool destroy = true)
	{
		var prefab = PrefabUtility.CreatePrefab(path, source);
		
		if (destroy == true)
		{
			Destroy(source);
		}
		
		return prefab;
	}
	
	public static void ClearSelection()
	{
		Selection.objects = new Object[0];
	}
	
	public static void AddToSelection(Object o)
	{
		var os = new List<Object>(Selection.objects);
		
		os.Add(o);
		
		Selection.objects = os.ToArray();
	}
	
	public static void AddToSelectionAndPing(Object o)
	{
		AddToSelection(o);
		
		EditorApplication.delayCall += () => EditorGUIUtility.PingObject(o);
	}
	
	public static string GetAssetDirectory(Object o)
	{
		var dir = AssetDatabase.GetAssetPath(o);
		
		if (string.IsNullOrEmpty(dir) == false && System.IO.Directory.Exists(dir) == false)
		{
			dir = dir.Substring(0, dir.LastIndexOf('/'));
		}
		
		return dir;
	}
	
	public static T GetAssetImporter<T>(Object asset)
		where T : AssetImporter
	{
		return GetAssetImporter<T>((AssetDatabase.GetAssetPath(asset)));
	}
	
	public static T GetAssetImporter<T>(string path)
		where T : AssetImporter
	{
		return AssetImporter.GetAtPath(path) as T;
	}
	
	public static string SaveDialog(string title, Object directory, string defaultName, string extension)
	{
		var dir = GetAssetDirectory(directory); if (string.IsNullOrEmpty(dir) == true) dir = "Assets";
		
		return SaveDialog(title, dir, defaultName, extension);
	}
	
	public static string SaveDialog(string title, string directory, string defaultName, string extension)
	{
		var path = EditorUtility.SaveFilePanel(title, directory, defaultName, extension);
		
		if (path.StartsWith(Application.dataPath) == true)
		{
			path = "Assets" + path.Substring(Application.dataPath.Length);
		}
		
		return path;
	}
	
	public static void ReimportAsset(Object asset)
	{
		ReimportAsset(AssetDatabase.GetAssetPath(asset));
	}
	
	public static void ReimportAsset(string path)
	{
		AssetDatabase.ImportAsset(path);
	}
	
	public static bool IsAsset(Object o)
	{
		return o != null && string.IsNullOrEmpty(UnityEditor.AssetDatabase.GetAssetPath(o)) == false;
	}
	
	public static void MakeTextureReadable(Texture2D texture, bool makeTruecolor = false)
	{
		if (texture != null)
		{
			var importer = GetAssetImporter<TextureImporter>(texture);
			
			if (importer != null && importer.isReadable == false)
			{
				importer.isReadable = true;
				
				if (makeTruecolor == true)
				{
					importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
				}
				
				ReimportAsset(importer.assetPath);
			}
		}
	}

	private static string[] squareNames = { "8", "16", "32", "64", "128", "256", "512", "1024", "2048", "4096", "8192" };

	private static int[] squareValues = { 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 };

	public static int DrawSize(Rect rect, int size)
	{
		return EditorGUI.IntPopup(rect, "", size, squareNames, squareValues);
	}
	
	public static T GetIndexOrDefault<T>(T[] array, int index)
	{
		if (array != null && index >= 0 && index < array.Length)
		{
			return array[index];
		}
		
		return default(T);
	}
	
	public static T GetIndexOrDefault<T>(T[] array, ref int index)
	{
		if (array != null && index >= 0 && index < array.Length)
		{
			return array[index];
		}
		
		index = -1;
		
		return default(T);
	}
	
	public static Texture2D CreateTempTexture(int width, int height, string encoded)
	{
		var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
		
		texture.hideFlags = HideFlags.HideAndDontSave;
		texture.LoadImage(System.Convert.FromBase64String(encoded));
		texture.Apply();
		
		return texture;
	}
	
	public static Rect Reserve(float height = 16.0f, bool indent = false)
	{
		var rect = default(Rect);
		
		EditorGUILayout.BeginVertical(NoError);
		{
			rect = EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.LabelField(string.Empty, GUILayout.Height(height), GUILayout.ExpandWidth(true), GUILayout.MinWidth(0.0f));
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndVertical();
		
		if (BaseRectSet == true)
		{
			rect.xMin = BaseRect.xMin;
			rect.xMax = BaseRect.xMax;
		}
		
		if (indent == true)
		{
			rect = EditorGUI.IndentedRect(rect);
		}
		
		return rect;
	}
	
	public static Texture[] GetMaterialTextures(Material material, string[] textureNames)
	{
		var textures = new Texture[textureNames.Length];
		
		for (var i = textureNames.Length - 1; i >= 0; i--)
		{
			textures[i] = material.GetTexture(textureNames[i]);
		}
		
		return textures;
	}
	
	public static string[] GetTexEnvNames(Material material)
	{
		var texEnvNames = new List<string>();
		
		if (material != null)
		{
			var shader = material.shader;
			
			if (shader != null)
			{
				var count = ShaderUtil.GetPropertyCount(shader);
				
				for (var i = 0; i < count; i++)
				{
					if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
					{
						texEnvNames.Add(ShaderUtil.GetPropertyName(shader, i));
					}
				}
			}
		}
		
		return texEnvNames.ToArray();
	}
	
	public static Texture[] CopyTextures(Material material)
	{
		var texEnvNames = GetTexEnvNames(material);
		var textures    = new Texture[texEnvNames.Length];
		
		for (var i = texEnvNames.Length - 1; i >= 0; i--)
		{
			textures[i] = material.GetTexture(texEnvNames[i]);
		}
		
		return textures;
	}
	
	public static void PasteTextures(Material material, Texture[] textures)
	{
		var texEnvNames = GetTexEnvNames(material);
		
		for (var i = texEnvNames.Length - 1; i >= 0; i--)
		{
			material.SetTexture(texEnvNames[i], textures[i]);
		}
	}
	
	public static void SaveTextureAsset(Texture2D texture, string path, bool overwrite = false)
	{
		var bytes = texture.EncodeToPNG();
		var fs    = new System.IO.FileStream(path, overwrite == true ? System.IO.FileMode.Create : System.IO.FileMode.CreateNew);
		var bw    = new System.IO.BinaryWriter(fs);
		
		bw.Write(bytes);
		
		bw.Close();
		fs.Close();
		
		ReimportAsset(path);
	}
}
#endif