using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Runtime;
using UnityEditor;
using UnityEngine;
//0,08556014000	0,03331612000	0,06979383000	0,13485200000	0,06124239000	0,04175849000	0,13635770000	0,06932896000	0,03750053000	0,12752840000	0,06459449000	0,04782093000	0,11446810000	0,05024698000	0,13584980000
//0,08595730000	0,03331627000	0,07096919000	0,13494070000	0,06128091000	0,04320582000	0,13646290000	0,06935849000	0,03906669000	0,12763390000	0,06460806000	0,05029973000	0,11487560000	0,05025502000	0,13867110000

[CreateAssetMenu(menuName = "Create FileImporter", fileName = "FileImporter", order = 0)]
public class FileImporter : ScriptableObject
{
    //public Dictionary<double[], GestureType> dataPoints = new ();
    //public List<Datapoint> listOfDataPoint = new List<Datapoint>();
    //public List<Datapoint> testSet = new List<Datapoint>();
    public double[] maxes = new double[21]{0.09021042,0.1111464,0.09921884,0.1323981,0.06017102,0.07532546,0.1339429,0.06814501,0.03681857,0.1253064,0.06339972,0.06077478,0.1132728,0.04930152,0.147451,0.9428114,0.9908907,0.9999788,0.30046,1.184354,0.4404907 };
    public double[] mines = new double[21]{0.06838486,0.08991423,0.01402679,0.1115697,0.03685664,0.01539352,0.1057315,0.04198776,0.01321025,0.09662139,0.03922486,0.01349914,0.09491879,0.03493643,0.04432947,-2.75817E-05,3.92234E-06,0.000278436,-0.4935929,0.7348156,0.008642531 };
    //public double[] maxes = new double[15]{0.09021042f,0.1111464f,0.09921884f,0.1323981f,0.06017102f,0.07532546f,0.1339429f,0.06814501f,0.03681857f,0.1253064f,0.06339972f,0.06077478f,0.1132728f,0.04930152f,0.147451f};
    //public double[] mines = new double[15]{0.06838486f,0.08991423f,0.01402679f,0.1115697f,0.03685664f,0.01539352f,0.1057315f,0.04198776f,0.01321025f,0.09662139f,0.03922486f,0.01349914f,0.09491879f,0.03493643f,0.04432947f };
    //public string path;
    public int dictionarySize;
    public int[][] tallyFilter ;
    
