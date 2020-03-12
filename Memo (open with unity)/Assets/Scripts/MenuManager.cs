using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour
{
    private Slider cardsSlider;
    private Text cardsText;

    public static int CARDS_NUM = 1;

    void Start()
    {
        cardsSlider = GameObject.Find("NumOfCards Slider").GetComponent<Slider>();
        cardsText = GameObject.Find("CardsNum Text").GetComponent<Text>();
    }

    public void TriggerMenuBehavior(int i)
    {
        switch (i)
        {  
            case 0: SceneManager.LoadScene("Game"); break; 
            case 1: Application.Quit(); break;
            default: break;
        }

    }
    void Update()
    {
        CARDS_NUM = (int)cardsSlider.value;
        cardsText.text = CARDS_NUM.ToString();
    }
}
