using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerPokecenterAI : CharacterControllerBase, IInteractable
{
    [SerializeField] string greetingText = "Ich heil jetzt mal ganz dreist deine Pokémon!";
    [SerializeField] string byeText = "Krass, dass das einfach umsonst ist, ne?";
    [SerializeField] AudioClip healMusic;

    public override CharacterData CharacterData { get => null; }

    public void Interact(Character player)
    {
        EventManager.Pause();
        character.Movement.LookInPlayerDirection();
        StartCoroutine(HealCoroutine());
    }

    private IEnumerator HealCoroutine()
    {
        yield return DialogBox.Instance.DrawText(greetingText, DialogBoxContinueMode.User, true);
        DialogBox.Instance.DrawText("Geht los!", DialogBoxContinueMode.External, true);
        PlayerData.Instance.HealAllPokemons();
        BgmHandler.Instance.PlayMFX(healMusic);
        yield return new WaitForSeconds(healMusic.length);
        EventManager.Unpause();
        DialogBox.Instance.DrawText(byeText, DialogBoxContinueMode.User, true);
    }
}
