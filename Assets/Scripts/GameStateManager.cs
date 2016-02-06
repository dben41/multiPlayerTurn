using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class GameStateManager : NetworkBehaviour
{
    //an enum, Player 1 = 1, Player 2 = 2.
    private static int playerTurn = 1;
    private static HashSet<int> connectedPlayers = new HashSet<int>(); 

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
        return playerTurn;
    }

    public static void setPlayerTurn(int i)
    {
        playerTurn = i;
    }

}
