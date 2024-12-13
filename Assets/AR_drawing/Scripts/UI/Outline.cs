using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Meta.XR.MRUtilityKit.SceneDecorator;


// [ExecuteInEditMode]
public class Outline : MonoBehaviour
{
    public RectTransform MyRectTransform;
    public RectTransform ChildRectTransform;



    private void Start()
    {
        //Set to the same size as target RectTransform
        MyRectTransform.sizeDelta = ChildRectTransform.sizeDelta;
    }

    // private void Update()
    // {
    //     MyRectTransform.sizeDelta = ChildRectTransform.sizeDelta + ChildRectTransform.sizeDelta / 20f;

    // }


    public void OnHover(bool value)
    {

        Debug.Log("hover " + value);
        DOTween.Kill(MyRectTransform);

        //If we are hovering, add border effect else scale down to target scale
        if (value)
            MyRectTransform.DOSizeDelta(ChildRectTransform.sizeDelta + ChildRectTransform.sizeDelta / 40f, 1f);
        else
            MyRectTransform.DOSizeDelta(ChildRectTransform.sizeDelta, 1f);

    }
}
