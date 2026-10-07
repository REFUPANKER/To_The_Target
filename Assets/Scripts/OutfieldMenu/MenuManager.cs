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
    [SerializeField] UIDocument settingsUi;
    [SerializeField] WeaponPicker weaponPicker;

    [Header("Movement Settings")]
    public bool CanMove = false;
    [SerializeField] Transform playerBody;
    [SerializeField] CharacterController controller;
    [SerializeField] CinemachineCamera camFPS, camSettings;
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
        InitMenuEvents();
        CanMove = false;
    }

    void Update()
    {
        if (isTrainingActive)
        {
            Move();
        }
    }

    void InitMenuEvents()
    {
        var root = uiDocument.rootVisualElement;

        var trainingButton = root.Q<Button>("training");
        trainingButton.clicked += OnTrainingButtonClicked;

        var settingsButton = root.Q<Button>("settings");
        settingsButton.clicked += OnSettingsButtonClicked;

        var settingsRoot = settingsUi.rootVisualElement;
        var backButton = settingsRoot.Q<Button>("settings_back");

        backButton.clicked += () => TurnBackFrom("Settings");
    }

    void OnSettingsButtonClicked()
    {
        camSettings.Priority += 2;
        settingsUi.rootVisualElement.visible = true;
        uiDocument.rootVisualElement.visible = false;
        TogglePause(true);
    }

    public void TurnBackFrom(string menuName)
    {
        switch (menuName)
        {
            case "Settings":
                camSettings.Priority -= 2;
                settingsUi.rootVisualElement.visible = false;
                break;
            case "Training":
                camFPS.Priority -= 2;
                trainingUi.SetActive(false);
                isTrainingActive = false;
                CanMove = false;
                weaponPicker.CanUse = false;
                break;
        }
        uiDocument.rootVisualElement.visible = true;
        TogglePause(true);
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
        uiDocument.rootVisualElement.visible = false;
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
        if (!CanMove || !isTrainingActive || weaponPicker.isInWeaponSpot) { return; }

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
