using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace lvl_0
{
    public class Shop : MonoBehaviour, IEventReceiver<ContextChangedEvent>
    {
        [SerializeField]
        private CanvasGroup m_shopButtonCanvasGroup;

        [SerializeField]
        private CanvasGroup m_shopUI;

        [SerializeField]
        private SpriteRenderer[] m_ShopEnvironmentSprites;

        [SerializeField]
        private TextMeshProUGUI m_pointsCostLabel;

        [SerializeField]
        private TextMeshProUGUI m_petHealthCostLabel;

        private int m_pointsCost = 10;
        private int m_petHealthCost = 50;

        private const string fmt = "000";

        void Start()
        {
            EventBus<ContextChangedEvent>.Register(this);
        }

        private void OnDestroy()
        {
            EventBus<ContextChangedEvent>.UnRegister(this);
        }

        private void Awake()
        {
            m_pointsCostLabel.SetText($"${m_pointsCost.ToString(fmt)}");
            m_petHealthCostLabel.SetText($"${m_petHealthCost.ToString(fmt)}");
            m_shopUI.alpha = 0f;
            m_shopUI.interactable = false;
            m_shopUI.blocksRaycasts = false;
        }

        public void OnEvent(ContextChangedEvent e)
        {
            switch (e.newContext)
            {
                case Context.Dead:
                case Context.Outside:
                    m_shopButtonCanvasGroup.alpha = 0f;
                    m_shopButtonCanvasGroup.interactable = false;
                    m_shopUI.alpha = 0f;
                    m_shopUI.interactable = false;
                    m_shopUI.blocksRaycasts = false;
                    for (var i = 0; i < m_ShopEnvironmentSprites.Length; i++)
                    {
                        m_ShopEnvironmentSprites[i].color = new Color(1f, 1f, 1f, 0f);
                    }
                    break;
                case Context.Shop:
                    m_shopButtonCanvasGroup.alpha = 0f;
                    m_shopButtonCanvasGroup.interactable = false;
                    m_shopUI.alpha = 1f;
                    m_shopUI.interactable = true;
                    m_shopUI.blocksRaycasts = true;
                    for (var i = 0; i < m_ShopEnvironmentSprites.Length; i++)
                    {
                        m_ShopEnvironmentSprites[i].color = new Color(1f, 1f, 1f, 1f);
                    }
                    break;
                case Context.Inside:
                    m_shopButtonCanvasGroup.alpha = 1f;
                    m_shopButtonCanvasGroup.interactable = true;
                    m_shopUI.alpha = 0f;
                    m_shopUI.interactable = false;
                    m_shopUI.blocksRaycasts = false;
                    for (var i = 0; i < m_ShopEnvironmentSprites.Length; i++)
                    {
                        m_ShopEnvironmentSprites[i].color = new Color(1f, 1f, 1f, 0f);
                    }
                    break;
            }
        }

        public void OnShopButtonClicked()
        {
            EventBus<ContextChangedEvent>.Raise(new ContextChangedEvent()
            {
                newContext = Context.Shop
            });
        }

        public void OnPetHealthButtonClicked()
        {
            EventBus<BankAccountEvent>.Raise(new BankAccountEvent()
            {
                transationType = TransactionType.PetHealthRequest,
                transactionAmount = m_petHealthCost
            });
        }

        public void OnPointsButtonClicked()
        {
            EventBus<BankAccountEvent>.Raise(new BankAccountEvent()
            {
                transationType = TransactionType.PointsRequest,
                transactionAmount = m_pointsCost
            });
        }
    }
}
