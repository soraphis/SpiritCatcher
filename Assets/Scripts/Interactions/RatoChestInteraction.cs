using UnityEngine;
using System.Collections;
using Assets;

public class RatoChestInteraction : MonoBehaviour {

    public IEnumerator Interact() {
        if(!Game.Instance.QuestPart1Variables.SendToExtractor) yield break;
        if(Player.Instance.Items.Contains("Extractor")) yield break;

        var Dialog = GetComponent<Dialog>();
        Dialog.currentStatement = Dialog.Container.StartingPoint;
        yield return StartCoroutine(DialogWindow.Instance.ShowDialog(Dialog));
        Player.Instance.Items.AddItem("Extractor");

        yield return null;
    }

}
