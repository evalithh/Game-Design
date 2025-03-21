using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupClass : MonoBehaviour
{ 
        [SerializeField] LayerMask PickupLayer;
        [SerializeField] Camera PlayerCamera;
        [SerializeField] float PickupRange;
        [SerializeField] private Transform Hand;
    
        
        private Rigidbody CurrentObjectRigidBody;
        private Collider CurrentObjectCollider;

    [SerializeField] FMODUnity.EventReference grab;
    [SerializeField] FMODUnity.EventReference ungrab;
    

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
        Ray Pickupray = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);

        if(Physics.Raycast(Pickupray, out RaycastHit hitInfo, PickupRange, PickupLayer))
        {
            if(CurrentObjectRigidBody)
            {
                CurrentObjectRigidBody.isKinematic = false;
                CurrentObjectCollider.enabled = true;

                CurrentObjectRigidBody = hitInfo.rigidbody;
                CurrentObjectCollider = hitInfo.collider; 

                CurrentObjectRigidBody.isKinematic = true;
                CurrentObjectCollider.enabled = false;
                    AudioManager.Instance.PlaySFX(grab, transform.position);

            }
            else
            {
                CurrentObjectRigidBody = hitInfo.rigidbody;
                CurrentObjectCollider = hitInfo.collider;

                CurrentObjectRigidBody.isKinematic = true;
                CurrentObjectCollider.enabled = false;
                    AudioManager.Instance.PlaySFX(ungrab, transform.position);
                }
            }
        }
        
        if(CurrentObjectRigidBody)
        {
            CurrentObjectRigidBody.position = Hand.position;
            CurrentObjectRigidBody.rotation = Hand.rotation;
        }
    }
}
