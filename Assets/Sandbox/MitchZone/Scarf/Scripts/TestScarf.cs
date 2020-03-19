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
        Shader.SetGlobalVector("_PlayerLocation", player.transform.forward);
        Shader.SetGlobalFloat("_Progress", dashProgress);
        Quaternion rot = player.transform.rotation;
        Quaternion rot2 = ball.transform.rotation;
        Shader.SetGlobalVector("_PlayerRotation", new Vector4(rot.x, rot.y, rot.z, rot.w));
        Shader.SetGlobalVector("_BallRotation", new Vector4(rot2.x, rot2.y, rot2.z, rot2.w));
    }

    public void Dash()
    {
        print("Scarf Dash");
        scarfAnimator.SetTrigger("dash");
    }
}
