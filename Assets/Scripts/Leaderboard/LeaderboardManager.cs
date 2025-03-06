using TMPro;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using Firebase;
using Unity.VisualScripting;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField scoreInputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private GameObject leaderboardItemPrefab;
    [SerializeField] private Transform leaderboardParent;
    [SerializeField] private GameObject leaderboardPopup;
    [SerializeField] private GameObject loadingPanel;
    void Start()
    {
        FirebaseManager.Instance.Load(OnInitializedFirebase);
        submitButton.onClick.AddListener(() => SubmitScore(FirebaseManager.Instance.user.UserId, nameInputField.text, scoreInputField.text));
    }
    void OnInitializedFirebase()
    {
        TextMeshProUGUI messageText = loadingPanel.GetComponentInChildren<TextMeshProUGUI>();
        if (FirebaseManager.Instance.user == null)
        {
            messageText.text = "Currently user loged out.";
            return;
        }
        Action mCallBackEmailNotVerified = () =>
            {
                messageText.text = "Email is not Verified!";
                return;
            };
        Action mCallbackSuccess = () =>
        {
            loadingPanel.SetActive(false);
        };
        Action<FirebaseException> callbackFailure = (pError) =>
        {
            print(pError);
            var errorMessage = pError.InnerException != null
               ? pError.InnerException.Message
               : pError.Message;
            messageText.text = ErrorManager.Instance.getTranslateError(errorMessage);
        };
        FirebaseManager.Instance.RefreshUserStatus(mCallbackSuccess, mCallBackEmailNotVerified, callbackFailure);

        loadingPanel.SetActive(false);
    }
    public void SubmitScore(string playerID, string playerName, string score)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            messageText.text = "Enter player name";
            return;
        }
        if (string.IsNullOrEmpty(score))
        {
            messageText.text = "Enter player score";
            return;
        }
        int newScore = int.Parse(score);
        DatabaseReference playerRef = FirebaseManager.Instance.databaseReference.Child("leaderboard").Child(playerID);

        playerRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int currentScore = 0;

                if (snapshot.Exists && snapshot.Child("score").Value != null)
                {
                    currentScore = int.Parse(snapshot.Child("score").Value.ToString());
                }

                if (newScore > currentScore) // Update only if higher
                {
                    LeaderboardEntry entry = new LeaderboardEntry(playerName, newScore);
                    string json = JsonUtility.ToJson(entry);
                    playerRef.SetRawJsonValueAsync(json);
                    messageText.text = "Score updated on leaderboard";
                }
            }
        });
    }
    public void GetLeaderboard()
    {
        DatabaseReference dbReference = FirebaseDatabase.DefaultInstance.GetReference("leaderboard");
        dbReference.OrderByChild("score").LimitToLast(10).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>();

                foreach (DataSnapshot child in snapshot.Children)
                {
                    string name = child.Child("name").Value.ToString();
                    int score = int.Parse(child.Child("score").Value.ToString());
                    leaderboard.Add(new LeaderboardEntry(name, score));
                }

                leaderboard.Reverse(); // Highest scores first
                DisplayLeaderboard(leaderboard);
            }
        });
    }

    void DisplayLeaderboard(List<LeaderboardEntry> leaderboard)
    {
        foreach (Transform child in leaderboardParent) Destroy(child.gameObject);

        foreach (var entry in leaderboard)
        {
            GameObject newItem = Instantiate(leaderboardItemPrefab, leaderboardParent);
            TextMeshProUGUI[] texts = newItem.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = entry.name;
            texts[1].text = entry.score.ToString();
        }
        leaderboardPopup.SetActive(true);
    }
    public void CloseLeaderboard()
    {
        leaderboardPopup.SetActive(false);
        messageText.text = "";
    }
}


[Serializable]
public class LeaderboardEntry
{
    public string name;
    public int score;

    public LeaderboardEntry(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}