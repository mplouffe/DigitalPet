namespace lvl0
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public struct DeadMessageEvent : IEvent
    {

    }

    public class DeadMessage : MonoBehaviour, IEventReceiver<DeadMessageEvent>
    {
        [SerializeField]
        private SpriteRenderer m_deadSignSprite;

        private bool m_isOnScreen;
        private float m_startShowing;
        private float m_duration = 5f;

        void Start()
        {
            EventBus.Register(this);
        }

        private void OnDestroy()
        {
            EventBus.UnRegister(this);
        }

        public void OnEvent(DeadMessageEvent e)
        {
            m_isOnScreen = true;
            m_startShowing = Time.time;
            m_deadSignSprite.color = new Color(1f, 1f, 1f, 1f);
        }

        // Update is called once per frame
        void Update()
        {
            if (m_isOnScreen)
            {
                if (Time.time - m_startShowing > m_duration)
                {
                    m_isOnScreen = false;
                    m_deadSignSprite.color = new Color(1f, 1f, 1f, 0f);
                }
            }
        }
    }
}
