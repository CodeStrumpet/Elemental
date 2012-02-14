using UnityEngine;
using System.Collections;

public class AnimateWaterTexture : MonoBehaviour {
    public int materialIndex = 0;
    public Vector2 uvAnimationRate = new Vector2( 1.0f, 0.0f );
    public string textureName = "_MainTex";
    public float waveSpeed = 0.1f;

    Vector2 uvOffset = Vector2.zero;

    void LateUpdate() 
    {
        uvOffset += (uvAnimationRate * Time.deltaTime) + (uvAnimationRate * waveSpeed * Mathf.Abs(Mathf.Sin(Time.time)));
        if( renderer.enabled )
        {
            renderer.materials[materialIndex].SetTextureOffset(textureName, uvOffset);
        }
    }
}
