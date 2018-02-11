using UnityEngine;
using System;
using System.Collections;

public static class ImageProcess
{

    /// <summary>
    /// Applies sepia effect to the texture.
    /// </summary>
    /// <param name="t"> Texture that is processed.</param>
    public static Texture2D SetSepia(Texture2D t)
    {
        Texture2D tex_ = new Texture2D(t.width, t.height, TextureFormat.ARGB32, true);
        Color[] colors = t.GetPixels();

        for (int i = 0; i < colors.Length; i++)
        {
            float alpha = colors[i].a;
            float grayScale = ((colors[i].r * .299f) + (colors[i].g * .587f) + (colors[i].b * .114f));
            Color c = new Color(grayScale, grayScale, grayScale);
            colors[i] = new Color(c.r * 1, c.g * 0.95f, c.b * 0.82f, alpha);
        }
        tex_.SetPixels(colors);
        tex_.Apply();
        return tex_;
    }

    /// <summary>
    /// Applies grayscale effect to the texture and changes colors to grayscale.
    /// </summary>
    /// <param name="t"> Texture that is processed.</param>
    public static Texture2D SetGrayscale(Texture2D t)
    {
        Texture2D tex_ = new Texture2D(t.width, t.height, TextureFormat.ARGB32, true);
        Color[] colors = t.GetPixels();
        for (int i = 0; i < colors.Length; i++)
            colors[i] = new Color((colors[i].r + colors[i].g + colors[i].b) / 3, (colors[i].r + colors[i].g + colors[i].b) / 3, (colors[i].r + colors[i].g + colors[i].b) / 3);
        
        tex_.SetPixels(colors);
        tex_.Apply();
        return tex_;
    }

    /// <summary>
    /// Pixelates the texture.
    /// </summary>
    /// <param name="t"> Texture that is processed.</param>
    /// <param name="size"> Size of the pixel.</param>
    public static Texture2D SetPixelate(Texture2D t, int size)
    {
        Texture2D tex_ = new Texture2D(t.width, t.height, TextureFormat.ARGB32, true);
        Rect rectangle = new Rect(0, 0, t.width, t.height);
        for (int xx = (int)rectangle.x; xx < rectangle.x + rectangle.width && xx < t.width; xx += size)
        {
            for (int yy = (int)rectangle.y; yy < rectangle.y + rectangle.height && yy < t.height; yy += size)
            {
                int offsetX = size / 2;
                int offsetY = size / 2;
                while (xx + offsetX >= t.width) offsetX--;
                while (yy + offsetY >= t.height) offsetY--;
                Color pixel = t.GetPixel(xx + offsetX, yy + offsetY);
                for (Int32 x = xx; x < xx + size && x < t.width; x++)
                    for (Int32 y = yy; y < yy + size && y < t.height; y++)
                        tex_.SetPixel(x, y, pixel);
            }
        }

        tex_.Apply();
        return tex_;
    }
	
    /// <summary>
    /// Inverts colors of the texture.
    /// </summary>
    /// <param name="t"> Texture that is processed.</param>
    public static Texture2D SetNegative(Texture2D t)
    {
        Texture2D tex_ = new Texture2D(t.width, t.height, TextureFormat.ARGB32, true);
        Color[] colors = t.GetPixels();
        Color pixel;

        for (int i = 0; i < colors.Length; i++)
        {
            pixel = colors[i];
            colors[i] = new Color(1 - pixel.r, 1 - pixel.g, 1 - pixel.b);
        }
        tex_.SetPixels(colors);
        tex_.Apply();
        return tex_;
    }
}

