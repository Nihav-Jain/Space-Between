using UnityEngine;
using System.Collections;

namespace Fiea.Rpp.Storytelling
{
    public class StoryParallel : StoryBehavior
    {

        public override StoryStatus Update()
        {
            int successCount = 0;
            int failureCount = 0;

            for (int i = 0; i < m_children.Count;i++ )
            {
                StoryBehavior behavior = m_children[i] as StoryBehavior;
                StoryStatus status = behavior.Tick();

                switch(status)
                {
                    case StoryStatus.STATUS_Success:
                        ++successCount;
                        break;
                    case StoryStatus.STATUS_Failure:
                        ++failureCount;
                        break;
                }

                if (failureCount == m_children.Count || successCount == m_children.Count)
                    return StoryStatus.STATUS_Success;
                return StoryStatus.STATUS_Running;
            }
                return base.Update();
        }
    }
}
