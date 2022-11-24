using System;
using Random = UnityEngine.Random;

[Serializable]
public class Layer
{
    public static double learnRate = 0.5f;
    
    public int nInputNodes;
    public int nOutputNodes;

    public double[,] weights;
    public double[] biases;
    public double[] activation;
    public double[] vector;
    public double[] error;
    public Layer(int nInputNodes, int nOutputNodes)
    {
        this.nInputNodes = nInputNodes;
        this.nOutputNodes = nOutputNodes;
        
        weights = new double[nInputNodes , nOutputNodes];
        biases = new double[nOutputNodes];
        vector = new double[nOutputNodes];
        error = new double[nOutputNodes];
        

        for (int o = 0; o < nOutputNodes; o++)
        {
            for (int i = 0; i < nInputNodes; i++)
            {
                weights[i,o] = Random.Range(-1f,1f);
            }
            biases[o] = Random.Range(-1f, 1f);
        }
    }
    
    
    public double[] Feed(double[] inputs)
    {
        double[] outputs = new double[nOutputNodes];
        for (int o = 0; o < nOutputNodes; o++)
        {
            double outputForNode = biases[o];
            for (int i = 0; i < inputs.Length; i++)
            {
                outputForNode += inputs[i] * weights[i, o];
            }

            vector[o] = outputForNode;
            outputs[o] = ActivationFunction(outputForNode);
        }

        activation = outputs;
        return outputs;
    }

    

    private double ActivationFunction(double outputForNode)
    {
        return 1 / (1 + (double)Math.Exp(-outputForNode));
    }
    double ActivationDerivative(double input)
    {
        double a = ActivationFunction(input);
        return a * (1 - a);
    }

    public void LayerError(double[] expectedValues)
    {
        
        for (int o = 0; o < nOutputNodes; o++)
        {
            error[o] = CostDerivative(activation[o],expectedValues[o]) * ActivationDerivative(vector[o]);
        }
        
    }
    public double CostDerivative(double predictedOutput, double expectedOutput)
    {
        if (predictedOutput == 0 || predictedOutput == 1)
        {
            return 0;
        }
        return (-predictedOutput + expectedOutput) / (predictedOutput * (predictedOutput - 1));
    }


    public void LayerError(Layer rightLayer, double[] doubles)
    {
        for (int o = 0; o < rightLayer.nInputNodes; o++)
        {
            for (int p = 0; p < rightLayer.nOutputNodes; p++)
            {
                error[o] += rightLayer.error[p] * rightLayer.weights[o,p];
            }
            error[o] *= ActivationDerivative(error[o]) *activation[o] ;
        }
    }

    public void UpdateBiases(int batchSize)
    {
        for (int n = 0; n < nOutputNodes; n++)
        {
            double rate = error[n]*((learnRate/batchSize));
            biases[n] += rate;
        }
    }

    public void UpdateWeights(int batchSize)
    {
        for (int o = 0; o < nOutputNodes; o++)
        {
            for (int i = 0; i < nInputNodes; i++)
            {
                weights[i,o] +=  error[o]*(learnRate/batchSize) ;
                
            }
        }
    }

    public void ResetErrors()
    {
        for (var i = 0; i < error.Length; i++)
        {
            error[i]= 0;
        }
    }
}