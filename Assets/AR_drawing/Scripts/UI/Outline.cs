using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Meta.XR.MRUtilityKit.SceneDecorator;


public class Outline : MonoBehaviour
{
    public RectTransform MyRectTransform;
    public RectTransform ChildRectTransform;

    private void OnEnable()
    {
        FocusModeMenu.FocusModeNotifier += FocusModeListener;
    }

    private void Start()
    {
        //Set to the same size as target RectTransform
        MyRectTransform.sizeDelta = ChildRectTransform.sizeDelta;
    }



    public void FocusModeListener(bool value)
    {

        DOTween.Kill(MyRectTransform);

        // if (value)
        //     MyRectTransform.DOSizeDelta(ChildRectTransform.sizeDelta + ChildRectTransform.sizeDelta / 40f, 1f);
        // else
        //     MyRectTransform.DOSizeDelta(ChildRectTransform.sizeDelta, 1f);


        if (value)
            MyRectTransform.DOScale(1.1f, 2f);
        else
            MyRectTransform.DOScale(1f, 2f);


    }
}
