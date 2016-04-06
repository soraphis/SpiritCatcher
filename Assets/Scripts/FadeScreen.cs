using UnityEngine;
using System.Collections;
using DG.Tweening;
using Gamelogic;
using UnityEngine.UI;

public class FadeScreen : Singleton<FadeScreen> {

    public Image[] fadescreen = new Image[0];

    public IEnumerator FadeIn(int i, float duration) {
        var t = fadescreen[i].DOFade(1, duration);
        yield return t.WaitForCompletion();
        /*for (float f = 0; f < 1f; f += (1f/duration) * Time.deltaTime) {
            var c = fadescreen[i].color;
            c.a = f;
            fadescreen[i].color = c;
            yield return null;
        }*/
    }

    public IEnumerator FadeOut(int i, float duration) {
        var t = fadescreen[i].DOFade(0, duration);
        yield return t.WaitForCompletion();
        /*for (float f = 0; f < 1f; f += (1f/duration)*Time.deltaTime) {
            var c = fadescreen[i].color;
            c.a = (1-f);
            fadescreen[i].color = c;
            yield return null;
        }*/
    }

}
