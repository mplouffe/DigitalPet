using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace lvl_0
{
    public struct PetDialogueEvent : IEvent
    {
        public string dialogue;
        public float dialogueDuration;
    }

    public class PetDialogueWindow : MonoBehaviour, IEventReceiver<PetDialogueEvent>, IEventReceiver<ContextChangedEvent>
    {
        [SerializeField]
        private TextMeshProUGUI m_petDialogueWindowText;

        [SerializeField]
        private CanvasGroup m_petDialogueCanvasGroup;

        private bool m_isShowingDialogue;
        private float m_dialogueWindowStart;
        private float m_dialogueWindowDuration;

        void Start()
        {
            EventBus<PetDialogueEvent>.Register(this);
            EventBus<ContextChangedEvent>.Register(this);
        }

        private void OnDestroy()
        {
            EventBus<PetDialogueEvent>.UnRegister(this);
            EventBus<ContextChangedEvent>.UnRegister(this);
        }

        void Awake()
        {
            m_petDialogueCanvasGroup.alpha = 0f;
            m_petDialogueCanvasGroup.interactable = false;
        }

        public void OnEvent(PetDialogueEvent e)
        {
            m_petDialogueWindowText.SetText(e.dialogue);
            m_dialogueWindowDuration = e.dialogueDuration;
            m_dialogueWindowStart = Time.time;
            m_petDialogueCanvasGroup.alpha = 1f;
            m_petDialogueCanvasGroup.interactable = true;
            m_isShowingDialogue = true;
        }

        public void OnCloseButtonClicked()
        {
            m_petDialogueCanvasGroup.alpha = 0f;
            m_petDialogueCanvasGroup.interactable = false;
            m_isShowingDialogue = false;
        }

        void Update()
        {
            if (m_isShowingDialogue)
            {
                if (Time.time - m_dialogueWindowStart > m_dialogueWindowDuration)
                {
                    m_petDialogueCanvasGroup.alpha = 0f;
                    m_petDialogueCanvasGroup.interactable = false;
                    m_isShowingDialogue = false;
                }
            }
        }

        public void OnEvent(ContextChangedEvent e)
        {
            if (m_isShowingDialogue)
            {
                if (e.newContext == Context.Outside)
                {
                    m_petDialogueCanvasGroup.alpha = 0f;
                    m_petDialogueCanvasGroup.interactable = false;
                    m_isShowingDialogue = false;
                }
            }
        }
    }
}
