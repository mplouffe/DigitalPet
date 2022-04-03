namespace lvl0
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public enum Context
    {
        Outside,
        Inside,
        Shop,
        Dead,
    }

    public struct ContextChangedEvent : IEvent
    {
        public Context newContext;
    }

    public class Outside : MonoBehaviour, IEventReceiver<ContextChangedEvent>
    {
        [SerializeField]
        private SpriteRenderer[] m_outsideEnvironmentSprites;

        [SerializeField]
        private CanvasGroup m_outsideButtonCanvasGroup;

        [SerializeField]
        private Cloud[] m_clouds;

        [SerializeField]
        private TextMeshProUGUI m_outsideButtonText;

        private Context m_currentContext = Context.Inside;

        void Start()
        {
            EventBus.Register(this);
        }

        private void OnDestroy()
        {
            EventBus.UnRegister(this);
        }

        public void OnEvent(ContextChangedEvent e)
        {
            if (m_currentContext != e.newContext)
            {
                switch (e.newContext)
                {
                    case Context.Outside:
                        for (var i = 0; i < m_outsideEnvironmentSprites.Length; i++)
                        {
                            m_outsideEnvironmentSprites[i].color = new Color(1f, 1f, 1f, 1f);
                        }
                        for (var i = 0; i < m_clouds.Length; i++)
                        {
                            m_clouds[i].SetFloating(true);
                        }
                        m_outsideButtonCanvasGroup.alpha = 1f;
                        m_outsideButtonCanvasGroup.interactable = true;
                        m_outsideButtonText.SetText("[[  In  ]]");
                        break;
                    case Context.Inside:
                        m_outsideButtonCanvasGroup.alpha = 1f;
                        m_outsideButtonCanvasGroup.interactable = true;
                        for (var i = 0; i < m_outsideEnvironmentSprites.Length; i++)
                        {
                            m_outsideEnvironmentSprites[i].color = new Color(1f, 1f, 1f, 0f);
                        }
                        for (var i = 0; i < m_clouds.Length; i++)
                        {
                            m_clouds[i].SetFloating(false);
                        }
                        m_outsideButtonText.SetText("[[  Out  ]]");
                        break;
                    case Context.Shop:
                        m_outsideButtonCanvasGroup.alpha = 1f;
                        m_outsideButtonCanvasGroup.interactable = true;
                        m_outsideButtonText.SetText("[[  Exit  ]]");
                        break;
                    case Context.Dead:
                        m_outsideButtonCanvasGroup.alpha = 0f;
                        m_outsideButtonCanvasGroup.interactable = false;
                        break;
                }

                m_currentContext = e.newContext;
            }
        }

        public void OnOutsideButtonClicked()
        {
            switch(m_currentContext)
            {
                case Context.Inside:
                    EventBus<ContextChangedEvent>.Raise(new ContextChangedEvent
                    {
                        newContext = Context.Outside
                    });
                    EventBus<JobEvent>.Raise(new JobEvent
                    {
                        workingStateChange = true,
                        workingState = WorkingState.NotWorking,
                    });
                    break;
                case Context.Outside:
                    EventBus<ContextChangedEvent>.Raise(new ContextChangedEvent
                    {
                        newContext = Context.Inside
                    });
                    EventBus<JobEvent>.Raise(new JobEvent
                    {
                        workingStateChange = true,
                        workingState = WorkingState.Working,
                    });
                    break;
                case Context.Shop:
                    EventBus<ContextChangedEvent>.Raise(new ContextChangedEvent
                    {
                        newContext = Context.Inside
                    });
                    EventBus<JobEvent>.Raise(new JobEvent
                    {
                        workingStateChange = true,
                        workingState = WorkingState.Working,
                    });
                    break;
            }
        }
    }
}
