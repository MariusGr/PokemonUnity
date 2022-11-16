using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextKeyManager
{
    readonly public static string TextKeyAttackerPokemon = "attacking-pkmn-name";
    readonly public static string TextKeyDefaultUsageText = "default-usage-text";

    public static string ReplaceKey(string key, string text, string replacement) => text.Replace($"<{key}>", replacement);
}
