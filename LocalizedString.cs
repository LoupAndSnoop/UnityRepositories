using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

/// <summary>This is a string that automatically changes to a string appropriate for the current language.</summary>
[System.Serializable]
public class LocalizedString : IEnumerable<(LocalizedString.Language, string)> {
    /// <summary>Languages actually supported by the game.</summary>
    public enum Language {
        English, Spanish, Japanese,
    }
    #region Static language control
    public const Language DEFAULT_LANGAGE = Language.English;
    /// <summary>Current system Language. </summary>
    public static Language language { get; private set; } = DEFAULT_LANGAGE;
    /// <summary>Assign the language we will use for strings.</summary>
    public static void AssignGlobalLanguage() {
        language = Application.systemLanguage switch {
            SystemLanguage.English => Language.English,
            SystemLanguage.Spanish => Language.Spanish,
            SystemLanguage.Japanese => Language.Japanese,
            _ => DEFAULT_LANGAGE
        };
    }
    #endregion

    /// <summary>Data structure holding the strings for each language.</summary>
    private EnumerableArray<Language, string> individualStrings = new();
    /// <summary>Default language at the time the localized string was made.</summary>
    public Language defaultLang { get; private set; } = language;

    public LocalizedString() { return; }
    public LocalizedString(string stringForCurrentLang) => individualStrings[language] = stringForCurrentLang;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    /// <summary>Enumerate all language-string pairs in the object. Expect a lot of null strings!</summary>
    public IEnumerator<(Language, string)> GetEnumerator() {
        foreach (Language lang in individualStrings.GetIndices()) {
            yield return (lang, individualStrings[lang]);
        }
    }

    /// <summary>Get the string that we should currently try to display. </summary>
    public string ApparentString {
        get {
            if (individualStrings[language] != null) return individualStrings[language]; // Look for a perfect match to current language
            else if (individualStrings[defaultLang] != null) return individualStrings[defaultLang]; // Try language it was made on
            return individualStrings[DEFAULT_LANGAGE]; // Try global default, which might be null
        }
    }

    /// <summary>Get/assign string for a given language.</summary>
    public string this[Language lang] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => individualStrings[lang];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => individualStrings[lang] = value;
    }
    /// <summary>Cast directly to current language string.</summary>
    public static implicit operator string(LocalizedString lstring) => lstring.ApparentString;

    /// <summary>Make a new localized string which is a copy of what we have right now. DEEP copy. </summary>
    public LocalizedString Clone() {
        LocalizedString copy = new();
        copy.defaultLang = defaultLang;
        foreach (Language lang in individualStrings.GetIndices()) {
            copy[lang] = individualStrings[lang];
        }
        return copy;
    }
}
