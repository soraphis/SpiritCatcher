using System;
using UnityEngine;
using System.Collections;

public static class Direction{

    public static Vector2 fromInt(int i) {
        switch(i) {
            case 0: return Vector2.right;
            case 1: return Vector2.up;
            case 2: return Vector2.left;
            case 3: return Vector2.down;
            default: throw new ArgumentOutOfRangeException("{i} has to be between 0 and 3");
        }
    }

    public static int toInt(Vector2 v) {
        if(v == Vector2.right)  return 0;
        if(v == Vector2.up)     return 1;
        if(v == Vector2.left)   return 2;
        if(v == Vector2.down)   return 3;
        throw new ArgumentException("Invalid Vector  {v} cant be mapped to [0,+-1] or [+-1,0]");
    }

    public static Vector2 rotate90(Vector2 v) {
        if (v == Vector2.right) return Vector2.up;
        if (v == Vector2.up)    return Vector2.left;
        if (v == Vector2.left)  return Vector2.down;
        if (v == Vector2.down)  return Vector2.right;
        throw new ArgumentException("Invalid Vector  {v} cant be mapped to [0,+-1] or [+-1,0]");
    }

}
