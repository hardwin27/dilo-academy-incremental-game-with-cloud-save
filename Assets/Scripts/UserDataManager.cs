using UnityEngine;

public static class UserDataManager
{
    private const string progressKey = "Progress";

    public static UserProgressData Progress;

    public static void Load()
    {
        //If no local save file found, create new
        if (!PlayerPrefs.HasKey(progressKey))
        {
            Progress = new UserProgressData();
            Save();
        }
        //If local save file found, overwrite the old one
        else
        {
            string json = PlayerPrefs.GetString(progressKey);
            Progress = JsonUtility.FromJson<UserProgressData>(json);
        }
    }

    public static void Save()
    {
        string json = JsonUtility.ToJson(Progress);
        PlayerPrefs.SetString(progressKey, json);
    }

    public static bool HasResources(int index)
    {
        return index + 1 <= Progress.ResourcesLevels.Count;
    }
}