    public static double InverseLerp(double a, double b, double value)
    { 
        return (value - a) / (b - a);
       
    }
    [Button(ButtonMode.AlwaysEnabled)]
    public (Datapoint[], Datapoint[]) ReadFile()
    {
        List<Datapoint> trainingSet = new List<Datapoint>();
        List<Datapoint> testSet = new List<Datapoint>();
        int testIndex = 15;
        tallyFilter = new int[17][];
        tallyFilter[0] = new []{1, 2, 3, 8, 34, 41, 42};
        tallyFilter[1] = new []{49,50,65,66,75};
        tallyFilter[2] = new []{3,22,23};
        tallyFilter[3] = new []{1, 12, 13, 60};
        tallyFilter[4] = new []{27, 35, 45, 46, 50};
        tallyFilter[5] = new []{0};//Delete Part
        tallyFilter[6] = new []{27, 35, 45, 46, 50};//Delete Part
        tallyFilter[7] = new []{60};// Swap Light aAnd Season
        tallyFilter[8] = new []{27, 35, 45, 46, 50};//Delete
        tallyFilter[9] = new []{7, 39, 46};
        tallyFilter[10] = new []{8, 12, 18, 21, 35, 44, 60, 68};
        tallyFilter[11] = new []{3,8,32,37,39,57,58};
        tallyFilter[12] = new []{31};
        tallyFilter[13] = new []{1,3,7};
        tallyFilter[14] = new []{11, 27, 40, 70};
        tallyFilter[15] = new []{7, 10, 16, 27, 55, 60, 62, 65, 68, 72, 75};
        tallyFilter[16] = new []{5, 8, 38, 41, 42, 43, 61, 62, 66, 70, 87, 90};

        //Read the text from directly from the test.txt file
        for (int parts = 0; parts < 17; parts++)
        {
            string pathPart = "Assets/Data/Participant "+ (parts+1)+".txt";
            if (parts is 5 or 6 or 8 or 7)
            {
                continue;
            }
            
            StreamReader reader = new StreamReader(pathPart);
            bool fileEmpty = false;
            while (!fileEmpty)
            {
                string nextLine = reader.ReadLine();
                if (nextLine == null)
                {
                    fileEmpty = true;
                    break;
                }

                string[] splitLine = nextLine.Split(";");
                string[] filterCheck = splitLine[^1].Split(",");
                bool check = false;
                for (int i = 0; i < tallyFilter[parts].Length; i++)
                {
                    if (int.Parse(filterCheck[1])==tallyFilter[parts][i])
                    {
                        check = true;
                    }
                }
                if (check)
                {
                    continue;
                }

                string[] values = splitLine[0].Split(",");
                double[] features = new double[18];
                for (int i = 0; i < 18; i++)
                {
                    double val = float.Parse(values[i]);
                    features[i] = InverseLerp(mines[i],maxes[i],val);
                }

                double[] label = {0,0,0,0,0,0};
                switch (splitLine[1])
                {
                    case "Light":
                        label[0] = 1;
                        break;
                    case "Season":
                        label[1] = 1;
                        break;
                    case "Location":
                        label[2] = 1;
                        break;
                    case "Preset":
                        label[3] = 1;
                        break;
                    case "RockOn":
                        label[4] = 1;
                        break;
                    case "Shaka":
                        label[5] = 1;
                        break;
                }

                if (parts== 2 &&0<int.Parse(filterCheck[1])&&int.Parse(filterCheck[1])<12)
                {
                    label = new double[]{ 0, 1, 0, 0, 0, 0 };
                }

                if (testIndex==parts)
                {
                    testSet.Add(new Datapoint(features,label));
                }
                else
                {
                    trainingSet.Add(new Datapoint(features,label));
                }
            }
            reader.Close();
        }
        
        return (trainingSet.ToArray(),testSet.ToArray());
        /*
        string newPath =
            @$"{Environment.CurrentDirectory}\Assets\{"excelGraph"}.csv"; // Directory path + data m. tidspunkt
        Debug.Log("printing to :" + newPath);
        StreamWriter sw = new StreamWriter(newPath); // StreamWriter laver en ny fil
        foreach (Datapoint datapoint in listOfDataPoint)
        {

            string toWrite = datapoint.features[0]+"";
            for (int i = 1; i < datapoint.features.Length; i++)
            {
                toWrite += ","+datapoint.features[i];
            }
            sw.WriteLine(toWrite);
            sw.Flush();
        }
        
        AssetDatabase.Refresh();
        foreach (var t in listOfDataPoint[0].features)
        {
            Debug.Log(t);
        }
        return listOfDataPoint;
        //StreamReader reader = new StreamReader(path);
        
        ////////      IGNORE THIS I NEED IT FOR LATER
        /*
         Light,
    Season,
    Location,
    Preset,
    RockOn,
    Shaka
        Debug.Log(reader.ReadLine());
        Debug.Log(reader.ReadLine());
        //bool fileEmpty = false;
        while (!fileEmpty)
        {
            string nextLine = reader.ReadLine();
            if (nextLine == null)
            {
                fileEmpty = true;
                break;
            }

            nextLine = nextLine.Replace("Light", ";Light");
            nextLine = nextLine.Replace("Season", ";Season");
            nextLine = nextLine.Replace("Location", ";Location");
            string[] splitLine = nextLine.Split(";");
            string vals = splitLine[0].Replace("[", "");
            vals = vals.Replace("]", "");
            string[] values = vals.Split(",");
            double[] arrayToFill = new double[15];
            for (int i = 0; i < 15; i++)
            {
                double doubley = double.Parse(values[i]);
                doubley = Mathf.Clamp(doubley, mins[i], maxs[i]);
                doubley = Mathf.InverseLerp(mins[i], maxs[i], doubley);
                arrayToFill[i] = doubley;
            }
            GestureType type;
            double[] gest = {0,0,0};
            switch (splitLine[1])
            {
                case "Light":
                    gest[0] = 1;
                    break;
                case "Season":
                    gest[1] = 1;
                    break;
                case "Location":
                    gest[2] = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            double[][] fasd ={arrayToFill,gest}; 
            listOfDataPoint.Add(fasd);
        }
        reader.Close();
        
        //////////////STUFF UDER HERE IS IRELEVANT------------------------
        
        
        string newPath =
            @$"{Environment.CurrentDirectory}\Assets/HandTrackingData/Data\{"testing"}.csv"; // Directory path + data m. tidspunkt
        Debug.Log("printing to :" + newPath);
        StreamWriter sw = new StreamWriter(newPath); // StreamWriter laver en ny fil
        foreach (double[][] arrayOfnumXges in listOfDataPoint)
        {
            
            string toWrite = arrayOfnumXges[0][0] + "";
            for (var i = 1; i < arrayOfnumXges[0].Length; i++)
            {
                toWrite += ";" + arrayOfnumXges[0][i];
            }

            toWrite += ";" + arrayOfnumXges[1][0];
            toWrite += ";" + arrayOfnumXges[1][1];
            toWrite += ";" + arrayOfnumXges[1][2];
            sw.WriteLine(toWrite);
            sw.Flush();
        }
       
        AssetDatabase.Refresh();
    */
        
    }/*
    [Button]
    public void CheckLabels()
    {
        int[] lables = new int[7];
        foreach (Datapoint datapoint in listOfDataPoint)
        {
            for (var i = 0; i < datapoint.label.Length; i++)
            {
                if ((int)datapoint.label[i] == 1)
                {
                    lables[i]++;
                    break;
                }
            }
        }

        for (int i = 0; i < lables.Length; i++)
        {
            Debug.Log(lables[i]);
        }
        
    }
    [Button]
    public void CheckValues()
    {
        double min  = double.MaxValue;
        double max = double.MinValue;
        foreach (Datapoint datapoint in listOfDataPoint)
        {
            max = datapoint.features.Prepend(max).Max();
            if ( datapoint.features.Prepend(min).Min()<-1)
            {
                Debug.Log("something is wreon");
            }
            min = datapoint.features.Prepend(min).Min();
            
        }

        Debug.Log(min + " "+ max);
    }
    */


