using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class MenuTarget : MonoBehaviour
{
    public OVRPassthroughLayer oVRPassthroughLayer;
    public MenuItem MenuItem;

    private void Start()
    {
        // transform.localScale = Vector3.zero;
    }


    public void WhenSelect()
    {
        Debug.Log("Yeet");
        // isActive = !isActive;
        DOTween.Kill(transform);
        // transform.DOScale(isActive ? 1f : 0f, 1f);
        if (FurnitureManager.Instance.guideUI)
        {
            transform.position = FurnitureManager.Instance.guideUI.transform.position + Vector3.up * 0.5f;
            //    transform.LookAt(Camera.main.transform);

            // Get the direction towards the camera
            Vector3 directionToCamera = Camera.main.transform.position - transform.position;

            // Flip the direction by inverting the forward vector
            Vector3 flippedDirection = -directionToCamera;

            // Make the transform look at the flipped direction
            transform.rotation = Quaternion.LookRotation(flippedDirection, Vector3.up);
        }
    }

    public void OnPassthroughValueChanged(float value)
    {
        oVRPassthroughLayer.textureOpacity = value;
    }

}
