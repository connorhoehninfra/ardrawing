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
}
