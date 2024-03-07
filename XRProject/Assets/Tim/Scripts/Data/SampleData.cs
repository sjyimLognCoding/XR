using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleData : MonoBehaviour, IDataPersistent
{
    Vector3 position;
    Quaternion rotation;

    public void LoadData(GameData data)
    {
        this.position = data.position;
        this.rotation = data.rotation;
    }

    public void SaveData(ref GameData data)
    {
        data.position = this.position;
        data.rotation = this.rotation;
    }

    private void Start()
    {
        position = transform.position;
        rotation = transform.rotation;
    }


}
