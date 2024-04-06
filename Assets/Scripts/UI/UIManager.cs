using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void SignOutButton()
    {
        Authentication.Instance.SignOut();
    }

    public void FindMatchButton()
    {
        //MatchMaking.Instance.FindMatch();
        MatchMakingManager.Instance.FindMatch();
    }

    //public void ReadyButton()
    //{
    //    MatchMaking.Instance.PlayerReady();
    //}
}
