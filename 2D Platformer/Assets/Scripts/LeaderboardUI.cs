using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private Transform entriesContainer;
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private Dropdown levelDropdown;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float entryHeight = 60f;
    [SerializeField] private Color currentUserColor = Color.yellow;
    [SerializeField] private string apiURL = "http://localhost:5158/api/UserService";

    private List<LeaderboardData> currentEntries = new List<LeaderboardData>();
    private List<GameObject> spawnedEntries = new List<GameObject>();

    private void Awake()
    {
        if (entryPrefab == null)
        {
            Debug.LogError("Entry prefab is not assigned!");
            return;
        }

        entryPrefab.SetActive(false);
        InitializeLevelDropdown();
    }

    private void InitializeLevelDropdown()
    {
        if (levelDropdown == null) return;

        levelDropdown.ClearOptions();
        levelDropdown.AddOptions(new List<string> { "All Levels", "Level 1", "Level 2", "Level 3" });
        levelDropdown.onValueChanged.AddListener(OnLevelSelected);
        LoadLeaderboard();
    }

    private void OnLevelSelected(int index)
    {
        if (index == 0) LoadLeaderboard();
        else LoadLeaderboardForLevel(index);
    }

    public void LoadLeaderboard()
    {
        StartCoroutine(FetchLeaderboardCoroutine());
    }

    public void LoadLeaderboardForLevel(int levelID)
    {
        StartCoroutine(FetchLeaderboardForLevelCoroutine(levelID));
    }

    private IEnumerator FetchLeaderboardCoroutine()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiURL + "/getLeaderboard"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<LeaderboardResponse>(www.downloadHandler.text);
                if (response.status) UpdateEntries(response.data);
            }
            else Debug.LogError("Leaderboard load failed: " + www.error);
        }
    }

    private IEnumerator FetchLeaderboardForLevelCoroutine(int levelID)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiURL + $"/getLeaderboard/{levelID}"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<LeaderboardResponse>(www.downloadHandler.text);
                if (response.status) UpdateEntries(response.data);
            }
            else Debug.LogError($"Level {levelID} leaderboard load failed: " + www.error);
        }
    }

    private void UpdateEntries(LeaderboardData[] entries)
    {
        currentEntries = new List<LeaderboardData>(entries);
        currentEntries.Sort((a, b) => b.levelScore.CompareTo(a.levelScore));
        UpdateLeaderboardUI();
    }

    private void UpdateLeaderboardUI()
    {
        ClearEntries();

        for (int i = 0; i < currentEntries.Count; i++)
        {
            var entry = currentEntries[i];
            var entryObj = Instantiate(entryPrefab, entriesContainer);
            entryObj.SetActive(true);

            var rectTransform = entryObj.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                Debug.LogError("Entry prefab is missing RectTransform component!");
                continue;
            }

            rectTransform.anchoredPosition = new Vector2(0, -entryHeight * i);

            SetEntryText(entryObj, "PositionText", (i + 1).ToString());
            SetEntryText(entryObj, "UsernameText", entry.username);
            SetEntryText(entryObj, "ScoreText", entry.levelScore.ToString());
            SetEntryText(entryObj, "StarsText", entry.levelStars.ToString());

            if (entry.userID == PlayerPrefs.GetInt("id")) HighlightUserEntry(entryObj);

            spawnedEntries.Add(entryObj);
        }

        UpdateContentSize();
    }

    private void SetEntryText(GameObject entry, string childName, string text)
    {
        var child = entry.transform.Find(childName);
        if (child != null && child.TryGetComponent<Text>(out var textComponent))
        {
            textComponent.text = text;
        }
    }

    private void HighlightUserEntry(GameObject entry)
    {
        foreach (var image in entry.GetComponentsInChildren<Image>())
        {
            image.color = currentUserColor;
        }
        foreach (var text in entry.GetComponentsInChildren<Text>())
        {
            text.color = Color.black;
        }
    }

    private void ClearEntries()
    {
        foreach (var entry in spawnedEntries)
        {
            Destroy(entry);
        }
        spawnedEntries.Clear();
    }

    private void UpdateContentSize()
    {
        var contentRect = entriesContainer.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, entryHeight * currentEntries.Count);
        scrollRect.verticalNormalizedPosition = 1;
    }

    [System.Serializable]
    private class LeaderboardResponse
    {
        public bool status;
        public LeaderboardData[] data;
    }

    [System.Serializable]
    private class LeaderboardData
    {
        public string username;
        public int levelScore;
        public int levelStars;
        public int levelID;
        public int userID;
    }
}