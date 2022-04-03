namespace lvl0
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public struct TextWindowEvent : IEvent
    {
        public string[] texts;
    }

    public class TextWindow : MonoBehaviour, IEventReceiver<TextWindowEvent>
    {
        [SerializeField]
        private CanvasGroup m_textWindowCanvasGroup;

        [SerializeField]
        private TextMeshProUGUI m_textField;

        [SerializeField]
        private CanvasGroup m_nextButtonCanvasGroup;

        [SerializeField]
        private CanvasGroup m_closeButtonCanvasGroup;

        private bool m_isShowing;
        private string[] m_texts;
        private int m_currentText;

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
            m_textWindowCanvasGroup.alpha = 0;
            m_textWindowCanvasGroup.interactable = false;

            m_closeButtonCanvasGroup.alpha = 0;
            m_closeButtonCanvasGroup.interactable = false;
            m_closeButtonCanvasGroup.blocksRaycasts = false;

            m_nextButtonCanvasGroup.alpha = 0;
            m_nextButtonCanvasGroup.interactable = false;
            m_nextButtonCanvasGroup.blocksRaycasts = false;
        }

        public void OnEvent(TextWindowEvent e)
        {
            if (!m_isShowing)
            {
                m_isShowing = true;
                m_textWindowCanvasGroup.alpha = 1;
                m_textWindowCanvasGroup.interactable = true;

                m_texts = (string[])e.texts.Clone();
                m_currentText = 0;

                m_textField.SetText(m_texts[0]);
                if (m_texts.Length > 1)
                {
                    m_nextButtonCanvasGroup.alpha = 1;
                    m_nextButtonCanvasGroup.interactable = true;
                    m_nextButtonCanvasGroup.blocksRaycasts = true;
                }
                else
                {
                    m_closeButtonCanvasGroup.alpha = 1;
                    m_closeButtonCanvasGroup.interactable = true;
                    m_closeButtonCanvasGroup.blocksRaycasts = true;
                }
            }
        }

        public void OnNextButtonClicked()
        {
            m_currentText++;
            if (m_currentText < m_texts.Length)
            {
                m_textField.SetText(m_texts[m_currentText]);

                if (m_texts.Length - m_currentText == 1)
                {
                    m_nextButtonCanvasGroup.alpha = 0;
                    m_nextButtonCanvasGroup.interactable = false;
                    m_nextButtonCanvasGroup.blocksRaycasts = false;

                    m_closeButtonCanvasGroup.alpha = 1;
                    m_closeButtonCanvasGroup.interactable = true;
                    m_closeButtonCanvasGroup.blocksRaycasts = true;
                }
            }
        }

        public void OnCloseButtonClicked()
        {
            m_textWindowCanvasGroup.alpha = 0;
            m_textWindowCanvasGroup.interactable = false;

            m_closeButtonCanvasGroup.alpha = 0;
            m_closeButtonCanvasGroup.interactable = false;
            m_closeButtonCanvasGroup.blocksRaycasts = false;

            m_nextButtonCanvasGroup.alpha = 0;
            m_nextButtonCanvasGroup.interactable = false;
            m_nextButtonCanvasGroup.blocksRaycasts = false;

            m_isShowing = false;
        }
    }
}
