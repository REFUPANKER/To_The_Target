using System.Collections.Generic;
using UnityEngine;

public class GameStateController : MonoBehaviour
{
    public bool isPaused = false;
    [SerializeField] List<GameObject> objsDisableEnable;

    void Start()
    {
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
        isPaused = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        switchComps(false);
    }

    public void ResumeGame()
    {
        switchComps(true);
        isPaused = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}