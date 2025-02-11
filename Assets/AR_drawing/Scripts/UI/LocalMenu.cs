using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;

public class LocalMenu : MenuTarget
{
    [SerializeField] Slider timeSlider;
    [SerializeField] GameObject markerPrefab;
    Transform guideUITransform;
    GuideHandler guideHandler;
    float movementMultiplier = 0.05f;
    float rotateMultiplier = 5f;
    float scaleMultiplier = 0.2f;

    float movementDirection;
    bool rotateClockwise, shouldIncreaseScale;

    bool shouldMove, shouldRotate, shouldScale;
    private Coroutine countDownCo;

    override public void WhenSelect()
    {
        if (!guideUITransform) guideUITransform = FurnitureManager.Instance.guideUI.transform;
        if (!guideHandler) guideUITransform.TryGetComponent<GuideHandler>(out guideHandler);
        DOTween.Kill(transform);


        //If the guide UI is still not set, return
        if (!guideUITransform) return;
        {
            transform.position = guideUITransform.transform.position + Vector3.up * 0.5f;

            // Get the direction towards the camera
            Vector3 directionToCamera = Camera.main.transform.position - transform.position;

            // Flip the direction by inverting the forward vector
            Vector3 flippedDirection = -directionToCamera;

            // Make the transform look at the flipped direction
            transform.rotation = Quaternion.LookRotation(flippedDirection, Vector3.up);
        }
    }

    private void Update()
    {
        if (shouldMove)
        {
            switch (movementDirection)
            {
                case 0:
                    {
                        guideUITransform.position = guideUITransform.position + guideUITransform.up * movementMultiplier * Time.deltaTime;
                        break;
                    }
                case 1:
                    {
                        guideUITransform.position = guideUITransform.position + guideUITransform.up * -movementMultiplier * Time.deltaTime;
                        break;
                    }
                case 2:
                    {
                        guideUITransform.position = guideUITransform.position + guideUITransform.right * -movementMultiplier * Time.deltaTime;
                        break;
                    }
                case 3:
                    {
                        guideUITransform.position = guideUITransform.position + guideUITransform.right * movementMultiplier * Time.deltaTime;
                        break;
                    }
                case 4:
                    {
                        guideUITransform.position = guideUITransform.position + guideUITransform.forward * -movementMultiplier * Time.deltaTime;
                        break;
                    }
                case 5:
                    {
                        guideUITransform.position = guideUITransform.position + guideUITransform.forward * movementMultiplier * Time.deltaTime;
                        break;
                    }
                default: break;
            }

        }
        else if (shouldRotate)
        {
            guideUITransform.rotation = Quaternion.Euler(guideUITransform.rotation.eulerAngles + (Vector3.up * rotateMultiplier * (rotateClockwise ? 1f : -1f)) * Time.deltaTime);
        }
        else if (shouldScale)
        {
            guideUITransform.localScale = guideUITransform.localScale + (Vector3.one * scaleMultiplier * (shouldIncreaseScale ? 1f : -1f) * Time.deltaTime);
        }
    }


    //0 - forward
    //1 - back
    //2 - left
    //3 - right
    //4 - up
    //5 - down
    public void PointerDownMove(int movementDirection)
    {
        this.movementDirection = movementDirection;
        shouldMove = true;
    }

    public void PointerDownRotate(bool rotateClockwise)
    {
        this.rotateClockwise = rotateClockwise;
        shouldRotate = true;
    }

    public void PointerDownScale(bool shouldIncreaseScale)
    {
        this.shouldIncreaseScale = shouldIncreaseScale;
        shouldScale = true;
    }

    public void PointerUp()
    {
        shouldMove = false;
        shouldRotate = false;
        shouldScale = false;
    }

    public void ConfirmPlacement()
    {
        countDownCo = StartCoroutine(StartTimer());
        Instantiate(markerPrefab, guideUITransform.position, Quaternion.identity);
    }


    IEnumerator StartTimer()
    {
        if (!guideHandler) guideUITransform.TryGetComponent<GuideHandler>(out guideHandler);
        guideHandler.ActivateGuide(true);
        float totalTime = 30f;
        timeSlider.value = 1f;

        float timeLeft = totalTime;
        while (timeLeft > 0f)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            timeLeft -= Time.deltaTime;
            timeSlider.value = 1f - timeLeft / totalTime;
        }

    }

    public void AdjustGuideOpacity(float value)
    {
        guideHandler.SetOpacity(value);

    }



    public void DoneWithDrawing()
    {
        StopCoroutine(countDownCo);
    }



}