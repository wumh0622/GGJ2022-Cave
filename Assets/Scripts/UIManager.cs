using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct BuffIcon
{
    public string buffName;
    public Sprite sprite;
}

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject debugConsole;
    public GameObject GameOverPanel;

    public Text scoreText;
    public Image buffIcon;

    public BuffIcon[] buffIcons;
    Dictionary<string, Sprite> iconData = new Dictionary<string, Sprite>();
    float itemTime;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }

        if(buffIcons.Length > 0)
        {
            foreach (var item in buffIcons)
            {
                iconData.Add(item.buffName, item.sprite);
            }
        }
        buffIcon.gameObject.SetActive(false);
    }

    void Start()
    {
        GameFlow.instance.player.onGetItem.AddListener(OnGetItem);
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

        if(GameFlow.instance.player._itemEffectSec > 0)
        {
            buffIcon.fillAmount = GameFlow.instance.player._itemEffectSec / itemTime;
        }
    }

    void OnGetItem(string itemName, float time)
    {
        buffIcon.gameObject.SetActive(true);
        buffIcon.sprite = iconData[itemName];
        itemTime = time;
    }

    public void ShowGameOver()
    {
        GameOverPanel.SetActive(true);
    }
}
