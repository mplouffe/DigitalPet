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

    public class Job : MonoBehaviour, IEventReceiver<JobEvent>, IEventReceiver<ContextChangedEvent>
    {
        [SerializeField]
        private CanvasGroup m_jobCanvasGroup;

        [SerializeField]
        private TextMeshProUGUI m_payDayCounter;

        [SerializeField]
        private TextMeshProUGUI m_earningCounter;

        [SerializeField]
        private TextMeshProUGUI m_earningLabel;

        [SerializeField]
        private TextMeshProUGUI m_salaryInfo;

        private bool m_isWorking = true;
        private int m_salary = 60;
        private float m_salaryPerSecond = 1;

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
            m_isWorking = true;
            m_earningLabel.color = Color.green;
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
                        m_isWorking = false;
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

        public void OnEvent(ContextChangedEvent e)
        {
            switch (e.newContext)
            {
                case Context.Inside:
                    m_jobCanvasGroup.alpha = 1f;
                    EventBus<JobEvent>.Raise(new JobEvent()
                    {
                        workingStateChange = true,
                        workingState = WorkingState.Working,
                    });
                    break;
                case Context.Outside:
                    m_jobCanvasGroup.alpha = 1f;
                    EventBus<JobEvent>.Raise(new JobEvent()
                    {
                        workingStateChange = true,
                        workingState = WorkingState.NotWorking,
                    });
                    break;
                case Context.Shop:
                    m_jobCanvasGroup.alpha = 1f;
                    EventBus<JobEvent>.Raise(new JobEvent()
                    {
                        workingStateChange = false,
                        workingState = WorkingState.NotWorking
                    });
                    break;
                case Context.Dead:
                    m_jobCanvasGroup.alpha = 0f;
                    EventBus<JobEvent>.Raise(new JobEvent()
                    {
                        workingStateChange = false,
                        workingState = WorkingState.NotWorking
                    });
                    break;
            }
        }
    }
}
