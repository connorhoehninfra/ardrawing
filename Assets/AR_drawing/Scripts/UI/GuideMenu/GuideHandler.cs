using Meta.XR.ImmersiveDebugger.UserInterface;
using UnityEngine;
using UnityEngine.UI;

public class GuideHandler : MonoBehaviour
{
    [SerializeField] Image guideImage;
    public void ActivateGuide(bool value)
    {
        guideImage.enabled = value;
    }

    public void SetOpacity(float value)
    {
        Color newColor = guideImage.color;
        newColor.a = value;
        guideImage.color = newColor;
    }

}
