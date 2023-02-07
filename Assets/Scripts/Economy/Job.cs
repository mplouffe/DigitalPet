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
        public bool payEvent;
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

        private float m_salary = 0.5f;
        private float m_moneyEarned = 0;
        private float m_timeSinceLastPayDay = 0;
        private float m_stateModifier = 0;

        private const int k_payInterval = 20;
        
        private const string fmtEarning = "000.00";
        private const string fmtPayday = "000000";

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
            m_earningLabel.color = Color.green;
            m_earningCounter.SetText(0.ToString(fmtEarning));
            m_salaryInfo.SetText($"${m_salary}/sec");
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            m_timeSinceLastPayDay += Time.deltaTime;

            if (m_timeSinceLastPayDay > k_payInterval)
            {
                EventBus<BankAccountEvent>.Raise(new BankAccountEvent()
                {
                    transactionAmount = (int)m_moneyEarned
                });
                m_moneyEarned = 0;
                m_earningCounter.SetText(0.ToString(fmtEarning));
                m_timeSinceLastPayDay = 0;
            }

            m_moneyEarned += m_salary * Time.deltaTime * m_stateModifier;
            m_earningCounter.SetText(m_moneyEarned.ToString(fmtEarning));

            m_payDayCounter.SetText(((int)(k_payInterval * 100 - (m_timeSinceLastPayDay * 100))).ToString(fmtPayday));
        }

        public void OnEvent(JobEvent e)
        {
            if (e.promotion)
            {
                m_salary += m_salary * 0.5f;
                m_salaryInfo.SetText($"${m_salary}/click");
                EventBus<JobDialogueEvent>.Raise(new JobDialogueEvent() {
                    dialogue = "Nice points!!!\nHave a raise!!!",
                    dialogueDuration = 10
                });
            }
        }

        public void OnEvent(ContextChangedEvent e)
        {
            switch (e.newContext)
            {
                case Context.Inside:
                    m_jobCanvasGroup.alpha = 1f;
                    m_jobCanvasGroup.interactable = true;
                    m_jobCanvasGroup.blocksRaycasts = true;
                    m_stateModifier = 1f;
                    m_earningLabel.color = Color.green;
                    break;
                case Context.Outside:
                    m_jobCanvasGroup.alpha = 1f;
                    m_jobCanvasGroup.interactable = true;
                    m_jobCanvasGroup.blocksRaycasts = true;
                    m_stateModifier = 0.25f;
                    m_earningLabel.color = Color.yellow;
                    break;
                case Context.Shop:
                    m_jobCanvasGroup.alpha = 0f;
                    m_jobCanvasGroup.interactable = false;
                    m_jobCanvasGroup.blocksRaycasts = false;
                    m_stateModifier = -0.01f;
                    m_earningLabel.color = Color.red;
                    break;
                case Context.Dead:
                    m_jobCanvasGroup.alpha = 0f;
                    m_stateModifier = 0f;
                    m_jobCanvasGroup.interactable = false;
                    m_jobCanvasGroup.blocksRaycasts = false;
                    break;
            }
        }

        
    }
}
