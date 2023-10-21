using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.UI;

namespace lvl_0
{
    public class InformationSlider : MonoBehaviour
    {
        [SerializeField]
        private RectTransform m_informationTabTransform;

        [SerializeField]
        private float m_margin;

        [SerializeField]
        private InformationSliderDirection m_slideDirection;

        [SerializeField]
        private float m_slideLength;
        private Duration m_slideDuration;

        [SerializeField]
        private Button m_openButton;

        [SerializeField]
        private Button m_closeButton;

        [SerializeField]
        private Image m_closeButtonImage;

        private InformationSliderState m_tabState;

        private Vector2 m_closedPosition;
        private Vector2 m_openedPosition;


        private void Awake()
        {
            var height = m_informationTabTransform.rect.height;
            var width = m_informationTabTransform.rect.width;
            m_tabState = InformationSliderState.Closed;
            m_slideDuration = new Duration(m_slideLength);
            m_closedPosition = m_informationTabTransform.anchoredPosition;
            switch (m_slideDirection)
            {
                case InformationSliderDirection.Up:
                    m_openedPosition = new Vector2(m_informationTabTransform.anchoredPosition.x, m_informationTabTransform.anchoredPosition.y + height + m_margin);
                    break;
                case InformationSliderDirection.Down:
                    m_openedPosition = new Vector2(m_informationTabTransform.anchoredPosition.x, m_informationTabTransform.anchoredPosition.y - height - m_margin);
                    break;
                case InformationSliderDirection.Left:
                    m_openedPosition = new Vector2(m_informationTabTransform.anchoredPosition.x + width + m_margin, m_informationTabTransform.anchoredPosition.y);
                    break;
                case InformationSliderDirection.Right:
                    m_openedPosition = new Vector2(m_informationTabTransform.anchoredPosition.x - width - m_margin, m_informationTabTransform.anchoredPosition.y);
                    break;
            }

            m_closeButton.interactable = false;
            m_openButton.interactable = true;
            m_closeButtonImage.raycastTarget = false;
        }

        private void Update()
        {
            switch (m_tabState)
            {
                case InformationSliderState.Opening:
                    m_slideDuration.Update(Time.deltaTime);
                    if (m_slideDuration.Elapsed())
                    {
                        m_informationTabTransform.anchoredPosition = m_openedPosition;
                        m_tabState = InformationSliderState.Open;
                        m_openButton.interactable = false;
                        m_closeButton.interactable = true;
                        m_closeButtonImage.raycastTarget = true;
                    }
                    else
                    {
                        m_informationTabTransform.anchoredPosition = Vector2.Lerp(m_closedPosition, m_openedPosition, m_slideDuration.CurvedDelta());
                    }
                    break;
                case InformationSliderState.Closing:
                    m_slideDuration.Update(Time.deltaTime);
                    if (m_slideDuration.Elapsed())
                    {
                        m_informationTabTransform.anchoredPosition = m_closedPosition;
                        m_tabState = InformationSliderState.Closed;
                        m_closeButton.interactable = false;
                        m_openButton.interactable = true;
                        m_closeButtonImage.raycastTarget = false;
                    }
                    else
                    {
                        m_informationTabTransform.anchoredPosition = Vector2.Lerp(m_openedPosition, m_closedPosition, m_slideDuration.CurvedDelta());
                    }
                    break;
            }
        }

        public void OpenTab()
        {
            if (m_tabState == InformationSliderState.Closed)
            {
                m_slideDuration = new Duration(m_slideLength);
                m_tabState = InformationSliderState.Opening;
            }
        }

        public void CloseTab()
        {
            if (m_tabState == InformationSliderState.Open)
            {
                m_slideDuration = new Duration(m_slideLength / 2);
                m_tabState = InformationSliderState.Closing;
            }
        }
    }

    public enum InformationSliderState
    {
        Closing,
        Closed,
        Opening,
        Open,
    }

    public enum InformationSliderDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}
