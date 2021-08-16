using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput)) ]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;

    private CharacterController controller;
    PlayerInput playerInput;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    Transform cameraTransform;

    InputAction moveAction;
    InputAction jumpAction;
    InputAction shootAction;
    [SerializeField] float rotationSpeed = 5;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        shootAction = playerInput.actions["Shoot"];
        ToggleCursorVisible();
    }


    void ToggleCursorVisible()
    {
        Cursor.visible = !Cursor.visible;
        Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        shootAction.performed += ShootGun;
    }
    private void OnDisable()
    {
        shootAction.performed -= ShootGun;
    }

    public GameObject bulletPrefab;
    public GameObject barrelTransform;
    public Transform bulletParent;
    private float bulletHitMissDistance = 25f;

    private void ShootGun(InputAction.CallbackContext obj)
    {
        GameObject bullet = Instantiate(bulletPrefab, barrelTransform.transform.position, Quaternion.identity, bulletParent);
        var bulletController = bullet.GetComponent<BulletController>();

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, Mathf.Infinity))
        {
            bulletController.target = hit.point;
            bulletController.hit = true;
        }
        else
        {
            bulletController.target = cameraTransform.position + cameraTransform.forward * bulletHitMissDistance; ;
            bulletController.hit = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ToggleCursorVisible();

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);

        move = move.x * cameraTransform.right + move.z * cameraTransform.forward;
        move.y = 0;

        controller.Move(move * Time.deltaTime * playerSpeed);

        // Changes the height position of the player..
        if (jumpAction.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        Quaternion rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}