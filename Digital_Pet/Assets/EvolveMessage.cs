namespace lvl0
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public struct EvolveMessageEvent : IEvent
    {

    }

    public class EvolveMessage : MonoBehaviour, IEventReceiver<EvolveMessageEvent>
    {
        [SerializeField]
        private SpriteRenderer m_evolveSignSprite;

        private bool m_isOnScreen;
        private float m_startShowing;
        private float m_duration = 5f;

        private float m_breathRate = 6;
        private float m_breathDepth = 0.25f;

        void Start()
        {
            EventBus.Register(this);
        }

        private void OnDestroy()
        {
            EventBus.UnRegister(this);
        }

        public void OnEvent(EvolveMessageEvent e)
        {
            m_isOnScreen = true;
            m_startShowing = Time.time;
            m_evolveSignSprite.color = new Color(1f, 1f, 1f, 1f);
        }

        // Update is called once per frame
        void Update()
        {
            if (m_isOnScreen)
            {
                var value = Mathf.Sin(Time.time * m_breathRate) * m_breathDepth;
                transform.localScale = new Vector3(transform.localScale.x, 1 + value, transform.localScale.z);

                if (Time.time - m_startShowing > m_duration)
                {
                    m_isOnScreen = false;
                    m_evolveSignSprite.color = new Color(1f, 1f, 1f, 0f);
                }
            }
        }
    }
}
