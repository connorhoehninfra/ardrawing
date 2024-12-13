using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHolder : MonoBehaviour
{
    public static MenuHolder Instance;
    public Transform Source;
    public Transform Target;
    public float MenuItemMoveMultiplier;
    public float MenuItemOffsetFromRightAxis;
    public float MenuItemOffsetMultiplierFromRightAxis;
    public static Action<bool> ActivateMenuNotifier;


    private void Awake()
    {
        if (Instance != this && Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }
    public void WhenSelected()
    {
        ActivateMenuNotifier?.Invoke(true);
    }

    public void WhenUnSelected()
    {
        ActivateMenuNotifier?.Invoke(false);
    }
}
