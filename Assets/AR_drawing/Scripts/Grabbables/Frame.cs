using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using DG.Tweening;

public class Frame : MonoBehaviour
{

    [SerializeField] private Transform uvBoxAnimationTrigger;
    [SerializeField] private Transform uvBoxTarget;
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
        if (Vector3.Distance(uvBoxAnimationTrigger.position, transform.position) > threshold) return;

        isBeingProcessed = true;
        doOnce = false;
        grabbable.enabled = false;
        rigidBody.isKinematic = true;
        myCollider.enabled = false;
        transform.DOMove(uvBoxTarget.position, 2f);
        transform.DORotate(uvBoxTarget.eulerAngles, 2f).OnComplete(() =>
        {
            isBeingProcessed = false;
            grabbable.enabled = true;
            myCollider.enabled = true;

        });

    }

    public void UserGrabbedBox(bool value)
    {
        isGrabbed = value;
    }




}
