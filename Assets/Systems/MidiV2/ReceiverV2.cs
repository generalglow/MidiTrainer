using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Devices;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

public class ReceiverV2 : MonoBehaviour
{
    public float StrikerHold = 0.1f;    //how long the striker is "held", allowing it to clear notes
    //game objects for note strikers. Toms are colour coded (legacy), cymbals are named.
    public GameObject RedStriker;
    public GameObject HHStriker;
    public GameObject LCrashStriker;
    public GameObject YellowStriker;
    public GameObject BlueStriker;
    public GameObject RCrashStriker;
    public GameObject RideStriker;
    public GameObject GreenStriker;

    public GameObject ChinaStriker;
    public GameObject KickStriker;
    public GameObject HHPStriker;

    //particle systems that play when notes are cleared
    public ParticleSystem RedParticle;
    public ParticleSystem YellowParticle;
    public ParticleSystem BlueParticle;
    public ParticleSystem GreenParticle;
    public ParticleSystem KickParticle;
    public ParticleSystem HHPParticle;
    public ParticleSystem GreyParticle;

    Queue<GameObject>strikers = new Queue<GameObject>(); //queue for resetting striker positions
    Dictionary<GameObject, float> strikerDelays = new Dictionary<GameObject, float>(); //dict containing strikers to test for bounds/actively playing note and their cooldown
    Queue<string> noteNumbers = new Queue<string>(); //queue containing played midi notes to be converted to strikerss
    Dictionary<int, string> kitMap = new Dictionary<int, string>();  //kit profile with mapping for midi note -> lane

    InputDevice inputDevice = null;
    void Start()
    {
        if(InputDevice.GetDevicesCount() > 0)
        {   //get the connected midi device and begin listening for played notes
            inputDevice = InputDevice.GetById(InputDevice.GetDevicesCount()-1);
            Debug.Log("Device Connected: " + inputDevice.Name);
            inputDevice.EventReceived += OnEventReceived;
            inputDevice.StartEventsListening();      
        }
        else
        {
            Debug.Log("No devices found, keyboard available");
        }
        //populate queue for checking striker positions
        strikers.Enqueue(RedStriker);
        strikers.Enqueue(HHStriker);
        strikers.Enqueue(LCrashStriker);
        strikers.Enqueue(YellowStriker);
        strikers.Enqueue(BlueStriker);
        strikers.Enqueue(RideStriker);
        strikers.Enqueue(RCrashStriker);
        strikers.Enqueue(GreenStriker);
        strikers.Enqueue(ChinaStriker);
        strikers.Enqueue(KickStriker);
        strikers.Enqueue(HHPStriker);
        //populate dictorionary for checking striker delays
        strikerDelays[RedStriker] = 0;
        strikerDelays[HHStriker] = 0;
        strikerDelays[LCrashStriker] = 0;
        strikerDelays[YellowStriker] = 0;
        strikerDelays[BlueStriker] = 0;
        strikerDelays[RideStriker] = 0;
        strikerDelays[RCrashStriker] = 0;
        strikerDelays[GreenStriker] = 0;
        strikerDelays[ChinaStriker] = 0;
        strikerDelays[KickStriker] = 0;
        strikerDelays[HHPStriker] = 0;
    }

    
    void Update()
    {   //check each striker's position against its delay and reposition on delay end
        foreach(GameObject g in strikers)
        {
            strikerDelays[g] -= Time.deltaTime;
            if(g.transform.position.z == 0.0f && strikerDelays[g] <= 0)
            {
                g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, 3.0f);
                strikerDelays[g] = 0;
            }
        }
        //play queued midi notes from device
        while(noteNumbers.Count > 0)
        {
            //Debug.Log("stack: " + noteNumbers.Peek());
            playNote(noteNumbers.Dequeue());
        }
        
