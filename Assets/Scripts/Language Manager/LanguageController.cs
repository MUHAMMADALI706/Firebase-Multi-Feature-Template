using System.Collections.Generic;
using UnityEngine;

public class LanguageController : GenericSingletonClass<LanguageController>
{
    private Dictionary<string, Dictionary<string, string>> translations = new Dictionary<string, Dictionary<string, string>>();
    public string currentLanguage = "en";

    void Start()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Translation/translations");
        OnInitialize(jsonFile.text);
    }

    public void OnInitialize(string json)
    {
        //currentLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        InitializeTranslations(json);
        currentLanguage = PlayerPrefs.GetString("Language", "en");
    }

    private void InitializeTranslations(string json)
    {
        translations = JsonUtility.FromJson<TranslationsData>(json).ToDictionary();
    }

    public void SetLanguage(string languageCode)
    {
        currentLanguage = languageCode;
    }
    public string GetLanguage()
    {
        return currentLanguage;
    }

    public string GetTranslatedText(string key)
    {
        if ((currentLanguage == "ru" || currentLanguage == "zh") && translations.ContainsKey(key))
        {
            if (translations[key].ContainsKey(currentLanguage))
            {
                return translations[key][currentLanguage];
            }
        }

        return key;
    }
}
