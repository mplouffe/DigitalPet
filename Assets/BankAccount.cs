namespace lvl0
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public struct BankAccountEvent : IEvent
    {
        public int transactionAmount;
    }

    public class BankAccount : MonoBehaviour, IEventReceiver<BankAccountEvent>
    {
        [SerializeField]
        private TextMeshProUGUI m_bankAmount;

        private int m_accountBalance = 0;

        private const string fmt = "0000000";

        void Start()
        {
            EventBus.Register(this);
        }

        private void OnDestroy()
        {
            EventBus.UnRegister(this);
        }

        public void OnEvent(BankAccountEvent e)
        {
            m_accountBalance += e.transactionAmount;
            m_bankAmount.SetText(m_accountBalance.ToString(fmt));
        }

        void Awake()
        {
            m_accountBalance = 0;
        }
    }
}
