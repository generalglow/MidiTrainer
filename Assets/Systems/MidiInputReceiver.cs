using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Devices;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

public class MidiInputReceiver : MonoBehaviour
{
    public GameObject RedStriker;
    public GameObject YellowStriker;
    public GameObject BlueStriker;
    public GameObject GreenStriker;
    public GameObject KickStriker;
    public GameObject HHPStriker;
    public ParticleSystem RedParticle;
    public ParticleSystem YellowParticle;
    public ParticleSystem BlueParticle;
    public ParticleSystem GreenParticle;
    public ParticleSystem KickParticle;
    public ParticleSystem HHPParticle;
    public ParticleSystem GreyParticle;
    [HideInInspector]
    public GameObject striker = null;

    Queue<GameObject> strikers = new Queue<GameObject>();
    
    MidiEvent currentEvent = null;
    
    static string noteNumber = null;
    Queue<string> noteNumbers = new Queue<string>();

    InputDevice inputDevice;
    void Start()
    {
        if(InputDevice.GetDevicesCount() > 0)
        {
            inputDevice = InputDevice.GetById(InputDevice.GetDevicesCount()-1);
            Debug.Log("Device Connected: " + inputDevice.Name);
            inputDevice.EventReceived += OnEventReceived;
            
            inputDevice.StartEventsListening();      
        }
        else
        {
            Debug.Log("No devices found, keyboard available");
        }
        strikers.Enqueue(RedStriker);
        strikers.Enqueue(YellowStriker);
        strikers.Enqueue(BlueStriker);
        strikers.Enqueue(GreenStriker);
        strikers.Enqueue(KickStriker);
        strikers.Enqueue(HHPStriker);
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject g in strikers)
        {
            if(g.transform.position.z == 0.0f)
            {
                g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, 3.0f);
            }
        }
        
        while(noteNumbers.Count > 0)
        {
            Debug.Log("stack: " + noteNumbers.Peek());
            playNote(noteNumbers.Dequeue());
        }

        getKeyboardInput();
    }

    private void OnEventReceived(object sender, MidiEventReceivedEventArgs e)
    {
        var midiDevice = (MidiDevice)sender;
        if(e.Event.EventType == MidiEventType.NoteOn)
        {
            NoteOnEvent n = (NoteOnEvent)e.Event;
            noteNumber = n.GetNoteId().NoteNumber.ToString();
            noteNumbers.Enqueue(n.GetNoteId().NoteNumber.ToString());
        }      
    }

    private void OnDestroy()
    {
        inputDevice.StopEventsListening();
        Debug.Log("Stopped Listening");
        inputDevice.Dispose();
    }

    void playNote(string noteNumber)
    {
        switch (noteNumber)
        {
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
                playYellowNote();
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
                playYellowNote();
                break;
            }
            case "48": //tom 1
            {
                playYellowNote();
                break;
            }
            case "49": //crash 1
            {
                playGreenNote();
                break;
            }
            case "50": //tom 3
            {
                playGreenNote();
                break;
            }
            case "51": //ride
            {
                playBlueNote();
                break;
            }
            case "53": //ride bell
            {
                playBlueNote();
                break;
            }
        
            case "57": //crash 2
            {
                playBlueNote();
                break;
            }
            case "59": //ride 2
            {
                playBlueNote();
                break;
            }
            default:
            {
                striker = null;
                Debug.Log("Unknown Note");
                break;
            }
        }     
          
    }

    void playRedNote()
    {
        RedStriker.transform.position = new Vector3(RedStriker.transform.position.x, RedStriker.transform.position.y, 0.0f);
        Instantiate(RedParticle, RedStriker.transform.position, Quaternion.identity);
        //Instantiate(GreyParticle, RedStriker.transform.position, Quaternion.identity);
    }

    void playYellowNote()
    {
        YellowStriker.transform.position = new Vector3(YellowStriker.transform.position.x, RedStriker.transform.position.y, 0.0f);
        Instantiate(YellowParticle, YellowStriker.transform.position, Quaternion.identity);
        //Instantiate(GreyParticle, YellowStriker.transform.position, Quaternion.identity);
    }

    void playBlueNote()
    {
        BlueStriker.transform.position = new Vector3(BlueStriker.transform.position.x, RedStriker.transform.position.y, 0.0f);
        Instantiate(BlueParticle, BlueStriker.transform.position, Quaternion.identity);
        //Instantiate(GreyParticle, BlueStriker.transform.position, Quaternion.identity);
    }

    void playGreenNote()
    {
        GreenStriker.transform.position = new Vector3(GreenStriker.transform.position.x, RedStriker.transform.position.y, 0.0f);
        Instantiate(GreenParticle, GreenStriker.transform.position, Quaternion.identity);
        //Instantiate(GreyParticle, GreenStriker.transform.position, Quaternion.identity);
    }

    void playKickNote()
    {
        KickStriker.transform.position = new Vector3(KickStriker.transform.position.x, RedStriker.transform.position.y, 0.0f);
        Instantiate(KickParticle, KickStriker.transform.position, Quaternion.identity);
        //Instantiate(GreyParticle, KickStriker.transform.position, Quaternion.identity);
    }

    void playHHPNote()
    {
        HHPStriker.transform.position = new Vector3(HHPStriker.transform.position.x, RedStriker.transform.position.y, 0.0f);
        Instantiate(HHPParticle, HHPStriker.transform.position, Quaternion.identity);
        //Instantiate(GreyParticle, HHPStriker.transform.position, Quaternion.identity);
    }

    void getKeyboardInput()
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
