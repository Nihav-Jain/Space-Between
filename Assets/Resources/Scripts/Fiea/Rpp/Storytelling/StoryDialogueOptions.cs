using UnityEngine;
using System.Collections;

namespace Fiea.Rpp.Storytelling
{
    public class StoryDialogueOptions : StoryBehavior
    {
        private string[] options;
        private string[] optionSuccessConditions;
        private GameObject gameObject;

        public StoryDialogueOptions(string[] options, string[] optionSuccessConditions, GameObject gameObject)
        {
            this.options = options;
            this.optionSuccessConditions = optionSuccessConditions;
            this.gameObject = gameObject;
        }

        public override void OnInititalize()
        {
            base.OnInititalize();
            // initialize the game object and add options and add event listener for option selection
            GameObject.Find("DialogueManager").GetComponent<DialogueManagerScript>().showOptions(this.options, optionSuccessConditions); 
        }

        public override StoryStatus Update()
        {
            //check if option has been selected
            // return success if yes
            // else return "running"
            return base.Update();
        }

        public override string ToString()
        {
            string objectString = "Dialogue Options:\n";
            int i;
            for (i = 0; i < options.Length;i++)
            {
                objectString += string.Format("{0} : {1}\n", options[i], optionSuccessConditions[i]);
            }
             return objectString;
        }

    }
}
