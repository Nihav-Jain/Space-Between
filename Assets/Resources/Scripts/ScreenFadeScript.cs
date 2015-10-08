using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFadeScript : MonoBehaviour {

    GameObject fadeScreen; //Screen we fade
    // Use this for initialization
    private const float fadeInDuration = 12.0f;
    private const float fadeOutDuration = 2.0f;
    private const float blackOutDuration = 0.5f;
	void Start () {
	    
    }

    void initFadeScreen()
    {
        this.fadeScreen = Instantiate(Resources.Load("Prefabs/FadeScreen", typeof(GameObject))) as GameObject;
        this.fadeScreen.name = "FadeScreen";
        this.fadeScreen.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        this.fadeScreen.GetComponent<Canvas>().worldCamera = Camera.main;
        this.fadeScreen.transform.SetParent(this.transform);
    }

    public void fadeIn()
    {
        if (this.fadeScreen == null) initFadeScreen();
        this.fadeScreen.transform.GetComponentInChildren<Image>().CrossFadeAlpha(0.0f, fadeInDuration, true);
    }

    public void fadeOut()
    {
        if (this.fadeScreen == null) initFadeScreen();
        this.fadeScreen.transform.GetComponentInChildren<Image>().CrossFadeAlpha(1.0f, fadeOutDuration, true);
    }
	
    public void blackOut()
    {
        if (this.fadeScreen == null) initFadeScreen();
        this.fadeScreen.transform.GetComponentInChildren<Image>().CrossFadeAlpha(1.0f, blackOutDuration, true);
    }

	// Update is called once per frame
	void Update () {
	}
}
