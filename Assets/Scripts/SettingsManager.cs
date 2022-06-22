using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SFB;

public class SettingsManager : MonoBehaviour
{
    public string contentFolderPath;

    [SerializeField]
    private float defaultPeriod = 0.5f;
    [SerializeField]
    private float defaultShowTime = 10f;
    [SerializeField]
    private float defaultPauseTime = 5f;
    
    [SerializeField]
    private TMP_InputField recordNameInput, periodInput, showTimeInput, pauseInput;
    [SerializeField]
    private GameObject folderPathText;
    [SerializeField]
    private Button startButton, contentFolderButton;
    
    private void OnEnable()
    {
        periodInput.SetTextWithoutNotify(defaultPeriod.ToString());
        showTimeInput.SetTextWithoutNotify(defaultShowTime.ToString());
        pauseInput.SetTextWithoutNotify(defaultPauseTime.ToString());

        contentFolderButton.onClick.AddListener(ReadFolderPath);

    }
    
    public void ReadFolderPath() {
        contentFolderPath = StandaloneFileBrowser.OpenFolderPanel("Выберите папку, в которой находится контент", "", false)[0];
        if(contentFolderPath.Length > 0) {
            folderPathText.GetComponent<TextMeshProUGUI>().text = contentFolderPath;
            startButton.interactable = true;
        }
    }
    
    public Settings GetValues() 
    {
        return new Settings() 
        {
            recordPeriod = float.Parse(periodInput.text),
            name = recordNameInput.text,
            contentFolder = contentFolderPath,
            pausetime = float.Parse(pauseInput.text),
            showtime = float.Parse(showTimeInput.text)
        };
    }

    private void OnDisable()
    {
        contentFolderButton.onClick.RemoveListener(ReadFolderPath);
    }
}