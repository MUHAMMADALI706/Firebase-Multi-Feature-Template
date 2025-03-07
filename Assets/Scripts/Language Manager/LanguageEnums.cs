using System;
using System.Collections.Generic;

[Serializable]
public class TranslationEntry
{
    public string key;
    public Translations translations; // Change the field to match JSON structure
}

[Serializable]
public class Translations
{
    public string ru;
    public string zh;
}

[Serializable]
public class TranslationsData
{
    public List<TranslationEntry> entries;

    public Dictionary<string, Dictionary<string, string>> ToDictionary()
    {
        var dict = new Dictionary<string, Dictionary<string, string>>();
        foreach (var entry in entries)
        {
            dict[entry.key] = new Dictionary<string, string>
            {
                { "ru", entry.translations.ru },
                { "zh", entry.translations.zh }
            };
        }
        return dict;
    }
}
