using System.Collections;
using System.Collections.Generic;
using System.IO;
using Runtime;
using UnityEngine;

[CreateAssetMenu(menuName = "Create CsvImporter", fileName = "CsvImporter", order = 0)]
public class CSVImporter : ScriptableObject
{
    public List<Datapoint> Datapoints = new List<Datapoint>();
    public string csvPath;
    [Button]
    public List<Datapoint> ReadCSV()
    {
        StreamReader reader = new StreamReader(csvPath);
        string skipFirstLine = reader.ReadLine();
        bool fileEmpty = false;
        while (!fileEmpty)
        {
            string nextLine = reader.ReadLine();
            if (nextLine == null)
            {
                fileEmpty = true;
                break;
            }

            string[] splitLine = nextLine.Split(",");
                
            double[] features = {double.Parse(splitLine[0]),double.Parse(splitLine[1])};
            
            double[] label = {1,0};
            if (splitLine[2] =="1")
            {
                label = new double[]{0,1} ;
            }
            Datapoints.Add(new Datapoint(features,label));
        }
        reader.Close();
        return Datapoints;
    }
}