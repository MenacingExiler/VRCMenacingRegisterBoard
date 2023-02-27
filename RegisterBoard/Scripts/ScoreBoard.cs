//Get instance ranking, live scoreboard
using UdonSharp;
//using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

using VRC.Udon;

public class ScoreBoard : UdonSharpBehaviour
{
    #region PUBLIC_METHODS
    void Start()
    {
        InitRegisterList();
        UpdatePlayerList();

        //Give crown to Ko-fi user
        /*
        if (Networking.LocalPlayer.displayName == ("MenacingExiler"))
        {
            //set crown as child of player (cant do that lol)
            Debug.Log("Ko-Fi supporter joined");
        }
        */
    }

    /// <summary>
    /// Updates Player List to include all current players in the session.
    /// </summary>
    public void UpdatePlayerList()
    {
        //refresh player list
        playerList = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
        VRCPlayerApi.GetPlayers(playerList);

        //construct and set the playerList string on the canvas
        allPlayerList.text = "";
        foreach(var player in playerList)
        {
            allPlayerList.text += player.displayName + "\n";
        }
    }

    /// <summary>
    /// Linearly searches through array to see if displayName matches
    /// </summary>
    /// <param name="_player">The player be searched for in the registeredPlayers array</param>
    /// <returns>(true) Player Found</returns>
    /// <remarks>WARNING: This could be and SHOULD be optimized if the player group gets larger.</remarks>
    private bool CheckRegister(VRCPlayerApi _player)
    {
        bool _ret = false;
        for (int i = 0; i < registerCount; i++)
        {
            _ret |= registeredPlayers[i].displayName == _player.displayName;
            if (_ret) return _ret;
        }

        return _ret;
    }

    /// <summary>
    /// Attempts to add the requested player corresponding to the RegisterID
    /// from the Networked variable to the board.
    /// </summary>
    public void UpdateRegistration()
    {
        // Attempt to retrieve the corresponding player tied to that ID.
        VRCPlayerApi _player = VRCPlayerApi.GetPlayerById(NET_RegisterID);
        if (_player != null)
        {
            // Check if the player is already in the list.
            if (!CheckRegister(_player)) //not in list yet
            {
                registeredPlayers[registerCount] = _player;
                registerText[registerCount].text = _player.displayName;
                registerCount++;
            }
            else //already in list. Remove player from list.
            {
                
                //reorganize it
                
                
                registerCount--;
            }
        }

    }

    //** Did not touch I'll let you handle this.
    // Teleport all registered players to designated play area
    public void StartGame()
    {
        /*
        //Array of players playing likely have nulls. Organize so null is not included.
        for (int i = 0; i > 4; i++)
        {
            if (registeredPlayers[i] != null)
            {
                playerPlayingCount++;
            }

        }
        */

        //Check if there is enough players
        if (registerCount < 2)
        {
            Debug.Log("Not enough players!");
            return;
        }
        else
        {
            Debug.Log("Game have enough players!");
        }
        
        /*
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
        */

        //foreach (VRCPlayerApi player in playerToTeleport)
        //{
        //player.TeleportTo(teleportToGameStart, Quaternion.identity, VRC_SceneDescriptor.SpawnOrientation.Default);
        //}

        //Start game in TurnManager
        //turnManager.StartSetup(playerPlayingCount);
    }

    /////***** GET/SET HELPERS *****///// 
    public Text[] RegisterText
    {
        get { return registerText; }
    }
    #endregion

    #region PRIVATE_METHODS
    private void InitRegisterList() { registeredPlayers = new VRCPlayerApi[maxReg]; }
    #endregion

    #region VRC_EVENT_OVERRIDES
    /* If player joins, update the player list and force 'late join' serialization */
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.IsOwner(gameObject))
            RequestSerialization();

        UpdatePlayerList();
    }

    //** Did not touch I'll let you handle this.
    // If player leave, update the board to avoid ghost register
    /*public override void OnPlayerLeft(VRCPlayerApi player)
    {
        //base.OnPlayerLeft(player);
        registeringP = player; //set player to variable that get checked later
        UpdatePlayerList();//send data of player that left to remove it
    }*/

    // Used for manual synchronization with the RequestSerialization call.
    public override void OnDeserialization()
    {
        /* Due to the nature of how Deserialization works we can never be 
           certain when the Networked variable changes UNLESS if we directly
           compare it with a previous value the same variable */
        if (NET_RegisterID != prevRegisterID)
        {
            prevRegisterID = NET_RegisterID;
            UpdateRegistration();
        }
    }
    #endregion

    #region PUBLIC_DATA
    public int maxPlayerCount = 16;         // max amount of players that can join the session
    public int maxReg = 4;                  // maximum amount of player that can be registered
    #endregion

    #region PRIVATE_DATA
    //[SerializeField] TurnManager turnManager;
    //Get list of Ko-Fi supporters to give crown to
    //see displayName in docs

    [SerializeField] Transform teleportToGameStart;         // Destination of where player get teleported to after hitting play.
    [SerializeField] Text allPlayerList;                    // Store all players in text.
    [SerializeField] VRCPlayerApi[] playerToTeleport;       // Organized list without null.
    [SerializeField] Text[] registerText = new Text[4];     // Store registered player names.
    private int registerCount = 0;                          // originally was orderToSet.

    // **Redundant can use VRCPlayerAPI.GetPlayerCount() instead.**
    //[UdonSynced] private int playerPlayingCount = 0;      // Number of active 'playing' players.

    //private VRCPlayerApi[] playerList = new VRCPlayerApi[16]; 
    private VRCPlayerApi[] playerList;                      // PlayerAPI array that contains all current 'playing' players. (Instance capacity)

    //private VRCPlayerApi[] registeredPlayers = new VRCPlayerApi[4]; 
    private VRCPlayerApi[] registeredPlayers;               // PlayerAPI array that contains all currently registered players. (must be 2 to 4)
    #endregion

    //** Since Networking stuff is nasty stuff I'm gonna separate it from the rest of the class ;p **//
    #region NETWORKED_METHODS

    /// <summary>
    /// Used to send PlayerIDs across the network and register them to the board.
    /// </summary>
    /// <param name="_id">VRCPlayerAPI Player ID to register</param>
    public void NET_Register(int _id)
    {
        SetOwner();
        NET_RegisterID = _id;

        /* Since owner doesn't have a deserialization call, you must 
           call it manually for only the owner */
        RequestSerialization();
        SendCustomNetworkEvent(NetworkEventTarget.Owner, "UpdateRegistration");
    }

    /// <summary>
    /// Sets ownership of ScoreBoard to the current LocalPlayer
    /// </summary>
    private void SetOwner()
    {
        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }
    }
    #endregion

    #region NETWORKED_DATA
    [UdonSynced] public int NET_RegisterID = -1;    // Networked variable that is used to indicate the current playerID to be registered.
    private int prevRegisterID = -1;                // Previous value of the networked variable; used to determine if a change occured for manual sync.

    //private int orderToSet        //*Changed to registerCount
    //private bool newRegister;     //*Some what redundant since we can simply set NET_RegisterID
                                    // to -1 and have the same effect for less memory
    #endregion
}