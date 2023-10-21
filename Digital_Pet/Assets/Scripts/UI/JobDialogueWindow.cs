using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace lvl_0
{
    public struct JobDialogueEvent : IEvent
    {
        public string dialogue;
        public float dialogueDuration;
    }

    public class JobDialogueWindow : MonoBehaviour, IEventReceiver<JobDialogueEvent>
    {
        [SerializeField]
        private TextMeshProUGUI m_jobDialogueText;

        [SerializeField]
        private CanvasGroup m_jobDialogueCanvasGroup;

        private bool m_isShowingDialogue;
        private float m_dialogueWindowStart;
        private float m_dialogueWindowDuration;

        void Start()
        {
            EventBus<JobDialogueEvent>.Register(this);
        }

        private void OnDestroy()
        {
            EventBus<JobDialogueEvent>.UnRegister(this);
        }
        void Awake()
        {
            m_jobDialogueCanvasGroup.alpha = 0f;
            m_jobDialogueCanvasGroup.interactable = false;
            m_jobDialogueCanvasGroup.blocksRaycasts = false;
        }

        public void OnEvent(JobDialogueEvent e)
        {
            m_jobDialogueText.SetText(e.dialogue);
            m_dialogueWindowDuration = e.dialogueDuration;
            m_dialogueWindowStart = Time.time;
            m_jobDialogueCanvasGroup.alpha = 1f;
            m_jobDialogueCanvasGroup.interactable = true;
            m_jobDialogueCanvasGroup.blocksRaycasts = true;
            m_isShowingDialogue = true;
        }

        public void OnCloseButtonClicked()
        {
            m_jobDialogueCanvasGroup.alpha = 0f;
            m_jobDialogueCanvasGroup.interactable = false;
            m_jobDialogueCanvasGroup.blocksRaycasts = false;
            m_isShowingDialogue = false;
        }

        void Update()
        {
            if (m_isShowingDialogue)
            {
                if (Time.time - m_dialogueWindowStart > m_dialogueWindowDuration)
                {
                    m_jobDialogueCanvasGroup.alpha = 0f;
                    m_jobDialogueCanvasGroup.interactable = false;
                    m_jobDialogueCanvasGroup.blocksRaycasts = false;
                    m_isShowingDialogue = false;
                }
            }
        }
    }
}
