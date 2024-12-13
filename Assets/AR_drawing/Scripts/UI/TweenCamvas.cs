
using System;
using Unity.VisualScripting;
using UnityEngine;

public class TweenCamvas : MonoBehaviour
{
    Transform myCamera;
    [SerializeField] private float m_CamvasOffsetDistance;
    [SerializeField] private float m_camvasMoveSpeed;
    [SerializeField] private float m_dotTweenThreshold;

    float m_height;
    void Start()
    {
        myCamera = Camera.main.transform;
        m_height = myCamera.transform.position.y;


        //Set the initial position

        // Position
        var cameraOffset = myCamera.transform.forward * m_CamvasOffsetDistance;
        cameraOffset.y = m_height;
        transform.position = myCamera.transform.position + cameraOffset;

        //Rotation
        var targetDirection = myCamera.transform.position - transform.position;
        targetDirection.y = 0f;
        var targetRotation = Quaternion.LookRotation(-targetDirection, Vector3.up);
        transform.rotation = targetRotation;

    }

    // Update is called once per frame
    void Update()
    {

        //Rotation
        var targetDirection = myCamera.transform.position - transform.position;
        targetDirection.y = 0f;
        var targetRotation = Quaternion.LookRotation(-targetDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime);


        if (Vector3.Dot(myCamera.transform.forward, transform.forward) > m_dotTweenThreshold) return;

        // Position
        var cameraOffset = myCamera.transform.forward * m_CamvasOffsetDistance;
        cameraOffset.y = m_height;
        transform.position = Vector3.Lerp(transform.position, myCamera.transform.position + cameraOffset, Time.deltaTime * m_camvasMoveSpeed);

    }
}
