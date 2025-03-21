using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
   [Header("Movement)")]
   private float moveSpeed;
   public float walkSpeed;
   public float sprintSpeed;

   public float groundDrag;

   [Header("Keybinds")]
   public KeyCode sprintKey = KeyCode.LeftShift;

   [Header("Ground Check")]
   public float playerHeight;
   public LayerMask whatIsGround;
   public bool grounded;

   public Transform orientation;

   float horizontalInput;
   float verticalInput;

   Vector3 moveDirection;

   Rigidbody rb;

    [SerializeField] FMODUnity.EventReference step;
    [SerializeField] FMOD.Studio.EventInstance stepEvent;

public MovementState state;

   public enum MovementState
   {
      walking,
      sprinting
   }

private void Start()
   {
    rb = GetComponent<Rigidbody>();
    rb.freezeRotation = true;
       stepEvent = AudioManager.Instance.PlayLoop(step, transform.position, gameObject);
   }

   private void Update()
   {
    grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
   
    MyInput();
    StateHandler();

      if(grounded)
        rb.drag = groundDrag;
        else
        rb.drag = 0;
        if (rb.velocity == Vector3.zero) stepEvent.setParameterByName("Volume", 0);
        else stepEvent.setParameterByName("Volume", 1);

   }

   private void FixedUpdate()
   {
    MovePlayer();

   }
   private void MyInput()
   {
    horizontalInput = Input.GetAxisRaw("Horizontal");
    verticalInput = Input.GetAxisRaw("Vertical"); 
   }

private void StateHandler()
{
   if(grounded && Input.GetKey(sprintKey))
   {
      state = MovementState.sprinting;
      moveSpeed = sprintSpeed;
            stepEvent.setParameterByName("Sprint", 1);
   }

   else if (grounded)
   {
      state = MovementState.walking;
      moveSpeed = walkSpeed;
            stepEvent.setParameterByName("Sprint", 0);
        }
}
   private void MovePlayer()
   {
    moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

    rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
   }

   }