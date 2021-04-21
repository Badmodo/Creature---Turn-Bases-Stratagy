using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    static string path = "C:/Users/3c5f2e60602afde9/Documents/Save/playerData.fun";  //Application.persistentDataPath + " / playerData.fun";

    public static void SavePlayer (PlayerController360 _player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
  
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(_player);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
