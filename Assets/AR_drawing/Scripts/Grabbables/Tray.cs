using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiquidVolumeFX;
using DG.Tweening;
public class Tray : MonoBehaviour
{
    [SerializeField] private LiquidVolume lv;
    [SerializeField] private float totalFillThreshold;
    List<Color> liquidColours;
    bool notifyPaperOnce = true;
    public int TrayIndex = -1;

    void Start()
    {
        liquidColours = new List<Color>();
        if (!lv) lv = GetComponentInChildren<LiquidVolume>();


        //Reset for the first tray
        if (TrayIndex == 0)
        {
            lv.level = 0f;
            lv.alpha = 0f;
        }
    }

    public void AddLiquid()
    {
        //Add liquid only for the first tray
        if (TrayIndex != 0) return;

        if (lv.level >= totalFillThreshold) return;

        lv.alpha = 1f;
        lv.level += 0.0001f;


        //Call the following code only once
        if (!notifyPaperOnce) return;

        //Begin fading the image in when the liquid hits the tray
        FindAnyObjectByType<Paper>().ImageFadeIn(0.5f);
        notifyPaperOnce = false;

    }

    public void PaperPlaced()
    {
        Color targetColor = Color.yellow;
        targetColor.a = 0.3f;
        DOTween.To(() => lv.liquidColor1, x => lv.liquidColor1 = x, targetColor, 2f).SetOptions(false);
    }
}
