using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchMakingManager : MonoBehaviour
{
    public static MatchMakingManager Instance { get; private set; }

    FirebaseDatabase database;
    DatabaseReference databaseReference;
    DatabaseReference playersReference;
    FirebaseAuth auth;
    string matchId;

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

    public void FindMatch()
    {
        string currentPlayerId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        // Change player status to "FindMatch" when they hit the Find Match button
        UpdateUserStatus(currentPlayerId, "FindMatch");

        // Add player to matchmaking queue
        databaseReference.Child("queues").Child(currentPlayerId).SetValueAsync(true);

        // Add elo of player in queue
        databaseReference.Child("queues").Child(currentPlayerId).Child("elo").SetValueAsync(1000);
    }

    public void UpdateUserStatus(string userId, string status)
    {
        playersReference.Child(userId).Child("status").SetValueAsync(status);
    }

    public void UpdateUserElo(string userId, int elo)
    {
        playersReference.Child(userId).Child("elo").SetValueAsync(elo);
    }

}
