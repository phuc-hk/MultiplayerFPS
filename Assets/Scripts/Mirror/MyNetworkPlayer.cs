using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SyncVar][SerializeField] string playerName = "Empty";
    [SyncVar][SerializeField] Color playColor = Color.white;

    public void SetPlayerName(string newName) { playerName = newName; }

    public void SetPlayerColor(Color newColor) { playColor = newColor; }
}
