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

    public int maxMapCount = 10;

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
        mapManager.onMapCreate.AddListener(OnRoomCreate);
        player.SetActive(true);
    }

    void OnRoomCreate(int count)
    {
        if(count + 1 > maxMapCount)
        {
            //mapManager.CreatMap_StartEnd();
        }
    }

}
