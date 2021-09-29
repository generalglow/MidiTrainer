using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;


public class ReaderV2 : MonoBehaviour
{
    public bool Play = false;
    public bool Restart = false;
    public string FilePath;
    public long BPM;
    public float Scale = 1.0f;
    public int Nominator = 8;
    public float AfterHit = 0.0f;

    public scoreTracker _ScoreTracker;
    
    public GameObject DefaultNote;
    public GameObject Kick;
    public GameObject Snare;
    public GameObject Tom1;
    public GameObject Tom2;
    public GameObject Tom3;
    public GameObject HihatOpen;
    public GameObject HihatClosed;
    public GameObject HihatPedal;
    public GameObject Crash1;
    public GameObject Crash2;
    public GameObject Ride;
    public GameObject RideBell;
    public GameObject China;
    public GameObject BarLine;
    public GameObject Stave;
    public GameObject Striker;

    //const float WHOLENOTE = 1920.0f; song 3
    const float WHOLEBAR = 2400.0f; //profound
    
    
    MidiFile midiFile;
    TimeSignature timeSig;


    public float mp3Offset;
    public float noteOffset;
    AudioSource mp3;

    float seek = 0.0f;    
    long QuarterNote;

    List<moveNote> chartNotes = new List<moveNote>();
    bool loaded = false;

    public void LoadChart()
    {
        //load file
        long offset = 0;
        long finalNote = 0;
        midiFile = MidiFile.Read(FilePath, null);
        Debug.Log("loaded: " + FilePath);
        //rip chunks from file
        if(midiFile.Chunks != null)
        {
            //load tempo map
            TempoMapManager tempoMapManager = midiFile.ManageTempoMap();
            TempoMap tempoMap = tempoMapManager.TempoMap;
            timeSig = tempoMap.TimeSignature.AtTime(offset);
            BPM = tempoMap.Tempo.AtTime(offset).BeatsPerMinute;
            QuarterNote = tempoMap.Tempo.AtTime(offset).MicrosecondsPerQuarterNote;
            QuarterNote /= 100000;
            Debug.Log(timeSig.Numerator + "/" + timeSig.Denominator + "  tempo: " + tempoMap.Tempo.AtTime(offset) + " == " + BPM + " BPM");
            Debug.Log("Adjusted 1/4 Note: " + QuarterNote);
           
            //initialise mp3
            mp3 = GetComponent<AudioSource>();
            mp3.time = mp3Offset;
            Debug.Log("mp3 Loaded with offset: " + mp3.time);


            //split track chunks
            foreach (TrackChunk t in midiFile.Chunks)
            {
                //get all notes from chunk
                using (var notesManager = t.ManageNotes())
                {
                    NotesCollection notes = notesManager.Notes;
                    
                    foreach(Note n in notes)
                    {
                        //set the first note to appear in the middle of the screen
                        if(offset == 0)
                        {
                            offset = n.Time;
                            Debug.Log("first note offset: " + offset);
                        }
                        chartNotes.Add(CreateNote((n.Time - offset + noteOffset), n.NoteNumber));        
                        finalNote = (n.Time - offset + (long)noteOffset) * QuarterNote;                       
                    }
                    Debug.Log("Total Notes: " + chartNotes.Count);
                }
                //scale notes
                foreach(moveNote n in chartNotes)
                {
                    n.transform.position = new Vector3(n.transform.position.x, (n.getTime() / 100.0f) * 3, n.transform.position.z);
                }

                
                //draw bar lines
                float barLinePos = 0.0f;
                float numBarLine = 1;
                while(barLinePos < finalNote){
                    GameObject b = Instantiate(BarLine, new Vector3(-1.0f, barLinePos/100.0f, 0.5f), Quaternion.identity);
                    b.transform.SetParent(gameObject.transform);
                    barLinePos += ((timeSig.Denominator * QuarterNote * 100)/timeSig.Numerator) * QuarterNote;
                    if(numBarLine == 1){
                        b.transform.localScale = new Vector3(b.transform.localScale.x, 0.025f, b.transform.localScale.z);
                    }
                    if(numBarLine == Nominator){
                        numBarLine = 1;
                    }
                    else{
                        numBarLine++;
                    }
                }
            }
            loaded = true;
        }
        else{
            Debug.Log("no chunks found");
        }  
    }

    public void UnloadChart(){//delete all notes and barlines on chart and remove audio clip
        for(int i = 0; i < transform.childCount; i++){
            Destroy(transform.GetChild(i).gameObject);
        }
        mp3.clip = null;
        transform.position = Vector3.zero;
        chartNotes = new List<moveNote>();
        Debug.Log("Chart and Audio Deleted");
    }

    void Update()
    {   
        if(loaded){
            if(Restart){
                seek = mp3Offset;
                for(int i = 0; i < transform.childCount; i ++){
                    transform.GetChild(i).gameObject.SetActive(true);
                }
                mp3.time = mp3Offset;
                gameObject.transform.position = new Vector3(transform.position.x, -seek * BPM/100 * Scale, transform.position.z);
                Restart = false;            
            }
            if(Play){
                if(mp3.time < mp3Offset && seek < mp3Offset){
                    Restart = true;
                    return;
                }
                seek = mp3.time - mp3Offset;
                gameObject.transform.position = new Vector3(transform.position.x, -seek * BPM/100 * Scale, transform.position.z);
                if(!mp3.isPlaying){
                    mp3.Play();
                }
            }
            else if(mp3.isPlaying){
                mp3.Pause();
            }
        }
    }

