using System.Collections;
using System.Collections.Generic;
using Tobii.Research.Unity;
using UnityEngine;
using UnityEngine.UI;

public class CalibrationManager : MonoBehaviour
{
    [SerializeField] private TrackBoxGuide trackBoxGuide;
    [SerializeField] private Calibration calibration;
    [SerializeField] private GameObject calibrationObject, calibrationText, startGroup, restartText;
    
    private bool _isCalibrationStarted;

    void OnEnable()
    {
        trackBoxGuide.TrackBoxGuideActive = true;
        calibrationText.SetActive(true);
        startGroup.SetActive(false);
        restartText.SetActive(false);
        calibrationObject.SetActive(true);

        _isCalibrationStarted = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _isCalibrationStarted = true;
            trackBoxGuide.TrackBoxGuideActive = false;
            calibrationText.SetActive(false);
            calibrationObject.SetActive(true);
        }

        if (_isCalibrationStarted && !calibration.CalibrationInProgress)
        {
            _isCalibrationStarted = false;
            if (!calibration.LatestCalibrationSuccessful)
            {
                restartText.SetActive(true);
                trackBoxGuide.TrackBoxGuideActive = false;
            }
            else
            {
                trackBoxGuide.TrackBoxGuideActive = false;
                restartText.SetActive(false);
                startGroup.SetActive(true);
                calibrationObject.SetActive(false);
            }
        }
    }
}
