using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using DG.Tweening;

public class Paper : MonoBehaviour
{
    [SerializeField] SpriteRenderer photoImage;
    [SerializeField] private Transform frame;
    [SerializeField] private Transform tray;
    [SerializeField] private Grabbable grabbable;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private Collider myCollider;
    [SerializeField] private float threshold;

    private bool isGrabbed = false;
    private bool doOnceCheckDistanceToFrame = true;
    private bool doOnceMakeImageAppearCompletely = true;
    private bool isBeingProcessed = false;
    float coolDownTime = 2f;


    private void Update()
    {
        if (!isGrabbed) return;
        if (!doOnceCheckDistanceToFrame) return;
        if (Vector3.Distance(frame.position, transform.position) > threshold) return;

        isBeingProcessed = true;
        isGrabbed = false;
        doOnceCheckDistanceToFrame = false;
        grabbable.enabled = false;
        rigidBody.isKinematic = true;
        myCollider.enabled = false;
        transform.parent = frame;
        transform.DOMove(frame.position, 2f);
        transform.DORotate(frame.eulerAngles, 2f).OnComplete(() =>
        {
            isBeingProcessed = false;
            grabbable.enabled = true;
            myCollider.enabled = true;
        });




    }


    public void UserGrabbedPaper(bool value)
    {
        isGrabbed = value;
        if (value)
            transform.parent = null;

        else if (!isBeingProcessed)
            rigidBody.isKinematic = false;

    }


    public void ImageFadeIn(float value)
    {
        photoImage.DOFade(value, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!doOnceMakeImageAppearCompletely) return;
        if (other.tag != "Tray") return;

        other.TryGetComponent<Tray>(out var tray);

        if (tray)
        {
            if (tray.TrayIndex == 1)
            {
                photoImage.DOFade(1f, 2f).OnComplete(() => tray.PaperPlaced());
                doOnceMakeImageAppearCompletely = false;
            }
        }
    }
}
