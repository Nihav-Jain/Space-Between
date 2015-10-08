using UnityEngine;
using System.Collections;

namespace Fiea.Rpp.Storytelling
{
    public class StorySequence : StoryBehavior
    {
        private bool isResponseCondSeq;
        private bool isSelectorElse;
        public StorySequence(bool isResponseConditionSeq=false, bool isSelectorElse=false)
        {
            this.isResponseCondSeq = isResponseConditionSeq;
            this.isSelectorElse = isSelectorElse;
        }
        public override StoryStatus Update()
        {
            while (m_childIndex < m_children.Count)
            {
                StoryBehavior currentBehavior = m_children[m_childIndex] as StoryBehavior;
                StoryStatus curStatus = currentBehavior.Tick();
                //Debug.Log(m_childIndex + " : " + curStatus);
                if (isResponseCondSeq && curStatus == StoryStatus.STATUS_Failure)
                    return StoryStatus.STATUS_Success;
                if (curStatus != StoryStatus.STATUS_Success)
                    return curStatus;
                else
                    ++m_childIndex;
            }
            if (isSelectorElse)
                return StoryStatus.STATUS_Running;
            return StoryStatus.STATUS_Success;
        }

        public override string ToString()
        {
            string objectString = string.Format("Sequence: Response Cond: {0}, Selector Else: {1}", this.isResponseCondSeq, this.isSelectorElse);
            return objectString;
        }

        public void StopPropogation()
        {
            foreach(StoryBehavior child in m_children)
            {
                if (child is StoryDialogue)
                    (child as StoryDialogue).StopPropogation();
            }
        }
    }
}
