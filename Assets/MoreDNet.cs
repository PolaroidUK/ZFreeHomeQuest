using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using static System.Math;
using Random = UnityEngine.Random;


[CreateAssetMenu(menuName = "Create MoreDNet", fileName = "MoreDNet", order = 0)]
public class MoreDNet : ScriptableObject
{
    public TestSetCreater TestSetCreater;
    public FileImporter FileImporter;
    Datapoint[] trainingSet;
    private Datapoint[] testSet;
    public double learnrate = 0.5f;
    public SimpleLayer outputLayer;
    public SimpleLayer middleLayer;
    public SimpleLayer firstLayer;
    public int inputSize;
    public int firstlayerSize = 10;
    public int middleLayerSize = 10;
    public int outputSize;
    public int biasCount;
    public int weightCount;



    public struct FeedForwardJob : IJobParallelFor
    {
        
        [ReadOnly]public NativeArray<double> featuresArray;
        [ReadOnly]public NativeArray<double> labelsArray;
        [ReadOnly]public NativeArray<double> biasArray;
        [ReadOnly]public NativeArray<double> weightArray;
        [ReadOnly] public int inSize;
        [ReadOnly] public int firstLSize;
        [ReadOnly] public int middleLSize;
        [ReadOnly] public int outputLSize;
        public NativeArray<double> outputArray;
        
        
        public void Execute(int index)
        {
            double[] output = new double[firstLSize];
            int tallyb = 0;
            int tallyw = 0;
            for (int i = 0; i < firstLSize; i++)
            {
                double outtemp = 0;
                for (var j = 0; j < inSize; j++)
                {
                    outtemp += featuresArray[j] * weightArray[i * inSize + j+index];
                    tallyw++;
                }
                output[i] = ActivationFunction(outtemp+ biasArray[tallyb]);
                tallyb++;
            }

            double[] outputTemp = new double[middleLSize];
            for (int i = 0; i < middleLSize; i++)
            {
                double outtemp = 0;
                for (var j = 0; j < firstLSize; j++)
                {
                    outtemp += output[j] * weightArray[i * firstLSize + j+index];
                    tallyw++;
                }
                outputTemp[i] = ActivationFunction(outtemp+ biasArray[tallyb]);
                tallyb++;
            }

            output = outputTemp;
            outputTemp = new double[outputLSize];
            for (int i = 0; i < outputLSize; i++)
            {
                double outtemp = 0;
                for (var j = 0; j < middleLSize; j++)
                {
                    outtemp += output[j] * weightArray[i * middleLSize + j+index];
                    tallyw++;
                }
                outputTemp[i] = ActivationFunction(outtemp+ biasArray[tallyb]);
                tallyb++;
            }

           // for (int i = 0; i < outputTemp.Length; i++)
           // {
         //       outputArray[i+index*outputTemp.Length] = outputTemp[i];
            //}
        }

        public void Dispose()
        {
            
            featuresArray.Dispose();
            labelsArray.Dispose();
            biasArray.Dispose();
            weightArray.Dispose();
            outputArray.Dispose();
        }
    }

