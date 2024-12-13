using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiquidVolumeFX;

public class Bowl : MonoBehaviour
{
    [SerializeField] private LiquidVolume lv;
    [SerializeField] private float totalFillThreshold;
    List<Color> liquidColours;
    void Start()
    {
        liquidColours = new List<Color>();
        if (!lv) lv = GetComponentInChildren<LiquidVolume>();
        lv.level = 0f;
        lv.alpha = 0f;
    }

    public void AddLiquid()
    {
        if (lv.level >= totalFillThreshold) return;

        lv.alpha = 1f;
        lv.level += 0.05f;
    }



    Color32 AverageColorFromTexture(Texture2D tex)
    {

        Color32[] texColors = tex.GetPixels32();

        int total = texColors.Length;

        float r = 0;
        float g = 0;
        float b = 0;

        for (int i = 0; i < total; i++)
        {

            r += texColors[i].r;

            g += texColors[i].g;

            b += texColors[i].b;

        }

        return new Color32((byte)(r / total), (byte)(g / total), (byte)(b / total), 0);

    }
}
