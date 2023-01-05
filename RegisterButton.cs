
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;


public class RegisterButton : UdonSharpBehaviour
{
    [SerializeField] public ScoreBoard scoreBoard;
    [UdonSynced] public int playerID;
    
    public int maxRegister = 4;

    //bool isRegistered;
    //private string nameToRegister;

    //When player interact with the button
    //Setup register list

    public override void Interact()
    {
        //Send player name to ScoreBoard
        //nameToRegister = Networking.LocalPlayer.displayName;
        Debug.Log("Player clicked name is: " + Networking.LocalPlayer.displayName);

        playerID = VRCPlayerApi.GetPlayerId(Networking.LocalPlayer);
        
        Debug.Log("Sending network event from clicker");
        
        
        //SendRegistrationRequest();
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SendRegistrationRequest");
    }

    private void SendRegistrationRequest()
    {
        //Send player name to scoreboard to check if player is registered yet
        scoreBoard.CheckRegister(playerID);
    }
}
