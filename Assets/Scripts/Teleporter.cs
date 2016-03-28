using UnityEngine;
using System.Collections;
using Assets.Scripts.Movement;

public class Teleporter : MonoBehaviour {

    public Transform Target;
    public Vector3 Direction;

    public void OnTriggerEnter2D(Collider2D other) {
        if(!other.gameObject.CompareTag(GameTag.PLAYER)) return;
        StartCoroutine(EnterTriggerTeleporter(other));
    }

    private IEnumerator EnterTriggerTeleporter(Collider2D other) {
        var mCTRL = other.gameObject.GetComponent<MovementController>();
        var mCMP  = other.gameObject.GetComponent<MovementComponent>();
        if(mCTRL != null) mCTRL.enabled = false;

        var origDistance = Vector3.Distance(other.transform.position, this.transform.position);
        int fullstop = 5;
        float distance;
        do {
            mCMP.FacingDirection = (this.transform.position - other.transform.position).normalized;
            mCMP.currentSpeed = mCMP.speedWalking;
            yield return null;
            distance = Vector3.Distance(other.transform.position, transform.position);
            if(origDistance - distance < 0.1f) fullstop--;
            if(fullstop <= 0) {
                if (mCTRL != null) mCTRL.enabled = true;
                yield break;
            }
        } while(distance > 0.05f);
        mCMP.currentSpeed = 0f;
        int oldlayer = other.gameObject.layer;
        other.gameObject.layer = GameLayer.IGNORECOLLISON;

        FadeScreen.Instance.StartCoroutine(FadeScreen.Instance.FadeIn(0, 0.125f));
        yield return new WaitForSeconds(0.25f);

        other.transform.position = Target.position;

        Camera.current.transform.position = new Vector3(Target.position.x , Target.position.y, Camera.current.transform.position.z);

        FadeScreen.Instance.StartCoroutine(FadeScreen.Instance.FadeOut(0, 0.125f));
        while (Vector3.Distance(other.transform.position, (Target.position + Direction)) > 0.05f)
        {
            mCMP.FacingDirection = ((Target.position + Direction) - other.transform.position).normalized;
            mCMP.currentSpeed = mCMP.speedWalking;
            yield return null;
        }

        other.gameObject.layer = oldlayer;
        if (mCTRL != null) mCTRL.enabled = true;
    }

}
