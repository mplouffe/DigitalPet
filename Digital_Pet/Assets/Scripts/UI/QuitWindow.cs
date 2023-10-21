using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lvl_0
{
    public struct QuitWindowEvent : IEvent
    { }

    public class QuitWindow : MonoBehaviour, IEventReceiver<QuitWindowEvent>
    {
        [SerializeField]
        private CanvasGroup m_quitWindowCanvasGroup;

        private bool m_isShowing;

        void Start()
        {
            EventBus<QuitWindowEvent>.Register(this);
        }

        private void OnDestroy()
        {
            EventBus<QuitWindowEvent>.UnRegister(this);
        }

        void Awake()
        {
            m_quitWindowCanvasGroup.alpha = 0f;
            m_quitWindowCanvasGroup.interactable = false;
        }

        public void OnEvent(QuitWindowEvent e)
        {
            if (!m_isShowing)
            {
                m_isShowing = true;
                m_quitWindowCanvasGroup.alpha = 1f;
                m_quitWindowCanvasGroup.interactable = true;
            }
            else
            {
                m_isShowing = false;
                m_quitWindowCanvasGroup.alpha = 0f;
                m_quitWindowCanvasGroup.interactable = false;
            }
        }

        public void OnQuitConfirmPressed()
        {
            EventBus<SceneChangeEvent>.Raise(new SceneChangeEvent()
            {
                nextScene = Scene.Title
            });
        }

        public void OnQuitCancelPressed()
        {
            m_isShowing = false;
            m_quitWindowCanvasGroup.alpha = 0f;
            m_quitWindowCanvasGroup.interactable = false;
        }
    }
}
