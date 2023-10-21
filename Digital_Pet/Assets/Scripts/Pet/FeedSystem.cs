using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace lvl_0
{
    public struct FeedEvent : IEvent
    {
        public bool approved;
        public int feedAmount;
    }

    public class FeedSystem : MonoBehaviour, IEventReceiver<FeedEvent>, IEventReceiver<ContextChangedEvent>
    {
        [SerializeField]
        private CanvasGroup m_feedCanvasGroup;

        [SerializeField]
        public TextMeshProUGUI m_feedLevelCounter;

        [SerializeField]
        private float m_lerpSpeed = 0f;

        private int m_feedLevel = 10;
        private int m_feedMultiplier = 20;
        private int m_feedBaseCost = 10;

        private bool m_blinkingFeedCost;
        private float m_blinkingDuration;
        private float m_blinkingLength;
        
        private float m_lerpValue;
        private int m_lerpMultiplier;
        private bool m_ascending;


        private string fmt = "00";

        void Start()
        {
            EventBus<FeedEvent>.Register(this);
            EventBus<ContextChangedEvent>.Register(this);
        }

        private void OnDestroy()
        {
            EventBus<FeedEvent>.UnRegister(this);
            EventBus<ContextChangedEvent>.UnRegister(this);
        }

        void Awake()
        {
            m_feedLevelCounter.SetText(m_feedLevel.ToString(fmt));
        }

        void Update()
        {
            if (m_blinkingFeedCost)
            {
                m_blinkingDuration += Time.deltaTime;
                if (m_blinkingDuration > m_blinkingLength)
                {
                    m_blinkingFeedCost = false;
                    m_blinkingDuration = 0;
                    m_lerpValue = 0;
                    m_lerpMultiplier = 1;
                    m_ascending = true;
                    m_feedLevelCounter.color = Color.black;
                }
                else
                {
                    m_feedLevelCounter.color = Color.Lerp(Color.black, Color.red, m_lerpValue);
                    m_lerpValue += m_lerpSpeed * m_lerpMultiplier;
                    if (m_lerpValue > 1)
                    {
                        m_lerpValue = 1;
                        m_lerpMultiplier = -1;
                    }
                    else if (m_lerpValue < 0)
                    {
                        m_lerpValue = 0;
                        m_lerpMultiplier = 1;
                    }
                }
            }
        }

        public void OnFeedButtonPressedEvent()
        {
            if (!m_blinkingFeedCost)
            {
                EventBus<BankAccountEvent>.Raise(new BankAccountEvent()
                {
                    transationType = TransactionType.FeedRequest,
                    transactionAmount = m_feedLevel
                });
            }
        }

        public void OnEvent(FeedEvent e)
        {
            if (e.approved)
            {
                EventBus<DigitalPetEvent>.Raise(new DigitalPetEvent()
                {
                    evolve = false,
                    feedingAmount = e.feedAmount,
                });
            }
            else
            {

            }
        }

        public void OnFeedLevelChanged(float value)
        {
            m_feedLevel = (int)(m_feedBaseCost + m_feedMultiplier * value);
            m_feedLevelCounter.SetText(m_feedLevel.ToString(fmt));
        }

        public void OnEvent(ContextChangedEvent e)
        {
            switch(e.newContext)
            {
                case Context.Shop:
                case Context.Dead:
                case Context.Outside:
                    m_feedCanvasGroup.alpha = 0f;
                    m_feedCanvasGroup.interactable = false;
                    break;
                case Context.Inside:
                    m_feedCanvasGroup.alpha = 1f;
                    m_feedCanvasGroup.interactable = true;
                    break;
            }
        }
    }
}
