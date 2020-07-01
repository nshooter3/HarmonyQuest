using Melody;
using UnityEngine;

public class MelodyGroundedChecker : MonoBehaviour
{
    public Renderer groundedIndicator;
    public Material groundedReference, slidingReference, inAirReference;
    private Material grounded, sliding, inAir;

    public MelodyController melodyController;

    // Start is called before the first frame update
    void Start()
    {
        grounded = new Material(groundedReference);
        inAir = new Material(inAirReference);
        sliding = new Material(slidingReference);
    }

    // Update is called once per frame
    void Update()
    {
        if (melodyController.melodyCollision.IsGrounded())
        {
            groundedIndicator.material = grounded;
        }
        else if (melodyController.melodyCollision.IsSliding())
        {
            groundedIndicator.material = sliding;
        }
        else
        {
            groundedIndicator.material = inAir;
        }
    }
}
