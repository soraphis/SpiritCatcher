using UnityEngine;
using System.Collections;
using Assets;

public class SerimDialogInteraction : MonoBehaviour {

    public IEnumerator Interact() {

        var Dialog = GetComponent<Dialog>();

        Dialog.SetProperty("whileFestival", Game.Instance.QuestPart1Variables.FestivalAktive);
        Dialog.SetProperty("afterFestival", Game.Instance.QuestPart1Variables.FestivalEnded);
        Dialog.SetProperty("beforeBanditFight", Game.Instance.QuestPart1Variables.BanditsAppeared);
        Dialog.SetProperty("afterBanditFight", Game.Instance.QuestPart1Variables.BanditsDefeated);
        Dialog.SetProperty("SendToSarim", Game.Instance.QuestPart1Variables.SendToSarim);
        Dialog.SetProperty("SendToRato", Game.Instance.QuestPart1Variables.SendToRato);

        Dialog.currentStatement = Dialog.Container.StartingPoint;
        yield return StartCoroutine(DialogWindow.Instance.ShowDialog(Dialog));

        if (Dialog.currentStatement.name == "QuestPartStart") {
            Player.Instance.Items.AddItem("Heilkraut");
            Game.Instance.QuestPart1Variables.SendToRato = true;
        }
        else if(Dialog.currentStatement.name == "FestivalSpeach") {
            var b = Dialog.GetProperty("FestivalSpeechListened", true);
            if (!b) Game.Instance.QuestPart1Variables.FestivalListened++;
            Dialog.SetProperty("FestivalSpeechListened", true);
        }

        yield return null;
    }

}
