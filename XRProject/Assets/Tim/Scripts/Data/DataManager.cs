using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataManager : MonoBehaviour
{
    private GameData gameData;
    public static DataManager instance { get; private set; }

    List<IDataPersistent> dataObjects;

    string fileName = "data";
    FileHandler handler;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(this);

        instance = this;
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }
    public void LoadGame()
    {
        gameData = handler.Load();

        if (this.gameData == null)
        {
            NewGame();
        }

        foreach (IDataPersistent obj in dataObjects)
        {
            obj.LoadData(gameData);
        }
    }
    public void SaveGame()
    {
        foreach (IDataPersistent obj in dataObjects)
        {
            obj.SaveData(ref gameData);
        }

        handler.Save(gameData);
    }

    private void Start()
    {
        handler = new FileHandler(Application.persistentDataPath, fileName);

        dataObjects = FindAllDataObjects();
        LoadGame();
    }

    private List<IDataPersistent> FindAllDataObjects()
    {
        IEnumerable<IDataPersistent> dataRef = FindObjectsOfType<MonoBehaviour>()
                                                .OfType<IDataPersistent>();

        return new List<IDataPersistent>(dataRef);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}