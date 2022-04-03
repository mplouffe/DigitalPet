namespace lvl0
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public enum WorkingState
    {
        Working,
        NotWorking
    }

    public struct JobEvent : IEvent
    {
        public bool workingStateChange;
        public WorkingState workingState;
        public bool promotion;
        public int newSalary;
    }

    public class Job : MonoBehaviour, IEventReceiver<JobEvent>
    {

        [SerializeField]
        private TextMeshProUGUI m_payDayCounter;

        [SerializeField]
        private TextMeshProUGUI m_earningCounter;

        [SerializeField]
        private TextMeshProUGUI m_earningLabel;

        [SerializeField]
        private TextMeshProUGUI m_salaryInfo;

        private bool m_isWorking = false;
        private int m_salary = 0;
        private float m_salaryPerSecond = 0;

        private float m_moneyEarned = 0;
        private float m_timeSinceLastPayDay = 0;

        private const int k_payInterval = 40;
        private const string fmt = "00000";

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
            m_salaryInfo.SetText($"${m_salary}/min");
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (m_isWorking)
            {
                m_moneyEarned += (m_salaryPerSecond * Time.deltaTime);
                m_earningCounter.SetText(((int)m_moneyEarned).ToString(fmt));
            }

            m_timeSinceLastPayDay += Time.deltaTime;

            if (m_timeSinceLastPayDay > k_payInterval)
            {
                EventBus<BankAccountEvent>.Raise(new BankAccountEvent()
                {
                    transactionAmount = (int)m_moneyEarned
                });
                m_moneyEarned = 0;
                m_timeSinceLastPayDay = 0;
            }

            m_payDayCounter.SetText(((int)(k_payInterval * 100 - (m_timeSinceLastPayDay * 100))).ToString(fmt));
        }

        public void OnEvent(JobEvent e)
        {
            if (e.workingStateChange)
            {
                switch (e.workingState)
                {
                    case WorkingState.Working:
                        m_isWorking = true;
                        m_earningLabel.color = Color.green;
                        break;
                    case WorkingState.NotWorking:
                        m_isWorking = true;
                        m_earningLabel.color = Color.red;
                        break;
                }
            }

            if (e.promotion)
            {
                m_salary = e.newSalary;
                m_salaryPerSecond = e.newSalary / 60;
                m_salaryInfo.SetText($"${m_salary}/min");
            }
        }
    }
}
