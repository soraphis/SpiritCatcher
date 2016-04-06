using UnityEngine;
using System.Collections;
using Assets;

public class RatoInteraction : MonoBehaviour {

    public GameObject HouseDoor;

    public void Start() {
        HouseDoor.GetComponent<Teleporter>().enabled = true;
        HouseDoor.GetComponent<ReadSignsInteract>().enabled = false;
    }

    public IEnumerator Interact() {

        var Dialog = GetComponent<Dialog>();

        Dialog.SetProperty("whileFestival", Game.Instance.QuestPart1Variables.FestivalAktive);
        Dialog.SetProperty("afterFestival", Game.Instance.QuestPart1Variables.FestivalEnded);
        Dialog.SetProperty("beforeBanditFight", Game.Instance.QuestPart1Variables.BanditsAppeared);
        Dialog.SetProperty("afterBanditFight", Game.Instance.QuestPart1Variables.BanditsDefeated);
        Dialog.SetProperty("SendToSarim", Game.Instance.QuestPart1Variables.SendToSarim);
        Dialog.SetProperty("SendToRato", Game.Instance.QuestPart1Variables.SendToRato);
        Dialog.SetProperty("SendToExtractor", Game.Instance.QuestPart1Variables.SendToExtractor);

        Dialog.SetProperty("PickedExtractor", Player.Instance.Items.Contains("Extractor"));
        Dialog.SetProperty("PickedSpirit", Player.Instance.team.Count > 0);


        Dialog.currentStatement = Dialog.Container.StartingPoint;
        yield return StartCoroutine(DialogWindow.Instance.ShowDialog(Dialog));

        if(Dialog.currentStatement.name == "QuestStart") {
            Game.Instance.QuestPart1Variables.SendToExtractor = true;
            HouseDoor.GetComponent<Teleporter>().enabled = false;
            HouseDoor.GetComponent<ReadSignsInteract>().enabled = true;
        } else if (Dialog.currentStatement.name == "GoodbyePart1") {
            Game.Instance.QuestPart1Variables.FestivalAktive = true;

            int hoursLeft = (int) (19 - Game.Instance.GameTime.Hour);
            Game.Instance.GameTime += new GameDate(hours:hoursLeft);

            HouseDoor.GetComponent<Teleporter>().enabled = true;
            HouseDoor.GetComponent<ReadSignsInteract>().enabled = false;
        } 

        yield return null;
    }
}
