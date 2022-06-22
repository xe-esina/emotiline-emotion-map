using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GalleryManager : MonoBehaviour {
    [SerializeField] private RawImage _galleryHolder;
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private RectTransform _galleryRect, _gazeCircleRect;
    [SerializeField] private Image _gazeImage;
    
    private List<GameObject> _trail;
    private Settings _curSettings;
    private Action _onGalleryEnd;
    
    public void ChangePointColor(Color newColor) {
        newColor.a = 0.5f;
        _gazeImage.color = newColor;
    }

    public void RenderGazePoint(Vector3 coord) {
        Vector3 scaledCoord = new Vector3(coord.x * transform.localScale.x, coord.y * transform.localScale.y, 0);
        _gazeCircleRect.transform.position =  _gazeCircleRect.transform.parent.position +  scaledCoord;
    }

    public void AddPointToTrail() {
        _trail ??= new List<GameObject>();
        _trail.Add(Instantiate(_gazeCircleRect.gameObject, _gazeCircleRect.parent));
    }
    
    private void ClearTrail() {
        _trail ??= new List<GameObject>();
        foreach (var obj in _trail) {
            Destroy(obj);
        }
        _trail.Clear();
    }

    public void PlayGallery(Settings settings, Action onGalleryEnd = null) {
        ClearTrail();
        _onGalleryEnd = onGalleryEnd;
        _curSettings = settings;
        StartCoroutine(ShowContent());
    }

    public void EndPlaying() {
        StopAllCoroutines();
        ClearTrail();
        _gazeCircleRect.gameObject.SetActive(false);
        _galleryHolder.enabled = false;
    }

    private IEnumerator ShowContent() {
        string[] files = Directory.GetFiles(_curSettings.contentFolder);
        foreach (string file in files) {
            if (file.EndsWith(".png") || file.EndsWith(".jpg")) {
                _galleryHolder.enabled = true;

                SetTextureFromImageFile(file);

                yield return new WaitForSeconds(_curSettings.showtime);
               
                ClearTrail();
                _galleryHolder.enabled = false;
                _gazeCircleRect.gameObject.SetActive(false);
                
                yield return new WaitForSeconds(_curSettings.pausetime);
                
                _gazeCircleRect.gameObject.SetActive(true);
            } else if (file.EndsWith(".mp4")) {
                _galleryHolder.texture = null;
                _galleryHolder.enabled = true;

                _videoPlayer.url = "file://" + file;

                _videoPlayer.Prepare();
                while (!_videoPlayer.isPrepared) {
                    yield return null;
                }

                SetTextureFromVideoFile();

                _videoPlayer.Play();
                while (_videoPlayer.isPlaying) {
                    ClearTrail();
                    yield return null;
                }

                _galleryHolder.enabled = false;
                _gazeCircleRect.gameObject.SetActive(false);
                ClearTrail();
                
                yield return new WaitForSeconds(_curSettings.pausetime);
                
                _gazeCircleRect.gameObject.SetActive(true);
            }

            ClearTrail();
        }

        _onGalleryEnd?.Invoke();
    }

    private void SetTextureFromImageFile(string file) {
        var fileData = File.ReadAllBytes(file);
        Texture2D tex = new(2, 2);
        tex.LoadImage(fileData);

        _galleryHolder.texture = tex;
        float multiplier = 504f / tex.height;
        _galleryRect.sizeDelta = new Vector2(tex.width * multiplier, 504);
    }

    private void SetTextureFromVideoFile() {
        var tex = _videoPlayer.texture;
        _galleryHolder.texture = tex;
        float multiplier = 504f / tex.height;
        _galleryRect.sizeDelta = new Vector2(tex.width * multiplier, 504);
    }
}