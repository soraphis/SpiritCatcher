using UnityEngine;
using System.Collections;
using Assets;

public class ReadSignsInteract : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D coll) {

        if(coll.gameObject.CompareTag("Player")) {
            this.SendMessage("Interact");
        }
    }

    public IEnumerator Interact() {
        if(!this.enabled) yield break;
        var Dialog = GetComponent<Dialog>();

        Dialog.currentStatement = Dialog.Container.StartingPoint;
        yield return StartCoroutine(DialogWindow.Instance.ShowDialog(Dialog));
        yield return null;
    }

}
