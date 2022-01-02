using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LoadingController : MonoBehaviour
{
    [SerializeField] private Button _localButton;
    [SerializeField] private Button _cloudButton;

    private void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                /*app = Firebase.FirebaseApp.DefaultInstance;*/

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });

        /*UserDataManager.Load();
        SceneManager.LoadScene(1);*/
        _localButton.onClick.AddListener(() =>
        {
            SetButtonInteractabl(false);
            UserDataManager.LoadFromLocal();
            SceneManager.LoadScene(1);
        });

        _cloudButton.onClick.AddListener(() =>
        {
            SetButtonInteractabl(false);
            StartCoroutine(UserDataManager.LoadFromCloud(() => SceneManager.LoadScene(1)));
        });
    }

    private void SetButtonInteractabl(bool interactable)
    {
        _localButton.interactable = interactable;
        _cloudButton.interactable = interactable;
    }
}
