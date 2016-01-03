using System.Collections;
using Gamelogic;
using UnityEngine;

namespace Assets.Scripts.Movement {
    public class Jumpover : MonoBehaviour {

        public Vector3 Direction = Vector3.zero;

        void Start() {
            Direction.Normalize();
        }

        void OnCollisionEnter2D(Collision2D collision) {
            if(Direction == Vector3.zero) return;
            
            if (!collision.gameObject.CompareTag(GameTag.PLAYER)) return;
            var move = collision.gameObject.GetComponent<MovementComponent>();
            if(move == null || move.FacingDirection != Direction.To2DXY()) return;
            if (Vector3.Dot((this.transform.position - collision.transform.position).normalized, Direction) < 0.75) return;

            

            StartCoroutine(JumpOver(collision));
        }

        private IEnumerator JumpOver(Collision2D collision) {
            var mCTRL = collision.gameObject.GetComponent<MovementController>();
            var mCMP = collision.gameObject.GetComponent<MovementComponent>();
            if (mCTRL != null) mCTRL.enabled = false;
            int oldlayer = collision.gameObject.layer;
            if(oldlayer == GameLayer.IGNORECOLLISON) yield break;
            collision.gameObject.layer = GameLayer.IGNORECOLLISON;
            mCMP.currentSpeed = 0f;
            yield return new WaitForSeconds(0.125f);

            var initPos = collision.transform.position;
            float distance;
            do {
                mCMP.FacingDirection = Direction;
                mCMP.currentSpeed = MovementComponent.speedRunning;
                yield return null;
                distance = Vector3.Distance(collision.transform.position, initPos + Direction * 1.4f);
            } while (distance > 0.06f);
            mCMP.currentSpeed = 0f;
            yield return new WaitForSeconds(0.125f);

            collision.gameObject.layer = oldlayer;
            if (mCTRL != null) mCTRL.enabled = true;
        }
    }
}
