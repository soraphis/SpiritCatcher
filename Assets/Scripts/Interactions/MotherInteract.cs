using UnityEngine;
using System.Collections;
using Assets;

public class MotherInteract : MonoBehaviour {

    public IEnumerator Interact() {

        var Dialog = GetComponent<Dialog>();

        Dialog.SetProperty("FestivalActive", Game.Instance.QuestPart1Variables.FestivalAktive);
        Dialog.SetProperty("AfterFestival", Game.Instance.QuestPart1Variables.FestivalEnded);

        Dialog.SetProperty("SendToSarim", Game.Instance.QuestPart1Variables.SendToSarim);

        Dialog.SetProperty("HasMedicine", Player.Instance.Items.Contains("Heilkraut"));


        Dialog.currentStatement = Dialog.Container.StartingPoint;
        yield return StartCoroutine(DialogWindow.Instance.ShowDialog(Dialog));

        if(Dialog.currentStatement.name == "StartQuest") {
            Game.Instance.QuestPart1Variables.SendToSarim = true;
        } else if (Dialog.currentStatement.name == "FinishQuest") {
            if(Player.Instance.Items["Heilkraut"] != null) {
                Player.Instance.Items.AddItem("Heilkraut", -1);
            } else {
                Debug.LogError("should not happen ... ever, because i check this condition twice");
            }
        } else if (Dialog.currentStatement.name == "MedicinRefresh") {
            Player.Instance.Items.AddItem("Heilkraut");
        }


        yield return null;
    }
}
