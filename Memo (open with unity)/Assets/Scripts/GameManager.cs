
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.InputSystem;

public static class ALL
{
    public static int value = 1;
}

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private Canvas canvas;
    //public Sprite sprite;
    private Text cardsText;
    private Text missedNumText;
    private Text wellPlayedText;
    private Button goToMenuButton;


    [SerializeField]
    private InputAction GoToMenuAction;
    private Object[] textures;
    private Sprite[] cardFaces;
    private Sprite cardBack;
    private GameObject[] cards;


    private int POINTS_NUM = 0;
    private int MISSED_NUM = 0;
    private bool hasStarted = false;
    private int count = 0;

    public static bool ALL_CARDS_INITIALIZED = false;
    // Start is called before the first frame update

    void Awake()
    {
        GoToMenuAction.performed += GoToMenu;
        GoToMenuAction.Enable();
    }
    void Start()
    {
        //GoToMenuAction.performed += GoToMenu;
        wellPlayedText = GameObject.Find("Well played Text").GetComponent<Text>();
        wellPlayedText.enabled = false;

        cardsText = GameObject.Find("CardsNum Text").GetComponent<Text>();
        cardsText.text = (MenuManager.CARDS_NUM*2).ToString();
       
        missedNumText = GameObject.Find("MissedNum Text").GetComponent<Text>();
        goToMenuButton = GameObject.Find("GoToMenu Button").GetComponent<Button>();
        goToMenuButton.onClick.AddListener(delegate { GoToMenu(); });

        textures = Resources.LoadAll("Cards", typeof(Texture2D));
        Shuffle(textures);
        System.Array.Resize(ref textures, MenuManager.CARDS_NUM);

        textures = textures.Concat(textures).ToArray();
        Shuffle(textures);
        cardFaces = new Sprite[textures.Length];
        for (int i = 0; i < cardFaces.Length; i++)
        {
            cardFaces[i] = ConvertToSprite((Texture2D)textures[i]);
        }
        cardBack = LoadNewSprite("revers");

        StartCoroutine(InitializeCards());
    }

    private IEnumerator InitializeCards()
    {
        cards = new GameObject[cardFaces.Length];
        int xpos = 80;
        int ypos = Screen.height - 150;
        int numElemPerSide = 1;
        for (int i = 0; i < cards.Length; i++)
        {

            cards[i] = new GameObject();
            Image image = cards[i].AddComponent<Image>();
            
            cards[i].AddComponent<Button>();
            image.sprite = cardBack;

            image.rectTransform.sizeDelta = new Vector2(60, 90);

            cards[i].transform.SetParent(canvas.transform, false);
            
            if (ypos - 50 < 0)
            {
                ypos = Screen.height - 150;
                xpos += 80;
            }
        

            cards[i].transform.position = new Vector3(xpos, ypos, 0);

            ypos -= 100;

            cards[i].AddComponent<Card>();
            Card card = cards[i].GetComponent<Card>();
            card.name = textures[i].name;
            card.cardFace = cardFaces[i];
            card.cardBack = cardBack;
            int temp = i;

            card.button = cards[i].GetComponent<Button>();
            card.button.onClick.AddListener(delegate { card.FlipCard(); });
            cards[i].GetComponent<Button>().enabled = false;
            cards[i].AddComponent<Outline>();

            Outline outline = cards[i].GetComponent<Outline>();
            outline.enabled = false;
            outline.effectDistance = new Vector2(3, 3);
            outline.effectColor = new Color(0.3f, 0.7f, 1, 0.9f);

            yield return new WaitForSeconds(0.1f);
        }
        ALL_CARDS_INITIALIZED = true;
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].GetComponent<Button>().enabled = true;
        }
    }


    void Update()
    {
        if (ALL_CARDS_INITIALIZED && Input.GetMouseButtonUp(0) && !Card.DO_NOT)
        {
            CheckCards();
        }
       
    }

    private void GoToMenu(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("Menu");
    }
    private void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private void CheckCards()
    {
        List<int> list = new List<int>();
        for (int i = 0; i < cards.Length; i++)
        {
            if (!cards[i].GetComponent<Card>().isHidden && !cards[i].GetComponent<Card>().isDone)
                list.Add(i);
        }
        if (list.Count == 2)
            CompareCards(list);
    }

    private void CompareCards(List<int> list)
    {
        Card.DO_NOT = true;
        Card card1 = cards[list[0]].GetComponent<Card>();
        Card card2 = cards[list[1]].GetComponent<Card>();

        if (card1.name.Equals(card2.name))
        {
            POINTS_NUM += 10;
            
            card1.isDone = true;
            card1.image.color = new Color(0.7f,0.7f,0.7f,0.7f);     
            card2.isDone = true;
            card2.image.color = new Color(0.7f, 0.7f, 0.7f, 0.7f);

            Card.DO_NOT = false;
            MenuManager.CARDS_NUM--;
            cardsText.text = (MenuManager.CARDS_NUM * 2).ToString();
        }
        else
        {
            POINTS_NUM -= 2;
            
            card1.FalseCheck();
            card2.FalseCheck();

            MISSED_NUM += 1;
            missedNumText.text = MISSED_NUM.ToString();
        }
        Text pointsText = GameObject.Find("PointsNum Text").GetComponent<Text>();
        pointsText.text = POINTS_NUM.ToString();

        if (MenuManager.CARDS_NUM == 0)
        {
            wellPlayedText.enabled = true;
            wellPlayedText.transform.SetAsLastSibling();
            ALL_CARDS_INITIALIZED = false;
        }

    }


    private static Sprite LoadNewSprite(string path)
    {
        Texture2D myTexture = Resources.Load<Texture2D>(path);
        Sprite sprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }
    private static Sprite ConvertToSprite(Texture2D texture)
    {
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }

    private void Shuffle(Object[] objects)
    {
        // Knuth Shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < objects.Length; t++)
        {
            Object tmp = objects[t];
   
            int r = Random.Range(t, objects.Length);
            objects[t] = objects[r];
            objects[r] = tmp;
        }
    }

}
