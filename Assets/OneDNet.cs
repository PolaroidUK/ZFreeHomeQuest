using System;
using System.Collections;
using System.Collections.Generic;
using Runtime;
using UnityEngine;


[CreateAssetMenu(menuName = "Create OneDNet", fileName = "OneDNet", order = 0)]
public class OneDNet : ScriptableObject
{
    public double input = 1;
    public double expected = 0;
    public double weight = 0.6;
    public double bias = 0.8;
    public double learnrate = 0.5;
    public double output = 0;
    [Button]
    public void Test()
    {
        double output = ActivationFunction((input * weight) + bias);
        Debug.Log(output);
    }

    [Button]
    public void Train()
    {
        double error = CostDerivative()*ActivationDerivative((input * weight) + bias);
        bias -= error*learnrate;
        weight -= error * learnrate * input;
        output = ActivationFunction((input * weight) + bias);
    }

    private double CostDerivative()
    {
        return (ActivationFunction((input * weight) + bias) - expected);
    }

    [Button]
    public void TrainAlot()
    {
        for (int i = 0; i < 100000; i++)
        {
            Train();
        }
    }
    
    
    double ActivationDerivative(double input)
    {
        double a = ActivationFunction(input);
        return a * (1 - a);
    }private double ActivationFunction(double outputForNode)
    {
        return 1 / (1 + Math.Exp(-outputForNode));
    }
}
