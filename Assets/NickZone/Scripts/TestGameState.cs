using UnityEngine;
using UnityEngine.SceneManagement;

public class TestGameState : MonoBehaviour
{
    public static TestGameState instance;

    public GameObject winText, loseText;
    private bool gameOver = false;
    private float messageTimer = 5.0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void Win()
    {
        if (gameOver == false)
        {
            gameOver = true;
            winText.SetActive(true);
        }
    }

    public void Lose()
    {
        if (gameOver == false)
        {
            gameOver = true;
            loseText.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver == true)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer < 0)
            {
                SceneManager.LoadScene("Controls");
            }
        }
    }
}
