using System;
using System.Security.Cryptography.X509Certificates;
using Assets.Soraphis.SaveGame;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Movement{
    public class MovementComponent : MonoBehaviour, Saveable {
        public float speedWalking = 120f; // steps per second
        public float speedRunning = 200f; // steps per second

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
            if (Game.Instance.CurrentGameState != Game.GameState.World) return;
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

                if(rigidbody.velocity.sqrMagnitude < 0.01f) {
                    anim.SetSpriteAnimation(anim.colCount, anim.rowCount, anim.rowNumber, anim.colNumber, 1, anim.fps);
                    anim.Pause();
                } else
                    anim.Pause(false);
                // anim.totalCells = rigidbody.velocity.sqrMagnitude > 0.01f ? 8 : 1;
            }
        }

        void OnCollisionEnter2D(Collision2D coll) {
            var collider = coll.collider;
            if (collider.gameObject.layer == (int)GameLayer.BLOCKING) {
//                currentSpeed = 0;
                rigidbody.velocity = Vector2.zero;
            }
        }

        public void Load(DataNode parent) {
            DataNode node = parent.GetChild("MoveComponent");
            if (node == null) return;

            var facing = node.GetChild("FacingDirection");
            FacingDirection = new Vector2(facing.GetChild("x").Get<float>(), facing.GetChild("y").Get<float>());

            var pos = node.GetChild("Position");
            transform.position = new Vector2(pos.GetChild("x").Get<float>(), pos.GetChild("y").Get<float>());
        }

        public DataNode Save() {
            DataNode node = new DataNode();
            node.Name = "MoveComponent";

            var facing = node.AddChild("FacingDirection");
            facing.AddChild("x", FacingDirection.x);
            facing.AddChild("y", FacingDirection.y);

            var pos = node.AddChild("Position");
            pos.AddChild("x", position.x);
            pos.AddChild("y", position.y);

            return node;
        }

        public void CreateDefault() {
            throw new NotImplementedException();
        }
    }
}
