using GameManager;
using Melody;
using UnityEngine;

public class MelodyGroundedChecker : ManageableObject
{
    public Renderer groundedIndicator;
    public Material groundedReference, slidingReference, inAirReference;
    private Material grounded, sliding, inAir;

    public MelodyController melodyController;

    // Start is called before the first frame update
    public override void OnStart()
    {
        grounded = new Material(groundedReference);
        inAir = new Material(inAirReference);
        sliding = new Material(slidingReference);
    }

    // Update is called once per frame
    public override void OnUpdate()
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
