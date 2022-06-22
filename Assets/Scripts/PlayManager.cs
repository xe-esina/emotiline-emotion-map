using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour {
    [SerializeField] private GameObject _mainMenu, _endOfRecord;
    [SerializeField] private GalleryManager _galleryManager;
    [SerializeField] private TextMeshProUGUI _valenceText, _arousalText, _emotionText, _andrewStringText;
    [SerializeField] private Image _emotionDiagramImage;
    
    private RecordManager _recordManager;
    private Record _currentRecord;
    private bool _isRecordPlaying;

    private void Awake() {
        _recordManager = new RecordManager();
    }

    public void EndPlaying() {
        if (!_isRecordPlaying) {
            ExitToMenu();
        }
        _galleryManager.EndPlaying(); 
        StopAllCoroutines();
        _endOfRecord.SetActive(true);
        
        _isRecordPlaying = false;
    }

    private void ExitToMenu() {
        _mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void StartPlaying() {
        _isRecordPlaying = true;
        _endOfRecord.SetActive(false);
        
        try {
            _currentRecord = _recordManager.LoadRecord();
            _galleryManager.PlayGallery(_currentRecord.settings, EndPlaying);
            StartCoroutine(PlayGazePoint());
        }
        catch {
            _mainMenu.SetActive(true);
            gameObject.SetActive(false);
            Debug.LogWarning("No record found");
        }
    }

    private IEnumerator PlayGazePoint() {
        foreach (var point in _currentRecord.points) {
            Emotion curEmotion = EmotionsManager.GetEmotion(point);
            Color emotionColor = EmotionsManager.GetColor(curEmotion);
            
            _emotionDiagramImage.color = emotionColor;
            
            if (point.coordinates == Vector3.zero) {
                _galleryManager.ChangePointColor(Color.clear);
            } else {
                _galleryManager.ChangePointColor(emotionColor);
                _galleryManager.RenderGazePoint(point.coordinates);
                _galleryManager.AddPointToTrail(); 
            }
            
            _valenceText.text = $"Валентность: {Math.Round(point.valence, 2)}";
            _arousalText.text = $"Активация: {Math.Round(point.arousal, 2)}";
            _emotionText.text = $"{curEmotion}";
            _andrewStringText.text = point.ECGData.keywords.ToString();

            yield return new WaitForSeconds(_currentRecord.settings.recordPeriod);
        }
    }
}