    [Button]
    public void Thready()
    {
        var featuresArray = new NativeArray<double>(trainingSet.Length*trainingSet[0].features.Length, Allocator.TempJob);
        var labelArray = new NativeArray<double>(trainingSet.Length*trainingSet[0].label.Length, Allocator.TempJob);
        var bArray = new NativeArray<double>(firstlayerSize+middleLayerSize+outputSize, Allocator.TempJob);
        var wArray = new NativeArray<double>(trainingSet.Length*trainingSet[0].features.Length*firstlayerSize+firstlayerSize*middleLayerSize+middleLayerSize*outputSize, Allocator.TempJob);
        for (var index = 0; index < trainingSet.Length; index++)
        {
            for (int i = 0; i < trainingSet[0].label.Length; i++)
            {
                
                labelArray[index*trainingSet[0].label.Length+i] =  trainingSet[index].label[i];
            }

            for (int i = 0; i < trainingSet[0].features.Length; i++)
            {
                featuresArray[index*trainingSet[0].label.Length+i]= trainingSet[index].features[i]; 
            }
        }

        int tally = 0;
        int tallyw = 0;
        for (int i = 0; i < firstlayerSize; i++)
        {
            bArray[tally] = firstLayer.bias[i];
            for (int j = 0; j < inputSize; j++)
            {
                wArray[tallyw] = firstLayer.weight[i*inputSize+j];
                tallyw++;
            }
            tally++;
        }
        for (int i = 0; i < middleLayerSize; i++)
        {
            bArray[tally] = middleLayer.bias[i];
            for (int j = 0; j < firstlayerSize; j++)
            {
                wArray[tallyw] = middleLayer.weight[i*firstlayerSize+j];
                tallyw++;
            }
            tally++;
        }
        for (int i = 0; i < outputSize; i++)
        {
            bArray[tally] = outputLayer.bias[i];
            for (int j = 0; j < middleLayerSize; j++)
            {
                wArray[tallyw] = outputLayer.weight[i*middleLayerSize+j];
                tallyw++;
            }
            tally++;
        }

        NativeArray<double> outArray = new NativeArray<double>(outputSize*trainingSet.Length, Allocator.TempJob);
        for (var index = 0; index < outArray.Length; index++)
        {
             outArray[index]=0;
            
        }

        var job = new FeedForwardJob
        {
            featuresArray = featuresArray,
            labelsArray = labelArray,
            weightArray = wArray,
            biasArray = bArray,
            inSize = inputSize,
            firstLSize = firstlayerSize,
            middleLSize = middleLayerSize,
            outputLSize = outputSize,
            outputArray = outArray
        };
        var jobHandle = job.Schedule(trainingSet.Length, 1);
        jobHandle.Complete();
        job.Dispose();
    }
        
    [Button]
    public void StartToFinish()
    {
        Init();
        
        Test(trainingSet);
        Test(testSet);
        for (int i = 0; i < 25; i++)
        {
            BatchTraining();
            Test(trainingSet);
            Test(testSet);
        }; 
    }

    [Button]
    public void TrainForTen()
    {
        Test(testSet);
        for (int i = 0; i < 25; i++)
        {
            BatchTraining();
            Test(testSet);
        }; 
    }
    
    [Button]
    public void Init()
    {
        //TestSetCreater.Create();
        //datapoints = TestSetCreater.datapoints.ToArray();
        //datapoints = FileImporter.GetTestData().ToArray();
        
        
        
        (trainingSet,testSet) = FileImporter.ReadFile();



        Debug.Log("working with "+trainingSet.Length+" training datapoints");
        Debug.Log("working with "+testSet.Length+" test datapoints");
        outputSize = trainingSet[0].label.Length;
        inputSize = trainingSet[0].features.Length;
        System.Random rng = new System.Random();
        outputLayer = new SimpleLayer(middleLayerSize, outputSize,rng);
        middleLayer = new SimpleLayer(firstlayerSize, middleLayerSize,rng);
        firstLayer = new SimpleLayer(inputSize, firstlayerSize,rng);
        biasCount = firstlayerSize + middleLayerSize + outputSize;
        weightCount = firstlayerSize*inputSize + firstlayerSize*middleLayerSize + middleLayerSize*outputSize;
    }
    
    [Button]
    public void Test(Datapoint[] set)
    {
        Debug.Log("-------------- New Test ---------");
        double cost = 0;
        double tally = 0;
        foreach (Datapoint datapoint in set)
        {
            var forward = FeedForward(datapoint.features);
            for (var i = 0; i < forward.Length; i++)
            {
                cost += Mathf.Abs((float)(forward[i]-datapoint.label[i]));
            }
            double highest = double.MinValue;;
            int indexH =0;
            for (int i = 0; i < forward.Length; i++)
            {
                if (forward[i] >highest)
                {
                    highest = forward[i];
                    indexH = i;
                }
            }
            
            for (int i = 0; i < forward.Length; i++)
            {
                if (forward[i] >highest)
                {
                    highest = forward[i];
                    indexH = i;
                }
            }

            if (datapoint.label[indexH]==1)
            {
                tally++;
            }
        }
        Debug.Log("Cost: "+cost/set.Length);
        Debug.Log("Accuracy: "+(int)((tally/set.Length)*100)+"%");
    }
    
