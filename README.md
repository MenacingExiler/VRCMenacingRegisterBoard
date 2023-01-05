# VRCMenacingRegisterBoard
 A register board for VRChat written in UdonSharp (Based on C#).
 
### What is it?
 
A UdonSharp script for registering board system that takes registered player and teleport player to play area when ready.

### How it works?
 
If player want to register to play later, player can hit register button and hit again to unregister. When player hit and stay registered, their name will be shown on board. Once all players are ready, they can hit the play button and then everyone get teleported to play area.

## How to try?

In the repository there is a prefab included as example using 3 of the scripts. In those 3 scripts those are: ScoreBoard for managing player board, RegisterButton to receive information of person who interact with register button (in that case the object holding the script) and EventClickPlay to check if there are enough players. The example prefab is hard coded to only run when there are 2-4 players registered and not start if condition is not met. This function is not working yet on network please see addendum for active issues.

### Addendum

The following project is a work in progress and does not fully work yet. In theory at current state this should work locally. In the future, the code should work over VRChat network. Inside folder includes a prefab used as example. The following is assigned:
Green button: Register button
Blue: Play button.
Requirements: >2 players, 2-4 players to play.

Fell free to report issues and contribute fixes. Make sure you are following [license](LICENSE)!

### License

This project is licensed under Apache License 2.0 open source license. Any contribution to this repository means you agree to follow the same [license](LICENSE) as notified.
