using UnityEngine;
using System.Collections;

namespace Fiea.Rpp.Storytelling
{
    public class DialogueTicker : MonoBehaviour {

        private StoryDialogue currentStoryDialogue;

        public void setStoryDialogue(StoryDialogue newDialogue)
        {
            currentStoryDialogue = newDialogue;
        }

        public void isStillRunning()
        {
            if (!currentStoryDialogue.stillRunning)
            {
                currentStoryDialogue.dialogue.Stop();
                this.CancelInvoke();
            }
            currentStoryDialogue.stillRunning = false;

        }

	    // Use this for initialization
	    void Start () {
	
	    }
	
	    // Update is called once per frame
	    void Update () {
	
	    }
    }
}