    public void CostTest()
    {
        double tally = 0;
        foreach (var datapoint in trainingSet)
        {
            var forward = FeedForward(datapoint.features);
            for (var i = 0; i < forward.Length; i++)
            {
                tally += Mathf.Abs((float)(forward[i]-datapoint.label[i]));
            }
        }
        Debug.Log(tally/trainingSet.Length);
    }

    
    public void Train(Datapoint[] batch)
    {
        foreach (Datapoint datapoint in batch)
        {
            FeedForward(datapoint.features);
            BackPropegation(datapoint.features, datapoint.label);
        }
        DescendAllLayers(batch.Length);
    }

    private void AccuracyTest()
    {
        double tally = 0;
        
        foreach (Datapoint datapoint in trainingSet)
        {
            var outp = FeedForward(datapoint.features);
            double highest = double.MinValue;;
            int indexH =0;
            for (int i = 0; i < outp.Length; i++)
            {
                if (outp[i] >highest)
                {
                    highest = outp[i];
                    indexH = i;
                }
            }
            
            for (int i = 0; i < outp.Length; i++)
            {
                if (outp[i] >highest)
                {
                    highest = outp[i];
                    indexH = i;
                }
            }

            if (datapoint.label[indexH]==1)
            {
                tally++;
            }
        }
        Debug.Log(tally/trainingSet.Length);
    }
    
    [Button]
    public void BatchTraining()
    {
        int batchNumber = 10;
        int batchSize = trainingSet.Length / batchNumber;
        int n = trainingSet.Length;  
        System.Random rng = new System.Random();
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            (trainingSet[k], trainingSet[n]) = (trainingSet[n], trainingSet[k]);
        }

