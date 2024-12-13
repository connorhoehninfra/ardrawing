using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CanvasHandler : MonoBehaviour
{
    public OVRPassthroughLayer oVRPassthroughLayer;
    public GameObject table;
    public GameObject cubePrefab;
    GameObject lastCube;
    bool isRotating = false;
    bool isScaled = false;



    public void OnPassthroughValueChanged(float value)
    {
        oVRPassthroughLayer.textureOpacity = value;
    }

    public void SpawnCube()
    {
        if (lastCube) Destroy(lastCube.gameObject);

        lastCube = Instantiate(cubePrefab, table.transform.position + Vector3.up * 1f, Quaternion.identity);
    }

    public void ToggleRotateCube()
    {
        if (!lastCube) return;

        if (isRotating)
            DOTween.Kill(lastCube.transform);
        else
            lastCube.transform.DORotate(Vector3.up * 360f, 1f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);

        isRotating = !isRotating;
    }

    public void ToggleScaleCube()
    {
        if (!lastCube) return;

        lastCube.transform.DOScale(isScaled ? 1f : 2f, 1f);
        isScaled = !isScaled;
    }
}
