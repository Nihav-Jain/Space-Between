using UnityEngine;
using System.Collections;

namespace Fiea.Rpp.Storytelling
{
    public class StoryCondition : StoryBehavior
    {
        private bool isConditionTrue;
        private string conditionName;
        private bool isDialogCondition;
        private bool causingStress;
        private bool instructionVisible;
        private bool settingRotationInstruction;
        private string conditionName2;
        public string defaultCondition {get; private set;}

        public StoryCondition(string name, bool isDialogCondition=false, bool isAgitated=false, string defaultCondition="",
            bool isInstructionVisible = false, bool isSettingRotationInstruction = false)
        {
            isConditionTrue = false;
            this.conditionName = name;
            this.isDialogCondition = isDialogCondition;
            this.causingStress = isAgitated;
            this.instructionVisible = isInstructionVisible;
            this.settingRotationInstruction = isSettingRotationInstruction;
            this.conditionName2 = null;
            this.defaultCondition = defaultCondition;
        }

        public override void OnInititalize()
        {
            int index = conditionName.IndexOf(" || ");
            if (index >= 0)
            {
                conditionName2 = conditionName.Substring(index + 4, conditionName.Length - (index + 4));
                conditionName = conditionName.Substring(0, index);
                GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().addCondition(conditionName2);
            }
            GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().addCondition(conditionName);
            if (isDialogCondition)
            {
                GameObject.Find("DialogueManager").GetComponent<DialogueManagerScript>().setCurrentCondition(this.conditionName);
                if(defaultCondition != null && defaultCondition.Length != 0)
                {
                    GameObject.Find("DialogueManager").GetComponent<DialogueManagerScript>().setNewConditionOIbject(this);
                    GameObject.Find("DialogueManager").GetComponent<DialogueManagerScript>().InvokeRepeating("dialogueOptionsResponseTimeOut", 1.0f, 1.0f);
                }
            }
            base.OnInititalize();
        }

        public override StoryStatus Update()
        {
            if (conditionName2 == null)
            {
                if (GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().getConditionStatus(conditionName))
                {
                    m_status = StoryStatus.STATUS_Success;
                    GameObject.Find("OxygenBars").GetComponent<OxygenBarScript>().setDrifterStressed(this.causingStress);       //Does this condition cause stress to the drifter?
                    if (this.instructionVisible) GameObject.Find("InitialScene").GetComponent<InitialScene>().setInstructionDisplayVisibility(1.0f);    //Is the instruction visible?
                    if (this.settingRotationInstruction) GameObject.Find("InitialScene").GetComponent<InitialScene>().setRotateInstruction();
                }
                else
                    m_status = StoryStatus.STATUS_Failure;
            }
            else if (GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().getConditionStatus(conditionName) ||
                GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().getConditionStatus(conditionName2))
            {
                m_status = StoryStatus.STATUS_Success;
                GameObject.Find("OxygenBars").GetComponent<OxygenBarScript>().setDrifterStressed(this.causingStress);       //Does this condition cause stress to the drifter?
            }
            else
                m_status = StoryStatus.STATUS_Failure;

            if (conditionName2 != null)
                Debug.Log(string.Format("Condition: {0} || {1} : {2}", conditionName, conditionName2, m_status));
            else
               Debug.Log(string.Format("Condition: {0} : {1}", conditionName, m_status));
            return m_status;
        }

        public override void OnTerminate(StoryStatus status)
        {
            if(isDialogCondition && status == StoryStatus.STATUS_Success)
            {
                GameObject.Find("DialogueManager").GetComponent<DialogueManagerScript>().removeOptions();
                
            }
            base.OnTerminate(status);
        }

        public override string ToString()
        {
            string objectString = string.Format("Story Condition: {0}\n isDialogueCondition: {1}\n causingStress: {2}", this.conditionName, this.isDialogCondition, this.causingStress);
            return objectString;
        }

    }
}
