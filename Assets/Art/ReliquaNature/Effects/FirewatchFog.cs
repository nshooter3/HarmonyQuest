using UnityEngine;

[ExecuteInEditMode]
public class FirewatchFog : MonoBehaviour {

    public Material material;

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        Graphics.Blit(src, dest, material);
    }
}
