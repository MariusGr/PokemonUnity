using System;
using System.Collections.Generic;

public static class TextKeyManager
{
    readonly public static string TextKeyAttackerPokemon = "attacking-pkmn-name";
    readonly public static string TextKeyDefaultUsageText = "default-usage-text";
    readonly public static string TextKeyPokemon = "pkmn-name";
    readonly public static char EscapeCharacter = '\\';
    readonly public static string NewParagraphMarker = "|";
    readonly public static string NewLineMarker = "\\n";
    private const char placeHolder = '\u0001';
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

    private static List<int> FindWord(string text, string word)
    {
        List<int> indexes = new();
        int wordLength = 0;

        int index = 0;
        while (index != -1)
        {
            index = text.IndexOf(word, index + wordLength);
            if (index != -1)
                indexes.Add(index);
            wordLength = word.Length;
        }
        return indexes;
    }

    /*
     * Removes markers from text and returns the indices where they were in the text before,
     * where indices refer to the position in the new, filtered text.
     */
    private static Queue<int> FilterMarkers(string text, string marker, out string filteredText)
    {
        Queue<int> indices = new();

        int CheckForMarker(int index, string currentMarker)
        {
            if (!text.Substring(index, currentMarker.Length).Equals(currentMarker))
                return 0;
            return currentMarker.Length;
        }

        var escapedMarker = $"{EscapeCharacter}{marker}";

        filteredText = "";
        int index = 0;
        while (index < text.Length)
        {
            // Check for escaped marker
            var stepLength = CheckForMarker(index, escapedMarker);
            if (stepLength > 0)
            {
                // Escaped marker found: Replace with unescaped version (i.e. removing the escape character)
                filteredText += text.Substring(index + 1, stepLength - 1);
                index += stepLength;
                continue;
            }

            // Check for marker
            stepLength = CheckForMarker(index, marker);
            if (stepLength > 0)
            {
                // Marker found: Replace marker by newline char and save the index to queue
                filteredText += text.Substring(index + stepLength, stepLength) + '\n';
                // Save the index of the newly added newline as paragraph start
                indices.Enqueue(filteredText.Length - 1);
                index += stepLength;
            }
        }

        return indices;
    }

    public static Queue<int> ProcessDialogText(string text, out string processedText, out string processedDialogText)
    {
        processedText = ProcessText(text);
        var paragraphIndices = FilterMarkers(processedText, NewParagraphMarker, out processedDialogText);
        return paragraphIndices;
    }

    public static string ProcessText(string text)
    {
        text = text.Replace("\\n", "\n");
        text = text.Replace("\\|", placeHolder.ToString());
        text = text.Replace('|', '\t');
        text = text.Replace(placeHolder, '|');
        return text;
    }
}