    public List<Datapoint> GetTestData()
    {
        List<Datapoint> datas = new List<Datapoint>();
        
        //Read the text from directly from the test.txt file
        for (int parts = 0; parts < 5; parts++)
        {
            string pathPart = "Assets/11-23-2022 2-09-46 PMPelleData.txt";
            StreamReader reader = new StreamReader(pathPart);
            bool fileEmpty = false;
            while (!fileEmpty)
            {
                string nextLine = reader.ReadLine();
                if (nextLine == null)
                {
                    fileEmpty = true;
                    break;
                }

                string[] splitLine = nextLine.Split(";");
                bool check = false;
                if (splitLine[2]=="0")
                {
                    check = true;
                }
                if (check)
                {
                    continue;
                }
                
                string[] values = splitLine[0].Split(",");
                double[] features = new double[21];
                for (int i = 0; i < 15; i++)
                {
                    double val = (double)float.Parse(values[i]);
                    features[i] = InverseLerp(mines[i],maxes[i],val);
                }

                features[15] = (double)float.Parse(values[15]);

                features[16] = (double)float.Parse(values[16]);
                features[17] = (double)float.Parse(values[17]);
                features[18] = (double)float.Parse(values[18]);
                features[19] = (double)float.Parse(values[19]);
                features[20] = (double)float.Parse(values[20]);
                double[] label = {0,0,0,0,0,0};
                switch (splitLine[1])
                {
                    case "Light":
                        label[0] = 1;
                        break;
                    case "Season":
                        label[1] = 1;
                        break;
                    case "Location":
                        label[2] = 1;
                        break;
                    case "Preset":
                        label[3] = 1;
                        break;
                    case "RockOn":
                        label[4] = 1;
                        break;
                    case "Shaka":
                        label[5] = 1;
                        break;
                }
                
                datas.Add(new Datapoint(features,label));
            }
            reader.Close();
            
        }

        Debug.Log("read "+datas.Count +" datapoints");
        return datas;
        
    }
}
public readonly struct Datapoint
{
    public readonly double[] features;
    public readonly double[] label;

    public Datapoint(double[] features, double[] label)
    {
        this.features = features;
        this.label = label;
    }
}