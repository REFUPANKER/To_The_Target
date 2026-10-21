using UnityEngine;
using UnityEngine.UIElements;

public class SettingsUIController : MonoBehaviour
{
    [SerializeField] UIDocument settingsUiDoc;
    [SerializeField] MenuManager mm;
    [SerializeField] GameStateController gameStateCtrl;
    private Slider mouseSensSlider,
        aimingSensSlider,
        sniperSensSlider,
        masterVolSlider,
        sfxVolSlider;
    private Button saveButton,
        closeButton;
    void Start()
    {
        // Root element
        var root = settingsUiDoc.rootVisualElement;

        // UI Elemanları (UI Builder'daki isimlerle birebir uyuşmalı)
        mouseSensSlider = root.Q<Slider>("MouseSensSlider");
        aimingSensSlider = root.Q<Slider>("AimingSensSlider");
        sniperSensSlider = root.Q<Slider>("SniperSensSlider");
        masterVolSlider = root.Q<Slider>("MasterVolumeSlider");
        sfxVolSlider = root.Q<Slider>("SfxVolumeSlider");
        saveButton = root.Q<Button>("SaveButton");
        closeButton = root.Q<Button>("CloseButton");

        // 1. Menü açıldığında mevcut kayıtlı değerleri UI'a yükle
        LoadData();

        // 2. Slider'lar değiştiğinde SADECE BELLEĞİ güncelle (Diske yazıp sistemi yormaz)
        mouseSensSlider.RegisterValueChangedCallback(evt =>
        {
            SettingsManager.Instance.SetMouseSensitivity(evt.newValue);
        });

        aimingSensSlider.RegisterValueChangedCallback(evt =>
        {
            SettingsManager.Instance.SetAimingSens(evt.newValue);
        });

        sniperSensSlider.RegisterValueChangedCallback(evt =>
        {
            SettingsManager.Instance.SetSniperSens(evt.newValue);
        });

        masterVolSlider.RegisterValueChangedCallback(evt =>
        {
            SettingsManager.Instance.SetMasterVolume(evt.newValue);
        });

        sfxVolSlider.RegisterValueChangedCallback(evt =>
        {
            SettingsManager.Instance.SetSfxVolume(evt.newValue);
        });

        // 3. Save butonuna tıklandığında bütün değişiklikleri toplu olarak diske kaydet
        saveButton.clicked += () =>
        {
            SettingsManager.Instance.SaveSettings();
            Debug.Log("Ayarlar kaydedildi!");
        };
        if (closeButton != null)
        {
            closeButton.clicked += () =>
            {
                if (mm != null)
                {
                    mm.TogglePause(false, true);
                }
                if (gameStateCtrl!=null)
                {
                    gameStateCtrl.ResumeGame();
                }
            };
        }
    }

    public void LoadData()
    {
        SettingsManager.Instance.LoadSettings();
        var current = SettingsManager.Instance.CurrentSettings;

        mouseSensSlider.value = current.mouseSensitivity;
        aimingSensSlider.value = current.aimingSens;
        sniperSensSlider.value = current.sniperSens;
        masterVolSlider.value = current.masterVolume;
        sfxVolSlider.value = current.sfxVolume;

    }
}
