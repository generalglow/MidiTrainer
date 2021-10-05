using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Melanchall.DryWetMidi.Devices;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

public class ReceiverV2 : MonoBehaviour
{
    public float StrikerHold = 0.1f;    //how long the striker is "held", allowing it to clear notes
    //game objects for note strikers. Toms are colour coded (legacy), cymbals are named.
    public Collider StrikerZone; //zone in which notes are playable

    //particle systems that play when notes are cleared
    public ParticleSystem RedParticle, YellowParticle, BlueParticle, GreenParticle, OrangeParticle, KickParticle, HHPParticle, GreyParticle; //particle systems that play when notes are cleared
    public bool Verbose = false;
    Dictionary<string, ParticleSystem> noteParticles = new Dictionary<string, ParticleSystem>(); //dictionary linking notes to their relevant particle systems
    Dictionary<string, int> particleLanes = new Dictionary<string, int>(); //dictionary linking particle systems to lanes
    Dictionary<int, string> kitMap = new Dictionary<int, string>();  //kit profile with mapping for midi note -> lane
    Dictionary<string, List<moveNote>> playableNotes = new Dictionary<string, List<moveNote>>(); //all the notes currently in range in order per lane
    Queue<string> noteNumbers = new Queue<string>(); //queue containing played midi notes to be converted to strikerss
    InputDevice inputDevice = null;
    void Start()
    {
        if(InputDevice.GetDevicesCount() > 0)
        {   //get the connected midi device and begin listening for played notes
            inputDevice = InputDevice.GetById(InputDevice.GetDevicesCount()-1);
            if(Verbose){Debug.Log("Device Connected: " + inputDevice.Name);}
            inputDevice.EventReceived += OnEventReceived;
            inputDevice.StartEventsListening();      
        }
        else
        {
            if(Verbose){Debug.Log("No devices found");}
        }
        //get the current kitmap
        kitMap = GameObject.FindObjectOfType<MidiMapManager>().KitMap;
        //link particle systems
        noteParticles["HHP"] = HHPParticle;
        noteParticles["KICK"] = KickParticle;
        noteParticles["SNARE"] = RedParticle;
        noteParticles["HIHAT"] = YellowParticle;
        noteParticles["LCRASH"] = GreenParticle;
        noteParticles["TOM1"] = YellowParticle;
        noteParticles["TOM2"] = BlueParticle;
        noteParticles["RCRASH"] = GreenParticle;
        noteParticles["RIDE"] = BlueParticle;
        noteParticles["RBELL"] = BlueParticle;
        noteParticles["TOM3"] = GreenParticle;
        noteParticles["CHINA"] = OrangeParticle;
        //link particle lanes
        particleLanes["HHP"] = -11;
        particleLanes["KICK"] = -1;
        particleLanes["SNARE"] = -9;
        particleLanes["HIHAT"] = -7;
        particleLanes["LCRASH"] = -5;
        particleLanes["TOM1"] = -3;
        particleLanes["TOM2"] = -1;
        particleLanes["RCRASH"] = 1;
        particleLanes["RIDE"] = 3;
        particleLanes["RBELL"] = 3;
        particleLanes["TOM3"] = 5;
        particleLanes["CHINA"] = 7;
    }

    
    void Update()
    {   //update playable notes
        updatePlayableNotes();
        //play queued midi notes from device
        while(noteNumbers.Count > 0)
        {
            //Debug.Log("stack: " + noteNumbers.Peek());
            playNote(noteNumbers.Dequeue());
        }
    }

    void updatePlayableNotes(){
        //add new notes
        foreach(Collider c in Physics.OverlapBox(StrikerZone.transform.position, StrikerZone.bounds.extents, StrikerZone.transform.rotation).Where(n=>n.GetComponent<moveNote>() != null).ToList()){
            var n = c.GetComponent<moveNote>();
            if(!playableNotes.ContainsKey(n.Note)){
                playableNotes[n.Note] = new List<moveNote>();
            }
            if(!playableNotes[n.Note].Contains(n)){
                playableNotes[n.Note].Add(n);
                if(Verbose){Debug.Log(n.Note + " added to playable notes");}
            }
        }
    }

    public void RemovePlayableNote(moveNote note){
        if(playableNotes[note.Note].Contains(note)){
            playableNotes[note.Note].Remove(note);
        }
    }

    private void OnEventReceived(object sender, MidiEventReceivedEventArgs e){   //detect notes played on midi device and convert to string
        var midiDevice = (MidiDevice)sender;
        if(e.Event.EventType == MidiEventType.NoteOn)
        {
            NoteOnEvent n = (NoteOnEvent)e.Event;
            noteNumbers.Enqueue(n.GetNoteId().NoteNumber.ToString());
        }      
    }

    private void OnDestroy(){    //stop listening to active midi device
        if(inputDevice != null){
            inputDevice.StopEventsListening();
            if(Verbose){Debug.Log("Stopped Listening to " + inputDevice.Name);}
            inputDevice.Dispose();
        }
    }

    void playNote(string noteNumber){
        if(playableNotes.ContainsKey(kitMap[int.Parse(noteNumber)])){
            if(playableNotes[kitMap[int.Parse(noteNumber)]].Count > 0){
                playableNotes[kitMap[int.Parse(noteNumber)]].First().gameObject.SetActive(false);
                Instantiate(noteParticles[kitMap[int.Parse(noteNumber)]], new Vector3(particleLanes[kitMap[int.Parse(noteNumber)]], StrikerZone.transform.position.y, StrikerZone.transform.position.z), Quaternion.identity);
                playableNotes[kitMap[int.Parse(noteNumber)]].Remove(playableNotes[kitMap[int.Parse(noteNumber)]].First());
            }
        }
        Instantiate(GreyParticle, new Vector3(particleLanes[kitMap[int.Parse(noteNumber)]], StrikerZone.transform.position.y, StrikerZone.transform.position.z), Quaternion.identity);
        if(Verbose){Debug.Log("played: " + kitMap[int.Parse(noteNumber)] + " / midi note: " + noteNumber);}      
    }
}
