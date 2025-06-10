using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameService : MonoBehaviour
{
    public string apiURL = "http://localhost:5158/api/UserService";
    public static GameService Instance { get; private set; }

    [System.Serializable]
    public class LoginRequest
    {
        public string Login;
        public string Password;
    }

    [System.Serializable]
    public class LoginResponse
    {
        public bool status;
        public UserData data;
    }

    [System.Serializable]
    public class UserData
    {
        public int id;
    }

    [System.Serializable]
    public class CompleteLevelSer
    {
        public int id;
        public int starsCount;
        public int levelScore;
        public int levelID;
    }

    [SerializeField] private InputField loginInput;
    [SerializeField] private InputField passwordInput;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator LoginCourtine(string login, string password)
    {
        LoginRequest request = new LoginRequest { Login = login, Password = password };
        string jsonData = JsonUtility.ToJson(request);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        using (UnityWebRequest www = new UnityWebRequest(apiURL + "/getUser", "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(jsonResponse);

                if (response.status)
                {
                    PlayerPrefs.SetInt("id", response.data.id);
                    PlayerPrefs.Save();

                    SceneManager.LoadScene("MainMenu");
                }
                else
                {
                    Debug.Log("Error: " + jsonResponse);
                }
            }
        }
    }

    private IEnumerator ComleteLevelCourtine(int id, int starsCount, int levelScore, int levelID)
    {
        CompleteLevelSer request = new CompleteLevelSer { id = id, starsCount = starsCount, levelScore = levelScore, levelID = levelID};
        string jsonData = JsonUtility.ToJson(request);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        using (UnityWebRequest www = new UnityWebRequest(apiURL + "/completeLevel", "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                CompleteLevelSer response = JsonUtility.FromJson<CompleteLevelSer>(jsonResponse);
            }
        }
    }

    public void CompleteLevel(int id, int starsCount, int levelScore, int levelID)
    {
        StartCoroutine(ComleteLevelCourtine(id, starsCount, levelScore, levelID));
        SceneManager.LoadScene(0);
    }
    public void LoginButton()
    {
        StartCoroutine(LoginCourtine(loginInput.text, passwordInput.text));
    }

    [System.Serializable]
    public class LeaderboardResponse
    {
        public bool Status;
        public LeaderboardEntry[] Data;
    }

    [System.Serializable]
    public class LeaderboardEntry
    {
        public string Username;
        public int LevelScore;
        public int LevelStars;
        public int LevelID;
        public int UserID;
    }

    public IEnumerator GetLeaderboard(Action<List<LeaderboardEntry>> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiURL + "/getLeaderboard"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                LeaderboardResponse response = JsonUtility.FromJson<LeaderboardResponse>(jsonResponse);

                if (response.Status)
                {
                    callback(new List<LeaderboardEntry>(response.Data));
                }
            }
        }
    }

    public IEnumerator GetLeaderboardForLevel(int levelID, Action<List<LeaderboardEntry>> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiURL + $"/getLeaderboard/{levelID}"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                LeaderboardResponse response = JsonUtility.FromJson<LeaderboardResponse>(jsonResponse);

                if (response.Status)
                {
                    callback(new List<LeaderboardEntry>(response.Data));
                }
            }
        }
    }
}