        getKeyboardInput();
    }

    private void OnEventReceived(object sender, MidiEventReceivedEventArgs e){   //detect notes played on midi device and convert 
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
            Debug.Log("Stopped Listening to " + inputDevice.Name);
            inputDevice.Dispose();
        }
    }

    void playNote(string noteNumber){
        switch (noteNumber){
            case "34": //hihat pedal 2
            {
                playHHPNote();
                break;
            }
            case "35": //kick
            {
                playKickNote();                                                                    
                break;
            }
            case "36": //kick 2
            {
                playKickNote();                                                                    
                break;
            }
            case "38": //snare
            {
                playRedNote();
                break;
            }
            case "39": //snare
            {
                playRedNote();
                break;
            }
            case "40": //snare
            {
                playRedNote();
                break;
            }
            case "42": //hihat closed
            {
                playHHNote();
                break;
            }
            case "43": //tom 3
            {
                playGreenNote();
                break;
            }
            case "44": //hihat pedal
            {
                playHHPNote();
                break;
            }
            case "45": //tom 2
            {
                playBlueNote();
                break;
            }
            case "46": //hihat open
            {
                playHHNote();
                break;
            }
            case "48": //tom 1
            {
                playYellowNote();
                break;
            }
            case "49": //crash 1
            {
                playLCrashNote();
                playRCrashNote();
                break;
            }
            case "50": //tom 3
            {
                playGreenNote();
                break;
            }
            case "51": //ride
            {
                playRideNote();
                break;
            }
            case "53": //ride bell
            {
                playRideNote();
                break;
            }
        
            case "57": //crash 2
            {
                playChinaNote();
                break;
            }
            case "59": //ride 2
            {
                playRideNote();
                break;
            }
            default:
            {
                Debug.Log("Played unknown midi note: " + noteNumber);
                break;
            }
        }
        Debug.Log("played: " + noteNumber);      
    }

    void playRedNote()
    {
        RedStriker.transform.position = new Vector3(RedStriker.transform.position.x, RedStriker.transform.position.y, 0.0f);
        Instantiate(RedParticle, RedStriker.transform.position, Quaternion.identity);
        //Instantiate(GreyParticle, RedStriker.transform.position, Quaternion.identity);
        strikerDelays[RedStriker] = StrikerHold;
    }

    void playHHNote()
    {
        HHStriker.transform.position = new Vector3(HHStriker.transform.position.x, RedStriker.transform.position.y, 0.0f);
        Instantiate(YellowParticle, HHStriker.transform.position, Quaternion.identity);
        //Instantiate(GreyParticle, YellowStriker.transform.position, Quaternion.identity);
        strikerDelays[HHStriker] = StrikerHold;
    }

    void playLCrashNote()
    {
        LCrashStriker.transform.position = new Vector3(LCrashStriker.transform.position.x, RedStriker.transform.position.y, 0.0f);
        Instantiate(GreenParticle, LCrashStriker.transform.position, Quaternion.identity);
        //Instantiate(GreyParticle, YellowStriker.transform.position, Quaternion.identity);
        strikerDelays[LCrashStriker] = StrikerHold;
    }
    void playYellowNote()
    {
        YellowStriker.transform.position = new Vector3(YellowStriker.transform.position.x, RedStriker.transform.position.y, 0.0f);
        Instantiate(YellowParticle, YellowStriker.transform.position, Quaternion.identity);
        //Instantiate(GreyParticle, YellowStriker.transform.position, Quaternion.identity);
        strikerDelays[YellowStriker] = StrikerHold;
    }

    void playBlueNote()
    {
        BlueStriker.transform.position = new Vector3(BlueStriker.transform.position.x, RedStriker.transform.position.y, 0.0f);
        Instantiate(BlueParticle, BlueStriker.transform.position, Quaternion.identity);
        //Instantiate(GreyParticle, BlueStriker.transform.position, Quaternion.identity);
        strikerDelays[BlueStriker] = StrikerHold;
    }

    void playRideNote()
    {
        RideStriker.transform.position = new Vector3(RideStriker.transform.position.x, RedStriker.transform.position.y, 0.0f);
        Instantiate(BlueParticle, RideStriker.transform.position, Quaternion.identity);
        //Instantiate(GreyParticle, BlueStriker.transform.position, Quaternion.identity);
        strikerDelays[RideStriker] = StrikerHold;
    }

    void playRCrashNote()
    {
        RCrashStriker.transform.position = new Vector3(RCrashStriker.transform.position.x, RedStriker.transform.position.y, 0.0f);
        Instantiate(GreenParticle, RCrashStriker.transform.position, Quaternion.identity);
        //Instantiate(GreyParticle, GreenStriker.transform.position, Quaternion.identity);
        strikerDelays[RCrashStriker] = StrikerHold;
    }

    void playGreenNote()
    {
        GreenStriker.transform.position = new Vector3(GreenStriker.transform.position.x, RedStriker.transform.position.y, 0.0f);
        Instantiate(GreenParticle, GreenStriker.transform.position, Quaternion.identity);
        //Instantiate(GreyParticle, GreenStriker.transform.position, Quaternion.identity);
        strikerDelays[GreenStriker] = StrikerHold;
    }

    void playChinaNote()
    {
        ChinaStriker.transform.position = new Vector3(ChinaStriker.transform.position.x, RedStriker.transform.position.y, 0.0f);
        Instantiate(KickParticle, ChinaStriker.transform.position, Quaternion.identity);
        //Instantiate(GreyParticle, KickStriker.transform.position, Quaternion.identity);
        strikerDelays[ChinaStriker] = StrikerHold;
    }

    void playKickNote()
    {
        KickStriker.transform.position = new Vector3(KickStriker.transform.position.x, RedStriker.transform.position.y, 0.0f);
        Instantiate(KickParticle, KickStriker.transform.position, Quaternion.identity);
        //Instantiate(GreyParticle, KickStriker.transform.position, Quaternion.identity);
        strikerDelays[KickStriker] = StrikerHold;
    }

    void playHHPNote()
    {
        HHPStriker.transform.position = new Vector3(HHPStriker.transform.position.x, RedStriker.transform.position.y, 0.0f);
        Instantiate(HHPParticle, HHPStriker.transform.position, Quaternion.identity);
        //Instantiate(GreyParticle, HHPStriker.transform.position, Quaternion.identity);
        strikerDelays[HHPStriker] = StrikerHold;
    }

    void getKeyboardInput() //used for testing when midi device is unavailable (legacy)
    {
        if(Input.GetKeyDown("j"))
        {
            playRedNote();
        }
        if(Input.GetKeyDown("k"))
        {
            playYellowNote();
        }
        if(Input.GetKeyDown("l"))
        {
            playBlueNote();
        }
        if(Input.GetKeyDown(";"))
        {
            playGreenNote();
        }
        if(Input.GetKeyDown("space"))
        {
            playKickNote();
        }
    }
}
