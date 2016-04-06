using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// EXAMPLE OF HOW TO IMPLEMENT AN DIALOGPRINTER
/// </summary>
public class DialogPrinter : MonoBehaviour {

    [SerializeField] private GameObject DialogWindowPanel = null;
    [SerializeField] private Text DialogNameField = null;
    [SerializeField] private Text DialogTextField = null;

    [SerializeField] private GameObject DialogAnswerPrefab = null;
    [SerializeField] private GameObject DialogAnswerPanel= null;
    [SerializeField] private GameObject DialogAnswerContent = null;

    public int selectedAnswer = -1;

    [SerializeField] private Dialog ExampleDialog = null;

    private void Start() {
        DialogWindowPanel.SetActive(false);
        DialogAnswerPanel.SetActive(false);
    }

    private void Update() {
        if(!DialogWindowPanel.activeSelf && Input.GetKeyDown(KeyCode.X)) {
            ExampleDialog.currentStatement = ExampleDialog.Container.StartingPoint;
            StartCoroutine(PrintDialog(ExampleDialog));
        }
    }

    public IEnumerator PrintDialog(Dialog dialog) {
        DialogWindowPanel.SetActive(true);

        while(dialog.currentStatement != null) {
            DialogNameField.text = dialog.currentStatement.speaker;
            DialogTextField.text = dialog.currentStatement.text;

            selectedAnswer = -1;
            var Answers = dialog.Answers;
            if (Answers.Length <= 0) break;
            if (Answers.Length == 1) {
                selectedAnswer = 0;
                continue;
            }
            DialogAnswerPanel.SetActive(Answers.Length > 0);
            { 
                int i = 0;
                foreach(var answer in Answers) {
                    var t = Instantiate(DialogAnswerPrefab);
                    t.transform.SetParent(DialogAnswerContent.transform, false);

                    var button = t.GetComponent<Button>();
                    var text = t.GetComponentInChildren<Text>();
                    text.text = answer.title;

                    if(answer.DoesPassConditions()) {
                        int locali = i;
                        button.onClick.AddListener(() => selectedAnswer = locali);
                    } else {
                        button.interactable = false;
                    }
                    
                    ++i;
                }
            }
            //DialogTextField.text = dialog.currentStatement.text;

            while (selectedAnswer < 0 || selectedAnswer > Answers.Length) yield return null;
            dialog.Answer = Answers[selectedAnswer];

            for(int i = 0; i < DialogAnswerContent.transform.childCount; ++i) { // remove all answer buttons
                Destroy(DialogAnswerContent.transform.GetChild(i).gameObject);
            }

            yield return null;
        }
        Debug.Log("dialog exits with: " + dialog.currentStatement.Id);
        DialogWindowPanel.SetActive(false);
        DialogAnswerPanel.SetActive(false);
    }
}
