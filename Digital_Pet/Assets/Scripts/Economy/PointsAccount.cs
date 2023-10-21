using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace lvl_0
{
    public struct PointsEvent : IEvent
    {
        public int pointsGained;
    }

    public class PointsAccount : MonoBehaviour, IEventReceiver<PointsEvent>
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
            EventBus<PointsEvent>.Register(this);
        }

        private void OnDestroy()
        {
            EventBus<PointsEvent>.UnRegister(this);
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
    }
}
