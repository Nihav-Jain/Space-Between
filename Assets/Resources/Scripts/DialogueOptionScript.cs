using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueOptionScript : MonoBehaviour {

    GameObject dialogueCanvas; //How to make a text object? Create a canvas and put text on that canvas
    GameObject dialogueText;
    private const int dialogueFontSize = 20;
    private const float dialgoueTextScale = 1.0f;
    
	void Start () {
        this.gameObject.layer = LayerMask.NameToLayer("UI");
    }
   
    void initText() //Set text objects
    {
        dialogueCanvas = new GameObject("Canvas"); //Make canvas and text
        dialogueText = new GameObject("Text");
        dialogueCanvas.layer = LayerMask.NameToLayer("UI");
        dialogueText.layer = LayerMask.NameToLayer("UI");
        dialogueText.transform.parent = dialogueCanvas.transform;
        dialogueCanvas.transform.parent = this.transform;

        // add a Canvas to the parent
        dialogueCanvas.AddComponent<Canvas>();
        // add a Recttransform to the child
        dialogueText.AddComponent<RectTransform>();

        // make a reference to the parent canvas 
        Canvas refCanvas = dialogueCanvas.GetComponent<Canvas>();
        refCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        refCanvas.worldCamera = Camera.main;
        dialogueCanvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        // add a text component to the child
        Text text =  dialogueText.AddComponent<Text>();
        text.alignment = TextAnchor.MiddleLeft;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        dialogueText.transform.localScale = new Vector3(dialgoueTextScale, dialgoueTextScale, dialgoueTextScale);
        // make a reference to the child rect transform and set its values
        RectTransform textRectTransform = dialogueText.GetComponent<RectTransform>();

        // set child anchors for resizing behaviour
        textRectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
        //textRectTransform.sizeDelta = new Vector2(1f, 1f);
        textRectTransform.anchorMin = new Vector2(0f, 0f);
        textRectTransform.anchorMax = new Vector2(0f, 0f);

        // set text font type and material at runtime from font stored in Resources folder
        Text textComponent = dialogueText.GetComponent<Text>();
        Font myFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

        textComponent.font = myFont;
        textComponent.fontSize = dialogueFontSize;
        // set the font text
        textComponent.text = "OPTION";

    }

    public void setText(string text)
    {
        if (dialogueText == null) this.initText(); //Init text if not already instantiated
        dialogueText.GetComponent<Text>().text = text;
    }

    public void setPosition(float x, float y)
    {
        dialogueText.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y); //Set the position on the overlay
    }
	// Update is called once per frame
	void Update () {
	
	}
}
