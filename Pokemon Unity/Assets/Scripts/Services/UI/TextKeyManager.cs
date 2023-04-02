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
    readonly public static Dictionary<Stat, string> statToString = new Dictionary<Stat, string>()
    {
        { Stat.Attack, "Angriff" },
        { Stat.SpecialAttack, "Spez. Angriff" },
        { Stat.Defense, "Verteidigung" },
        { Stat.SpecialDefense, "Spez. Verteidigung" },
        { Stat.Speed, "Initiative" },
        { Stat.Accuracy, "Genauigkeit" },
        { Stat.Evasion, "Fluchtwert" },
    };

    readonly private static string placeHolder = "#+#+#+#+";
    readonly private static Dictionary<int, string> statModificationToDescription = new Dictionary<int, string>()
    {
        { 3, "steigt extrem" },
        { 2, "steigt stark" },
        { 1, "steigt" },
        { -1, "sinkt" },
        { -2, "sinkt stark" },
        { -3, "sinkt extrem" },
    };

    public static string ReplaceKey(string key, string text, string replacement) => text.Replace($"<{key}>", replacement);
    public static string GetStatModificationDescription(int amount)
    {
        if (amount == 0)
            return "";

        return statModificationToDescription[Math.Min(3, Math.Max(-3, amount))];
    }

    public static string PlaceNewLineChars(string text)
    {
        text = text.Replace("\\n", "\n");
        text = text.Replace("\\|", placeHolder);
        text = text.Replace('|', '\t');
        text = text.Replace(placeHolder, "|");
        return text;
    }
}
