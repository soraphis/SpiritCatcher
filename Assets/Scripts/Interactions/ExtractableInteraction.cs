using UnityEngine;
using System.Collections;
using Assets;
using Assets.Soraphis.Spirits.Scripts;

public class ExtractableInteraction : MonoBehaviour {

    public string SpiritType;

    public IEnumerator Interact() {

        var Dialog = GetComponent<Dialog>();
        Dialog.SetProperty("SendToExtractor", Game.Instance.QuestPart1Variables.SendToExtractor);

        Dialog.SetProperty("PickedExtractor", Player.Instance.Items.Contains("Extractor"));
        Dialog.SetProperty("PickedSpirit", Player.Instance.team.Count > 0);

        Dialog.currentStatement = Dialog.Container.StartingPoint;
        yield return StartCoroutine(DialogWindow.Instance.ShowDialog(Dialog));

        if (Dialog.currentStatement.name == "ChooseSpirit") {
            Player.Instance.team.Add(Spirit.GenerateSpirit(SpiritType, 3));
        }

        yield return null;
    }
}
