using UnityEngine;
using System.Collections;

namespace Fiea.Rpp.Storytelling
{
    public class StoryDialogue : StoryBehavior
    {
        private float startDelay;
        public AudioSource dialogue;
        private bool isLooping;
        private GameObject gameObject;
        private string subtitle;
        public bool stillRunning;
        private bool isJess;

        public StoryDialogue(string audioPath, GameObject gameObject, float startDelay=0f, bool loop=false, string subtitle="", string character="Nick")
        {
            this.startDelay = startDelay;
            dialogue = gameObject.AddComponent<AudioSource>();
            dialogue.clip = Resources.Load(audioPath) as AudioClip;
            dialogue.loop = loop;
            this.isLooping = loop;
            this.gameObject = gameObject;
            this.subtitle = subtitle;
            if (character == "Jess")
                isJess = true;
            else
                isJess = false;
        }

        public override void OnInititalize()
        {
            base.OnInititalize();
            dialogue.PlayDelayed(startDelay+0.001f);   // consider startDelay + 0.001f if this does not work
            GameObject.Find("SoundManager").GetComponent<SoundManager>().setJessSpeaking(isJess);
            GameObject.Find("SubtitleHandler").GetComponent<DialogueHandler>().displaySubtitle(this.subtitle);
            if (isLooping)
            {
                stillRunning = true;
                gameObject.AddComponent<DialogueTicker>();
                gameObject.GetComponent<DialogueTicker>().setStoryDialogue(this);
                gameObject.GetComponent<DialogueTicker>().InvokeRepeating("isStillRunning", 0.5f, 0.5f);
            }
        }

        public override StoryStatus Update()
        {
            if (GameObject.Find("MainGame").GetComponent<MainScript>().isPaused()) return StoryStatus.STATUS_Running;
            stillRunning = true;
            if(!dialogue.isPlaying)
                m_status = StoryStatus.STATUS_Success;
            return m_status;
        }

        public override void OnTerminate(StoryStatus status)
        {
            GameObject.Find("SoundManager").GetComponent<SoundManager>().setJessSpeaking(false);
            GameObject.Find("SubtitleHandler").GetComponent<DialogueHandler>().displaySubtitle("");
        }

        public override string ToString()
        {
            string objectString = string.Format("Dialogue: {0}\n islooping: {1}\n startDelay: {2}", this.subtitle, this.isLooping, this.startDelay);
            return objectString;
        }

        public void StopPropogation()
        {
            GameObject.Find("SoundManager").GetComponent<SoundManager>().setJessSpeaking(false);
            if (dialogue.isPlaying)
                dialogue.Stop();
        }
    }
}
