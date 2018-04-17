using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{

    public GameObject gameManager;                          //GameManager prefab to instantiate.

    void Awake()
    {
        if (GameManager.instance == null)                           //Check if a GameManager has already been assigned to static variable GameManager.instance or if it's still null
        {
            Instantiate(gameManager);                           //Instantiate gameManager prefab
        }
    }
}