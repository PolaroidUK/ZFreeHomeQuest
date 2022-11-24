using System.Collections;
using System.Collections.Generic;
using Runtime;
using UnityEngine;

public class ComputeShaderServer : MonoBehaviour
{
    public ComputeShader shader;
    public RenderTexture rTexture;
    [Button]
    public void RunShader()
    {
        
    }
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (rTexture == null) { 
            rTexture = new RenderTexture( Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            rTexture.enableRandomWrite = true;
            rTexture.Create();
        }

        int workgroupX = Mathf.CeilToInt(Screen.width/8.0f);
        int workgroupY = Mathf.CeilToInt(Screen.height/8.0f);
        
        
        int kernel = shader.FindKernel("CSMain");
        shader.SetTexture(kernel,"Result",rTexture);
        shader.Dispatch(kernel,workgroupX,workgroupY,1);
        Graphics.Blit(rTexture,destination);
    } 
}
