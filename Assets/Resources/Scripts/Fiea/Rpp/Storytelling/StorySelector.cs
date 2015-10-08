using UnityEngine;
using System.Collections;

namespace Fiea.Rpp.Storytelling
{
    // Precondition:
    // First child of the selector must be a StorySequence and the first child of that story Sequence must be a StoryCondition
    public class StorySelector : StoryBehavior
    {

        public override StoryStatus Update()
        {
            while (m_childIndex < m_children.Count)
            {
                StoryBehavior currentBehavior = m_children[m_childIndex] as StoryBehavior;
                StoryStatus curStatus = currentBehavior.Tick();

                if (curStatus != StoryStatus.STATUS_Failure)
                {
                    m_childIndex = 0;
                    return curStatus;
                }
                else
                    ++m_childIndex;
            }
            m_childIndex = 0;
            return StoryStatus.STATUS_Running;  // consider STATUS_Failure too
        }
    }
}
