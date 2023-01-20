
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;


public class RegisterButton : UdonSharpBehaviour
{
    #region PUBLIC_METHODS
    public override void Interact()
    {
        //Send player name to ScoreBoard
        //nameToRegister = Networking.LocalPlayer.displayName;
        Debug.Log("Player clicked name is: " + Networking.LocalPlayer.displayName);

        //playerID = VRCPlayerApi.GetPlayerId(Networking.LocalPlayer);
        scoreBoard.NET_Register(VRCPlayerApi.GetPlayerId(Networking.LocalPlayer));

        Debug.Log("Sending network event from clicker");
    }
    #endregion

    #region PRIVATE_DATA
    [SerializeField] ScoreBoard scoreBoard;
    #endregion

    //bool isRegistered;
    //private string nameToRegister;
    //When player interact with the button
    //Setup register list

    //[UdonSynced] public int playerID;
    //public int maxRegister = 4; //** Redundant, use scoreBoard.maxReg instead.
}
