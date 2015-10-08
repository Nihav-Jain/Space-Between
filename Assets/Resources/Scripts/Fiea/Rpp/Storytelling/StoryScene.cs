using UnityEngine;
using System.Collections;

namespace Fiea.Rpp.Storytelling
{
    public class StoryScene : StoryBehavior
    {
        private Scenes m_Scene;
        private GameObject gameObject;

        public StoryScene(Scenes myScene, GameObject gameObject)
        {
            m_Scene = myScene;
            this.gameObject = gameObject;
        }

        public override void OnInititalize()
        {
            base.OnInititalize();
            GameObject.Find("InitialScene").GetComponent<InitialScene>().startScene(m_Scene);
        }

        public override StoryStatus Update()
        {
            if (GameObject.Find("InitialScene").GetComponent<InitialScene>().isSceneCompleted)
                m_status = StoryStatus.STATUS_Success;
            Debug.Log(string.Format("Scene: {0} : {1}", m_Scene, m_status));
            return m_status;
        }

        public override string ToString()
        {
            string objectString = string.Format("Scene: {0}", this.m_Scene);
            return objectString;
        }
    }
}
