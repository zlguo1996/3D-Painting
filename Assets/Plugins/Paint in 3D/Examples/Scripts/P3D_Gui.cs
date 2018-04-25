using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_Gui))]
public class P3D_Gui_Editor : P3D_Editor<P3D_Gui>
{
	protected override void OnInspector()
	{
		DrawDefault("Header");
		DrawDefault("Footer");
	}
}
#endif

// This component shows a simple GUI with FPS
[ExecuteInEditMode]
[DisallowMultipleComponent]
[AddComponentMenu(P3D_Helper.ComponentMenuPrefix + "GUI")]
public class P3D_Gui : MonoBehaviour
{
	[Multiline]
	public string Header;

	[Multiline]
	public string Footer;

	private float counter;

	private int frames;

	private float fps;

	private static GUIStyle whiteStyle;

	private static GUIStyle blackStyle;

	protected virtual void Update()
	{
		counter += Time.deltaTime;
		frames += 1;

		if (counter >= 1.0f)
		{
			fps = (float)frames / counter;

			counter = 0.0f;
			frames = 0;
		}
	}

	protected virtual void OnGUI()
	{
		var r0 = new Rect(5, 50 + 55 * 0, 100, 50);
		var r1 = new Rect(5, 50 + 55 * 1, 100, 50);
		var r2 = new Rect(5, 50 + 55 * 2, 100, 50);
		
		if (GUI.Button(r0, "Reload") == true)
		{
			LoadLevel(GetCurrentLevel());
		}

		if (GUI.Button(r1, "Prev") == true)
		{
			var index = GetCurrentLevel() - 1;

			if (index < 0)
			{
				index = GetLevelCount() - 1;
			}

			LoadLevel(index);
		}

		if (GUI.Button(r2, "Next") == true)
		{
			var index = GetCurrentLevel() + 1;

			if (index >= GetLevelCount())
			{
				index = 0;
			}

			LoadLevel(index);
		}

		// Draw FPS?
		if (fps > 0.0f)
		{
			DrawText("FPS: " + fps.ToString("0"), TextAnchor.UpperLeft);
		}

		// Draw header?
		if (string.IsNullOrEmpty(Header) == false)
		{
			DrawText(Header, TextAnchor.UpperCenter, 150);
		}

		// Draw footer?
		if (string.IsNullOrEmpty(Footer) == false)
		{
			DrawText(Footer, TextAnchor.LowerCenter, 150);
		}
	}

#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
	private static int GetCurrentLevel()
	{
		return Application.loadedLevel;
	}

	private static int GetLevelCount()
	{
		return Application.levelCount;
	}

	private static void LoadLevel(int index)
	{
		Application.LoadLevel(index);
	}
#else
	private static int GetCurrentLevel()
	{
		return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
	}

	private static int GetLevelCount()
	{
		return UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
	}

	private static void LoadLevel(int index)
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(index);
	}
#endif

	public static void DrawText(string text, TextAnchor anchor, int offsetX = 15, int offsetY = 15)
	{
		if (string.IsNullOrEmpty(text) == false)
		{
			if (whiteStyle == null || blackStyle == null)
			{
				whiteStyle = new GUIStyle();
				whiteStyle.fontSize = 20;
				whiteStyle.fontStyle = FontStyle.Bold;
				whiteStyle.wordWrap = true;
				whiteStyle.normal = new GUIStyleState();
				whiteStyle.normal.textColor = Color.white;

				blackStyle = new GUIStyle();
				blackStyle.fontSize = 20;
				blackStyle.fontStyle = FontStyle.Bold;
				blackStyle.wordWrap = true;
				blackStyle.normal = new GUIStyleState();
				blackStyle.normal.textColor = Color.black;
			}

			whiteStyle.alignment = anchor;
			blackStyle.alignment = anchor;

			var sw   = (float)Screen.width;
			var sh   = (float)Screen.height;
			var rect = new Rect(0, 0, sw, sh);

			rect.xMin += offsetX;
			rect.xMax -= offsetX;
			rect.yMin += offsetY;
			rect.yMax -= offsetY;

			rect.x += 1;
			GUI.Label(rect, text, blackStyle);

			rect.x -= 2;
			GUI.Label(rect, text, blackStyle);

			rect.x += 1;
			rect.y += 1;
			GUI.Label(rect, text, blackStyle);

			rect.y -= 2;
			GUI.Label(rect, text, blackStyle);

			rect.y += 1;
			GUI.Label(rect, text, whiteStyle);
		}
	}
}
