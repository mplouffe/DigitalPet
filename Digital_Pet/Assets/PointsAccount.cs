namespace lvl0
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public struct PointsEvent : IEvent
    {
        public int pointsGained;
    }

    public class PointsAccount : MonoBehaviour, IEventReceiver<PointsEvent>, IEventReceiver<ContextChangedEvent>
    {
        [SerializeField]
        private CanvasGroup m_pointsAccountCanvasGroup;

        [SerializeField]
        private TextMeshProUGUI m_pointsCounter;

        [SerializeField]
        private int[] m_pointsLevels;

        private int m_currentLevel = 0;

        private int m_points;

        private const string fmt = "0000000";

        void Start()
        {
            EventBus.Register(this);
        }

        private void OnDestroy()
        {
            EventBus.UnRegister(this);
        }

        public void OnEvent(PointsEvent e)
        {
            m_points += e.pointsGained;
            m_pointsCounter.SetText(m_points.ToString(fmt));

            if (m_points > m_pointsLevels[m_currentLevel])
            {
                EventBus<JobEvent>.Raise(new JobEvent()
                {
                    promotion = true
                });
                m_currentLevel++;
            }
        }

        public void OnEvent(ContextChangedEvent e)
        {
            switch (e.newContext)
            {
                case Context.Inside:
                    m_pointsAccountCanvasGroup.alpha = 1f;
                    break;
                case Context.Outside:
                    m_pointsAccountCanvasGroup.alpha = 0f;
                    break;
                case Context.Shop:
                    m_pointsAccountCanvasGroup.alpha = 1f;
                    break;
                case Context.Dead:
                    m_pointsAccountCanvasGroup.alpha = 0f;
                    break;
            }
        }

    }
}
