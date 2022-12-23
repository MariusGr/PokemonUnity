using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using System.IO;

public class SaveGameManager : MonoBehaviour, ISaveGameManager
{
    public static SaveGameManager Instance;

    private string path => Application.persistentDataPath + "/savegame.sav";
    private Dictionary<string, ISavable> savables = new Dictionary<string, ISavable>();

    public SaveGameManager()
    {
        Instance = this;
        Services.Register(this as ISaveGameManager);
    }

    private void Start()
    {
        LoadGame();
    }

    public void Register(ISavable savable)
    {
        savables[savable.GetKey()] = savable;
    }

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
        JSONObject json;

        // create file of non-existent
        if (!File.Exists(path))
            InitializeJSON();
        // read file if existent
        else
            jsonText = File.ReadAllText(path);

        // Length of 3 is an estimated minimum length the file should have to be valid.
        // This length will be below that of the file is empty or has been non-existent.
        if (File.ReadAllText(path).Length < 3)
        {
            InitializeJSON();
            json = new JSONObject();
        }
        else
            json = (JSONObject)JSON.Parse(jsonText);

        return json;
    }

    public void LoadGame()
    {
        JSONObject json = LoadFromJSON();

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
