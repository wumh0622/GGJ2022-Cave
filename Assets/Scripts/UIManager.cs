using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject debugConsole;

    public Text scoreText;


    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            if(!debugConsole.activeInHierarchy)
            {
                debugConsole.SetActive(true);
            }
            else
            {
                debugConsole.SetActive(false);
            }
        }

        if(scoreText)
        {
            scoreText.text = ScoreManager.instance.currentScore.ToString();
        }
    }
}
