using UnityEngine;

public class moveNote : MonoBehaviour
{
    public ReceiverV2 Receiver;
    public string Note = "";
    public string MidiNote = "";
    public int Lane = 0;
    public float Limit = -100;
    public float Time = 0;
    void Update()
    {
        if(gameObject.transform.position.y < Limit && gameObject.activeSelf && Lane != 10)
        {
            Receiver.RemovePlayableNote(this);
            gameObject.SetActive(false);
        }        
    }
}
