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
    public PlayerController player;
    public DeadZone[] deadZones;

    public int maxMapCount = 10;
    bool lavaIsStop = false;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        player.gameObject.SetActive(false);
    }

    public void Start()
    {
        mapManager.CreatMap_Start();
        mapManager.onMapCreate.AddListener(OnRoomCreate);
        player.gameObject.SetActive(true);
        foreach (DeadZone item in deadZones)
        {
            item.GameStart();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(lavaIsStop)
            {
                lavaIsStop = false;
                foreach (DeadZone item in deadZones)
                {
                    item.GameStart();
                }
            }
            else
            {
                StopLava();
            }
            
        }
    }

    public void Dead()
    {
        player.enabled = false;
        Invoke("StopLava", 10.0f);
    }

    public void StopLava()
    {
        foreach (DeadZone item in deadZones)
        {
            item.GameStop();
        }
        lavaIsStop = true;
    }

    void OnRoomCreate(int count)
    {
        if(count + 1 > maxMapCount)
        {
            //mapManager.CreatMap_StartEnd();
        }
    }

}
