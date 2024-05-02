using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchMaking : MonoBehaviour
{
    public static MatchMaking Instance { get; private set; }

    FirebaseDatabase database;
    DatabaseReference usersReference;
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
        usersReference = database.GetReference("users");

        // Initialize Firebase Auth
        auth = FirebaseAuth.DefaultInstance;
    }


    public void UpdateUserStatus(string userId, string status)
    {
        usersReference.Child(userId).Child("status").SetValueAsync(status);
    }

    public void UpdateUserLevel(string userId, int level)
    {
        usersReference.Child(userId).Child("level").SetValueAsync(level);
    }


    public void FindMatch()
    {
        // For example, update user status to looking for match
        string currentPlayerId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        UpdateUserStatus(currentPlayerId, "lookingForMatch");

        // Placeholder matchmaking logic
        Debug.Log("Looking for match...");

        // Get current player's level
        usersReference.Child(currentPlayerId).Child("level").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to get current player's level: " + task.Exception);
                return;
            }

            int currentPlayerLevel = Convert.ToInt32(task.Result.Value);

            // Query users to find matching players
            usersReference.GetValueAsync().ContinueWith(usersTask =>
            {
                if (usersTask.IsFaulted)
                {
                    Debug.LogError("Failed to query users: " + usersTask.Exception);
                    return;
                }

                DataSnapshot usersSnapshot = usersTask.Result;

                // List to store players with the same level and looking for a match
                List<string> potentialMatches = new List<string>();

                foreach (DataSnapshot userSnapshot in usersSnapshot.Children)
                {
                    string userId = userSnapshot.Key;
                    //Debug.Log("Doi thu ID: " + userId);
                    //Debug.Log("Nguoi choi ID:" + currentPlayerId);
                    if (userId != currentPlayerId)
                    {
                        // Check if the user has the same level and is looking for a match
                        int userLevel = Convert.ToInt32(userSnapshot.Child("level").Value);
                        string userStatus = Convert.ToString(userSnapshot.Child("status").Value);
                        //Debug.Log(userStatus);
                        if (userLevel == currentPlayerLevel && userStatus == "lookingForMatch")
                        {
                            potentialMatches.Add(userId);
                        }
                    }
                }

                Debug.Log("Co " + potentialMatches.Count + "doi thu ne");
                // If there is at least one potential match, start the match
                if (potentialMatches.Count > 0)
                {
                    // Select the first potential match and create a match
                    string opponentId = potentialMatches[0];
                    List<string> playerIds = new List<string> { currentPlayerId, opponentId };
                    // Add more player IDs as needed
                    CreateMatch(playerIds);
                    Debug.Log("Match found");
                }
                else
                {
                    Debug.Log("No match found. Waiting for another player...");
                }
            });
        });
    }

    private void CreateMatch(List<string> playerIds)
    {
        // Create a match in the database
        matchId = Guid.NewGuid().ToString();
        DatabaseReference matchRef = FirebaseDatabase.DefaultInstance.GetReference("matches").Child(matchId);

        foreach (string playerId in playerIds)
        {
            matchRef.Child("players").Child(playerId).SetValueAsync(false);

            // Update each player's status to indicate they are in a match
            UpdateUserStatus(playerId, "InMatch");
        }

        Debug.Log("Match created. Players: " + string.Join(", ", playerIds.ToArray()));

        // Start the match, transition to the gameplay scene, etc.
        // Transition to the lobby scene
        SceneManager.LoadScene("LobbyScene");
    }

    public void PlayerReady()
    {
        // Get the current player's ID
        string currentPlayerId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        Debug.Log("Player " + currentPlayerId + " is ready.");

        // Update player's readiness in the "matches" reference
        //string matchId = SceneManager.GetActiveScene().name; // Assuming each lobby scene corresponds to a unique match
        DatabaseReference matchRef = FirebaseDatabase.DefaultInstance.GetReference("matches").Child(matchId);
        matchRef.Child("players").Child(currentPlayerId).SetValueAsync(true);

        // Check if all players in the match are ready
        matchRef.Child("players").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to retrieve players' readiness status: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            bool allPlayersReady = true;

            foreach (DataSnapshot playerSnapshot in snapshot.Children)
            {
                bool isReady = (bool)playerSnapshot.GetValue(true);
                if (!isReady)
                {
                    allPlayersReady = false;
                    break;
                }
            }

            // If all players are ready, start the match
            if (allPlayersReady)
            {
                StartMatch();
            }
        });

    }

   

    void StartMatch()
    {
        // Start the match
        SceneManager.LoadScene("MainScene");
    }
}
