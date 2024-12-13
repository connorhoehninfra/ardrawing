using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiquidVolumeFX;
public class DropperCap : MonoBehaviour
{

    private bool isGrabbed = false;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private LiquidVolume lv;
    [SerializeField] private GameObject spillPrefab;
    [SerializeField] private Transform spillPoint;
    private bool isRightHand;

    private void Start()
    {
        if (!rigidbody) rigidbody = GetComponent<Rigidbody>();
        if (!lv) lv = GetComponentInChildren<LiquidVolume>();
    }

    private void Update()
    {
        if (!isGrabbed) return;

        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
            ReleaseDrop();
    }

    public void UserGrabbedDropperCap(bool value)
    {
        isGrabbed = value;

        if (isGrabbed)
            transform.parent = null;
        else
            rigidbody.isKinematic = false;

    }


    public void ReleaseDrop()
    {

        Debug.Log("Spawned drop");
        GameObject oneSpill = Instantiate(spillPrefab, spillPoint.position, Quaternion.identity) as GameObject;
        oneSpill.GetComponent<Renderer>().material.color = lv.liquidColor1;
    }

}
