using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;

public class TowerSpawnLoc : NetworkBehaviour
{
    [SerializeField]
    GameObject towerPrefab;
    [SerializeField]
    GameObject towerSpawn;


    public override void OnStartServer()
    {
        SpawnTower();
    }

    void SpawnTower()
    {
        GameObject go = GameObject.Instantiate(towerPrefab, towerSpawn.transform.position, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(go);
    }



    // public override void OnStartLocalPlayer()


}