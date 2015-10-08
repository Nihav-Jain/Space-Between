using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Fiea.Rpp.Storytelling
{
    public class DialogueManagerScript : MonoBehaviour
    {

        // Use this for initialization
        ArrayList dialogueOptions; //Options at any point in time
        private const int maxOptions = 3; //For dialogue options
        private const float dialogueTextOffsetX = 100.0f;
        private const float dialogueTextOffsetY = 50.0f; //For debugging, sets an example offset for dialogue
        private const float dialogueStartTextOffsetY = 300.0f;
        private Vector3 offsetFromCamera = new Vector3(0, 0, 10.0f); //Sets offset from camera
        private bool listeniningForResponse;
        private string currentCondition;
        private string[] optionSuccessConditions;
        private bool optionsActive = false;
        private KeyCode[] optionSelectionCodes = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3 };
        private StoryCondition dialogueCondition;
        private int timerCount;
        private const int maxTimerCount = 15;
      

        public void setCurrentCondition(string name)
        {
            currentCondition = name;
        }

        void Start() {
            timerCount = maxTimerCount;
        listeniningForResponse = false;
        dialogueOptions = new ArrayList();
        this.gameObject.layer = LayerMask.NameToLayer("UI");
        initOptions();
        hideResponseTime();
	}
        void initOptions()
        {
            for (int i = 0; i < maxOptions; i++)
            {
                GameObject dialogueOption = new GameObject("Option");
                dialogueOption.transform.parent = this.transform;
                dialogueOption.AddComponent<DialogueOptionScript>();
                dialogueOption.GetComponent<DialogueOptionScript>().setText("OPTION"); //Default text
                dialogueOption.GetComponent<DialogueOptionScript>().setPosition(dialogueTextOffsetX,
                     dialogueStartTextOffsetY-(i * dialogueTextOffsetY)); //Puts a list of dialogue for example at specified offsets
                dialogueOption.gameObject.SetActive(false);
                dialogueOptions.Add(dialogueOption);
            }
        }

        public void showOptions(string[] options, string[] optionSuccessConditions)
        {
            this.optionSuccessConditions = optionSuccessConditions;
            this.optionsActive = true;
            //GameObject.Find("MainGame").GetComponent<MainScript>().Pause();
            GameObject.Find("InitialScene").GetComponent<InitialScene>().setInstructionDisplayVisibility(0.0f);
            listeniningForResponse = true;
            showResponseTime();
            for (int i = 0; i < options.Length; i++)
            {
                (dialogueOptions[i] as GameObject).SetActive(true);
                (dialogueOptions[i] as GameObject).GetComponent<DialogueOptionScript>().setText((i+1) + ". " + options[i]);
            }
        }

        public bool isOptionsActive()
        {
            return this.optionsActive;
        }

        public void removeOptions()
        {
            foreach (GameObject option in dialogueOptions)
            {
                option.SetActive(false);
            }
            this.optionsActive = false;
            hideResponseTime();
            
        }

        // Update is called once per frame
        void Update() 
        {
           if(listeniningForResponse)
           {
               for (int i = 0; i < optionSuccessConditions.Length;i++ )
               {
                   if (Input.GetKeyDown(optionSelectionCodes[i]))
                   {
                       GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().updateCondition(currentCondition, true);
                       GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().updateCondition(optionSuccessConditions[i], true);
                       listeniningForResponse = false;
                       timerCount = maxTimerCount;
                       this.CancelInvoke();
                       //GameObject.Find("MainGame").GetComponent<MainScript>().Pause();
                   }
               }
           }
	    }

        public void stopListeningForResponse()
        {
            listeniningForResponse = false;
        }

        public void setNewConditionOIbject(StoryCondition newCondition)
        {
            dialogueCondition = newCondition;
        }

        public void dialogueOptionsResponseTimeOut()
        {
            timerCount--;
            Debug.Log("REsponse option timer count = " + timerCount);
            GameObject.Find("ResponseTime").GetComponent<Text>().text = timerCount.ToString();
            if (timerCount <= 0 )
            {
                Debug.Log(string.Format("{0} timed out, setting to default condition {1}", currentCondition, dialogueCondition.defaultCondition));
                GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().updateCondition(currentCondition, true);
                GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().updateCondition(dialogueCondition.defaultCondition, true);
                listeniningForResponse = false;
                timerCount = maxTimerCount;
                this.CancelInvoke();
            }
        }

        public void showResponseTime()
        {
            GameObject.Find("TimeToRespond").GetComponent<Text>().enabled = true;
            GameObject.Find("ResponseTime").GetComponent<Text>().enabled = true;
        }

        public void hideResponseTime()
        {
            GameObject.Find("TimeToRespond").GetComponent<Text>().enabled = false;
            GameObject.Find("ResponseTime").GetComponent<Text>().enabled = false;
        }


    }

}
