using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace lvl_0
{
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
            EventBus<TextPanelEvent>.Register(this);
        }

        private void OnDestroy()
        {
            EventBus<TextPanelEvent>.UnRegister(this);
        }

        void Awake()
        {
            m_canvasGroup.alpha = 0f;
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
