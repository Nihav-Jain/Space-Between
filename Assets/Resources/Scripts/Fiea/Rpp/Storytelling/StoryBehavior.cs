using UnityEngine;
using System.Collections;

namespace Fiea.Rpp.Storytelling
{
    public enum StoryStatus
    {
        STATUS_None,
        STATUS_Success,
        STATUS_Running,
        STATUS_Failure
    };

    // Node Behaviour
    public class StoryBehavior
    {
        protected StoryStatus m_status;
        protected ArrayList m_children;
        protected int m_childIndex;

        public ArrayList Children
        {
            get
            {
                return m_children;
            }
        }

        public StoryBehavior()
        {
            m_children = new ArrayList();
            m_childIndex = -1;
            m_status = StoryStatus.STATUS_None;
        }

        public virtual StoryStatus Update() {return m_status;}
        public virtual void OnInititalize() 
        {
            m_childIndex = 0;
            m_status = StoryStatus.STATUS_Running;
        }
        public virtual void OnTerminate(StoryStatus status) {}

        public StoryStatus Tick()
        {
            if(m_status == StoryStatus.STATUS_None)
            {
                OnInititalize();
            }

            m_status = Update();

            if(m_status != StoryStatus.STATUS_Running)
            {
                OnTerminate(m_status);
            }

            return m_status;
        }

        public void addChild(StoryBehavior newChild)
        {
            this.m_children.Add(newChild);
        }

        public StoryBehavior getChildAt(int index)
        {
            if (index >= 0 && index < this.m_children.Count)
                return this.m_children[index] as StoryBehavior;
            return null;

        }

    }
}
