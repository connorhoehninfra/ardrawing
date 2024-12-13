using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brush : MonoBehaviour
{
    private bool isGrabbed = false, doOnce = true;
    [SerializeField] private MeshRenderer paintBrushRenderer;
    [SerializeField] private Color paintColor;
    [SerializeField] private Transform bowl;


    private void Update()
    {
        if (!isGrabbed) return;
        if (!doOnce) return;
        if (Vector3.Distance(bowl.position, transform.position) > 0.1f) return;

        paintBrushRenderer.materials[0].color = paintColor;
        doOnce = false;


    }


    public void UserGrabbedBrush(bool value)
    {
        isGrabbed = value;
    }



}
