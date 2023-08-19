using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScriptedSequence : ScriptableObject
{
    public ScriptedSequence[] scriptedSequences;
    [System.Serializable]
    public struct LeafNode
    {
        [SerializeField] public GameObject gameObject;
        [SerializeField] public PrefabScript.PrefabType type;
        [SerializeField] public float xPosition;
        [SerializeField] public float timeBeforeSpawning;
    }
    public LeafNode[] leafNodes;
    public int[] indexes;
    [HideInInspector] public int indexesIndex = 0;
    public int difficulty;

    static public bool GetNextGameobject(ScriptedSequence scriptedSequence, out GameObject gameObject, out float timeBeforeSpawning,
        out float xPosition,out PrefabScript.PrefabType type)
    {
        int index = scriptedSequence.indexes[scriptedSequence.indexesIndex];
        if (index >= 1000)
        {
            if(!GetNextGameobject(scriptedSequence.scriptedSequences[ index % 1000], out gameObject, out timeBeforeSpawning, out xPosition,
                out type))
            {
                scriptedSequence.indexesIndex++;
            }
        }
        else
        {
            LeafNode node = scriptedSequence.leafNodes[index];
            gameObject = node.gameObject;
            timeBeforeSpawning = node.timeBeforeSpawning;
            xPosition = node.xPosition;
            type = node.type;
            scriptedSequence.indexesIndex++;
            if(scriptedSequence.indexesIndex == scriptedSequence.indexes.Length)
            {
                return false;
            }
        }
        return true;
    }
}
