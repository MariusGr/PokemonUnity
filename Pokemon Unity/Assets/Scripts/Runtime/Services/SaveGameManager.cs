using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using System.IO;

public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager Instance;

    private string path => Application.persistentDataPath + "/savegame.sav";
    private Dictionary<string, ISavable> savables = new Dictionary<string, ISavable>();

    public SaveGameManager() => Instance = this;
    private void Start() => LoadGame();
    public void Register(ISavable savable) => savables[savable.GetKey()] = savable;

    public void InitializeJSON()
    {
        try
        {
            if (!File.Exists(path))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            File.WriteAllText(path, "{}");
        }
        catch (Exception e)
        {
            Debug.LogWarning(string.Format(
                "Could not access gesture json file {0}: {1}",
                path,
                e
            ));
        }

        foreach (KeyValuePair<string, ISavable> entry in savables)
            entry.Value.LoadDefault();
    }

    public void WriteJSON(JSONObject json)
    {
        string jsonString = json.ToString();
        File.WriteAllText(path, jsonString);
        Debug.Log($"Wrote to json {path}: {jsonString}");
    }

    public JSONObject LoadFromJSON()
    {
        string jsonText = "{}";

        // create file of non-existent
        if (!File.Exists(path))
        {
            InitializeJSON();
            return null;
        }

        // read file if existent
        jsonText = File.ReadAllText(path);

        // Length of 3 is an estimated minimum length the file should have to be valid.
        // This length will be below that of the file is empty or has been non-existent.
        if (File.ReadAllText(path).Length < 3)
        {
            InitializeJSON();
            return null;
        }

        Debug.Log($"Loading game from {path}...");
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
