using System;
using System.Collections;
using Gamelogic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Dialog : Singleton<Dialog> {

    [SerializeField] private GameObject DialogPanel = null;
    [SerializeField] private Text text = null;
    [SerializeField] private Image image = null;

    private TextGenerationSettings settings;
    private TextGenerator generator;

    private void Start() {
        settings = text.GetGenerationSettings(new Vector2(text.rectTransform.rect.width, text.rectTransform.rect.height));
        generator = new TextGenerator();
    }

    private void Update() {
       /* if(Game.Instance.CurrentGameState != Game.GameState.World) return;
        if (Input.GetKeyDown(KeyCode.X)) {
            StartCoroutine(
                ShowDialog(
                    "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, " +
                    "sed diam nonumy eirmod tempor invidunt ut labore et dolore " +
                    "magna aliquyam erat, sed diam voluptua. At vero eos et accusam " +
                    "et justo duo dolores et ea rebum. Stet clita kasd gubergren, " +
                    "no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem " +
                    "ipsum dolor sit amet, consetetur sadipscing elitr, sed diam " +
                    "nonumy eirmod tempor invidunt ut labore et dolore magna " +
                    "aliquyam erat, sed diam voluptua. At vero eos et accusam et " +
                    "justo duo dolores et ea rebum. Stet clita kasd gubergren, no " +
                    "sea takimata sanctus est Lorem ipsum dolor sit amet."));
        }*/
    }

    public IEnumerator ShowDialog(string s) {
        // start dialog:
        DialogPanel.SetActive(true);
        Game.Instance.CurrentGameState = Game.GameState.Menu;
        Player.Instance.CurrentActionState = Player.ActionState.Talking;
        generator.Populate(s, settings);

        char[] chars = s.ToCharArray();
        for (int i = 0; i < generator.lineCount; i+=3) {
            int j = Mathf.Min(i + 3, generator.lineCount - 1);
            int r = generator.lines[i].startCharIdx;

            text.text = "";
            image.gameObject.SetActive(false);
            int z;
            z = j == i || j >= generator.lineCount ? chars.Length : generator.lines[j].startCharIdx;
            while (r < z) {
                text.text = text.text + chars[r];
                if(chars[r] == ' ')
                    yield return new WaitForSeconds(0.05f);
                r++;
            }
            image.gameObject.SetActive(j < generator.lineCount - 1);
            while (!Input.GetKeyDown(KeyCode.X)) yield return null;
        }
        // end dialog:
        DialogPanel.SetActive(false);
        Game.Instance.CurrentGameState = Game.GameState.World;
        Player.Instance.CurrentActionState = Player.ActionState.Default;
    }
	
}
