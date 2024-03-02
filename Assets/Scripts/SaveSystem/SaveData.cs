using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    // General Stats
    public string ProfileName = "";
    public string CurrScene = "";
    public bool[] ChestCollected;
    public int ChestsCount = 0;
    public int TinkerGemCount = 0;
    public int AsheGemCount = 0;
    public int Points = 0;
    public float PlayerTimeInSeconds = 0;

    // Achievments
    public bool BeatTheTutorial;
    public bool BeatTheCave;
    public bool BeatTheAirDungeon;
    public bool BeatTheFinalLevel;
    public bool TenGems;
    public bool TwentyGems;
    public bool ThirtyGems;
    public bool MTB;
    public bool RUNNN;
    public bool LightItUp;
    public bool TheFuture;
    
    // Challenge Achievements
    // Run Past Torches 50 times
    // Do Slime thing
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string a_Json)
    {
        JsonUtility.FromJsonOverwrite(a_Json, this);
    }
}

public interface ISaveable
{
    void PopulateSaveData(SaveData a_SaveData);
    void LoadFromSaveData(SaveData a_SaveData);
}
