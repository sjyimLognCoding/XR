public interface IDataPersistent
{
    void LoadData(GameData data);
    void SaveData(ref GameData data);
}