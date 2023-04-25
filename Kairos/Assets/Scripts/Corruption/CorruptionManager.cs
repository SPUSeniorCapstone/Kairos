using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionManager : MonoBehaviour
{
    static CorruptionManager main;
    public static CorruptionManager Main
    {
        get { if (main == null) main = FindObjectOfType<CorruptionManager>(); return main; }
    }

    public List<CorruptionPair> blockPairs;


    [Serializable]
    public struct CorruptionPair
    {
        public int normalBlock;
        public int corruptedVariant;
    }
}
