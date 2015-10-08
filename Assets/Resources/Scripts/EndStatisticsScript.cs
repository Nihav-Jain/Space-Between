using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Fiea.Rpp.Storytelling;

public class EndStatisticsScript : MonoBehaviour {

    public static float fadeInTime = 5.0f;
    private bool silenced;      //Silenced all sound in game?
    public static float silenceAlpha = 0.8f;    //At what alpha to silence?
    public static string saveGameDataLocation = "/saveGame.gd";
    private static int totalAttempts;

    private int timesLeftAndDied = 0;
    private int timesSavedAndDied = 0;
    private int timesLeftAndEscaped = 0;
    private int timesSavedAndEscaped = 0;
    private int timesTriedAndNickDied = 0;

    private string statusOfCompletion;

    private static string GET_ANALYTICS_URL = "http://nihavjain.info/demo/behavior-tree-builder/getanalytics.php";
    private static string UPDATE_ANALYTICS_URL = "http://nihavjain.info/demo/behavior-tree-builder/updateanalytics.php";

    private const string GAME_ANALYTICS_SAVED_BOTH = "SavedBoth";
    private const string GAME_ANALYTICS_TRIED_SAVING_BUT_FAILED = "TriedSavingButFailed";
    private const string GAME_ANALYTICS_TRIED_SAVING_BUT_DIED = "TriedSavingButDied";
    private const string GAME_ANALYTICS_DID_NOT_SAVE = "DidNotSave";
    private const string GAME_ANALYTICS_DID_NOT_TRY_SAVING_AND_DIED = "DidNotTrySavingAndDied";

    private string analyticToSend;
    private bool analyticNoted;
    // Use this for initialization
    void Start()
    {
        analyticNoted = false;
        foreach (CanvasRenderer canvasRenderer in this.transform.GetComponentsInChildren<CanvasRenderer>())
        {
            canvasRenderer.SetAlpha(0.0f);
        }
    }

    void saveDataValues()
    {
        
    }

    void loadDataValues()
    {
       
    }

    public void setStatisticsVisibility(float value)
    {
        foreach (Text text in this.gameObject.GetComponentsInChildren<Text>())
        {
            text.CrossFadeAlpha(1.0f, fadeInTime, true);     //The instruction for attaching will be visible
        }
    }

    public void endGame()
    {
        GameObject.Find("ScreenFader").GetComponent<ScreenFadeScript>().fadeOut();
        this.setStatisticsVisibility(1.0f);
        if (!analyticNoted)
        {
            determineOutcome();
            analyticNoted = true;
        }

    }

    void determineOutcome()
    {
        if(GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().getConditionStatus(StoryConditionValues.EscapePodReached))
        {
            if (!GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().getConditionStatus(StoryConditionValues.isAttached))
            {
                timesLeftAndEscaped++;
                analyticToSend = GAME_ANALYTICS_DID_NOT_SAVE;
                statusOfCompletion = String.Format("You saved yourself and left Nick!");
            }
            else if (GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().getConditionStatus(StoryConditionValues.DrifterDead))
            {
                timesTriedAndNickDied++;
                analyticToSend = GAME_ANALYTICS_TRIED_SAVING_BUT_FAILED;
                statusOfCompletion = String.Format("You tried to save Nick, but he didn't make it...");
            }
            else
            {
                timesSavedAndEscaped++;
                analyticToSend = GAME_ANALYTICS_SAVED_BOTH;
                statusOfCompletion = String.Format("You survived and saved Nick!");
            }
        }
        else
        {
            if (!GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().getConditionStatus(StoryConditionValues.isAttached))
            {
                timesLeftAndDied++;
                analyticToSend = GAME_ANALYTICS_DID_NOT_TRY_SAVING_AND_DIED;
                statusOfCompletion = String.Format("You betrayed Nick and still died...");
            }
            else
            {
                timesSavedAndDied++;
                analyticToSend = GAME_ANALYTICS_TRIED_SAVING_BUT_DIED;
                statusOfCompletion = String.Format("You tried to save Nick, but you both died...");
            }
        }

        analyticNoted = true;
        WWW updateGameAnalytics = this.updateAnalytics(analyticToSend);
    }

    void silenceSounds()
    {
        AudioListener.volume = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (this.transform.GetComponentInChildren<CanvasRenderer>().GetAlpha() >= silenceAlpha) this.silenceSounds();
	}

    private WWW updateAnalytics(string updateCondition)
    {
        WWWForm postRequestData = new WWWForm();
        postRequestData.AddField("analytic", updateCondition);

        WWW postRequest = new WWW(UPDATE_ANALYTICS_URL, postRequestData);
        StartCoroutine(WaitForPOSTRequest(postRequest));
        return postRequest;
    }

    private IEnumerator WaitForPOSTRequest(WWW postRequest)
    {
        yield return postRequest;

        if (postRequest.error == null)
        {
            if (postRequest.text == "412" || postRequest.text == "400")
                Debug.Log("Invalid Request");
            else if (postRequest.text == "2000")
                Debug.Log("Database connection error");
            else if (postRequest.text == "2001")
                Debug.Log("Error running query");
            else
            {
                Debug.Log("Request ok => " + postRequest.text);
                JSONObject analytics = new JSONObject(postRequest.text);
                Debug.Log(analytics);
                Debug.Log(analytics.type);
                foreach(JSONObject analytic in analytics.list)
                {
                    string conditionName = "";
                    string counter = "";
                    analytic.GetField(ref conditionName, "condition_name");
                    analytic.GetField(ref counter, "count");
                    int count = Convert.ToInt32(counter);
                    Debug.Log(string.Format("{0} {1}", conditionName, count));
                    switch(conditionName)
                    {
                        case GAME_ANALYTICS_SAVED_BOTH:
                            timesSavedAndEscaped = count;
                            break;
                        case GAME_ANALYTICS_TRIED_SAVING_BUT_DIED:
                            timesSavedAndDied = count;
                            break;
                        case GAME_ANALYTICS_TRIED_SAVING_BUT_FAILED:
                            timesTriedAndNickDied = count;
                            break;
                        case GAME_ANALYTICS_DID_NOT_SAVE:
                            timesLeftAndEscaped = count;
                            break;
                        case GAME_ANALYTICS_DID_NOT_TRY_SAVING_AND_DIED:
                            timesLeftAndDied = count;
                            break;
                    }
                }

                totalAttempts = timesLeftAndDied + timesSavedAndDied + timesSavedAndEscaped + timesTriedAndNickDied + timesLeftAndEscaped;

                GameObject.Find("Subtitles").GetComponent<RawImage>().enabled = false;

                GameObject.Find("WhatYouDid").GetComponent<Text>().text = statusOfCompletion;
                GameObject.Find("SaveNickAndEscape").GetComponent<Text>().text = ConvertToPercentage(timesSavedAndEscaped);           //Set text of all values
                GameObject.Find("LeftNickAndEscape").GetComponent<Text>().text = ConvertToPercentage(timesLeftAndEscaped);
                GameObject.Find("SaveNickAndDied").GetComponent<Text>().text = ConvertToPercentage(timesSavedAndDied);
                GameObject.Find("LeftNickAndDied").GetComponent<Text>().text = ConvertToPercentage(timesLeftAndDied);
                GameObject.Find("TriedToSaveNickAndNickDied").GetComponent<Text>().text = ConvertToPercentage(timesTriedAndNickDied);
            }
        }
        else
        {
            Debug.Log("Request error => " + postRequest.error);
        }

    }

    private static string ConvertToPercentage(int input)
    {
        return string.Format("{0:0.0%}", input / (double)totalAttempts);
    }

}


