using UnityEngine;
 
public class ScreenCapture
{
	static Texture2D tex;

	public static Texture2D Capture() {
		return Capture(new Rect(0f,0f, Screen.width,Screen.height), 0,0);
	}

	public static Texture2D Capture(Rect source, int destX, int destY) {
		int texWidth = destX + Mathf.RoundToInt(source.width);
		int texHeight = destY + Mathf.RoundToInt(source.height);
		if (tex == null) {
			tex = new Texture2D(texWidth, texHeight, TextureFormat.RGB24, false);
		} else {
			if ((tex.width != texWidth) || (tex.height != texHeight)) {
				tex.Resize(texWidth, texHeight);
			}
		}
		tex.ReadPixels(source, destX, destY, false);
		tex.Apply();
		return tex;
	}
}
