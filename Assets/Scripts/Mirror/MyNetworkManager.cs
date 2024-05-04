using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("Client connected to server");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        MyNetworkPlayer player =  conn.identity.GetComponent<MyNetworkPlayer>();
        player.SetPlayerName($"Player {numPlayers}");

        Color color = new(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        player.SetPlayerColor(color);
        Debug.Log($"There are {numPlayers} connected");
    }
}
