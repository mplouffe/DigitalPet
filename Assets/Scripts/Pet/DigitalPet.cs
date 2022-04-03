namespace lvl0
{
    using System.Collections;
    using System.Collections.Generic;
    using Unity.Profiling;
    using UnityEngine;

    public struct DigitalPetEvent : IEvent
    {
        public bool evolve;
        public int feedingAmount;
    }

    public enum DigitalPetEpoch
    {
        Infant,
        Adolescent,
        Teenage,
        Adult,
        Dead
    }

    public class DigitalPet : MonoBehaviour, IEventReceiver<DigitalPetEvent>, IEventReceiver<ContextChangedEvent>
    {
        [SerializeField]
        private SpriteRenderer petSprite;

        [SerializeField]
        private Sprite[] petSpriteSheet;

        [SerializeField]
        private float m_lifeExpectancy = 120f;

        [SerializeField]
        private float m_fullness = 100f;
        private float m_hungerDecay = 0.05f;
        private float m_lastNotifiedOfHunger = 0;
        private float m_delayBetweenHungerNotifications = 45;

        [SerializeField]
        private float m_activity = 100f;
        private float m_activityDecay = 0.03f;
        private float m_outdoorsActivityBoost = 0.1f;
        private float m_lastNotifiedOfBoredom = 0;
        private float m_delayBetweenBoredomNotifications = 45;

        [SerializeField]
        private float m_breathDepth = 0.05f;

        [SerializeField]
        private float m_breathRate = 2f;

        private bool m_isBreathing = true;

        [SerializeField]
        private float m_age = 0;
        private DigitalPetEpoch m_currentEpoch = DigitalPetEpoch.Infant;

        private Context m_currentContext = Context.Inside;

        private int m_petLevel = 1;

        void Start()
        {
            EventBus.Register(this);
        }

        private void OnDestroy()
        {
            EventBus.UnRegister(this);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (m_isBreathing)
            {
                var value = Mathf.Sin(Time.time * m_breathRate) * m_breathDepth;
                transform.localScale = new Vector3(transform.localScale.x, 1 + value, transform.localScale.z);
            }

            switch (m_currentEpoch)
            {
                case DigitalPetEpoch.Infant:
                case DigitalPetEpoch.Adolescent:
                case DigitalPetEpoch.Teenage:
                case DigitalPetEpoch.Adult:
                    switch (m_currentContext)
                    {
                        case Context.Outside:
                            m_age += Time.deltaTime;
                            if (m_fullness > 0)
                            {
                                m_fullness = Mathf.Max(0, m_fullness - m_hungerDecay);
                            }

                            if (m_activity < 100)
                            {
                                m_activity = Mathf.Max(0, m_activity + m_outdoorsActivityBoost);
                            }

                            if (m_fullness < 40 && Time.time - m_lastNotifiedOfHunger > m_delayBetweenHungerNotifications)
                            {
                                EventBus<PetDialogueEvent>.Raise(new PetDialogueEvent()
                                {
                                    dialogue = "I'm hungry...",
                                    dialogueDuration = 15
                                });
                                m_lastNotifiedOfHunger = Time.time;
                            }

                            if (m_activity >= 90 && Time.time - m_lastNotifiedOfBoredom > m_delayBetweenBoredomNotifications)
                            {
                                EventBus<PetDialogueEvent>.Raise(new PetDialogueEvent()
                                {
                                    dialogue = "I'm feeling great.",
                                    dialogueDuration = 15
                                });
                                m_lastNotifiedOfBoredom = Time.time;
                            }

                            if (m_age - m_lifeExpectancy > 0)
                            {
                                EventBus<DigitalPetEvent>.Raise(new DigitalPetEvent()
                                {
                                    evolve = true,
                                    feedingAmount = 0,
                                });
                            }
                            break;
                        case Context.Inside:
                            m_age += Time.deltaTime;
                            if (m_fullness > 0)
                            {
                                m_fullness = Mathf.Max(0, m_fullness - m_hungerDecay);
                            }

                            if (m_activity > 0)
                            {
                                m_activity = Mathf.Max(0, m_activity - m_activityDecay);
                            }

                            if (m_fullness < 40 && Time.time - m_lastNotifiedOfHunger > m_delayBetweenHungerNotifications)
                            {
                                EventBus<PetDialogueEvent>.Raise(new PetDialogueEvent()
                                {
                                    dialogue = "I'm hungry...",
                                    dialogueDuration = 15
                                });
                                m_lastNotifiedOfHunger = Time.time;
                            }

                            if (m_activity < 40 && Time.time - m_lastNotifiedOfBoredom > m_delayBetweenBoredomNotifications)
                            {
                                EventBus<PetDialogueEvent>.Raise(new PetDialogueEvent()
                                {
                                    dialogue = "Can we go outside?",
                                    dialogueDuration = 15
                                });
                                m_lastNotifiedOfBoredom = Time.time;
                            }

                            if (m_age - m_lifeExpectancy > 0)
                            {
                                EventBus<DigitalPetEvent>.Raise(new DigitalPetEvent()
                                {
                                    evolve = true,
                                    feedingAmount = 0,
                                });
                            }
                            break;
                        case Context.Shop:
                            break;
                    }
                    break;
            }



        }

        public void OnEvent(DigitalPetEvent e)
        {
            if (e.evolve)
            {
                if (m_currentEpoch < DigitalPetEpoch.Dead)
                {
                    m_currentEpoch++;
                    if (m_currentEpoch >= DigitalPetEpoch.Dead)
                    {
                        // Game over here
                        m_isBreathing = false;
                        GameManagerSystem.Instance.petLevel = m_petLevel;
                        EventBus<ContextChangedEvent>.Raise(new ContextChangedEvent()
                        {
                            newContext = Context.Dead
                        });
                        var coroutine = DeathPause();
                        StartCoroutine(coroutine);
                    }
                    else
                    {
                        var lifeExpectancyModifier = 0f;
                        if (m_activity >= 80)
                        {
                            lifeExpectancyModifier += 15;
                        }
                        else if (m_activity >= 60)
                        {
                            lifeExpectancyModifier += 7;
                        }
                        else if (m_activity >= 40)
                        {
                            lifeExpectancyModifier += 0;
                        }
                        else if (m_activity >= 20)
                        {
                            lifeExpectancyModifier -= 7;
                        }
                        else
                        {
                            lifeExpectancyModifier -= 15;
                        }

                        if (m_fullness >= 80)
                        {
                            lifeExpectancyModifier += 15;
                        }
                        else if (m_fullness >= 60)
                        {
                            lifeExpectancyModifier += 7;
                        }
                        else if (m_fullness >= 40)
                        {
                            lifeExpectancyModifier += 0;
                        }
                        else if (m_fullness >= 20)
                        {
                            lifeExpectancyModifier -= 7;
                        }
                        else
                        {
                            lifeExpectancyModifier -= 15;
                        }

                        m_lifeExpectancy = m_age + m_lifeExpectancy + lifeExpectancyModifier;
                    }

                    petSprite.sprite = petSpriteSheet[(int)m_currentEpoch];
                }
            }

            m_fullness += e.feedingAmount;
        }

        public void OnEvent(ContextChangedEvent e)
        {
            m_currentContext = e.newContext;
        }

        private IEnumerator DeathPause()
        {
            yield return new WaitForSeconds(2.0f);
            EventBus<SceneChangeEvent>.Raise(new SceneChangeEvent()
            {
                nextScene = Scene.Death
            });
        }
    }
}
