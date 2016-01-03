using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace Assets.Scripts.Movement{
    public class MovementComponent : MonoBehaviour {
        public const float speedWalking = 12f; // steps per second
        public const float speedRunning = 20f; // steps per second

        private Vector2 position;
        public Vector2 FacingDirection;
        [HideInInspector] public float currentSpeed;

        private static readonly Vector2[] rotations = {
            Vector2.up,
            Vector2.left,
            Vector2.down,
            Vector2.right
        };

        private AnimatedTextureExtendedUV[] animation = null;

        private new Rigidbody2D rigidbody;
        private Renderer[] renderers;
        void Start() {
            rigidbody = this.GetComponent<Rigidbody2D>();
            animation = GetComponentsInChildren<AnimatedTextureExtendedUV>();
            renderers = GetComponentsInChildren<Renderer>();
        }

        void Update() {
            if(Game.Instance.CurrentGameState != Game.GameState.World) return;
            this.position = transform.position;
//            transform.Translate(FacingDirection * currentSpeed * Time.deltaTime);
            this.rigidbody.velocity = FacingDirection*currentSpeed;
           
        }

        void LateUpdate() {
            //            if(Vector2.Distance(position, transform.position) <= 0.001f) {
            //                var v = transform.position;
            //                float c = 1; // coordinate rounding
            //                if(currentSpeed <= 0.1f || Mathf.Abs(FacingDirection.x) <= 0.1f)
            //                    v.x = Mathf.Round(c*v.x)/c;
            //                if (currentSpeed <= 0.1f || Mathf.Abs(FacingDirection.y) <= 0.1f)
            //                    v.y = Mathf.Round(c*v.y)/c;
            //                transform.position = v;
            //                return;
            //            }
            foreach (var renderer in renderers) {
                renderer.sortingOrder = -(int)transform.position.y;
            }
            
            // character has moved, play animation
            foreach(var anim in animation) {
                if(anim == null) continue;
                anim.rowNumber = Array.IndexOf(rotations, FacingDirection);
                if (anim.rowNumber == -1) anim.rowNumber = 0;
                anim.totalCells = rigidbody.velocity.sqrMagnitude > 0.01f ? 9 : 1;
            }
        }

        void OnCollisionEnter2D(Collision2D coll) {
            var collider = coll.collider;
            if (collider.gameObject.layer == (int)GameLayer.BLOCKING) {
//                currentSpeed = 0;
                rigidbody.velocity = Vector2.zero;
            }
        }
    }
}
