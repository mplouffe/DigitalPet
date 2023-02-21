namespace lvl0
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public struct NextButtonEvent : IEvent
    {
        public bool requestOn;
    }

    public class NextButton : MonoBehaviour, IEventReceiver<NextButtonEvent>
    {

        [SerializeField]
        private CanvasGroup m_canvasGroup;

        void Start()
        {
            EventBus.Register(this);
        }

        private void OnDestroy()
        {
            EventBus.UnRegister(this);
        }

        void Awake()
        {
            m_canvasGroup.alpha = 0f;
            m_canvasGroup.interactable = false;
        }

        public void OnEvent(NextButtonEvent e)
        {
            if (e.requestOn)
            {
                m_canvasGroup.alpha = 1f;
                m_canvasGroup.interactable = true;
            }
            else
            {
                m_canvasGroup.alpha = 0f;
                m_canvasGroup.interactable = false;
            }
        }
    }
}
