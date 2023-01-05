//Handle things happen when player click play
//Reminder: This script starts once 2 to 4 players have submitted to play list
//The game is not made for > 4 players. However game allow overfill anyways. Remember to warn players only 2-4 is recommended.
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.SocialPlatforms.Impl;
using VRC.SDKBase;
using VRC.Udon;
public class EventClickPlay : UdonSharpBehaviour
{
    //Get list of registered players and see if there are enough players to play
    [SerializeField] private ScoreBoard scoreBoard;
    public Text[] registerList;
    
    
    //Set spawnpoint location
    private Transform playPosition;

    void Start()
    {
        //Find and set position of spawn point
        playPosition = GameObject.Find("SpawnPoint").transform;
        
        //Get list of registers stored in ScoreBoard
        registerList = scoreBoard.registerText;
    }
    
    
    //Player clicked play button. Teleport all players to start!
    public override void Interact()
    {
        if (Networking.IsMaster == true)
        {
            Debug.Log("Player clicked. Player is Master. Game should start.");
            scoreBoard.StartGame();
        }
        else
        {
            Debug.Log("Player is not master");
        }
    }

}
