using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public List<GameObject> players = new List<GameObject>();

    public Transform thisTransform;
    public GameObject playerPrefab;
    public GameObject uncontrollablePlayerPrefab;

    public GameObject CreatePlayer(float x, float y, float z, string name)
    {
        GameObject toCreate = Instantiate(playerPrefab, new Vector3(x, y, z), Quaternion.identity, thisTransform);
        toCreate.name = name;
        players.Add(toCreate);
        Debug.LogWarning("client player created");
        return toCreate;
    }

    public GameObject CreateUncontrollablePlayer(float x, float y, float z, string name)
    {
        GameObject toCreate = Instantiate(uncontrollablePlayerPrefab, new Vector3(x, y, z), Quaternion.identity, thisTransform);
        toCreate.name = name;
        //toCreate.AddComponent<PlayerReferencer>();
        players.Add(toCreate);
        Debug.LogWarning("server player created");
        return toCreate;
    }

    public GameObject RemovePlayer(string name)
    {
        if(name != null || name != string.Empty)
        {
            GameObject playerToHandle = GetPlayerTransformByName(name).gameObject;
            players.Remove(playerToHandle);

            Debug.LogWarning("player " + playerToHandle.name + " removed!");
            Destroy(playerToHandle);
        }

        Debug.LogWarning("player requested to remove is null (set the name of the player when calling the RemovePlayer function!)");
        return null;
    }

    public Transform GetPlayerTransformByName(string name)
    {
        for(int i = 0; i < this.transform.childCount; i++)
        {
            if (name == this.transform.GetChild(i).name)
            {
                return this.transform.GetChild(i).transform;
            }
        }

        return null;
    } 
}
