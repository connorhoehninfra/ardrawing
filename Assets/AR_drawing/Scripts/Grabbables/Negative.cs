using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using DG.Tweening;

public class Negative : MonoBehaviour
{
    [SerializeField] private Transform frame;
    [SerializeField] private Grabbable grabbable;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private Collider myCollider;
    [SerializeField] private float threshold;

    private bool isGrabbed = false, doOnce = true;
    private bool isBeingProcessed = false;




    private void Update()
    {
        if (!isGrabbed) return;
        if (!doOnce) return;
        if (Vector3.Distance(frame.position, transform.position) > threshold) return;

        isBeingProcessed = true;
        doOnce = false;
        grabbable.enabled = false;
        rigidBody.isKinematic = true;
        myCollider.enabled = false;
        transform.parent = frame;
        transform.DOMove(frame.position, 2f);
        transform.DORotate(frame.eulerAngles, 2f).OnComplete(() =>
        {
            isBeingProcessed = false;
        });

    }


    public void UserGrabbedNegative(bool value)
    {
        isGrabbed = value;
    }


}
