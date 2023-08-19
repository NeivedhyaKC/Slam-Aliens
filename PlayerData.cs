using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class PlayerData 
{
    private int ArcadeHighScore;
    [System.Serializable]  
    public struct LevelData
    {
        public bool isLocked; 
        public int NoOfStars;
        public int levelHighScore;
    }
    private List<LevelData> levelsData;
    public void Serialize()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        EnemyFactory enemyFactory = GameManager.Instance.GetEnemyFactory();
        string path = Application.persistentDataPath + "/AxeSwing.fun";
        levelsData = new List<LevelData>();
        FileStream fileStream;
        if (File.Exists(path))
        {
            fileStream = new FileStream(path, FileMode.Open);
            fileStream.Position = 0;
            PlayerData tempPlayerData = binaryFormatter.Deserialize(fileStream) as PlayerData;
            this.ArcadeHighScore = tempPlayerData.ArcadeHighScore;
            this.levelsData = new List<LevelData>(tempPlayerData.levelsData);
            fileStream.Close();
        }
        else
        {
            ArcadeHighScore = 0;
            levelsData.Add(new LevelData() { isLocked = false, levelHighScore = 0, NoOfStars = 0 });
            for (int i = 1; i < enemyFactory.LevelsCount(); i++)
                levelsData.Add(new LevelData() { isLocked = true });
        }
    }
    public void Deserialize()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/AxeSwing.fun";
        FileStream fileStream = new FileStream(path, FileMode.Create);
        binaryFormatter.Serialize(fileStream, this);
        fileStream.Close();
    }

    public PlayerData()
    {
        //BinaryFormatter binaryFormatter = new BinaryFormatter();
        //string path = Application.persistentDataPath + "/AxeSwing.fun";
        //levelsData = new List<LevelData>();
        //FileStream fileStream;
        //if(File.Exists(path))
        //{
        //    fileStream = new FileStream(path, FileMode.Open);
        //    PlayerData tempPlayerData = binaryFormatter.Deserialize(fileStream) as PlayerData;
        //    this.ArcadeHighScore = tempPlayerData.ArcadeHighScore;
        //    this.levelsData =new List<LevelData>(tempPlayerData.levelsData);
        //    fileStream.Close();
        //}
        //else
        //{
        //    ArcadeHighScore = 0;
        //    levelsData.Add(new LevelData() { isLocked = false, levelHighScore = 0, NoOfStars = 0 });
        //    for (int i = 1; i < enemyFactory.LevelsCount(); i++)
        //        levelsData.Add(new LevelData() { isLocked = true });
        //}
    }
    public void SetLevel(int level, bool isLocked, int NoOfStars, int highScore)
    {
        Debug.Assert(level > -1 && level < levelsData.Count);
         levelsData[level]= new LevelData() { isLocked = isLocked, levelHighScore=highScore, NoOfStars=NoOfStars};
    }
    public LevelData GetLevel(int level) 
    {
        if (level >= levelsData.Count) return new LevelData() { NoOfStars=-1};
        return levelsData[level]; 
    }
    public List<LevelData> GetLevels()
    {
        return new List<LevelData>(levelsData);
    }
    public void SetArcadeHighScore(int highScore)
    {
        ArcadeHighScore = highScore;
    }
    public int GetArcadeHighScore() { return ArcadeHighScore; }
    public int GetLevelCount() { return levelsData.Count; }
    public int GetUnlockedLevelCount()
    {
        for(int i =0; i< levelsData.Count; i++)
        {
            if (levelsData[i].isLocked == true)
                return i;
        }
        return levelsData.Count;
    }

    ~PlayerData()
    {
        //BinaryFormatter binaryFormatter = new BinaryFormatter();
        //string path = Application.persistentDataPath + "/AxeSwing.fun";
        //FileStream fileStream = new FileStream(path, FileMode.Create);
        //binaryFormatter.Serialize(fileStream, this);
        //fileStream.Close();
    }

}
