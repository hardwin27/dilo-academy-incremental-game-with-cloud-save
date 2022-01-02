using Firebase.Storage;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class UserDataManager
{
    private const string progressKey = "Progress";

    public static UserProgressData Progress = new UserProgressData();

    public static void LoadFromLocal()
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

    public static IEnumerator LoadFromCloud(System.Action onComplete)
    {
        StorageReference targetStorage = GetTargetCloudStorage();

        bool isCompleted = false;
        bool isSuccessful = false;
        const long maxAllowedSize = 1024 * 1024; //1mb (?)
        targetStorage.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) =>
        {
            //Download data from the cloud
            if (!task.IsFaulted)
            {
                string json = Encoding.Default.GetString(task.Result);
                Progress = JsonUtility.FromJson<UserProgressData>(json);
                isSuccessful = true;
            }

            isCompleted = true;
        });

        while(!isCompleted)
        {
            yield return null;
        }

        if(isSuccessful)
        {
            //Save the downloaded data if download is succeessful
            Save(true);
        }
        else
        {
            //if download unsuccessful, download local data
            LoadFromLocal();
        }

        onComplete?.Invoke();
    }

    public static void Save(bool uploadToCloud = false)
    {
        string json = JsonUtility.ToJson(Progress);
        PlayerPrefs.SetString(progressKey, json);

        if(uploadToCloud)
        {
            AnalyticsManager.SetUserProperties("gold", Progress.gold.ToString());

            byte[] data = Encoding.Default.GetBytes(json);
            StorageReference targetStorage = GetTargetCloudStorage();

            targetStorage.PutBytesAsync(data);
        }
    }

    public static StorageReference GetTargetCloudStorage()
    {
        //using device ID as the file name that will be stored in the cloud
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;

        return storage.GetReferenceFromUrl($"{storage.RootReference}/{deviceId}");
    }

    public static bool HasResources(int index)
    {
        return index + 1 <= Progress.ResourcesLevels.Count;
    }
}
