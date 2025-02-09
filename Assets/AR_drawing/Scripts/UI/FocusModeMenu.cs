using UnityEngine;
using DG.Tweening;
using System;
public class FocusModeMenu : MenuTarget
{
    [SerializeField] OVRPassthroughLayer oVRPassthroughLayer;
    bool isFocusMode = false;
    Tween focusTween, normalTween;

    public static Action<bool> FocusModeNotifier;


    private void Start()
    {
        if (!oVRPassthroughLayer)
            oVRPassthroughLayer = FindAnyObjectByType<OVRPassthroughLayer>();

        // focusTween = DOTween.To(() => oVRPassthroughLayer.textureOpacity, x => oVRPassthroughLayer.textureOpacity = x, 0.1f, 2f).SetAutoKill(false).Pause();
        // normalTween = DOTween.To(() => oVRPassthroughLayer.textureOpacity, x => oVRPassthroughLayer.textureOpacity = x, 1f, 2f).SetAutoKill(false).Pause();
    }

    public override void WhenSelect()
    {
        base.WhenSelect();
        isFocusMode = !isFocusMode;

        FocusModeNotifier?.Invoke(isFocusMode);
        DOTween.Kill(oVRPassthroughLayer.textureOpacity);


        if (isFocusMode)
            DOTween.To(() => oVRPassthroughLayer.textureOpacity, x => oVRPassthroughLayer.textureOpacity = x, 0.1f, 2f);
        else
            DOTween.To(() => oVRPassthroughLayer.textureOpacity, x => oVRPassthroughLayer.textureOpacity = x, 1f, 2f);
    }
}
