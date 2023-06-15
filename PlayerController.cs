using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool Moving { get; private set; } = true;
    private bool isRunning => Running && Input.GetKey(sprintKey);
    private bool isBacking => Input.GetKey(backKey);


    [Header("Functional Options")]
    [SerializeField] private bool Running = true;
    

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode backKey = KeyCode.S;

    [Header("Movement Parameters")]
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private float Runspeed = 6.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 100)] private float lookUpLimit = 80.0f;
    [SerializeField, Range(1, 100)] private float lookDownLimit = 80.0f;


    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;
    private float rotationY = 0;


    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Moving)
        {
            HandleMovementInput();
            MovingBackInput();
            HandleMouseLook();
            ApplyFinalMovemnts();
        }
    }

    private void MovingBackInput()
    {
        if (isBacking)
            Runspeed = 3.0f;
        else
            Runspeed = 6.0f;
   
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2((isRunning ? Runspeed:speed) * Input.GetAxis("Vertical"), (isRunning ? Runspeed : speed) * Input.GetAxis("Horizontal"));
        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -lookUpLimit,lookDownLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    private void ApplyFinalMovemnts()
    {
        if(!characterController.isGrounded)
                moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);
    }
}
