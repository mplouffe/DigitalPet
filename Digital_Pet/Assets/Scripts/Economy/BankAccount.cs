using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace lvl_0
{
    public enum TransactionType
    {
        Deposit,
        FeedRequest,
        PointsRequest,
        PetHealthRequest
    }

    public struct BankAccountEvent : IEvent
    {
        public TransactionType transationType;
        public int transactionAmount;
    }

    public class BankAccount : MonoBehaviour, IEventReceiver<BankAccountEvent>, IEventReceiver<ContextChangedEvent>
    {
        [SerializeField]
        private TextMeshProUGUI m_bankAmount;

        [SerializeField]
        private CanvasGroup m_bankAccountCanvasGroup;

        private int m_accountBalance = 0;

        private const string fmt = "0000000";

        void Start()
        {
            EventBus<BankAccountEvent>.Register(this);
            EventBus<ContextChangedEvent>.Register(this);
        }

        private void OnDestroy()
        {
            EventBus<BankAccountEvent>.UnRegister(this);
            EventBus<ContextChangedEvent>.UnRegister(this);
        }

        public void OnEvent(BankAccountEvent e)
        {
            switch(e.transationType)
            {
                case TransactionType.Deposit:
                    m_accountBalance += e.transactionAmount;
                    m_bankAmount.SetText(m_accountBalance.ToString(fmt));
                    break;
                case TransactionType.FeedRequest:
                    if (m_accountBalance - e.transactionAmount >= 0)
                    {
                        m_accountBalance -= e.transactionAmount;
                        m_bankAmount.SetText(m_accountBalance.ToString(fmt));
                        EventBus<FeedEvent>.Raise(new FeedEvent()
                        {
                            approved = true,
                            feedAmount = e.transactionAmount
                        });
                    }
                    else
                    {
                        EventBus<FeedEvent>.Raise(new FeedEvent()
                        {
                            approved = false,
                            feedAmount = 0
                        });
                    }
                    break;
                case TransactionType.PointsRequest:
                    if (m_accountBalance - e.transactionAmount >= 0)
                    {
                        m_accountBalance -= e.transactionAmount;
                        m_bankAmount.SetText(m_accountBalance.ToString(fmt));
                        EventBus<PointsEvent>.Raise(new PointsEvent()
                        {
                            pointsGained = e.transactionAmount * 2
                        });
                    }
                    break;
                case TransactionType.PetHealthRequest:
                    if (m_accountBalance - e.transactionAmount >= 0)
                    {
                        m_accountBalance -= e.transactionAmount;
                        m_bankAmount.SetText(m_accountBalance.ToString(fmt));
                        EventBus<DigitalPetEvent>.Raise(new DigitalPetEvent()
                        {
                            healthInvestment = e.transactionAmount / 2
                        });
                    }
                    break;
            }
        }

        void Awake()
        {
            m_accountBalance = 0;
        }

        public void OnEvent(ContextChangedEvent e)
        {
            switch (e.newContext)
            {
                case Context.Inside:
                    m_bankAccountCanvasGroup.alpha = 1f;
                    break;
                case Context.Outside:
                    m_bankAccountCanvasGroup.alpha = 0f;
                    break;
                case Context.Shop:
                    m_bankAccountCanvasGroup.alpha = 1f;
                    break;
                case Context.Dead:
                    m_bankAccountCanvasGroup.alpha = 0f;
                    break;
            }
        }
    }
}
