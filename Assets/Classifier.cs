using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Runtime;
using UnityEngine;
using Random = System.Random;

[CreateAssetMenu(menuName = "Create Classifier", fileName = "Classifier", order = 0)]
public class Classifier : ScriptableObject
{
    public CSVImporter Importer;
    public FileImporter FileImporter;
    public TestSetCreater TestSetCreater;
    public List<Datapoint> Datapoints;
    public int[] addLayers;
    public Layer[] Layers;
    public string newPath = @$"{Environment.CurrentDirectory}\Assets\{"networkSaveData"}.csv";
    public string path;
    [Button]
    public void init()
    {
        Datapoints = TestSetCreater.datapoints;
        //Datapoints = FileImporter.ReadFile();
        
        // add layers plus one becuase addlayers dosent haev output layer in it
        Layers = new Layer[addLayers.Length + 1];
        
        if (addLayers.Length != 0)
        {
            Layers[0] = new Layer(Datapoints[0].features.Length,addLayers[0]);
            for (int i = 1; i < addLayers.Length; i++)
            {
                Layers[i] = new Layer(addLayers[i - 1], addLayers[i]);
            }

            Layers[^1] = new Layer(addLayers[^1], Datapoints[0].label.Length);
        }
        
    }

    [Button]
    public void AccuracyTest()
    {
        double accuracy = 0;
        foreach (var datapoint in Datapoints)
        {
            double[] result = Predict(datapoint);
            
            double highest = double.MinValue;
            int highestIndex = 0;
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i]>highest)
                {
                    highest = result[i];
                    highestIndex = i;
                }
            }
            double highestReal = double.MinValue;
            int highestRealIndex = 0;
            for (int i = 0; i < datapoint.label.Length; i++)
            {
                if (datapoint.label[i]>highest)
                {
                    highestReal = result[i];
                    highestRealIndex = i;
                }
            }

            if (highestIndex==highestRealIndex)
            {
                accuracy++;
            }
        }

        Debug.Log(accuracy/Datapoints.Count);
    }
    
    [Button]
    public void Test()
    {
        Debug.Log(Cost()/Datapoints.Count);
    }

    
    private double Cost()
    {
        double cost = 0;
        foreach (var datapoint in Datapoints)
        {
            double[] result = Predict(datapoint);
            for (int i = 0; i < datapoint.label.Length; i++)
            {
                double temp = result[i] - datapoint.label[i];
                cost += temp * temp;
            }
        }

        return cost * 0.5f;
    }

    private double[] Predict(Datapoint datapoint)
    {
        double[] input = datapoint.features;
        for (int i = 0; i < Layers.Length; i++)
        {
            input = Layers[i].Feed(input);
        }
        return input;
    }


    [Button]
    public void Train()
    {
        foreach (Datapoint datapoint in Datapoints)
        {
            double[] input = datapoint.features;
            for (int i = 0; i < Layers.Length; i++)
            {
                input = Layers[i].Feed(input);
            }
            Layers[^1].LayerError(datapoint.label);
            for (int i = Layers.Length - 2; i >= 1; i--)
            {
                Layers[i].LayerError(Layers[i+1],Layers[i].activation);
            }
            Layers[0].LayerError(Layers[1],datapoint.features);
        }
        for (int i = 0; i < Layers.Length; i++)
        {
            Layers[i].UpdateBiases(Datapoints.Count);
            Layers[i].UpdateWeights(Datapoints.Count);
        }
        
    }
    [Button]
    public void TestTrainTest()
    {
        AccuracyTest();
        Train();
        AccuracyTest();
    }
    
    [Button]
    public void BatchTrain()
    {
        AccuracyTest();
        Random rng = new Random();
        int n = Datapoints.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            (Datapoints[k], Datapoints[n]) = (Datapoints[n], Datapoints[k]);
        } 
        for (int b = 0; b < 4; b++)
        {
            int batchSize = Datapoints.Count/200;
            List<Datapoint> batch= Datapoints.GetRange(b*batchSize,batchSize);
            foreach (Datapoint datapoint in batch)
            {
                double[] input = datapoint.features;
                for (int i = 0; i < Layers.Length; i++)
                {
                    input = Layers[i].Feed(input);
                }
                Layers[^1].LayerError(datapoint.label);
                for (int i = Layers.Length - 2; i >= 1; i--)
                {
                    Layers[i].LayerError(Layers[i+1],Layers[i].activation);
                }
                Layers[0].LayerError(Layers[1],datapoint.features);
            }
            foreach (var layer in Layers)
            {
                layer.UpdateBiases(batchSize);
                layer.UpdateWeights(batchSize);
                layer.ResetErrors();
            }
            AccuracyTest();
        }
    } 
    
    
    [Button]
    public void SaveNetwork()
    {
        
        Debug.Log("printing to :" + newPath);
        StreamWriter sw = new StreamWriter(newPath);
        string startLine = "";
        startLine += Layers[0].nInputNodes+","+Layers[0].nOutputNodes;
        for (int i = 1; i < Layers.Length; i++)
        {
            startLine += ","+Layers[i].nInputNodes+","+Layers[i].nOutputNodes;
        }
        sw.WriteLine(startLine);
        sw.Flush();
        for (int i = 0; i < Layers.Length; i++)
        {
            string toWrite = "";
            
            for (int k = 0; k < Layers[i].weights.GetLength(1); k++)
            {
                for (int j = 0; j < Layers[i].weights.GetLength(0); j++)
                {
                    toWrite += Layers[i].weights[j,k] + ",";
                }

                toWrite += Layers[i].biases[k] +",";
            }

            
            sw.WriteLine(toWrite.Remove(toWrite.Length - 1, 1));
            sw.Flush();
        }
    }

    [Button]
    public void LoadNetwork()
    {
        StreamReader reader = new StreamReader(path);
        reader.ReadLine();//skip first Line
        bool fileEmpty = false;
        int tally = 0;
        while (!fileEmpty)
        {
            string nextLine = reader.ReadLine();
            if (nextLine == null)
            {
                fileEmpty = true;
                break;
            }

            string[] splitLine = nextLine.Split(",");
            Debug.Log(splitLine.Length);
            int index = 0;
            for (int k = 0; k < Layers[tally].weights.GetLength(1); k++)
            {
                for (int j = 0; j < Layers[tally].weights.GetLength(0); j++)
                {
                    Layers[tally].weights[j,k] = double.Parse(splitLine[index]);
                    index++;
                }

                Layers[tally].biases[k] = double.Parse(splitLine[index]);
                index++;
            }

            tally++;
        }
        reader.Close();
    }
}