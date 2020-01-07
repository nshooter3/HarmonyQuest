using UnityEngine;

// [ExecuteInEditMode ]
public class TestScarf : MonoBehaviour
{
    public float dashProgress = 0f;
    [SerializeField]
    private TestPlayerScarf player;
    [SerializeField]
    private Transform ball;

    private Animator scarfAnimator;

    

    void Start()
    {
        scarfAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        Shader.SetGlobalVector("_BallLocation", ball.transform.position);
        Shader.SetGlobalVector("_PlayerLocation", player.transform.position);
        Shader.SetGlobalFloat("_Progress", dashProgress);
        print(ball.transform.position);
    }

    public void Dash()
    {
        print("Scarf Dash");
        scarfAnimator.SetTrigger("dash");
    }
}
