using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScriptedStruct : ScriptableObject
{
    [SerializeField] public Vector4[] XCoordEnemyChanceMovetime;
    [SerializeField] public LeanTweenType[] leanTweenTypes;
    public int index;
    public bool pathScriptEnable = true;
}
