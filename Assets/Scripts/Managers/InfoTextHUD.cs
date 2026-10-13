using UnityEngine;
using TMPro;
using System.Collections;

public class InfoTextHUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI subtitle;
    [SerializeField] GameObject subtitleHolder;
    [SerializeField] float typewriterSpeed = 0.05f;
    string currentText = string.Empty;

    void Start()
    {
        Hide();
    }
    public void Show(string text)
    {
        if (currentText == text && subtitleHolder.activeSelf) return;

        currentText = text;
        subtitleHolder.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(TypewriterRoutine(text));
    }

    public void Hide()
    {
        if (!subtitleHolder.activeSelf) return;

        StopAllCoroutines();
        subtitle.text = string.Empty;
        subtitleHolder.SetActive(false);
        currentText = string.Empty;
    }

    IEnumerator TypewriterRoutine(string text)
    {
        subtitle.text = string.Empty;

        foreach (char c in text)
        {
            subtitle.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }
    }
}
