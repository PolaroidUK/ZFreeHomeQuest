using System.Collections;
using System.Collections.Generic;
using Runtime;
using UnityEngine;

[CreateAssetMenu(menuName = "Create TestSetCreater", fileName = "TestSetCreater", order = 0)]
public class TestSetCreater : ScriptableObject  
{
    public int total = 10000;
    public List<Datapoint> datapoints ;
    [Button]
    public void Create()
    {
        datapoints = new List<Datapoint>();
        for (int i = 0; i < total; i++)
        {
            double[] feats = new double[21];
            double[] label = new double[]{1,0,0,0};
            int rng = 0;
            if (i<total/2)
            {
                if (i<total/4||i>total-total/4)
                {
                    rng = 0;
                    feats = new double[]
                    {
                        Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),
                        Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),
                        Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),
                        Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),
                        Random.Range(0f,1f),
                    };
                   // feats = new double[]{Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f)};
                    label = new double[]{1,0,0,0};
                }
                else
                {
                    feats = new double[]
                    {
                        Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(-1f,0f),Random.Range(-1f,0f),
                        Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(-1f,0f),Random.Range(-1f,0f),
                        Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(-1f,0f),Random.Range(-1f,0f),
                        Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(-1f,0f),Random.Range(-1f,0f),
                        Random.Range(0f,1f),
                    };
                    //feats = new double[]{Random.Range(0f,1f),Random.Range(-1f,0f),Random.Range(0f,1f)};
                    label = new double[]{0,1,0,0};
                }
            }else if (i<total/4||i>total-total/4)
            {
                feats = new double[]
                {
                    Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(-1f,0f),
                    Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(-1f,0f),
                    Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(-1f,0f),
                    Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(-1f,0f),
                    Random.Range(-1f,0f),
                };
                //feats = new double[]{Random.Range(-1f,0f),Random.Range(0f,1f),Random.Range(0f,1f)};
                label = new double[]{0,0,1,0};
            }
            else
            {
                feats = new double[]
                {
                    Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(0f,1f),Random.Range(0f,1f),
                    Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(0f,1f),Random.Range(0f,1f),
                    Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(0f,1f),Random.Range(0f,1f),
                    Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(0f,1f),Random.Range(0f,1f),
                    Random.Range(-1f,0f),
                };
                //feats = new double[]{Random.Range(-1f,0f),Random.Range(-1f,0f),Random.Range(0f,1f)};
                label = new double[]{0,0,0,1};
            }
           /* double[] feats = new double[]
            {
                Random.Range(0f,1f),Random.Range(0f,1f)
            };
            double[] label = new double[]{0,0,0,0};
            if (i<total/2)
            {   
                feats[0] *= -1;
            }
            if (i<total/4||i>total-total/4)
            {
                feats[1] *= -1;
            }

            if (feats[0]>0&&feats[1]>0)
            {
                label[0] = 1;
            }else if (feats[0] < 0 && feats[1] > 0)
            {
                label[1] = 1;
            }else if (feats[0] < 0 && feats[1] < 0)
            {
                label[2] = 1;
            }
            else if(feats[0]>0&&feats[1]<0)
            {
                label[3] = 1;
            }
            else
            {
                return;
            }
            */
            datapoints.Add(new Datapoint(feats,label));
            
        }
    }
}
