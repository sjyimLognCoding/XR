using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 position;
    public Quaternion rotation;

    public float[] floats;

    //optional
    public GameData()
    {
        position = Vector3.zero;
        rotation = Quaternion.identity;
        floats = new float[25];
    }
}
