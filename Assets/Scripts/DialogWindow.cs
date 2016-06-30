using System.Collections;
using System.Linq;
using Gamelogic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets {
    public class DialogWindow : Singleton<DialogWindow> {

        [SerializeField] private GameObject DialogPanel = null;
        [SerializeField] private Text text = null;
        [SerializeField] private Image image = null;

        [SerializeField] private GameObject AnswerPanel = null;
        [SerializeField] private GameObject ButtonPrefab = null;

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

        public IEnumerator ShowDialog(Dialog dialog) {
            Game.Instance.CurrentGameState = Game.GameState.Menu;
            Player.Instance.CurrentActionState = Player.ActionState.Talking;
            AnswerPanel.SetActive(false);

            DialogPanel.SetActive(true);
            while (dialog.currentStatement != null) {
                // Debug.Log(dialog.currentStatement.Name);
                if (! string.IsNullOrEmpty(dialog.currentStatement.text)) {
                    yield return ShowMessage(dialog.currentStatement.text);
                }

                var answers = dialog.Answers.Where(ans => ans.DoesPassConditions()).ToArray();
                var selectedAnswer = -1;

                if (answers.Count() <= 0) break;
                if (answers.Count() == 1) selectedAnswer = 0;
                else {
                    AnswerPanel.SetActive(true);
                    // player selects answer
                    AnswerPanel.transform.DestroyChildren();
                    int i = 0;
                    foreach(var answer in answers) {
                        var locali = i;
                        var b = GameObject.Instantiate(ButtonPrefab);
                        b.transform.SetParent(AnswerPanel.transform, false);
                        b.GetComponentInChildren<Text>().text = answer.title;
                        b.GetComponent<Button>().onClick.AddListener(() => selectedAnswer = locali);
                        i++;
                    }
                    //var children = AnswerPanel.transform.GetChildren().ToArray();
                    yield return new WaitUntil(() => selectedAnswer != -1);
                    AnswerPanel.SetActive(false);
                }

                dialog.Answer = answers[selectedAnswer];

            }
            DialogPanel.SetActive(false);
            AnswerPanel.SetActive(false);
            yield return new WaitForEndOfFrame();
            // end dialog:
            

            Game.Instance.CurrentGameState = Game.GameState.World;
            Player.Instance.CurrentActionState = Player.ActionState.Default;
        }


        private IEnumerator ShowMessage(string s) {

            
            generator.Populate(s, settings);

            char[] chars = s.ToCharArray();
            for (int i = 0; i < generator.lineCount; i+=3) {
                int j = Mathf.Min(i + 3, generator.lineCount);
                int r = generator.lines[i].startCharIdx;


                text.text = "";
                image.gameObject.SetActive(false);
                int z;
                z = j == i || j >= generator.lineCount ? chars.Length : generator.lines[j].startCharIdx;
                yield return null;
                while (r < z) {
                    text.text = text.text + chars[r];
                    if(chars[r] == ' ')
                        if(Input.GetKey(KeyCode.X))
                            yield return new WaitForSeconds(0.001f);
                        else
                            yield return new WaitForSeconds(0.05f);
                    r++;
                }
                image.gameObject.SetActive(j < generator.lineCount - 1);
                while (!Input.GetKeyDown(KeyCode.X)) yield return null;
            }
            
        }
	
    }
}
