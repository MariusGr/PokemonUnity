using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using System.IO;

public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager Instance;
    private static readonly Dictionary<string, ISavable> savables = new Dictionary<string, ISavable>();
    public static void Register(ISavable savable) => savables[savable.GetKey()] = savable;

    [SerializeField] private bool removeSaveGame;
    private string Path => Application.persistentDataPath + "/savegame.sav";

    public SaveGameManager() => Instance = this;
    private void Start() => LoadGame();

    public void InitializeJSON()
    {
        try
        {
            if (!File.Exists(Path))
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));

            File.WriteAllText(Path, "{}");
        }
        catch (Exception e)
        {
            Debug.LogWarning(string.Format(
                "Could not access gesture json file {0}: {1}",
                Path,
                e
            ));
        }

        foreach (KeyValuePair<string, ISavable> entry in savables)
            entry.Value.LoadDefault();
    }

    public void WriteJSON(JSONObject json)
    {
        string jsonString = json.ToString();
        File.WriteAllText(Path, jsonString);
        Debug.Log($"Wrote to json {Path}: {jsonString}");
    }

    public JSONObject LoadFromJSON()
    {
        bool saveGameExists = File.Exists(Path);

        if (removeSaveGame && saveGameExists)
            File.Delete(Path);

        // create file if non-existent
        if (!saveGameExists || removeSaveGame)
        {
            InitializeJSON();
            return null;
        }

        // read file if existent
        string jsonText = File.ReadAllText(Path);

        // Length of 3 is an estimated minimum length the file should have to be valid.
        // This length will be below that of the file is empty or has been non-existent.
        if (File.ReadAllText(Path).Length < 3)
        {
            InitializeJSON();
            return null;
        }

        Debug.Log($"Loading game from {Path}...");
        return (JSONObject)JSON.Parse(jsonText);
    }

    public void LoadGame()
    {
        JSONObject json = LoadFromJSON();

        if (json is null)
            return;

        foreach (KeyValuePair<string, ISavable> entry in savables)
            entry.Value.LoadFromJSON(json);
    }

    public void SaveGame()
    {
        JSONObject json = new JSONObject();

        foreach (KeyValuePair<string, ISavable> entry in savables)
            json.Add(entry.Key, entry.Value.ToJSON());

        WriteJSON(json);
    }
}
