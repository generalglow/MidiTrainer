using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongHUD : MonoBehaviour
{
    // Start is called before the first frame update
    public Button PlayButton;
    public Button PauseButton;
    public Button RestartButton;
    public Button QuitButton;
    public ReaderV2 Chart;
    public GameObject SongBrowser;
    public GameObject Highway;
    public GameObject StrikerLine;
    void Start()
    {
        PlayButton.onClick.AddListener(PlaySong);
        PauseButton.onClick.AddListener(PauseSong);
        RestartButton.onClick.AddListener(RestartSong);
        QuitButton.onClick.AddListener(Quit);
    }

    // Update is called once per frame
    void PlaySong(){
        Chart.Play = true;
    }

    void PauseSong(){
        Chart.Play = false;
    }

    void RestartSong(){
        Chart.Restart = true;
    }

    void Quit(){
        PauseSong();
        Chart.UnloadChart();
        Highway.SetActive(false);
        StrikerLine.SetActive(false);
        SongBrowser.SetActive(true);
        gameObject.SetActive(false);
    }
}
