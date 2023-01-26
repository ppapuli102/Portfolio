using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public enum Type1{
    Normal,
    Fire,
    Fighting,
    Water,
    Flying,
    Grass,
    Poison,
    Electric,
    Ground,
    Psychic,
    Rock,
    Ice,
    Bug,
    Dragon,
    Ghost,
    Dark,
    Steel,
    Fairy
}

public enum Type2{
    None,
    Normal,
    Fire,
    Fighting,
    Water,
    Flying,
    Grass,
    Poison,
    Electric,
    Ground,
    Psychic,
    Rock,
    Ice,
    Bug,
    Dragon,
    Ghost,
    Dark,
    Steel,
    Fairy
}

[CreateAssetMenu(menuName = "Unit", fileName = "new Unit")]
public class UnitConstructor : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    public RuntimeAnimatorController animator;
    public Dictionary<string, int> unitStats;

}
