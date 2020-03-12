using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static bool DO_NOT = false;
    private string name;

    public bool isHidden;
    public bool isDone;

    public Sprite cardBack;
    public Sprite cardFace;
    public Image image;
    public Button button;
    private Outline outline;
    private GameManager manager;


    private float maxScale = 1.1f;
    private float currentScale = 1f;
    private bool initialized = false;
    private bool goDown = false;

    private bool scaleUp = false;
    private bool scaleDown = false;

    void Start()
    {
        isHidden = true;
        isDone = false;
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        outline = GetComponent<Outline>();
        manager = GetComponent<GameManager>();
    }
    void Update()
    {
        if (!initialized)
            InitializationAnimation();

        if (scaleUp)
        {
            if (currentScale < maxScale)
                currentScale += 0.03f;
            image.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
        }
        if (scaleDown)
        {
            if (currentScale > 1)
                currentScale -= 0.03f;
            if (currentScale <= 1)
            {
                currentScale = 1;
                scaleDown = false;
            }
            image.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
        }

    }
    private void InitializationAnimation()
    {
        if (currentScale > maxScale)
            goDown = true;

        if (currentScale < maxScale && !goDown)
            currentScale += 0.03f;
        else if (goDown)
            currentScale -= 0.03f;

        if (currentScale < 1)
        {
            currentScale = 1;
            initialized = true;
        }
        image.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
    }
    public void FlipCard()
    {
        if (isDone) return;

        if (isHidden && !DO_NOT)
        {
            image.sprite = cardFace;
            isHidden = false;
        }
      
       
    }
    public void FalseCheck()
    {
        StartCoroutine(Pause());
    }
    private IEnumerator Pause()
    {
        yield return new WaitForSeconds(1);
        if (isHidden)
        {
            image.sprite = cardFace;
            isHidden = false;
        }
        else if (!isHidden)
        {
            image.sprite = cardBack;
            isHidden = true;
        }
        DO_NOT = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.ALL_CARDS_INITIALIZED && !isDone)
        {
            scaleUp = true;
            button.enabled = true;
            outline.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GameManager.ALL_CARDS_INITIALIZED)
        {
            scaleUp = false;
            scaleDown = true;
            button.enabled = false;
            outline.enabled = false;
        }
    }
}
