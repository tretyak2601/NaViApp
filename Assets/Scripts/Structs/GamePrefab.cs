using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePrefab : MonoBehaviour {
    private int id;

    [HideInInspector] public string bestOf;
    [SerializeField] Text scoreText;
    [SerializeField] Image mask;
    [SerializeField] public GameObject score;
    [SerializeField] Button scoreButton;

    public Image naviLogo;
    public Image enemyLogo;
    public Text date;
    public Text c1name;
    public Text c2name;

    private void Start()
    {
        scoreButton.onClick.AddListener(ShowScore);
    }

    public void ShowScore()
    {
        if (bestOf != string.Empty)
        {
            score.SetActive(true);
            mask.enabled = false;
            scoreText.text = bestOf;
            scoreText.fontSize += 10;
            scoreText.color = ScreenSelected.choosed;
            scoreButton.onClick.RemoveAllListeners();
        }
    }
}
