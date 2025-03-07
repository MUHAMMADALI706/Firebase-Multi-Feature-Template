using UnityEngine;
using TMPro;

public class LanguageTranslator : MonoBehaviour
{
    private TextMeshProUGUI textMeshProComponent;
    private TextMeshProUGUI textComponent;
    private string originalTextKey;

    void Start()
    {
        textMeshProComponent = GetComponent<TextMeshProUGUI>();
        textComponent = GetComponent<TextMeshProUGUI>();

        if (textMeshProComponent != null)
        {
            originalTextKey = textMeshProComponent.text;
        }
        else if (textComponent != null)
        {
            originalTextKey = textComponent.text;
        }

        TranslateText();
    }

    public void TranslateText()
    {
        if (LanguageController.Instance == null || string.IsNullOrEmpty(originalTextKey))
        {
            return;
        }

        string translatedText = LanguageController.Instance.GetTranslatedText(originalTextKey);

        if (textMeshProComponent != null)
        {
            textMeshProComponent.text = translatedText;
        }
        else if (textComponent != null)
        {
            textComponent.text = translatedText;
        }
    }

    public void SetTranslationKey(string newTranslationKey)
    {
        originalTextKey = newTranslationKey;
        TranslateText();
    }
}
