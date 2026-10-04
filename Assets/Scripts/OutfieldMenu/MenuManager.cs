using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Cinemachine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;
    [SerializeField] Transform playerPosition;
    [SerializeField] GameObject trainingUi;
    [SerializeField] WeaponPicker weaponPicker;

    [Header("Movement Settings")]
    public bool CanMove = false;
    [SerializeField] Transform playerBody;
    [SerializeField] CharacterController controller;
    [SerializeField] CinemachineCamera camFPS;
    [SerializeField] Animator anims;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float mouseSensitivity = 100f;
    [SerializeField] float clampMin = -70f;
    [SerializeField] float clampMax = 70f;
    private float gravity = -19.62f;

    Vector3 moveDirection;
    bool isGrounded;
    float rotx;

    bool isTrainingActive = false;

    void Start()
    {
        var root = uiDocument.rootVisualElement;
        var trainingButton = root.Q<Button>("training");
        trainingButton.clicked += OnTrainingButtonClicked;

        CanMove = false;
    }

    void Update()
    {
        if (isTrainingActive)
        {
            Move();
        }
    }

    void OnTrainingButtonClicked()
    {
        camFPS.Priority += 2;
        trainingUi.SetActive(true);
        isTrainingActive = true;
        CanMove = true;
        controller.enabled = false;
        playerBody.SetPositionAndRotation(playerPosition.position, playerPosition.rotation);
        controller.enabled = true;
        weaponPicker.CanUse = true;
        uiDocument.gameObject.SetActive(false);
        TogglePause(false);
    }

    void TogglePause(bool pause)
    {
        if (pause)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
    }

    void Move()
    {
        if (!CanMove && !isTrainingActive && weaponPicker.isInWeaponSpot) { return; }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        rotx -= mouseY;
        rotx = Mathf.Clamp(rotx, clampMin, clampMax);
        camFPS.transform.localRotation = Quaternion.Euler(rotx, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        isGrounded = controller.isGrounded;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = playerBody.right * x + playerBody.forward * z;
        if (move.magnitude > 1f) { move.Normalize(); }

        moveDirection.x = move.x * moveSpeed;
        moveDirection.z = move.z * moveSpeed;

        if (isGrounded) { moveDirection.y = -2f; } else { moveDirection.y += gravity * Time.deltaTime; }

        controller.Move(moveDirection * Time.deltaTime);
        anims.SetFloat("velocity", controller.velocity.magnitude);
    }
}
