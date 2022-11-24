using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Runtime;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class CubeGrid : MonoBehaviour
{
    public Transform CubePrefab;
    public ComputeShader CubeShader;

    private ComputeBuffer _cubesPositionBuffer;

    public MoreDNet net;
    // Grid size
    public int CubesPerAxis = 80;
    // Cube objects
    private Transform[] _cubes;

    private ComputeBuffer shaderOutput;
    public float[]  inputNodes ;
    //public float[] weights    ;
    //public float[]  bias       ;
    public float[]  outputNodes;
    // Array containing all y positions of cubes.
    // Will be put on the compute buffer
    private float[] _cubesPositions;
    private ComputeBuffer bBuffer;
    private ComputeBuffer wBuffer;
    private ComputeBuffer iBuffer;
    private ComputeBuffer zBuffer;
    public ShaderLayer testLayer;
    public ShaderLayer nextTestLayer;
    [SerializeField] private Stopwatch stopwatch = new Stopwatch();

    private void Start()
    {
        
        inputNodes = new float[100];
        outputNodes =new float[6];
        for (var i = 0; i < 100; i++)
        {
            inputNodes[i]= Random.Range(-1f, 1f);
        }
        
        testLayer = new ShaderLayer(100, 200,CubeShader);
        nextTestLayer = new ShaderLayer(200,6,CubeShader);
    }
    [Button(ButtonMode.EnabledInPlayMode)]
    public void RunShader()
    {
        CalculateLayer(inputNodes,testLayer);
        CalculateLayer(testLayer.activation,nextTestLayer);
    }

    
    [Button]
    public void TimeTest()
    {
        
        stopwatch.Start();
        RunShader();
        Debug.Log("shader : "+stopwatch.Elapsed);
        stopwatch.Restart();
    }
    private void CalculateLayer(float[] input,ShaderLayer layer)
    {
        
        layer.bBuffer.SetData(layer.bias);
        layer.wBuffer.SetData(layer.weights);
        layer.zBuffer.SetData(layer.z);
        layer.iBuffer.SetData(input);
        layer.computeShader.SetBuffer(0,"output",layer.oBuffer);
        layer.computeShader.SetBuffer(0,"bias",layer.bBuffer);
        layer.computeShader.SetBuffer(0,"input",layer.iBuffer);
        layer.computeShader.SetBuffer(0,"weight",layer.wBuffer);
        layer.computeShader.SetBuffer(0,"z",layer.zBuffer);
        layer.computeShader.SetInt("inputSize",layer.inputSize);
        layer.computeShader.SetInt("outputSize",layer.outputSize);
        int workGroups =Mathf.CeilToInt(layer.outputSize / 8.0f);
        layer.computeShader.Dispatch(0,1,workGroups,1);
        layer.oBuffer.GetData(layer.activation);
    }

    private void OnDestroy()
    {
        //shaderOutput.Release();
        //bBuffer.Release();
        //wBuffer.Release();
        //iBuffer.Release();
        //zBuffer.Release();
        testLayer.ReleaseBuffers();
        nextTestLayer.ReleaseBuffers();
    }
}
[Serializable]
public class ShaderLayer
{
    public ComputeBuffer oBuffer;
    public ComputeBuffer bBuffer;
    public ComputeBuffer wBuffer;
    public ComputeBuffer iBuffer;
    public ComputeBuffer zBuffer;
    public ComputeShader computeShader;
    public int inputSize; 
    public int outputSize;
    public float[] weights;
    public float[]  bias;
    public float[] z;
    public float[]  activation;

    public ShaderLayer(int inputSize, int outputSize,ComputeShader computeShader)
    {
        this.inputSize = inputSize;
        this.outputSize = outputSize;
        this.computeShader = computeShader;
        weights = new float[inputSize*outputSize];
        bias = new float[outputSize];
        activation = new float[outputSize];
        z= new float[outputSize];
        for (int o = 0; o < outputSize; o++)
        {
            z[o] = 0;
            bias[o] = Random.Range(-1f,1f);
            for (int i = 0; i < inputSize; i++)
            {
                weights[i+inputSize*o] = Random.Range(-1f,1f);
            }
        }
        oBuffer = new ComputeBuffer(outputSize, sizeof(float));
        bBuffer = new ComputeBuffer(outputSize,sizeof(float));
        wBuffer = new ComputeBuffer(inputSize*outputSize,sizeof(float));
        iBuffer = new ComputeBuffer(inputSize,sizeof(float));
        zBuffer = new ComputeBuffer(outputSize,sizeof(float));
    }

    public void ReleaseBuffers()
    {
        oBuffer.Release();
        bBuffer.Release();
        wBuffer.Release();
        iBuffer.Release();
        zBuffer.Release();
    }
}
