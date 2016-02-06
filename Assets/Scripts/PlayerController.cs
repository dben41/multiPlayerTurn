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
    //determines which player's turn it is
    [SyncVar]
    public int turn;
    //determines which player the current player is
    public uint player;
    private NetworkIdentity objNetId;

    Text playerText;
    Text identityText;

    void OnDestroy()
    {
        GameStateManager.removePlayer((int)player);
    }

    void Start()
    {
        //The network manager assigns network identities dynamically, 
        //so just use this value as the current player's id
        player =  this.GetComponent<NetworkIdentity>().netId.Value;

        //add player id to network
        GameStateManager.addPlayer((int)player);

        //get the current turn
        turn = GameStateManager.getPlayerTurn();

        //initialize player text, tells who's turn it is
        playerText = GameObject.FindGameObjectWithTag("PlayerTurnText").GetComponent<Text>();
        playerText.text = "It's working";
        //tells you which player you are
        identityText = GameObject.FindGameObjectWithTag("IdentityText").GetComponent<Text>();
        identityText.text = "Player " + turn;
        identityText.color = new Color(Random.value, Random.value, Random.value, Random.value);
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        { 
            //checks to see if the player clicked
            CheckIfClicked();
            //makes sure that the player turn text is synced
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
  
    //updates state on the server
    [Command]
    void CmdChangeTurn()
    {
        if (turn == player) GameStateManager.nextTurn();
        turn = GameStateManager.getPlayerTurn();
        //syncs this all clients connected on server 
        RpcUpdateTurn(turn);
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

    [Command]
    void CmdChangeColor(GameObject go, Color c)
    {
        objNetId = go.GetComponent<NetworkIdentity>();
        objNetId.AssignClientAuthority(connectionToClient);
        RpcUpdateTower(go, c);
        objNetId.RemoveClientAuthority(connectionToClient);
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