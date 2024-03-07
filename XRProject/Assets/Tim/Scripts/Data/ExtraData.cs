using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExtraData : MonoBehaviour, IDataPersistent
{
    float[] someRandomNum;

    private void Start()
    {
        someRandomNum = new float[25];

        for (int i = 0; i < 25; i++)
        {
            someRandomNum[i] = (Random.Range(0.0f, 10.0f));
        }

        print(someRandomNum[10]);
    }

    public void SaveData(ref GameData data)
    {
        data.floats = someRandomNum;
    }

    public void LoadData(GameData data)
    {
        someRandomNum = data.floats;
    }
}
