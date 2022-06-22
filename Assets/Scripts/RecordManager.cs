using System;
using System.Collections.Generic;
using System.IO;
using SFB;
using UnityEngine;
using Random = UnityEngine.Random;

public class RecordManager {

    private Record _record;
    
    public Record CreateRecord(Settings _settings) 
    {
        _record = new Record {
            settings = _settings,
            points = new List<Point>()
        };
        return _record;
    }

    public Record LoadRecord()
    {
        try {
            string path =
                StandaloneFileBrowser.OpenFilePanel("Выберите файл, в котором находится история", "", "", false)[0];
            string jsonString = File.ReadAllText(path);
            
            _record = JsonUtility.FromJson<Record>(jsonString);
            return _record;
        }
        catch (Exception e) {
            Debug.LogWarning("Loading aborted");
            return null;
        }
    }

    public void AddPoint(Vector3 pointCoord, float valence, float arousal, AnalysisResult ecgData) 
    {
        Point point = new Point() {
            coordinates = pointCoord,
            valence = valence,
            arousal = arousal,
            ECGData = ecgData
        };
        
        _record.points.Add(point);
    }
    
    public void SaveRecord(string folderPath) {
        string txt = JsonUtility.ToJson(_record);

        string fullpath = folderPath + "\\";
        if (_record.settings.name.Length > 0) {
            fullpath += _record.settings.name + ".txt";
        } else {
            fullpath += "Record_" + DateTime.Today + ".txt";
        }

        File.WriteAllText(fullpath, txt);
    }


}

[Serializable]
public class Record {
    public Settings settings;
    public List<Point> points;
}

[Serializable]
public class Point {
    public Vector3 coordinates;
    public float valence;
    public float arousal;
    public AnalysisResult ECGData;
}

[Serializable]
public class Settings {
    public string name;
    public string contentFolder;
    public float recordPeriod;
    public float showtime, pausetime;
}


[Serializable]
public class AnalysisResult {
    public Dictionary<string, float> generalResult;
    public Dictionary<string, float> spectralResult;
    public List<string> keywords;
}