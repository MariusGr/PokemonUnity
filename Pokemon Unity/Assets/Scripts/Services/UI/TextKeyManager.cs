using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextKeyManager
{
    readonly public static string TextKeyAttackerPokemon = "attacking-pkmn-name";
    readonly public static string TextKeyDefaultUsageText = "default-usage-text";
    readonly public static string TextKeyPokemon = "pkmn-name";
    readonly public static char NewLineCharacter = '|';

    readonly private static string placeHolder = "#+#+#+#+";

    public static string ReplaceKey(string key, string text, string replacement) => text.Replace($"<{key}>", replacement);

    public static string PlaceNewLineChars(string text)
    {
        text = text.Replace("\\|", placeHolder);
        text = text.Replace('|', '\n');
        text = text.Replace(placeHolder, "|");
        return text;
    }
}
