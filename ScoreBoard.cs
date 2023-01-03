//Get instance ranking, live scoreboard
using UdonSharp;
//using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class ScoreBoard : UdonSharpBehaviour
{
    [SerializeField] private TurnManager turnManager;
    //Get list of Ko-Fi supporters to give crown to
    //see displayName in docs

    //Destination of where player get teleported to after hitting play
    public Vector3 teleportToGameStart;
    
    //maximum amount of player that can be registered
    public int maxReg = 4;
    
    //Get all player lists
    //
    //all players possible in instance?
    private VRCPlayerApi[] playerList = new VRCPlayerApi[16]; 
    //Store all players in text
    public Text allPlayerList;
    //
    //put registered players here (must be 2 to 4)
    private VRCPlayerApi[] registeredPlayers = new VRCPlayerApi[4]; 
    //Store array with only registered players.
    //
    //number of players
    [UdonSynced] int playerPlayingCount = 0; 
    public VRCPlayerApi[] playerToTeleport; //Organized list without null
    
    //Store registered player names
    public Text[] registerText = new Text[4];
    
    
    
    //int uniquePlayerCount;
    //int winCount[uniquePlayerCount];
    
    //Variables for saving syncs because UDON CANNOT PASS VARIABLEkrdliesfnkSGhwk
    //CheckRegister()
    private VRCPlayerApi registeringP;
    //UpdateRegistration()
    private int orderToSet;
    private bool newRegister;
    void Start()
    {
        //For debugging purpose
        //registeringP = Networking.LocalPlayer;
        
        VRCPlayerApi.GetPlayers(playerList);
        foreach (VRCPlayerApi player in playerList)
        {
            if(player == null) continue;
            Debug.Log("Players in instance: " + player.displayName);
        }
        
        SendCustomEventDelayedSeconds("UpdatePlayerList", 2);

        //Give crown to Ko-fi user
        /*
        if (Networking.LocalPlayer.displayName == ("MenacingExiler"))
        {
            //set crown as child of player (cant do that lol)
            Debug.Log("Ko-Fi supporter joined");
        }
        */
    }

    //Updating list of players
    void UpdatePlayerList()
    {
        //refresh player list
        VRCPlayerApi.GetPlayers(playerList);
        
        //renew player list and player list text
        for (int i = 0; i < registerText.Length; i++)
        {
            if (registerText[i].text == registeringP.displayName)
            {
                newRegister = false;
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "UpdateRegistration");
                //UpdateRegistration();
                
            }
        }

        //Debug.Log("Updating list of player names");
        //Updating text of registered players
        for (int i = 0; i < playerToTeleport.Length; i++)
        {
            registerText[i].text = playerToTeleport[i].displayName;
        }
    }
    
    //If player joins, update the player list
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        string allPlayers;
        //refresh player list
        VRCPlayerApi.GetPlayers(playerList);

        //Add player to player list
        foreach (VRCPlayerApi playerNew in playerList)
        {
            if(playerNew == null) continue;
            allPlayerList.text = allPlayerList + playerNew.displayName + "\n";
        }
    }

    //If player leave, update the board to avoid ghost register
    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        //base.OnPlayerLeft(player);
        registeringP = player; //set player to variable that get checked later
        UpdatePlayerList();//send data of player that left to remove it
        
        
    }
    
    //New player registered into board.
    public void CheckRegister(int iDToCheck)
    {
        VRCPlayerApi playerToRegister;
        playerToRegister = VRCPlayerApi.GetPlayerById(iDToCheck);
        //make player registering into variable accessible in this script
        registeringP = playerToRegister;
        
        for (int i = 0; i < 4; i++)
        {
            //Debug.Log(registerText[i] + " debug");
            
            //Check if player is already in the list.
            //
            //Player was already registered and clicking again will unregister it. Reset to default.
            if (registerText[i].text == registeringP.displayName)
            {
                Debug.Log("Player unregistered!");
                orderToSet = i;
                newRegister = false;
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "UpdateRegistration");
                //UpdateRegistration();
                Debug.Log("Name should be reset");
                break;
            }
            //Player is not on list. Fill player name to it.
            else if (registerText[i].text == "Empty")
            {
                //Debug.Log("empty text detected filling in");
                //passRegister[i] = nameToRegister;
                orderToSet = i;
                newRegister = true;
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "UpdateRegistration");
                //UpdateRegistration();
                Debug.Log("New name should be registered");
                break;
            }

            //If code reaches here, it means list is full.
            //idk what to do here for now
            Debug.Log("Player list is full");
        }
    }
    
    //Receive name that needs to be updated to text board and memory then organize it on list
    public void UpdateRegistration()
    {
        Debug.Log("Registering...");
        //The name did not exist before. Add it to the list.
        if (newRegister)
        {
            //Update registered players list
            //registerList[orderToSet].text = (orderToSet + 1) + ". " + nameToSet;
            registerText[orderToSet].text = registeringP.displayName; //Update board text
            //registeredPlayers[orderToSet] = playerToSet; //Update memory
            
            Debug.Log("Filled!");
        }
        //Player clicked again to unregister. Remove it from the list.
        else
        {
            //Update registered to empty
            registerText[orderToSet].text = "Empty";
            //registeredPlayers[orderToSet] = null; //Update memory
            //reorganize the order
            if (orderToSet < 4)
            {
                for (int i = orderToSet; i < 4; i++)
                {
                    //Reorganize text
                    registerText[orderToSet].text = registerText[orderToSet + 1].text;
                    registerText[orderToSet + 1].text = "Empty";
                    
                    //Reorganize player register list
                    //registeredPlayers[orderToSet] = registeredPlayers[orderToSet + 1];
                    //registeredPlayers[orderToSet + 1] = null;
                }
            }
            Debug.Log("Unfilled!");
        }
        //scoreBoard.registerList = registerUpdate;
    }
    
    //Sync to late joiners
    public override void OnDeserialization()
    {
        //UpdateRegistration();
    }

    //Teleport all registered players to designated play area
    public void StartGame()
    {
        
        //Array of players playing likely have nulls. Organize so null is not included.
        for (int i = 0; i > 4; i++)
        {
            if (registeredPlayers[i] != null)
            {
                playerPlayingCount++;
            }
            
        }
        
        //Check if there is enough players
        if (playerPlayingCount < 2)
        {
            Debug.Log("Not enough players!");
            return;
        }
        else
        {
            Debug.Log("Game have enough players!");
        }
        
        //Set number of players playing same as number registered
        VRCPlayerApi[] playerToTeleport = new VRCPlayerApi[playerPlayingCount];
        
        //shrink arrays of players playing to go down to number of players playing
        for (int i = 0; i < playerPlayingCount; i++)
        {
            playerToTeleport[i] = registeredPlayers[i];
            
            //Then teleport matching players to play area
            if (Networking.LocalPlayer == playerToTeleport[i])
            {
                Networking.LocalPlayer.TeleportTo(teleportToGameStart, Quaternion.identity);
            }
        }
        
        //foreach (VRCPlayerApi player in playerToTeleport)
        //{
            //player.TeleportTo(teleportToGameStart, Quaternion.identity, VRC_SceneDescriptor.SpawnOrientation.Default);
        //}
        
        //Start game in TurnManager
        turnManager.StartSetup(playerPlayingCount);
    }

    
}