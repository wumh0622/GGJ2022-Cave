using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject Menu;
    public GameObject tutorial;

    bool tutorialMode = false;

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown)
        {
            if(!tutorialMode)
            {
                Menu.SetActive(false);
                tutorial.SetActive(true);
                tutorialMode = true;
            }
            else
            {
                SceneManager.LoadScene(1);
            }

        }
    }
}
