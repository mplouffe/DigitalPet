namespace lvl0
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

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
            EventBus.Register(this);
        }

        private void OnDestroy()
        {
            EventBus.UnRegister(this);
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

        // Update is called once per frame
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