    moveNote CreateNote(float pos, SevenBitNumber noteNumber)
    {
        GameObject n;
        //pos *= 100;
        switch (noteNumber.ToString())
        {
            case "34": //hihat pedal 2
            {
                n = Instantiate(HihatPedal, new Vector3(-11.0f, pos, 0f), Quaternion.identity);
                n.GetComponent<moveNote>().Lane = 10;
                break;
            }
            case "35": //kick
            {
                n = Instantiate(Kick, new Vector3(-1.0f, pos, 0f), Quaternion.identity);
                n.GetComponent<moveNote>().Lane = 0;                                                                    
                break;
            }
            case "36": //kick 2
            {
                n = Instantiate(Kick, new Vector3(-1.0f, pos, 0f), Quaternion.identity);
                n.GetComponent<moveNote>().Lane = 0;                                                                   
                break;
            }
            case "38": //snare
            {
                n = Instantiate(Snare, new Vector3(-9.0f, pos, 0f), Quaternion.identity);
                n.GetComponent<moveNote>().Lane = 1;
                break;
            }
            case "39": //snare
            {
                n = Instantiate(Snare, new Vector3(-9.0f, pos, 0f), Quaternion.identity);
                n.GetComponent<moveNote>().Lane = 1;
                break;
            }
            case "40": //snare
            {
                n = Instantiate(Snare, new Vector3(-9.0f, pos, 0f), Quaternion.identity);
                n.GetComponent<moveNote>().Lane = 1;
                break;
            }
            case "41": //tom 3
            {
                n = Instantiate(Tom3, new Vector3(5.0f, pos, 0f), Quaternion.identity);
                n.GetComponent<moveNote>().Lane = 7;
                break;
            }
            case "42": //hihat closed
            {
                n = Instantiate(HihatClosed, new Vector3(-7.0f, pos, 0f), Quaternion.identity);
                n.GetComponent<moveNote>().Lane = 2;
                break;
            }
            case "43": //tom 3
            {
                n = Instantiate(Tom3, new Vector3(5.0f, pos, 0f), Quaternion.identity);
                n.GetComponent<moveNote>().Lane = 7;
                break;
            }
            case "44": //hihat pedal
            {
                n = Instantiate(HihatOpen, new Vector3(-11.0f, pos, 0f), Quaternion.identity);
                n.GetComponent<moveNote>().Lane = 10;
                break;
            }
            case "45": //tom 2
            {
                n = Instantiate(Tom2, new Vector3(-1.0f, pos, 0f), Quaternion.identity);
                n.GetComponent<moveNote>().Lane = 5;
                break;
            }
            case "46": //hihat open
            {
                n = Instantiate(HihatOpen, new Vector3(-7.0f, pos, 0f), Quaternion.identity);
                n.GetComponent<moveNote>().Lane = 2;
                break;
            }
            case "48": //tom 1
            {
                n = Instantiate(Tom1, new Vector3(-3.0f, pos, 0f), Quaternion.identity);
                n.GetComponent<moveNote>().Lane = 4;
                break;
            }
            case "49": //left crash
            {
                n = Instantiate(Crash1, new Vector3(-5.0f, pos, 0f), Quaternion.identity);
                n.GetComponent<moveNote>().Lane = 3;
                break;
            }
            case "50": //tom 3
            {
                n = Instantiate(Tom3, new Vector3(5.0f, pos, 0f), Quaternion.identity);
                n.GetComponent<moveNote>().Lane = 8;
                break;
            }
            case "51": //ride
            {
                n = Instantiate(Ride, new Vector3(3.0f, pos, 0f), Quaternion.identity);
                n.GetComponent<moveNote>().Lane = 7;
                break;
            }
            case "53": //ride bell
            {
                n = Instantiate(RideBell, new Vector3(3.0f, pos, 0f), Quaternion.identity);
                n.GetComponent<moveNote>().Lane = 7;
                break;
            }
           
            case "57": //china
            {
                n = Instantiate(China, new Vector3(7.0f, pos, 0f), Quaternion.identity);
                n.GetComponent<moveNote>().Lane = 9;
                break;
            }
            case "59": //ride 2
            {
                n = Instantiate(Ride, new Vector3(3.0f, pos, 0f), Quaternion.identity);
                n.GetComponent<moveNote>().Lane = 7;
                break;
            }
            case "83": //right crash 2
            {
                n = Instantiate(Crash2, new Vector3(1.0f, pos, 0f), Quaternion.identity);
                n.GetComponent<moveNote>().Lane = 6;
                break;
            }
            default:
            {
                n = Instantiate(DefaultNote, new Vector3(noteNumber - 45.0f, pos, 0f), Quaternion.identity);
                break;
            }
        }
        n.transform.SetParent(gameObject.transform);
        n.GetComponent<moveNote>().midiNote = noteNumber.ToString();
        n.GetComponent<moveNote>().Limit = Striker.transform.position.y - AfterHit;
        n.GetComponent<moveNote>().setTime(pos);
        n.GetComponent<moveNote>().ScoreTracker = _ScoreTracker;
        return n.GetComponent<moveNote>();
    }
}
