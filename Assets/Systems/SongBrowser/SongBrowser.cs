using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using UnityEngine.Networking;

public class SongBrowser : MonoBehaviour
{
    public GameObject Chart;
    public GameObject Highway;
    public GameObject SongHUD;
    public GameObject StrikerLine;
    public string SongDirectory;
    public bool Verbose = false;
    public Button MidiMapManagerBtn;
    public GameObject MidiMapManager;
    public GameObject ListEntryTemplate;
    List<SongInformation>SongLibrary = new List<SongInformation>();
    Transform trackList;
    Scrollbar scrollBar;
    ReaderV2 midiReader;
    AudioSource audioPlayer;
    void Start()
    {
        midiReader = Chart.GetComponent<ReaderV2>();
        audioPlayer = Chart.GetComponent<AudioSource>();
        scrollBar = GetComponentInChildren<Scrollbar>();
        trackList = transform.GetChild(0);
        locateSongs();
        if(SongLibrary.Count > 0){
            createTracklist();
        }
        MidiMapManagerBtn.onClick.AddListener(ShowMidiMapManager);
    }

    // Update is called once per frame
    void Update()
    {
        trackList.transform.position = new Vector3(trackList.transform.position.x, (scrollBar.value * (SongLibrary.Count * 25)), trackList.transform.position.z);
    }

    void ShowMidiMapManager(){
        MidiMapManager.SetActive(true);
    }

    void locateSongs(){
        if(Verbose){Debug.Log("Locating Songs... ");}
        //add each folder within the song directory that contains a midi file, mp3 m,  to the collection
        foreach(string s in new List<string>(Directory.EnumerateDirectories(SongDirectory))){
            if(Verbose){Debug.Log("Searching " + s);}
            if(Directory.GetFiles(s, "*.mid").Length == 1){
                var songTitle = Path.GetFileName(s); //strips away the file path
                if(Verbose){Debug.Log(songTitle + " midi located");}
                if(Directory.GetFiles(s, "*.mp3").Length == 1){
                    if(Verbose){Debug.Log(songTitle + " mp3 located");}
                    //check if song info exists
                    if(Directory.GetFiles(s, "info.txt").Length == 1){
                        if(Verbose){Debug.Log("song info located");}
                        loadSongInformation(s);
                    }
                    else{
                        if(Verbose){Debug.Log("Song information not found: missing info.txt");}
                    }
                    
                }
                else{
                    if(Verbose){Debug.Log("Incorrect number of mp3 files present {should be 1)");}
                }
            }
            else{
                if(Verbose){Debug.Log("Incorrect number of midi files present {should be 1)");}
            }
        }
        if(Verbose){Debug.Log("Song library containts " + SongLibrary.Count + " playable Tracks:");}
        foreach(SongInformation s in SongLibrary){
            if(Verbose){Debug.Log(s.Artist + ":" + s.Title);}
        }
    }
    void createTracklist(){
        
        scrollBar.value = 0;
        for(int i = 0; i < SongLibrary.Count; i++){ //create a list entry with this transform's first child (the tracklist) as parent for scrolling
            var listEntry = Instantiate(ListEntryTemplate, new Vector3(trackList.transform.position.x, trackList.transform.position.y - (i * 25), trackList.transform.position.z), Quaternion.identity, trackList);
            listEntry.transform.GetComponentInChildren<Text>().text = "Title: " + SongLibrary[i].Title + "\n\n" + 
                                                                      "Artist: " + SongLibrary[i].Artist +"\n\n" + 
                                                                      "Duration: " + SongLibrary[i].Duration + "\n\n" +
                                                                      "MP3 Offset: " + SongLibrary[i].MP3Offset + "\n\n" +
                                                                      "Note Offset: " + SongLibrary[i].NoteOffset;
            listEntry.name = SongLibrary[i].Title;
            listEntry.GetComponentInChildren<Button>().onClick.AddListener(delegate{playSong(listEntry.name);});
        }
    }

    void loadSongInformation(string directory){
        var newSong = new SongInformation();
        newSong.FilePath = directory;
        string[] lines = File.ReadAllLines(directory+"/info.txt");
        for(int l = 0; l < lines.Length; l++)
        {
            string[] info = lines[l].Split(':');
            switch (info[0].ToLower()){
                case("title"):
                    newSong.Title = info[1];
                break;
                case("artist"):
                    newSong.Artist = info[1];
                break;
                case("duration"):
                    newSong.Duration = info[1];
                break;
                case("mp3offset"):
                    newSong.MP3Offset = info[1];
                break;
                case("noteoffset"):
                    newSong.NoteOffset = info[1];
                break;
                default:
                    Debug.Log("Song information line " + l + " : '" + info[1] + "' invalid");
                break;
            }
        }
        //error catching/missing information (e.g. auto detect track duration, name)
        SongLibrary.Add(newSong);
        if(Verbose){Debug.Log("Added " + newSong.Title + " to library");};
    }

    void playSong(string songTitle){
        var track = SongLibrary.Where(s=>s.Title == songTitle).First();
        if(Verbose){Debug.Log("Loading " + track.Title);}
        StartCoroutine(GetAudioClip(track.FilePath + '/' + track.Title + ".mp3"));
        midiReader.FilePath = track.FilePath + '/' + track.Title + ".mid";
        midiReader.mp3Offset = track.MP3Offset != null ? float.Parse(track.MP3Offset) : 0;
        if(Verbose){Debug.Log("Browser mp3 offset: "+ midiReader.mp3Offset);}
        midiReader.noteOffset = track.NoteOffset != null ? float.Parse(track.NoteOffset) : 0;
        midiReader.LoadChart();
    }


    IEnumerator GetAudioClip(string mp3filepath)
    {
        //string url = string.Format("file://{0}", mp3filepath); 
        if(Verbose){Debug.Log("Attempting to open MP3 from: \n" + mp3filepath + "\n");}
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(mp3filepath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                audioPlayer.clip = DownloadHandlerAudioClip.GetContent(www);
                if(Verbose){Debug.Log("Audio Clip Loaded");}
                Highway.SetActive(true);
                StrikerLine.SetActive(true);
                SongHUD.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }

}
