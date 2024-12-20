using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Furniture : MonoBehaviour
{

    MeshRenderer furnitureMesh;
    Material furnitureMaterial;
    float defaultAlpha;
    PointableUnityEventWrapper pointableUnityEventWrapper;
    void Start()
    {
        furnitureMesh = transform.parent.GetComponentInChildren<MeshRenderer>();
        furnitureMaterial = furnitureMesh.materials[0];
        defaultAlpha = furnitureMaterial.color.a;

        // pointableUnityEventWrapper = GetComponent<PointableUnityEventWrapper>();
        // pointableUnityEventWrapper.WhenHover.AddListener(WhenHover);
        // pointableUnityEventWrapper.WhenUnhover.AddListener(WhenUnHover);
        // pointableUnityEventWrapper.WhenSelect.AddListener(WhenSelect);
        // pointableUnityEventWrapper.WhenUnselect.AddListener(WhenUnselect);
    }


    public void WhenHover(PointerEvent arg0)
    {
        DOTween.Kill(furnitureMaterial);
        furnitureMaterial.DOFade(defaultAlpha + 0.2f, 0.5f);
    }


    public void WhenUnHover(PointerEvent arg0)
    {
        DOTween.Kill(furnitureMaterial);
        furnitureMaterial.DOFade(defaultAlpha, 0.5f);
    }

    public void WhenSelect(PointerEvent arg0)
    {
        DOTween.Kill(furnitureMaterial);
        furnitureMaterial.DOFade(0.9f, 0.5f);
    }

    public void WhenUnselect(PointerEvent arg0)
    {
        DOTween.Kill(furnitureMaterial);
        furnitureMaterial.DOFade(defaultAlpha, 0.5f);
    }
}
