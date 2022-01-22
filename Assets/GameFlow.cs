using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum GameFlowType
{
    GameStart,
    InGame,
    End
}

public class GameFlow : MonoBehaviour
{

    public static GameFlow instance;

    public MapManager mapManager;
    public GameObject player;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        player.SetActive(false);
    }

    public void Start()
    {
        mapManager.CreatMap_Start();
        player.SetActive(true);
    }

}
