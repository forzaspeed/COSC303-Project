using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float sprintSpeed = 9.0f;

    [SerializeField] private float jumpPower = 8.0f;
    [SerializeField] private float gravity = 10.0f;

    private Camera MainCamera;
    private CharacterController characterController;

    private InputAction moveInput;
    private InputAction jumpInput;
    private bool jumped = false;
    private InputAction runInput;
    private float currentMoveSpeed = 0.0f;
    private Vector3 moveDirection = Vector3.zero;

    private float lookAngle = 0.0f;

    private float LookSensitivity = 0.2f;
    private void Start()
    {
        MainCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();

        moveInput = InputSystem.actions.FindAction("Move");
        runInput = InputSystem.actions.FindAction("Sprint");
        jumpInput = InputSystem.actions.FindAction("Jump");
        jumpInput.started += Jumped;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentMoveSpeed = walkSpeed;

    }

    // Update is called once per frame
    private void Update()
    {
        Vector2 moveVector = moveInput.ReadValue<Vector2>();
        Vector2 mouseDelta = new Vector2(Mouse.current.delta.x.ReadValue(), Mouse.current.delta.y.ReadValue());
        if (!characterController.isGrounded) jumped = false;
        currentMoveSpeed = runInput.IsPressed() ? sprintSpeed : walkSpeed;


        HandleMovement(moveVector);
        Camera(mouseDelta);
    }

    private void HandleMovement(Vector2 moveVector)
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float oldY = moveDirection.y;

        Vector2 newSpeed = new Vector2(moveVector.y * currentMoveSpeed, moveVector.x * currentMoveSpeed);

        moveDirection = (forward * newSpeed.x) + (right * newSpeed.y);
        if (jumped && characterController.isGrounded)
            moveDirection.y = jumpPower;
        else moveDirection.y = oldY;
        if (!characterController.isGrounded) moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);
    }
    private void Jumped(InputAction.CallbackContext _)
    {
        jumped = true;
    }
    
    private void Camera(Vector2 mouseDelta)
    {
        lookAngle += -mouseDelta.y * LookSensitivity;
        lookAngle = Mathf.Clamp(lookAngle, -90, 90);

        MainCamera.transform.localRotation = Quaternion.Euler(lookAngle, 0, 0);
        transform.rotation *= Quaternion.Euler(0, mouseDelta.x * LookSensitivity, 0);
    }
}
