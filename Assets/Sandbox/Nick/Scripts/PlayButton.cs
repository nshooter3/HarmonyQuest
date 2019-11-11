using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public void HitPlay()
    {
        SceneManager.LoadScene("CombatTest");
    }
}
