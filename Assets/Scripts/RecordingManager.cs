using System.Collections;
using System.IO;
using System.Linq;
using Tobii.Research.Unity;
using UnityEngine;
using System.Globalization;


public class RecordingManager : MonoBehaviour {
    public string sergeiPath;
    public string andrewPath;

    [SerializeField] private GameObject _mainMenu, _exitText, _mouseFakingWarning;
    [SerializeField] private SettingsManager _settingsManager;
    [SerializeField] private GalleryManager _galleryManager;
    [SerializeField] private Camera mainCamera;
    
    private RecordManager _recordManager;
    private EyeTracker _eyeTracker;
    private Record _currentRecord;
    private bool _isRecordingRunning;
    private bool _isMouseInsteadOfEyes;
  
    private void Awake() {
        _recordManager = new RecordManager();
        _eyeTracker = EyeTracker.Instance;
    }

    public void StartRecording() {
        _exitText.SetActive(false);
        _currentRecord = _recordManager.CreateRecord(_settingsManager.GetValues());

        _isRecordingRunning = true;
        _galleryManager.PlayGallery(_currentRecord.settings, EndRecording);
        StartCoroutine(RecordGazePoint());
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (_isRecordingRunning) {
                EndRecording();
            } else {
                ExitToMenu();
            }
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            _isMouseInsteadOfEyes = !_isMouseInsteadOfEyes;
            _mouseFakingWarning.SetActive(_isMouseInsteadOfEyes);
        }
    }

    private void ExitToMenu() {
        _mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    private void EndRecording() {
        StopAllCoroutines();
        _isRecordingRunning = false;
        _exitText.SetActive(true);
        _galleryManager.EndPlaying();
        _recordManager.SaveRecord(_settingsManager.contentFolderPath);
    }

    private IEnumerator RecordGazePoint() {
        while (_isRecordingRunning) {
            IGazeData data = _eyeTracker.LatestGazeData;

            if (data.CombinedGazeRayScreenValid || _isMouseInsteadOfEyes) {
                Vector3 origin = _isMouseInsteadOfEyes
                    ? mainCamera.ScreenToWorldPoint(Input.mousePosition)
                    : data.CombinedGazeRayScreen.origin;
                
                _galleryManager.ChangePointColor(_isMouseInsteadOfEyes ? Color.white : Color.clear);
                _galleryManager.RenderGazePoint(origin);
                LoadEmotionData(out float valence, out float arousal, out AnalysisResult ecgData);
                _recordManager.AddPoint(origin, valence, arousal, ecgData);
            }

            yield return new WaitForSeconds(_currentRecord.settings.recordPeriod);
        }
    }

    private void LoadEmotionData(out float valence, out float arousal, out AnalysisResult ecgData)
    {
        string[] sergeiData = File.ReadLines(sergeiPath).Last().Split(' ');
        valence = float.Parse(sergeiData[0], CultureInfo.InvariantCulture);
        arousal = float.Parse(sergeiData[1], CultureInfo.InvariantCulture);
        
        string jsonString = File.ReadAllText(andrewPath);
        AnalysisResult ar = JsonUtility.FromJson<AnalysisResult>(jsonString);

        ecgData = ar;
    }
}