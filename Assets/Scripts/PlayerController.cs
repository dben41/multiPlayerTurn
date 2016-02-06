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
    public int turn;
    public uint player;
    private NetworkIdentity objNetId;


    Text playerText;


    void Start()
    {
        player =  this.GetComponent<NetworkIdentity>().netId.Value;

        //add player id to network
        GameStateManager.addPlayer((int)player);

        //get initial player
        turn = GameStateManager.getPlayerTurn();

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
            CmdGetTurn();
            changeTurnText();
        }
    }

    void CheckIfClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //update the variable
            CmdGetTurn();
            //check if it's my turn, if not, exit out
            if (turn != player) return;
            CmdChangeTurn();

            //change the color
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

        
        //updates state on the server
        [Command]
        void CmdChangeTurn()
        {
            if (turn == player) GameStateManager.nextTurn();
            turn = GameStateManager.getPlayerTurn();
            //syncs this all clients connected on server 
            RpcUpdateTurn(turn);
        }
        
       [ClientRpc]
        void RpcUpdateTurn(int i)
        {
            turn = i;
            changeTurnText();
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

    /*
     * Changes the turn text of the player
     */
    void changeTurnText()
    {
        if (turn == player)
            playerText.text = "Your turn. ";
        else if (turn != player)
            playerText.text = "Opponent's turn. ";
    }
    
}