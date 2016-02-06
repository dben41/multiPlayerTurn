using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class GameStateManager : NetworkBehaviour
{
    //an enum, Player 1 = 1, Player 2 = 2.
    public static int playerTurn = -1;
    public static ArrayList connectedPlayers = new ArrayList(); 

    //public static GameStateManager instance;

    //call this every time a player connects
    public static void addPlayer(int networkId)
    {
        connectedPlayers.Add(networkId);
    }

    //call this every time a player disconnects
    public static void removePlayer(int networkId)
    {
        connectedPlayers.Remove(networkId);
    }

    public static int getPlayerTurn()
    {
        if (playerTurn == -1) playerTurn = (int)connectedPlayers[0];
        return playerTurn;
    }

    /*
     * Increment the player's turn
     */
    public static void nextTurn()
    {
        int currentIndex = connectedPlayers.IndexOf(playerTurn);
        if (currentIndex == connectedPlayers.Count - 1) playerTurn = (int)connectedPlayers[0];
        else playerTurn = (int)connectedPlayers[currentIndex + 1];
    }
}
