using UnityEngine;
using System.Collections;
using Assets.Scripts.Movement;
#if UNITY_EDITOR 
using UnityEditor;

/*
namespace Editor {

    [CustomEditor(typeof(Teleporter))]
    class TeleporterEditor : UnityEditor.Editor {
        void OnSceneGUI() {
            var teleporter = (Teleporter) target;
            EditorGUI.BeginChangeCheck();
            Vector3 pos = Handles.PositionHandle(teleporter.TargetPosition, Quaternion.identity);
            if (EditorGUI.EndChangeCheck()) {
                teleporter.TargetPosition = pos;
                Undo.RecordObject(target, "Move LookAt Point");
            }
        }
    }

}*/

#endif

public class Teleporter : MonoBehaviour {

    //public Transform Target;
    //public Vector3 Direction;
    //public float Distance;
    [PositionHandle] public Vector3 TargetPosition;
    public bool toOutdoor = true;


    public void OnTriggerEnter2D(Collider2D other) {
        if(!other.gameObject.CompareTag(GameTag.PLAYER)) return;
        StartCoroutine(EnterTriggerTeleporter(other));
    }

    private IEnumerator EnterTriggerTeleporter(Collider2D other) {
        if(! this.enabled) yield break;

        var mCTRL = other.gameObject.GetComponent<MovementController>();
        // var mCMP  = other.gameObject.GetComponent<MovementComponent>();
        if(mCTRL != null) mCTRL.enabled = false;

        // var pos = this.GetComponent<Collider2D>().bounds.center;
        // var origDistance = Vector3.Distance(other.transform.position, pos);

        //        int fullstop = 5;
        //        float distance;
        //        do {
        //            mCMP.FacingDirection = (pos - other.transform.position).normalized;
        //            mCMP.currentSpeed = mCMP.speedWalking;
        //            yield return null;
        //            distance = Vector3.Distance(other.transform.position, pos);
        //            if(origDistance - distance < 0.1f) fullstop--;
        //            if(fullstop <= 0) {
        //                if (mCTRL != null) mCTRL.enabled = true;
        //                yield break;
        //            }
        //            
        //        } while(distance > 3f);
        //        mCMP.currentSpeed = 0f;
        //        int oldlayer = other.gameObject.layer;
        //        other.gameObject.layer = GameLayer.IGNORECOLLISON;

        if (other.CompareTag("Player")) { 
            yield return FadeScreen.Instance.StartCoroutine(FadeScreen.Instance.FadeIn(0, 0.125f));
            Player.Instance.CurrentActionState = Player.ActionState.Cutszene;
        }
        other.transform.position = TargetPosition;
        Player.Instance.isOutdoor = toOutdoor;
        yield return null;
        //Camera.current.transform.position = new Vector3(TargetPosition.x , TargetPosition.y, Camera.current.transform.position.z);
        if(other.CompareTag("Player")) {
            yield return FadeScreen.Instance.StartCoroutine(FadeScreen.Instance.FadeOut(0, 0.125f));
            Player.Instance.CurrentActionState = Player.ActionState.Default;
        }
//        
//        while (Vector3.Distance(other.transform.position, targetpos) > 1f)
//        {
//            mCMP.FacingDirection = (targetpos - other.transform.position).normalized;
//            mCMP.currentSpeed = mCMP.speedWalking;
//            yield return null;
//        }

        //        other.gameObject.layer = oldlayer;
        if (mCTRL != null) mCTRL.enabled = true;
    }

    /* #if UNITY_EDITOR
        public void OnDrawGizmosSelected() {
            Handles.PositionHandle(TargetPosition, Quaternion.identity);
        }
    #endif */

}
