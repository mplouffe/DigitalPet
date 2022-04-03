namespace lvl0
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Shop : MonoBehaviour, IEventReceiver<ContextChangedEvent>
    {
        [SerializeField]
        private CanvasGroup m_shopButtonCanvasGroup;

        void Start()
        {
            EventBus.Register(this);
        }

        private void OnDestroy()
        {
            EventBus.UnRegister(this);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnEvent(ContextChangedEvent e)
        {
            switch (e.newContext)
            {
                case Context.Dead:
                case Context.Outside:
                case Context.Shop:
                    m_shopButtonCanvasGroup.alpha = 0f;
                    m_shopButtonCanvasGroup.interactable = false;
                    break;
                case Context.Inside:
                    m_shopButtonCanvasGroup.alpha = 1f;
                    m_shopButtonCanvasGroup.interactable = true;
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
    }
}
