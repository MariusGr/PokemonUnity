using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using MyBox;

[CreateAssetMenu(fileName = "StoryEvent", menuName = "Story Events/Story Event")]
public class StoryEvent : BaseScriptableObject, ISavable
{
    public static bool EventHappening { get; private set; } = false;
    [field: SerializeField] public Data[] StoryEvents { get; private set; }
    [field: SerializeField] public bool Happened { get; private set; }

    public enum StoryEventType
    {
        Dialog,
        Teleporter,
    }

    [Serializable]
    public class Data
    {
        public StoryEventType Type;
        public StoryEventData Value
        {
            get
            {
                return Type switch
                {
                    StoryEventType.Dialog => dialogEvent,
                    StoryEventType.Teleporter => teleporterEvent,
                    _ => null,
                };
            }
        }

        [ConditionalField(nameof(Type), false, StoryEventType.Teleporter)]
        public TeleporterEvent teleporterEvent;
        [ConditionalField(nameof(Type), false, StoryEventType.Dialog)]
        public DialogEvent dialogEvent;
    }

    private void OnEnable() => SaveGameManager.Register(this);
    public string GetKey() => Id;

    public void TryInvoke()
    {
        if (Happened)
            return;

        EventHappening = true;
        Happened = true;

        List<IEnumerator> routines = new List<IEnumerator>();
        foreach (var storyEvent in StoryEvents)
            routines.Add(storyEvent.Value.Invoke());

        EventManager.Instance.PerformCoroutines(routines, () => EventHappening = false);
    }

    public JSONNode ToJSON()
    {
        JSONNode json = new JSONObject();
        json.Add("happened", Happened);
        return json;
    }

    public void LoadFromJSON(JSONNode json) => Happened = json["happened"];

    public void LoadDefault() => Happened = false;
}
