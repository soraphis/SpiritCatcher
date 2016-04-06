using UnityEngine;
using System.Collections;
using Assets;

public class SimpleDialogInteraction : MonoBehaviour {
    public Dialog Dialog;

    public IEnumerator Interact() {

        Dialog.SetProperty("whileFestival", Game.Instance.QuestPart1Variables.FestivalAktive);
        Dialog.SetProperty("afterFestival", Game.Instance.QuestPart1Variables.FestivalEnded);
        Dialog.SetProperty("beforeBanditFight", Game.Instance.QuestPart1Variables.BanditsAppeared);
        Dialog.SetProperty("afterBanditFight", Game.Instance.QuestPart1Variables.BanditsDefeated);

        Dialog.currentStatement = Dialog.Container.StartingPoint;
        yield return StartCoroutine(DialogWindow.Instance.ShowDialog(Dialog));

        if(Dialog.currentStatement.name == "FestivalSpeach") {
            var b = Dialog.GetProperty("FestivalSpeechListened", true);
            if(! b) Game.Instance.QuestPart1Variables.FestivalListened++;
            Dialog.SetProperty("FestivalSpeechListened", true);
        }

        yield return null;
    }
}
