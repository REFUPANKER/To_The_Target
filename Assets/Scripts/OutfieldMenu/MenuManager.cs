using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Cinemachine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;
    [SerializeField] Transform playerPosition;
    [SerializeField] GameObject trainingUi;
    [SerializeField] TextMeshProUGUI bulletCounter;
    [SerializeField] UIDocument settingsUi, trainingSettingsUi;
    [SerializeField] SettingsUIController settingsUiCtrl, trainingUiCtrl;
    [SerializeField] WeaponPicker weaponPicker;

    [Header("Movement Settings")]
    public bool CanMove = false;
    [SerializeField] Transform playerBody;
    [SerializeField] CharacterController controller;
    [SerializeField] CinemachineCamera camFPS, camSettings;
    [SerializeField] Animator anims;
    [SerializeField] float clampMin = -70f;
    [SerializeField] float clampMax = 70f;
    [SerializeField] float moveSpeed = 8, aimingMoveSpeed = 4;
    private float gravity = -19.62f;

    Vector3 moveDirection;
    bool isGrounded, paused;
    float rotx;

    bool isTrainingActive = false;
    private float mouseSens, defaultMoveSpeed;
    void Start()
    {
        mouseSens = SettingsManager.Instance.CurrentSettings.mouseSensitivity;
        defaultMoveSpeed = moveSpeed;
        CanMove = false;
        trainingSettingsUi.rootVisualElement.visible = false;
        InitMenuEvents();
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

        var playButton = root.Q<Button>("play");
        playButton.clicked += () =>
        {
            SceneManager.LoadScene("MainGame");
        };

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
        settingsUiCtrl.LoadData();
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
                camSettings.Priority = 0;
                settingsUi.rootVisualElement.visible = false;
                break;
            case "Training":
                camFPS.Priority = 0;
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
        bulletCounter.text = "";
        isTrainingActive = true;
        CanMove = true;
        controller.enabled = false;
        playerBody.SetPositionAndRotation(playerPosition.position, playerPosition.rotation);
        controller.enabled = true;
        weaponPicker.CanUse = true;
        uiDocument.rootVisualElement.visible = false;
        TogglePause(false);
    }

    public void TogglePause(bool pause, bool ShowSettings = false)
    {
        if (pause)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            weaponPicker.CanUse = false;
            weaponPicker.PauseResume(true);
            if (ShowSettings)
            {
                trainingUiCtrl.LoadData();
                trainingSettingsUi.rootVisualElement.visible = true;
            }
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
            weaponPicker.PauseResume(false);
            trainingSettingsUi.rootVisualElement.visible = false;
        }
        paused = !paused;
    }

    void Move()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause(paused, true);
        }
        if (!paused || !CanMove || !isTrainingActive || weaponPicker.isInWeaponSpot) { return; }

        if (weaponPicker.ActiveGun != null)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                mouseSens = weaponPicker.ActiveGun.aimingSens;
                moveSpeed = aimingMoveSpeed;
            }
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                mouseSens = SettingsManager.Instance.WorkingSettings.mouseSensitivity;
                moveSpeed = defaultMoveSpeed;
            }
        }
        else
        {
            mouseSens = SettingsManager.Instance.WorkingSettings.mouseSensitivity;
        }


        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;
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
