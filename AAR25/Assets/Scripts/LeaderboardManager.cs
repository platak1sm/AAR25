using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

[System.Serializable]
public class LeaderboardEntry
{
    public string name;
    public string score; // String to match API
}

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries;
}

public class LeaderboardManager : MonoBehaviour
{
    private const string API_URL = "https://matekorni-001-site1.jtempurl.com/api/leaderboard";
    private const string BASIC_AUTH = "Basic MTEyMzk4MDU6NjAtZGF5ZnJlZXRyaWFs";
    private const string API_KEY = "BDEi5aq1VC7oTBZ4tDYXTEyN2WeFaYUCilKCl1OUREHhJIlul4mnRgXqf6Yw2nh5gIWQzAm1bMCzVjKclVdp31IShDQEY1chEow7WVmVilDxHvJFZ1BSKSnOo7B6TXgb";
    private const string LOCAL_LEADERBOARD_KEY = "ARLeaderboard";
    private const int MAX_ENTRIES = 10;

    public static LeaderboardManager Instance { get; private set; }

    void Awake()
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

    public void SubmitScore(string playerName, int score, System.Action<bool> callback)
    {
        StartCoroutine(SubmitScoreCoroutine(playerName, score, callback));
    }

    private IEnumerator SubmitScoreCoroutine(string playerName, int score, System.Action<bool> callback)
    {
        LeaderboardEntry entry = new LeaderboardEntry { name = playerName, score = score.ToString() };
        string json = JsonUtility.ToJson(entry);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest($"{API_URL}/add", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Authorization", BASIC_AUTH);
            request.SetRequestHeader("XApiKey", API_KEY);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Submitted score to API: {playerName}, Score: {score}");
                callback?.Invoke(true);
            }
            else
            {
                Debug.LogError($"API submit failed: {request.error}, Response: {request.downloadHandler.text}");
                callback?.Invoke(false);
            }
        }

        // Save locally as fallback
        List<LeaderboardEntry> localLeaderboard = LoadLocalLeaderboard();
        localLeaderboard.Add(entry);
        localLeaderboard = localLeaderboard.OrderByDescending(e => int.Parse(e.score)).Take(MAX_ENTRIES).ToList();
        SaveLocalLeaderboard(localLeaderboard);
    }

    public void GetTopScores(System.Action<List<LeaderboardEntry>> callback)
    {
        StartCoroutine(GetTopScoresCoroutine(callback));
    }

    private IEnumerator GetTopScoresCoroutine(System.Action<List<LeaderboardEntry>> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{API_URL}/list"))
        {
            request.SetRequestHeader("Authorization", BASIC_AUTH);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log($"Raw API response: {jsonResponse}");

                List<LeaderboardEntry> entries = null;
                try
                {
                    // Try parsing as direct List<LeaderboardEntry> (array)
                    entries = JsonUtility.FromJson<WrappedLeaderboardEntries>("{\"entries\":" + jsonResponse + "}").entries;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"JSON parse failed: {ex.Message}. Trying fallback parsing.");
                    try
                    {
                        // Try parsing as LeaderboardData (object with entries)
                        LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(jsonResponse);
                        entries = data?.entries ?? new List<LeaderboardEntry>();
                    }
                    catch (System.Exception ex2)
                    {
                        Debug.LogError($"Fallback parse failed: {ex2.Message}. Using local leaderboard.");
                        entries = LoadLocalLeaderboard();
                    }
                }

                Debug.Log($"Fetched {entries.Count} leaderboard entries from API");
                callback?.Invoke(entries);
            }
            else
            {
                Debug.LogError($"API fetch failed: {request.error}, Response: {request.downloadHandler.text}");
                callback?.Invoke(LoadLocalLeaderboard());
            }
        }
    }

    private List<LeaderboardEntry> LoadLocalLeaderboard()
    {
        if (!PlayerPrefs.HasKey(LOCAL_LEADERBOARD_KEY))
        {
            return new List<LeaderboardEntry>();
        }
        string json = PlayerPrefs.GetString(LOCAL_LEADERBOARD_KEY);
        LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);
        return data?.entries ?? new List<LeaderboardEntry>();
    }

    private void SaveLocalLeaderboard(List<LeaderboardEntry> leaderboard)
    {
        LeaderboardData data = new LeaderboardData { entries = leaderboard };
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(LOCAL_LEADERBOARD_KEY, json);
        PlayerPrefs.Save();
        Debug.Log($"Saved local leaderboard: {json}");
    }

    [System.Serializable]
    private class WrappedLeaderboardEntries
    {
        public List<LeaderboardEntry> entries;
    }
}