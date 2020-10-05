using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SequenceData", menuName = "ScriptableObjects/SequenceScriptableObject", order = 1)]
public class Sequence : ScriptableObject
{
    public enum Arrow
    {
        UP = 0,
        LEFT = 1,
        RIGHT = 2,
        DOWN = 3,
    }

    [Serializable]
    public class SequenceNote
    {
        public Arrow Arrow;
        public float Time = -1f;
    }

    public List<SequenceNote> Arrows;
}
