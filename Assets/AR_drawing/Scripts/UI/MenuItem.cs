using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class MenuItem : MonoBehaviour
{
    private MenuHolder m_menuHolder;
    private float m_menuItemMoveMultiplier;
    private bool m_isActivated = false;
    Vector3 startingScale;

    [SerializeField] private int m_menuItemID;


    private void OnEnable()
    {
        MenuHolder.ActivateMenuNotifier += ActivateMenuListener;
    }


    private void OnDisable()
    {
        MenuHolder.ActivateMenuNotifier -= ActivateMenuListener;

    }

    void Start()
    {
        m_menuHolder = MenuHolder.Instance;
        m_menuItemMoveMultiplier = m_menuHolder.MenuItemMoveMultiplier;
        startingScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isActivated)
        {
            //Subtracting m_menuItemID from 3 since we have 3 elements
            Vector3 targetPosition = m_menuHolder.Target.position +
                                     m_menuHolder.Target.right * (3 - m_menuItemID) * m_menuHolder.MenuItemOffsetMultiplierFromRightAxis +
                                     m_menuHolder.Target.right * m_menuHolder.MenuItemOffsetFromRightAxis;

            float tweenTime = Time.deltaTime * m_menuHolder.MenuItemMoveMultiplier * (3 - m_menuItemID);
            transform.position = Vector3.Lerp(transform.position, targetPosition, tweenTime);
            // transform.LookAt(m_menuHolder.Target.position + m_menuHolder.Target.up * -5f, Vector3.up);
            transform.rotation = m_menuHolder.Target.rotation * Quaternion.Euler(new Vector3(90f, 90f, 0f));
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, m_menuHolder.Source.position, Time.deltaTime * 5f);
        }
    }

    private void ActivateMenuListener(bool value)
    {
        m_isActivated = value;
        DOTween.Kill(transform);
        transform.DOScale(m_isActivated ? startingScale : Vector3.zero, 0.5f);
    }
}
