using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationObject : MonoBehaviour
{
    public Vector2Int position;

    public bool Corrupted
    {
        get { return corrupted; }
        set { UpdateCorrupted(value); }
    }
    bool corrupted = false;

    public GameObject model;
    public GameObject corruptedModel;

    public void UpdateCorrupted(bool value)
    {
        corrupted = value;
        model.SetActive(!value);
        corruptedModel.SetActive(value);
    }
}
