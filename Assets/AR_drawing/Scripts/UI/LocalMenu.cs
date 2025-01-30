using UnityEngine;
using DG.Tweening;

public class LocalMenu : MenuTarget
{
    Transform guideUI;
    float movementMultiplier = 0.05f;
    float rotateMultiplier = 5f;
    float scaleMultiplier = 0.2f;

    float movementDirection;
    bool rotateClockwise, shouldIncreaseScale;

    bool shouldMove, shouldRotate, shouldScale;


    override public void WhenSelect()
    {
        if (!guideUI) guideUI = FurnitureManager.Instance.guideUI.transform;
        base.WhenSelect();
        Debug.Log("Yeet");
        DOTween.Kill(transform);


        //If the guide UI is still not set, return
        if (!guideUI) return;
        {
            transform.position = guideUI.transform.position + Vector3.up * 0.5f;

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
                        guideUI.position = guideUI.position + guideUI.up * movementMultiplier * Time.deltaTime;
                        break;
                    }
                case 1:
                    {
                        guideUI.position = guideUI.position + guideUI.up * -movementMultiplier * Time.deltaTime;
                        break;
                    }
                case 2:
                    {
                        guideUI.position = guideUI.position + guideUI.right * -movementMultiplier * Time.deltaTime;
                        break;
                    }
                case 3:
                    {
                        guideUI.position = guideUI.position + guideUI.right * movementMultiplier * Time.deltaTime;
                        break;
                    }
                case 4:
                    {
                        guideUI.position = guideUI.position + guideUI.forward * -movementMultiplier * Time.deltaTime;
                        break;
                    }
                case 5:
                    {
                        guideUI.position = guideUI.position + guideUI.forward * movementMultiplier * Time.deltaTime;
                        break;
                    }
                default: break;
            }

        }
        else if (shouldRotate)
        {
            guideUI.rotation = Quaternion.Euler(guideUI.rotation.eulerAngles + (Vector3.up * rotateMultiplier * (rotateClockwise ? 1f : -1f)) * Time.deltaTime);
        }
        else if (shouldScale)
        {
            guideUI.localScale = guideUI.localScale + (Vector3.one * scaleMultiplier * (shouldIncreaseScale ? 1f : -1f) * Time.deltaTime);
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


}