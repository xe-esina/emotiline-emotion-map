using UnityEngine;

public class EmotionsManager {
    // 'EEG Valence', 'EEG Arousal', 'rmssd', 'sdnn', 'si', 'hf%', 'vlf%'
    private static readonly float[] ValenceCoef = { 0.8627f, -0.0195f, 0.0003f, -0.0005f, -0.00003f, 0.0129f, 0.0129f };
    // 'EEG Valence', 'EEG Arousal', 'hr', 'sdnn', 'si'
    private static readonly float[] ArousalCoef = { -0.0588f, 0.7897f, 0.0057f, 0.0009f, 0.0001f };
    
    public static Emotion GetEmotion(Point point)
    {

        float valence = ((point.valence + 1f) * 5f) * ValenceCoef[0] +
                        ((point.arousal + 1f) * 5f) * ValenceCoef[1] +
                        point.ECGData.generalResult["rmssd"] * ValenceCoef[2] +
                        point.ECGData.generalResult["sdnn"] * ValenceCoef[3] +
                        point.ECGData.generalResult["si"] * ValenceCoef[4] +
                        point.ECGData.spectralResult["hf%"] * ValenceCoef[5] +
                        point.ECGData.spectralResult["vlf%"] * ValenceCoef[6];

        float arousal = ((point.valence + 1f) * 5f) * ArousalCoef[0] +
                        ((point.arousal + 1f) * 5f) * ArousalCoef[1] +
                        point.ECGData.generalResult["hr"] * ArousalCoef[2] +
                        point.ECGData.generalResult["sdnn"] * ArousalCoef[3] +
                        point.ECGData.generalResult["si"] * ArousalCoef[4];

        valence = valence / 5f - 1;
        arousal = arousal / 5f - 1;


        if (valence == 0f && arousal == 0f)
            return Emotion.Нейтральность;
        
        Vector2 coord = new Vector2(valence, arousal).normalized;
        float angle = Vector2.Angle(Vector2.right, coord);
        float clampedAngle = Mathf.Round((angle / 45f)) * 45;
        if (arousal < 0f)
            clampedAngle += 180;

        return (Emotion) (int)clampedAngle;
    }

    public static Color GetColor(Emotion emotion)
    {
        return emotion switch
        {
            Emotion.Удовольствие => Color.red,
            Emotion.Возбуждение => Color.yellow,
            Emotion.Активация => Color.green,
            Emotion.Расстройство => Color.cyan,
            Emotion.Неудовольствие => Color.blue,
            Emotion.Подавленность => Color.black,
            Emotion.Усталость => Color.gray,
            Emotion.Расслабленность => Color.magenta,
            Emotion.Нейтральность => Color.white
        };
    }
}
// Add more emotions if you want to!
public enum Emotion {
    Нейтральность = -1,
    Удовольствие = 0,
    Возбуждение = 45,
    Активация = 90,
    Расстройство = 135,
    Неудовольствие = 180,
    Подавленность = 225,
    Усталость = 270,
    Расслабленность = 315
}