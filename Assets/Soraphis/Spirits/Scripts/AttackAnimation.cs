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
        Physical,
        Qelle,
        Pflanze,
        Wind,
        Flamme,
        Ozean,
        Sand,
        Solar,
        Lava,
        Sumpf,
        Oedland,
        Sturm,
        Asche,
        Eis,
        Metall,
        Schatten,
        Kaltbrand,
    }

    public enum AttackName {
        Tackle, Bite, IceSpike, Lightsphere, AxialEccentricity
    }

    public static class AttackAnimation {

        /* TEMPLATE
        public static IEnumerator METHODNAME(BattleController ctx, int player, Action handler) {
            var attacker = ctx.Spirits[player];
            var target = ctx.Spirits[1 - player];

            var prefab = Resources.Load("<PREFAB NAME>", typeof(GameObject)) as GameObject;
            var instance = Object.Instantiate(prefab, attacker.transform.position, Quaternion.identity) as GameObject;
            if (instance == null) throw new Exception("wups");
            instance.transform.SetParent(ctx.transform, true);

            // animate prefab

            GameObject.Destroy(instance);

            yield return null;
            handler.Invoke();
        }
        */

        public static IEnumerator Tackle(BattleController ctx, int player, Action handler) {
            var attacker = ctx.Spirits[player];
            var target = ctx.Spirits[1-player];

            var ori_pos = attacker.rectTransform.position;
            var dir = target.rectTransform.position - attacker.rectTransform.position;

            var c = attacker.rectTransform.DOLocalMove(dir/4, 0.4f).SetEase(Ease.Linear);
            yield return c.WaitForCompletion();
            c = attacker.rectTransform.DOMove(ori_pos, 0.3f).SetEase(Ease.Linear);
            target.rectTransform.DOPunchPosition(dir.normalized, 0.2f);
            yield return c.WaitForCompletion();

            handler.Invoke();
        }

        public static IEnumerator Bite(BattleController ctx, int player, Action handler) {
            // var attacker = ctx.Spirits[player];
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

            if(xdir < 0) { 
                icespike.transform.localEulerAngles = new Vector3(0, -180f, -60f);
            }

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

        public static IEnumerator Lightsphere(BattleController ctx, int player, Action handler) {
            var attacker = ctx.Spirits[player];
            var target = ctx.Spirits[1 - player];
            // -
            var prefab = Resources.Load("LightSphere", typeof(GameObject)) as GameObject;
            var lightsphere = Object.Instantiate(prefab, attacker.transform.position, Quaternion.identity) as GameObject;
            if (lightsphere == null) throw new Exception("wups");
            lightsphere.transform.SetParent(ctx.transform, true);


            var sphere = lightsphere.transform.GetChildren().First(c => c.name == "sphere");
            var ray = lightsphere.transform.GetChildren().First(c => c.name == "rays");

            // var t = lightsphere.transform.DOJump(target.transform.position, 100, 1, 0.4f).SetEase(Ease.OutExpo);
            var t = DOTween.Sequence();
            t.Join(lightsphere.transform.DOLocalMoveY(200, 0.35f).SetEase(Ease.OutExpo));
            t.Append(lightsphere.transform.DOMoveY(target.transform.position.y, 0.15f).SetEase(Ease.Linear));
            t.Insert(0, lightsphere.transform.DOMoveX(target.transform.position.x, 0.5f).SetEase(Ease.Linear));

            yield return t.WaitForCompletion();

            ray.gameObject.SetActive(true);
            t = DOTween.Sequence();
            t.Join(ray.DOScale(0, 0.3f).From(true));
            t.Join(sphere.GetComponent<Image>().DOFade(0, 0.3f));
            yield return t.WaitForCompletion();
            yield return new WaitForSeconds(0.3f);
            GameObject.Destroy(lightsphere);
            // -
            yield return null;
            handler.Invoke();
        }

        public static IEnumerator AxialEccentricity(BattleController ctx, int player, Action handler) {
            // var attacker = ctx.Spirits[player];
            var target = ctx.Spirits[1 - player];

            var prefab = Resources.Load("AxialEccentricity", typeof(GameObject)) as GameObject;
            var instance = Object.Instantiate(prefab, target.transform.position, Quaternion.identity) as GameObject;
            if (instance == null) throw new Exception("wups");
            instance.transform.SetParent(ctx.transform, true);

            // animate prefab
            var children = instance.transform.GetChildren();
            var t = DOTween.Sequence();
            for(int i = 0; i < children.Count; i++) {
                var locali = i;
                t.Append(children[i].DOScale(0, 0.1f).From());
                t.Join(children[i].GetComponent<Image>().DOFade(0, 0.3f).SetEase(Ease.InExpo).SetDelay(0.2f).OnStart(() => children[locali].gameObject.SetActive(true)));
            }

            yield return t.WaitForCompletion();

            GameObject.Destroy(instance);

            yield return null;
            handler.Invoke();
        }

    }
}