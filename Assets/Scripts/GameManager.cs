using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private const int COIN_SCORE_AMOUNT = 5;
    public static GameManager Instance { get; set; }

    private bool isGameStarted = false;
    private PlayerController playerController;

    // UI and the UI fields
    public TextMeshProUGUI scoreText, coinText, modifierText;
    private float score, coinScore, modifierScore;

    private void Awake()
    {
        Instance = this;
        modifierScore = 1.0f;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        //UpdateScores();
        scoreText.text = score.ToString("0");
        coinText.text = coinScore.ToString("0");
        modifierText.text = "x" + modifierScore.ToString("0.0");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (MobileInput.Instance.Tap && !isGameStarted)
        {
            isGameStarted = true;
            playerController.StartRunning();
        }

        if (isGameStarted)
        {
            // bump score up
            score += (Time.deltaTime * modifierScore);
            scoreText.text = score.ToString("0");
        }
    }

    /*public void UpdateScores()
    {
        scoreText.text = score.ToString();
        coinText.text = coinScore.ToString();
        modifierText.text = "x" + modifierScore.ToString("0.0");
    }*/

    public void GetCoin()
    {
        coinScore += COIN_SCORE_AMOUNT;
        coinText.text = coinScore.ToString("0");
    }

    public void UpdateModifier(float modifierAmount)
    {
        modifierScore = 1.0f + modifierAmount;
        modifierText.text = "x" + modifierScore.ToString("0.0");
    }
}
