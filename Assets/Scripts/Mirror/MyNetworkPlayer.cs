using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SyncVar][SerializeField] string playerName = "Empty";
    [SyncVar][SerializeField] Color playColor = Color.white;

    #region Server
    [Server]
    public void SetPlayerName(string newName) { playerName = newName; }

    [Server]
    public void SetPlayerColor(Color newColor) { playColor = newColor; }

    [Command] private void SetNewName(string newName) 
    {       
        playerName = newName;
        RcpSetNewName();
    }

    

    #endregion

    #region Client
    [ContextMenu("Set New Name")]
    private void SetNewName()
    {
        SetNewName("New Player Name");
    }

    [ClientRpc]
    private void RcpSetNewName()
    {
        Debug.Log(playerName);
    }

    #endregion
}
