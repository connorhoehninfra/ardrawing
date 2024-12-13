using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class UVBox : MonoBehaviour
{
    [SerializeField] private Material uvLightMaterial;
    [SerializeField] private GameObject pointLight;
    private bool lightisOn = false;

    private void Start()
    {
        uvLightMaterial.DisableKeyword("_EMISSION");
        uvLightMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
        pointLight.SetActive(false);
    }




    public void OnOpenBox()
    {
        if (lightisOn) return;


        lightisOn = true;
        StartCoroutine(DoWithDelay(() =>
        {
            uvLightMaterial.EnableKeyword("_EMISSION");
            uvLightMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
            pointLight.SetActive(true);
        }, 1f));
    }



    IEnumerator DoWithDelay(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

}
