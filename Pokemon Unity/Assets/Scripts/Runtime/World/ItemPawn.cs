using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPawn : MonoBehaviour, IInteractable, ISavable
{
    [SerializeField] Item item;
    [SerializeField] AudioClip pickUpMusic;

    private void Awake() => SaveGameManager.Instance.Register(this);

    public string GetKey()
    {
        GridVector position = new GridVector(transform.position);
        return $"{item.data.GetType()}_{position.x}_{position.y}";
    }

    public void Interact(Character player)
    {
        EventManager.Pause();
        StartCoroutine(TakeItem(player));
    }

    public void LoadDefault() { }
    public void LoadFromJSON(JSONObject json) => gameObject.SetActive(!json[GetKey()]["taken"]);

    public JSONNode ToJSON()
    {
        JSONNode json = new JSONObject();
        json.Add("taken", !gameObject.activeSelf);
        return json;
    }

    private IEnumerator TakeItem(Character player)
    {
        PlayerData.Instance.GiveItem(item);
        BgmHandler.Instance.PlayMFX(pickUpMusic);
        DialogBox.Instance.DrawText(
            $"{player.characterData.name} findet {item.data.name}!", DialogBoxContinueMode.External);
        yield return new WaitForSeconds(pickUpMusic.length);
        DialogBox.Instance.Close();
        gameObject.SetActive(false);
        EventManager.Unpause();
    }
}
