using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public GameObject player;
    public int moveScoreMutiply = 1;

    public int currentScore;

    float currentMoveDistance;
    int moveScore;
    int gameScore;
    Vector3 initPlayerPos;

    private void Awake()
    {
        if(!instance)
        {
            instance = this;
        }
    }

    void Start()
    {
        initPlayerPos = player.transform.position;
    }

    void Update()
    {
        currentMoveDistance = player.transform.position.x - initPlayerPos.x;
        moveScore = (int)currentMoveDistance * moveScoreMutiply;
        currentScore = moveScore + gameScore;
    }

    public void AddScore(int score)
    {
        gameScore += score;
    }
}
