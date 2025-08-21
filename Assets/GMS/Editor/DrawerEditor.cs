using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMS.Editor
{
    /// <summary>
    /// Util class for drawing GUI
    /// </summary>
    public static  class DrawerEditor
    {
        public static void GetGUIBoxStyle(Color color, out  GUIStyle boxStyle)
        {
            boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.padding = new RectOffset(10, 10, 10, 10);
            boxStyle.margin = new RectOffset(0, 0, 10, 10);
            boxStyle.normal.background = MakeTexture(1, 1, color);
        }
        
        public static Texture2D MakeTexture(int width, int height, Color color)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = color;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
        
        public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
            Color[] res = result.GetPixels(0);
            float incX = (1.0f / targetWidth);
            float incY = (1.0f / targetHeight);
            for (int px = 0; px < res.Length; px++)
            {
                res[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
            }
            result.SetPixels(res, 0);
            result.Apply();
            return result;
        }
    }
}

