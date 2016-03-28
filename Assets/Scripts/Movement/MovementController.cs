using UnityEngine;
using System.Collections.Generic;
using Assets.Soraphis.SaveGame;

namespace Assets.Scripts.Movement {
    public class MovementController : MonoBehaviour {

        private static readonly Dictionary<KeyCode, Vector2> KeyDirectionMapping = new Dictionary<KeyCode, Vector2>(4) {
            {KeyCode.RightArrow, Vector2.right},
            {KeyCode.UpArrow, Vector2.up},
            {KeyCode.LeftArrow, Vector2.left},
            {KeyCode.DownArrow, Vector2.down}
        };

        private MovementComponent movement;

        private void Start() {
            movement = this.GetComponent<MovementComponent>();
        }

        private void Update() {
            if (Player.Instance.CurrentActionState == Player.ActionState.Talking) return;
            if (Player.Instance.CurrentActionState == Player.ActionState.Cutszene) return;

            Vector2 dir;
            FindDirection(out dir);
            dir.Normalize();
            if (dir == Vector2.zero) { 
                movement.currentSpeed = 0;
                return;
            }else if(dir == movement.FacingDirection) {
                movement.currentSpeed = Input.GetKey(KeyCode.LeftShift)
                    ? movement.speedRunning
                    : movement.speedWalking;
            } else {
                
                movement.FacingDirection = (dir);
            }
        }

        private void FindDirection(out Vector2 dir) {
            dir = Vector2.zero;
            foreach(var kvpair in KeyDirectionMapping) {
                //if (Input.GetKey(kvpair.Key)) dir += kvpair.Value;
                //*
                if(Input.GetKey(kvpair.Key)) {
                    dir = kvpair.Value; 
                    if(movement.FacingDirection == dir) return; // keeps direction if possible*/
                }
            }
        }
    }

   
}
