using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogueHandler : MonoBehaviour
{
    //private AudioSource currentSound;

    private Text DialogueBox;
    private string[] currentSceneDialogue;
    GameObject dialogueCanvas; //How to make a text object? Create a canvas and put text on that canvas
    GameObject dialogueText;
    private const int dialogueFontSize = 20;
    private const float dialgoueTextScale = 1.0f;
    Text sub;

    private bool subtitlesActive = false;

    //Use this for initialization
    void Start()
    {
        sub = this.gameObject.GetComponent<Text>();
        if(sub == null)
        {
            sub = this.gameObject.AddComponent<Text>();
        }
    }


    
    public void displaySubtitle(string subtitle)
    {

        //DialogueBox.text = subtitle;
        GameObject.Find("Subtitles").GetComponentInChildren<Text>().text = subtitle;
       
        //Debug.Log();
        //Debug.Log(sub);
        //abc.text = subtitle;
        
    }

    public void hideSubtitle()
    {
        GameObject.Find("Subtitles").GetComponentInChildren<Text>().enabled = false;
        //DialogueBox.text = "";
    }

    public void showSubtitle()
    {
        GameObject.Find("Subtitles").GetComponentInChildren<Text>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        //LoopTroughScene();
    }

}
