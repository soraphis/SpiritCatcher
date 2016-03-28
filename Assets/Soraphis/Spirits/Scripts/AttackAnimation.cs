using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using Gamelogic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.Soraphis.Spirits.Scripts {

    public enum AttackType {
        Tackle, Bite, IceSpike
    }

    public static class AttackAnimation {

        public static IEnumerator Tackle(BattleController ctx, int player, Action handler) {
            var attacker = ctx.Spirits[player];
            var target = ctx.Spirits[1-player];

            var ori_pos = attacker.rectTransform.position;
            var dir = target.rectTransform.position - attacker.rectTransform.position;

            var c = attacker.rectTransform.DOLocalMove(dir/3, 0.4f);
            yield return c.WaitForCompletion();
            c = attacker.rectTransform.DOMove(ori_pos, 0.3f);
            target.rectTransform.DOPunchPosition(dir.normalized, 0.2f);
            yield return c.WaitForCompletion();

            handler.Invoke();
        }

        public static IEnumerator Bite(BattleController ctx, int player, Action handler) {
            var attacker = ctx.Spirits[player];
            var target = ctx.Spirits[1 - player];

            var prefab = Resources.Load("Bite", typeof(GameObject)) as GameObject;

            var teeth = Object.Instantiate(prefab, target.transform.position, Quaternion.identity) as GameObject;
            if(teeth == null) throw new Exception("wups");
            var teeth1 = teeth.transform.GetChild(0).GetComponent<RectTransform>();
            var teeth2 = teeth.transform.GetChild(1).GetComponent<RectTransform>();

            teeth.transform.SetParent(ctx.transform, true);

            var seq = DOTween.Sequence();
            seq.Append(teeth1.DOLocalMoveY(0, 0.1f).SetEase(Ease.Linear))
                .Join(teeth2.DOLocalMoveY(0, 0.1f).SetEase(Ease.Linear))
                .SetDelay(0.2f);

            yield return seq.WaitForCompletion();

            seq = DOTween.Sequence();
            seq.Append(teeth.transform.DOScale(2, 0.3f))
                .Join(teeth1.GetComponent<Image>().DOFade(0, 0.3f))
                .Join(teeth2.GetComponent<Image>().DOFade(0, 0.3f))
                .SetDelay(0.2f); ;

            yield return seq.WaitForCompletion();
            GameObject.Destroy(teeth);

            yield return null;
            handler.Invoke();
        }

        public static IEnumerator IceSpike(BattleController ctx, int player, Action handler) {
            var attacker = ctx.Spirits[player];
            var target = ctx.Spirits[1 - player];

            var xdir = (target.transform.position - attacker.transform.position).x;
            xdir = Mathf.Sign(xdir);

            var prefab = Resources.Load("IceSpike", typeof(GameObject)) as GameObject;

            var icespike = Object.Instantiate(prefab, attacker.transform.position, Quaternion.identity) as GameObject;
            if (icespike == null) throw new Exception("wups");

            icespike.transform.SetParent(ctx.transform, true);

            var thunder = icespike.transform.GetChildren().Where(c => c.name == "thunder");
            var spike = icespike.transform.GetChildren().Where(c => c.name == "spike");


            var seq = DOTween.Sequence();
            seq.Append(icespike.transform.DOMove(target.transform.position, 0.4f).SetEase(Ease.InCubic));
            yield return seq.WaitForCompletion();
            foreach(var t in spike) {   t.gameObject.SetActive(false);  }
            seq = DOTween.Sequence();
            foreach(var t in thunder) {
                t.gameObject.SetActive(true);
                seq.Join(
                    t.DOLocalRotateQuaternion(Quaternion.AngleAxis(Random.Range(20, 50), Vector3.forward), 0.3f)
                        .SetRelative(true));
            }
            yield return seq.WaitForCompletion();
            GameObject.Destroy(icespike);

            yield return null;
            handler.Invoke();
        }

    }
}