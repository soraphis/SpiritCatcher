using System;
using UnityEngine;
using System.Collections;

public abstract class GameConstant<T>{

    private readonly T value;
    protected GameConstant(T value){
        this.value = value;
    }

    public static implicit operator T (GameConstant<T> t)
    {
        return t.value;
    }

}

public class GameTag : GameConstant<string> {

    public static readonly GameTag PLAYER = new GameTag("Player");

    protected GameTag(string value) : base(value) {}
}

public class GameLayer : GameConstant<string> {

    public static readonly GameLayer DEFAULT = new GameLayer("Default", 0);
    public static readonly GameLayer TRANSPARENTFX = new GameLayer("TransparentFX", 1);
    public static readonly GameLayer IGNORERAYCAST = new GameLayer("Ignore Raycast", 2);
    public static readonly GameLayer WATER = new GameLayer("Water", 4);
    public static readonly GameLayer UI = new GameLayer("UI", 5);

    public static readonly GameLayer BLOCKING = new GameLayer("Blocking", 8);
    public static readonly GameLayer ENTRANCE = new GameLayer("Entrance", 9);
    public static readonly GameLayer IGNORECOLLISON = new GameLayer("Ignore Collision", 10);

    private readonly int number;
    protected GameLayer(string value, int n) : base(value) {
        this.number = n; 
    }

    public static implicit operator int(GameLayer t)
    {
        return t.number;
    }
} 

