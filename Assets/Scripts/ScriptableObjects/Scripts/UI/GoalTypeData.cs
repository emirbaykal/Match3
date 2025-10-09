using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GoalTypeData", menuName = "Scriptable Objects/GoalTypeData")]
public class GoalTypeData : ScriptableObject
{
    public GoalType Type;
    public Sprite Sprite;
    public String Name;
}

public enum GoalType
{
    MoveCounter,
    ScorePoint,
    IceCounter,
    BushCounter
}