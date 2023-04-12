using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public abstract class SavableFile
{
    public abstract string SavePath { get; set; }

    public SavableFile()
    {
        if (string.IsNullOrEmpty(SavePath))
            Debug.LogError("ERROR! The save path wasn't set for this savable file!!!");
    }

    public void Save()
    {
        string json = JsonConvert.SerializeObject(this);
        File.WriteAllText(SavePath, json);
    }
}
