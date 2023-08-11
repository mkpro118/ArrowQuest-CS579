using UnityEngine;
using UnityEngine.UI;

public class UpdateScore : MonoBehaviour
{
    private Text scoreText;

    private int score = 0;

    private void Start()
    {
        scoreText = GetComponent<Text>();
    }

    private void Update()
    {
        scoreText.text = "Score: " + score;
    }

    public void Increment() => score++;
}
