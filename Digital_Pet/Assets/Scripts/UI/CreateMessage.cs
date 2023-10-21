using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lvl_0
{
    public struct CreateMessageEvent : IEvent
    { }

    public class CreateMessage : MonoBehaviour, IEventReceiver<CreateMessageEvent>
    {
        [SerializeField]
        private SpriteRenderer m_createSignSprite;

        
        private bool m_isOnScreen;

        [SerializeField]
        private float m_breathRate = 6;
        
        [SerializeField]
        private float m_breathDepth = 0.25f;

        void Start()
        {
            EventBus<CreateMessageEvent>.Register(this);
        }

        private void OnDestroy()
        {
            EventBus<CreateMessageEvent>.UnRegister(this);
        }

        void Awake()
        {
            m_createSignSprite.color = new Color(1f, 1f, 1f, 0f);
            m_isOnScreen = false;
        }

        public void OnEvent(CreateMessageEvent e)
        {
            m_isOnScreen = true;
            m_createSignSprite.color = new Color(1f, 1f, 1f, 1f);
        }

        void Update()
        {
            if (m_isOnScreen)
            {
                var value = Mathf.Sin(Time.time * m_breathRate) * m_breathDepth;
                transform.localScale = new Vector3(transform.localScale.x, 1 + value, transform.localScale.z);
            }
        }
    }
}
