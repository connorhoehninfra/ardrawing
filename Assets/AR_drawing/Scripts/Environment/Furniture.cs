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
    PointableUnityEventWrapper pointableUnityEventWrapper;
    bool isSelected = false;


    void Start()
    {
        furnitureMesh = transform.parent.GetComponentInChildren<MeshRenderer>();
        furnitureMaterial = furnitureMesh.materials[0];
    }


    public void WhenHover(PointerEvent arg0)
    {
        // if (isSelected) return;
        // DOTween.Kill(furnitureMaterial);
        // furnitureMaterial.DOFade(hoverAlpha, 0.5f);
    }


    public void WhenUnHover(PointerEvent arg0)
    {
        // if (isSelected) return;
        // DOTween.Kill(furnitureMaterial);
        // furnitureMaterial.DOFade(defaultAlpha, 0.5f);
    }

    public void WhenSelect(PointerEvent arg0)
    {
        // if (isSelected) return;
        // isSelected = true;
        // DOTween.Kill(furnitureMaterial);
        // furnitureMaterial.DOFade(1, 0.5f);
        FurnitureManager.Instance.RegisterAsSelected(this, arg0.Pose.position, arg0.Pose.rotation);
    }

    public void DeSelect()
    {
        // isSelected = false;
        // DOTween.Kill(furnitureMaterial);
        // furnitureMaterial.DOFade(defaultAlpha, 0.5f);
    }


    public void WhenUnselect(PointerEvent arg0)
    {
        // DOTween.Kill(furnitureMaterial);
        // furnitureMaterial.DOFade(defaultAlpha, 0.5f);
    }
}
