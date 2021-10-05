using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MidiMapManager : MonoBehaviour //class for saving and and loading midi to lane mapping. Outputs to txt file.
{
    public string MapDirectory; //directory to folder, should end in '\'
    public InputField MapName;  //name of text file, without '.txt
    public InputField InputTemplate;    //template input field for instantiation
    public InputField HHP, Kick, Snare, Hihat, LCrash, Tom1, Tom2, RCrash, Ride, RBell, Tom3, China; //note input fields
    public Button SaveButton, LoadButton, NewButton, CloseButton; //HUD buttons
    public bool Verbose = false;
    public Dictionary<int, string> KitMap {get;set;} = new Dictionary<int, string>();   //midi note inputs matched to drum note outputs
    
    void Start(){
        SaveButton.onClick.AddListener(saveMap);
        NewButton.onClick.AddListener(clearUI);
        LoadButton.onClick.AddListener(loadMap);
        CloseButton.onClick.AddListener(quit);

        //testing
        MapName.text = "TD11";
        loadMap();
        quit();
        //end testing
    }

    void Update(){
        updateUI();
    }

    void createNewInputField(Transform parent, int offset = 0, string text = ""){
        var newInputField = Instantiate(InputTemplate, parent, false); //create extra children
        newInputField.transform.position = newInputField.transform.parent.TransformPoint((offset) * 40 + 180,0,0);
        newInputField.text = text;
    }

    void updateUI(){//cycle through each input field and check the amount of alternate notes match the number of inputs displayed
        foreach(Dropdown d in GetComponentsInChildren<Dropdown>()){
            if(d.transform.parent.childCount-2 > int.Parse(d.options[d.value].text)){
                for(int i = 2; i < d.transform.parent.childCount; i++){//-2 to ignore drop down and default field
                    Destroy(d.transform.parent.GetChild(i).gameObject);//destroy extra children
                }
            }
            if(d.transform.parent.childCount-2 < int.Parse(d.options[d.value].text)){
                for(int j = 0; j < int.Parse(d.options[d.value].text); j++){
                    createNewInputField(d.transform.parent, j+1);
                }
            }
        }
    }

    void saveMap(){ //save kit map to text file
        if(GetComponentsInChildren<InputField>().ToList().Where(i=>i.text == "").Count() == 0){//ensure all fields have input
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(MapDirectory + MapName.text + ".txt")){ //create  new text file at given directory
                foreach(InputField j in transform.GetComponentsInChildren<InputField>().ToList()){                  //write inputs to lines
                    file.WriteLine(j.transform.parent.name.Substring(0,j.transform.parent.name.Length-5).ToUpper() + ":" + j.text);
                }
                if(Verbose){Debug.Log("Saved! (" + MapDirectory + MapName.text + ".txt)");}
            }
        }
        else{
            if(Verbose){Debug.Log("Not all fields contain midi notes");}
        }
    }

    void clearUI(){
        foreach(Dropdown d in GetComponentsInChildren<Dropdown>()){
            d.value = 0; //reset additional inputs
            for(int i = 2; i < d.transform.parent.childCount; i++){//-2 to ignore drop down and default field
                Destroy(d.transform.parent.GetChild(i).gameObject);//destroy extra children
            }
        }
        foreach(InputField i in GetComponentsInChildren<InputField>()){
            i.text = "";
        }
    }

    void loadInputField(InputField inputField, string drum, string value){//set input field value, create new input field if required
        if(inputField.text != ""){
            createNewInputField(inputField.transform.parent, inputField.transform.parent.childCount-1, value);
            inputField.transform.parent.GetComponentInChildren<Dropdown>().value++;
            if(Verbose){Debug.Log("set additional " + drum + " to " + value);}
        }
        else{
            inputField.text = value;
            if(Verbose){Debug.Log("set " + drum + " to " + value);}
        }
    }

    void loadMap(){//load map from text file
        string filename = MapDirectory + MapName.text + ".txt";
        string mapName = MapName.text;
        clearUI();
        string[] lines = System.IO.File.ReadAllLines(filename);
        for(int l = 1; l < lines.Length; l++)//parse lines to initialise fields
        {
            KitMap[int.Parse(lines[l].Split(':')[1])] = lines[l].Split(':')[0];
            switch(lines[l].Split(':')[0].ToUpper()){
                case "HHP":
                    loadInputField(HHP, "Hihat Pedal", lines[l].Split(':')[1]);
                break;
                case "KICK":
                    loadInputField(Kick, "Kick", lines[l].Split(':')[1]);
                break;
                case "SNARE":
                    loadInputField(Snare, "Snare", lines[l].Split(':')[1]);
                break;
                case "HIHAT":
                    loadInputField(Hihat, "Hihat", lines[l].Split(':')[1]);
                break;
                case "LCRASH":
                    loadInputField(LCrash, "Left Crash", lines[l].Split(':')[1]);
                break;
                case "TOM1":
                    loadInputField(Tom1, "Tom 1", lines[l].Split(':')[1]);
                break;
                case "TOM2":
                    loadInputField(Tom2, "Tom 2", lines[l].Split(':')[1]);
                break;
                case "RCrash":
                    loadInputField(RCrash, "Right Crash", lines[l].Split(':')[1]);
                break;
                case "RIDE":
                    loadInputField(Ride, "Ride", lines[l].Split(':')[1]);
                break;
                case "RBELL":
                    loadInputField(RBell, "Ride Bell", lines[l].Split(':')[1]);
                break;
                case "TOM3":
                    loadInputField(Tom3, "Tom 3", lines[l].Split(':')[1]);
                break;
                case "CHINA":
                    loadInputField(China, "China", lines[l].Split(':')[1]);
                break;
                default:
                    Debug.Log("Midi-map note unkown: " + lines[l].Split(':')[0].ToUpper());
                break;
            }
        }
        MapName.text = mapName; //reset the MapName text field to contain the name after clearing UI
        if(Verbose){
            foreach(KeyValuePair<int, string> p in KitMap){
                Debug.Log(p.Key + " : " + p.Value);
            }
        }

    }

    void quit(){
        gameObject.SetActive(false);
    }
        
}
