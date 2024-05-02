using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchMakingManager : MonoBehaviour
{
    public static MatchMakingManager Instance { get; private set; }

    FirebaseDatabase database;
    DatabaseReference databaseReference;
    DatabaseReference playersReference;
    FirebaseAuth auth;
    string matchId;
    string currentPlayerId;

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

    void Start()
    {
        // Initialize Firebase Database
        database = FirebaseDatabase.DefaultInstance;
        databaseReference = database.RootReference;
        playersReference = database.GetReference("players");

        // Initialize Firebase Auth
        auth = FirebaseAuth.DefaultInstance;
    }

    //Method add player to match queue when hit Find match button
    public void FindMatch()
    {
        currentPlayerId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        // Change player status to "FindMatch" when they hit the Find Match button
        UpdateUserStatus(currentPlayerId, "FindMatch");

        // Add player to matchmaking queue
        databaseReference.Child("queues").Child(currentPlayerId).SetValueAsync(true);

        // Add elo of player in queue
        databaseReference.Child("queues").Child(currentPlayerId).Child("elo").SetValueAsync(1000);

        ListenToPlayerStatus();

        ListenToPlayerMatchID();
    }

    //Method set player readiness when hit ready buttton
    public void SetPlayerReady()
    {
        // Update player's readiness status in the match
        databaseReference.Child("matches").Child(matchId).Child("players").Child(currentPlayerId).SetValueAsync(true);

        ListenToMatchStatus();
    }

    public void UpdateUserStatus(string userId, string status)
    {
        playersReference.Child(userId).Child("status").SetValueAsync(status);
    }

    public void UpdateUserElo(string userId, int elo)
    {
        playersReference.Child(userId).Child("elo").SetValueAsync(elo);
    }

    void ListenToPlayerStatus()
    {
        databaseReference.Child("players").Child(currentPlayerId).Child("status").ValueChanged += PlayerStatusChanged;
    }

    void PlayerStatusChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.Snapshot.Exists)
        {
            string newStatus = args.Snapshot.Value.ToString();
            if (newStatus == "InLobby")
            {
                GoToLobby();
            }
        }
    }

    public void GoToLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    void ListenToPlayerMatchID()
    {
        databaseReference.Child("players").Child(currentPlayerId).Child("matchID").ValueChanged += PlayerMatchIDChanged;
    }

    void PlayerMatchIDChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.Snapshot.Exists)
        {
            string newStatus = args.Snapshot.Value.ToString();
            if (newStatus != null)
            {
                matchId = newStatus;
            }
        }
    }

    void ListenToMatchStatus()
    {
        databaseReference.Child("matches").Child(matchId).Child("status").ValueChanged += MatchStatusChanged;
    }

    void MatchStatusChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.Snapshot.Exists)
        {
            string newStatus = args.Snapshot.Value.ToString();
            if (newStatus == "matched")
            {
                GoToMainScene();
            }
        }
    }

    public void GoToMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
