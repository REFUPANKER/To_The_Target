using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public class GameSettings
{
    public float mouseSensitivity = 100f;
    public float aimingSens = 15f;
    public float sniperSens = 15f;
    public float jumpHeight = 15f;
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public int qualityLevel = 2;
    public bool fullscreen = true;
}

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public GameSettings CurrentSettings { get; private set; } = new GameSettings();
    public GameSettings WorkingSettings { get; private set; } = new GameSettings();

    private string jsonFilePath;

    public static event Action<GameSettings> OnSettingsChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        jsonFilePath = Path.Combine(Application.persistentDataPath, "settings.json");
        LoadSettings();
    }

    public void LoadSettings()
    {
        try
        {
            if (File.Exists(jsonFilePath))
            {
                string json = File.ReadAllText(jsonFilePath);
                CurrentSettings = JsonUtility.FromJson<GameSettings>(json) ?? new GameSettings();
            }
            else
            {
                CurrentSettings = new GameSettings();
                SaveSettings();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load settings: " + e.Message);
            CurrentSettings = new GameSettings();
        }

        CopySettings(CurrentSettings, WorkingSettings);
    }

    public void SaveSettings()
    {
        try
        {
            CopySettings(WorkingSettings, CurrentSettings);

            string json = JsonUtility.ToJson(CurrentSettings, true);
            File.WriteAllText(jsonFilePath, json);

            OnSettingsChanged?.Invoke(CurrentSettings);
            Debug.Log("Settings successfully saved to disk.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save settings: " + e.Message);
        }
    }

    public void DiscardChanges()
    {
        CopySettings(CurrentSettings, WorkingSettings);
    }

    private void CopySettings(GameSettings source, GameSettings destination)
    {
        destination.mouseSensitivity = source.mouseSensitivity;
        destination.aimingSens = source.aimingSens;
        destination.sniperSens = source.sniperSens;
        destination.jumpHeight = source.jumpHeight;
        destination.masterVolume = source.masterVolume;
        destination.musicVolume = source.musicVolume;
        destination.sfxVolume = source.sfxVolume;
        destination.qualityLevel = source.qualityLevel;
        destination.fullscreen = source.fullscreen;
    }

    // Sadece belleği güncelleyen metodlar (Performans için diske yazmaz)
    public void SetMouseSensitivity(float value) { WorkingSettings.mouseSensitivity = value; }
    public void SetAimingSens(float value) { WorkingSettings.aimingSens = value; }
    public void SetSniperSens(float value) { WorkingSettings.sniperSens = value; }
    public void SetJumpHeight(float value) { WorkingSettings.jumpHeight = value; }
    public void SetMasterVolume(float value) { WorkingSettings.masterVolume = value; }
    public void SetMusicVolume(float value) { WorkingSettings.musicVolume = value; }
    public void SetSfxVolume(float value) { WorkingSettings.sfxVolume = value; }
}