        for (int i = 0; i < batchNumber; i++)
        {
            Datapoint[] batch = new Datapoint[batchSize];
            for (int j = 0; j < batchSize; j++)
            {
                batch[j] = trainingSet[j + i * batchSize];
            }
            Train(batch);
        }
    }

    #region Network

    private void Feed(double[] input, SimpleLayer layer)
    {
        for (int i = 0; i < layer.activation.Length; i++)
        {
            layer.z[i] = Z(input, layer.weight,i, layer.bias[i]);
            layer.activation[i] = ActivationFunction(layer.z[i]);
        }
    }
    
    private double[] FeedForward(double[] input)
    {
        Feed(input, firstLayer);
        Feed(firstLayer.activation,middleLayer);
        Feed(middleLayer.activation,outputLayer);
        return outputLayer.activation;
    }
    private void BackPropegation(double[] input, double[] expected)
    {
        // output layer
        //double[] errors = new double[expected.Length];
        
        for (int i = 0; i < expected.Length; i++)
        {
            double costDerivative = CostDerivative(outputLayer.activation[i], expected[i]);
            outputLayer.errors[i] = costDerivative * ActivationDerivative(outputLayer.activation[i]);
        }
        middleLayer.errors = GradientDecent(middleLayer,outputLayer.weight,firstLayer.activation, outputLayer.errors);
        firstLayer.errors = GradientDecent(firstLayer,middleLayer.weight,input, middleLayer.errors);
        UpdateDelta(outputLayer, middleLayer.activation);
        UpdateDelta(middleLayer, firstLayer.activation);
        UpdateDelta(firstLayer, input);
    }

    public void UpdateDelta(SimpleLayer layer,double[] nextLayerActivation)
    {
        for (int i = 0; i < layer.activation.Length; i++)
        {
            double errorOnNodeI = layer.errors[i] * learnrate;
            layer.bDelta[i] += errorOnNodeI;
            for (int j = 0; j < nextLayerActivation.Length; j++)
            {
                layer.wDelta[i * nextLayerActivation.Length + j] += errorOnNodeI * nextLayerActivation[j];
            }
            layer.errors[i] = 0;
        }
    }
    private void DescendAllLayers(int batchSize)
    {
        DescendLayer(firstLayer,batchSize);
        DescendLayer(middleLayer,batchSize);
        DescendLayer(outputLayer,batchSize);
    }

    private void DescendLayer(SimpleLayer layer,int batchSize)
    {
        for (int i = 0; i < layer.weight.Length; i++)
        {
            layer.weight[i] -= layer.wDelta[i] *(learnrate/batchSize);
            layer.wDelta[i] = 0;
        }
        for (int i = 0; i < layer.bias.Length; i++)
        {
            layer.bias[i] -= layer.bDelta[i] *(learnrate/batchSize);
            layer.bDelta[i] = 0;
        }
    }

    private void ApplyAllGradientsLegacy(double[] biasDelta,double[] weightDelta,int batchSize)
    {
        ApplyGradietn(outputLayer, middleLayer.activation);
        ApplyGradietn(middleLayer, firstLayer.activation);
        //ApplyGradietn(firstLayer, input);
    }

    private void ApplyGradietn(SimpleLayer layer,double[] nextLayerActivation)
    {
        for (int i = 0; i < layer.activation.Length; i++)
        {
            double errorOnNodeI = layer.errors[i] * learnrate;
            layer.bias[i] -= errorOnNodeI;
            for (int j = 0; j < nextLayerActivation.Length; j++)
            {
                layer.weight[i * nextLayerActivation.Length + j] -= errorOnNodeI * nextLayerActivation[j];
            }
            layer.errors[i] = 0;
        }
    }

    private double[] GradientDecent(SimpleLayer layer,double[] lastLayerWeights,double[] nextLayerActivations, double[] errors)
    {
        double[] errorsB = new double[layer.activation.Length];
        for (int i = 0; i < layer.activation.Length; i++)
        {
            for (int j = 0; j < errors.Length; j++)
            {
                double d = ActivationDerivative(errors[j] * lastLayerWeights[j*layer.activation.Length+i]);
                errorsB[i] += d * layer.z[i];
            }
        }
        return errorsB;
    }


    

    #endregion
    public void InitializeRandomWeights(SimpleLayer layer,System.Random rng)
    {
        
    }

    #region Math

    public static double Z(double[] input, double[] w, int nodeIndex, double b)
    {
        double outp = 0;
        for (var i = 0; i < input.Length; i++) outp += input[i] * w[nodeIndex * input.Length + i];
        return outp + b;
    }

    public double CostDerivative(double predictedOutput, double expectedOutput)
    {
        if (predictedOutput is 0 or 1) return 0;
        return (-predictedOutput + expectedOutput) / (predictedOutput * (predictedOutput - 1));
    }

    private double ActivationDerivative(double input)
    {
        var a = ActivationFunction(input);
        return a * (1 - a);
    }

    private static double ActivationFunction(double outputForNode)
    {
        return 1 / (1 + Math.Exp(-outputForNode));
    }

    #endregion
}
[Serializable]
public struct SimpleLayer
{
    public double[] weight;
    public double[] bias;
    public double[] activation;
    public double[] z;
    public double[] errors;
    public double[] bDelta;
    public double[] wDelta;
    public SimpleLayer(int inputSize,int outputSize,System.Random rng)
    {
        weight = new double[inputSize * outputSize];
        bias = new double[outputSize];
        activation = new double[outputSize];
        z= new double[outputSize];
        errors = new double[outputSize];
        bDelta = new double[outputSize];
        wDelta = new double[inputSize * outputSize];
        for (int o = 0; o < outputSize; o++)
        {
            bias[o] = 0;
            //for (int i = 0; i < inputSize; i++)
            //{
             //   weight[o*inputSize+i] = Random.Range(-1f,1f);
           // }
        }  
        for (int i = 0; i < weight.Length; i++)
        {
            weight[i] = RandomInNormalDistribution(rng, 0, 1) / Sqrt(inputSize);
        }

        double RandomInNormalDistribution(System.Random rng, double mean, double standardDeviation)
        {
            double x1 = 1 - rng.NextDouble();
            double x2 = 1 - rng.NextDouble();

            double y1 = Sqrt(-2.0 * Log(x1)) * Cos(2.0 * PI * x2);
            return y1 * standardDeviation + mean;
        }
    }
}
