namespace lvl0
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public struct TextPanelEvent : IEvent
    {
        public string textToDisplay;
    }

    public class TextPanel : MonoBehaviour, IEventReceiver<TextPanelEvent>
    {

        [SerializeField]
        private TextMeshProUGUI m_text;

        [SerializeField]
        private Image m_panel;

        [SerializeField]
        private CanvasGroup m_canvasGroup;

        void Start()
        {
            EventBus.Register(this);
        }

        void Awake()
        {
            m_canvasGroup.alpha = 0f;
        }

        private void OnDestroy()
        {
            EventBus.UnRegister(this);
        }

        public void OnEvent(TextPanelEvent e)
        {
            if (e.textToDisplay == null)
            {
                m_canvasGroup.alpha = 0f;
            }
            else
            {
                if (m_canvasGroup.alpha < 1f)
                {
                    m_canvasGroup.alpha = 1f;
                }

                m_text.SetText(e.textToDisplay);
                EventBus<NextButtonEvent>.Raise(new NextButtonEvent()
                {
                    requestOn = true
                });
            }
        }
    }
}
