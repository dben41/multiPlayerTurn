using UnityEngine;

using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    [SyncVar]
    public Color cubeColor;
    [SyncVar]
    private GameObject objectID;

    [SyncVar]
    private int turn;

    public SyncListUInt serverNetworkId;// = new SyncListUInt();

    private uint player;
    private NetworkIdentity objNetId;


    Text playerText;


    void Start()
    {
        player =  this.GetComponent<NetworkIdentity>().netId.Value;

        //add player id to network
        //serverNetworkId.Add(player);

        CmdStartList();
        CmdAddToList();

        //initialize player text
        playerText = GameObject.FindGameObjectWithTag("PlayerTurnText").GetComponent<Text>();

        playerText.text = "It's working";
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        { 
            CheckIfClicked();
        }



    }
    void CheckIfClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //update the variable
            CmdGetTurn(); //the rpc method would be more robust and reduce traffic
            if (turn != player) return;
            CmdChangeTurn();

            //Debug.Log("Player  " + player + ": turn after CmdChangeTurn gets called: " + turn);
            //check if it's my turn, if not, exit out
            //else exit out
            Debug.Log("");
            objectID = GameObject.FindGameObjectsWithTag("Tower")[0];
            cubeColor = new Color(Random.value, Random.value, Random.value, Random.value);
            CmdChangeColor(objectID, cubeColor);
        }
    }

    [Command]
    void CmdChangeColor(GameObject go, Color c)
    {
        objNetId = go.GetComponent<NetworkIdentity>();
        objNetId.AssignClientAuthority(connectionToClient);
        RpcUpdateTower(go, c);
        objNetId.RemoveClientAuthority(connectionToClient);
    }

    [Command]
    void CmdStartList()
    {
        if(serverNetworkId == null) serverNetworkId = new SyncListUInt();
    }

    [Command]
    void CmdAddToList()
    {
        serverNetworkId.Add(player);
    }

    private uint getNextPlayer() {
        int currentPlayerIndex = serverNetworkId.IndexOf(player);
        int numberOfPlayers = serverNetworkId.Count;
        uint nextPlayer;

        if (currentPlayerIndex == numberOfPlayers - 1) nextPlayer = serverNetworkId[0];
        else nextPlayer = serverNetworkId[currentPlayerIndex+1];
        return nextPlayer;
    }


    //updates state on the server
    [Command]
    void CmdChangeTurn()
    {

        if(turn == player) GameStateManager.setPlayerTurn((int)getNextPlayer());


        //if (turn == player) GameStateManager.setPlayerTurn(2);
        //else GameStateManager.setPlayerTurn(1);
        turn = GameStateManager.getPlayerTurn();
        //syncs this all clients connected on server 
        RpcUpdateTurn(turn);
    }

    [ClientRpc]
    void RpcUpdateTurn(int i)
    {
        turn = i;
        //playerText.text = "Player " + turn + "'s turn.";
        if (turn == player)
            playerText.text = "Your turn. ";
        else if(turn != player)
            playerText.text = "Opponent's turn. ";
    }
    

    [Command]
    void CmdGetTurn()
    {
        turn = GameStateManager.getPlayerTurn();
    }

    /*
     * Being sent from server to
     */
    [ClientRpc]
    void RpcUpdateTower(GameObject go, Color c)
    {
        go.GetComponent<Renderer>().material.color = c;
    }
}