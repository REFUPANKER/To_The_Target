using System.Collections.Generic;
using UnityEngine;

public class GameStateController : MonoBehaviour
{
    public bool isPaused = false;
    [SerializeField] List<GameObject> objsDisableEnable;
    [SerializeField] UnityEngine.UIElements.UIDocument pauseMenu;
    [SerializeField] SettingsUIController pauseMenuCtrl;
    [SerializeField] Movement player;
    void Start()
    {
        pauseMenu.rootVisualElement.visible = false;
        ResumeGame();
    }

    void switchComps(bool state)
    {
        foreach (var item in objsDisableEnable)
        {
            item.SetActive(state);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        player.PauseResume(true);
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuCtrl.LoadData();
        pauseMenu.rootVisualElement.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        switchComps(false);
    }

    public void ResumeGame()
    {
        player.PauseResume(false);
        switchComps(true);
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenu.rootVisualElement.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}