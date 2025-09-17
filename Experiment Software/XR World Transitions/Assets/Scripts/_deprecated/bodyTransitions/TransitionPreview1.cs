using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransitionPreview : MonoBehaviour
{

    [SerializeField] public Transform playerCamera;
    [SerializeField] public Transform portal; // portal in the destination world
    [SerializeField] public Transform otherPortal; // original world

    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void Update()
    {
        Vector3 playerOffsetFromPortal = playerCamera.position - otherPortal.position;
        transform.position = playerCamera.position + playerOffsetFromPortal;

        float angularDifferenceBetweenPortalRotations = Quaternion.Angle(portal.rotation, otherPortal.rotation);
        Quaternion portalRotationsDifference = Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, Vector3.up);
        Vector3 newCameraDirection = portalRotationsDifference * playerCamera.forward;
        transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);
  
    }
}
