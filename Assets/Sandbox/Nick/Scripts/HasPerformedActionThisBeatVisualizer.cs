using HarmonyQuest.Audio;
using UnityEngine;

public class HasPerformedActionThisBeatVisualizer : MonoBehaviour
{
    public MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        meshRenderer.enabled = FmodFacade.instance.HasPerformedActionThisBeat();
    }
}
