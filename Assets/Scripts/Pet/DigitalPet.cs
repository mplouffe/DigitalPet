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
        public float healthInvestment;
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

        private float m_lifeExpectancy = 120f;
        private float m_totalInvestment = 0f;
        private float m_totalTimeOutside = 0f;

        [SerializeField]
        private int m_firstEpochScore;
        [SerializeField]
        private int m_secondEpochScore;
        [SerializeField]
        private int m_thirdEpochScore;

        [SerializeField]
        private float m_fullness = 100f;
        private float m_hungerDecay = 0.05f;
        private float m_lastNotifiedOfHunger = 0;
        private float m_delayBetweenHungerNotifications = 45;
        private float m_totalFeeding = 0f;

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

        private int m_feedingLevelBaseRequirement = 100;
        private int m_investmentLevelBaseRequirement = 50;
        private int m_outsideBaseRequirement = 5;

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

            if (m_currentEpoch != DigitalPetEpoch.Dead)
            { 
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
                        EventBus<DeadMessageEvent>.Raise(new DeadMessageEvent());
                        GameManagerSystem.Instance.petLevel = m_firstEpochScore + m_secondEpochScore + m_thirdEpochScore;
                        EventBus<ContextChangedEvent>.Raise(new ContextChangedEvent()
                        {
                            newContext = Context.Dead
                        });
                        petSprite.sprite = petSpriteSheet[10];
                        var coroutine = DeathPause();
                        StartCoroutine(coroutine);
                    }
                    else
                    {
                        EventBus<EvolveMessageEvent>.Raise(new EvolveMessageEvent());
                        // This is bad but I'm running out of time...
                        bool evolvedWell = false;
                        switch (m_currentEpoch)
                        {
                            case DigitalPetEpoch.Adolescent:
                                if (
                                    m_totalFeeding > m_feedingLevelBaseRequirement &&
                                    m_totalInvestment > m_investmentLevelBaseRequirement &&
                                    m_totalTimeOutside > m_outsideBaseRequirement)
                                {
                                    m_firstEpochScore = 1;
                                    evolvedWell = true;
                                }
                                else
                                {
                                    m_firstEpochScore = 0;
                                }
                                petSprite.sprite = petSpriteSheet[1];
                                break;
                            case DigitalPetEpoch.Teenage:
                                if (m_firstEpochScore == 1)
                                {
                                    if (
                                        m_totalFeeding > m_feedingLevelBaseRequirement * 2 &&
                                        m_totalInvestment > m_investmentLevelBaseRequirement * 2 &&
                                        m_totalTimeOutside > m_outsideBaseRequirement * 2)
                                    {
                                        m_secondEpochScore = 2;
                                        evolvedWell = true;
                                    }
                                    else
                                    {
                                        m_secondEpochScore = 1;
                                    }
                                    petSprite.sprite = petSpriteSheet[2];
                                }
                                else
                                {
                                    if (
                                        m_totalFeeding > m_feedingLevelBaseRequirement &&
                                        m_totalInvestment > m_investmentLevelBaseRequirement &&
                                        m_totalTimeOutside > m_outsideBaseRequirement)
                                    {
                                        m_secondEpochScore = 1;
                                        evolvedWell = true;
                                    }
                                    else
                                    {
                                        m_secondEpochScore = 0;
                                    }
                                    petSprite.sprite = petSpriteSheet[3];
                                }
                                break;
                            case DigitalPetEpoch.Adult:
                                if (m_firstEpochScore + m_secondEpochScore == 3)
                                {
                                    if (
                                        m_totalFeeding > m_feedingLevelBaseRequirement * 4 &&
                                        m_totalInvestment > m_investmentLevelBaseRequirement * 4 &&
                                        m_totalTimeOutside > m_outsideBaseRequirement * 4)
                                    {
                                        m_thirdEpochScore = 3;
                                        evolvedWell = true;
                                        petSprite.sprite = petSpriteSheet[4];
                                    }
                                    else
                                    {
                                        m_thirdEpochScore = 2;
                                        petSprite.sprite = petSpriteSheet[5];
                                    }
                                }
                                else if (m_firstEpochScore + m_secondEpochScore == 2)
                                {
                                    if (
                                        m_totalFeeding > m_feedingLevelBaseRequirement * 3 &&
                                        m_totalInvestment > m_investmentLevelBaseRequirement * 3 &&
                                        m_totalTimeOutside > m_outsideBaseRequirement * 3)
                                    {
                                        m_thirdEpochScore = 1;
                                        evolvedWell = true;
                                        petSprite.sprite = petSpriteSheet[5];
                                    }
                                    else
                                    {
                                        m_thirdEpochScore = 0;
                                        petSprite.sprite = petSpriteSheet[6];
                                    }
                                }
                                else if (m_firstEpochScore + m_secondEpochScore == 1)
                                {
                                    if (
                                        m_totalFeeding > m_feedingLevelBaseRequirement * 2 &&
                                        m_totalInvestment > m_investmentLevelBaseRequirement * 2 &&
                                        m_totalTimeOutside > m_outsideBaseRequirement * 2)
                                    {
                                        m_thirdEpochScore = 1;
                                        evolvedWell = true;
                                        petSprite.sprite = petSpriteSheet[7];
                                    }
                                    else
                                    {
                                        m_thirdEpochScore = 0;
                                        petSprite.sprite = petSpriteSheet[8];
                                    }
                                }
                                else
                                {
                                    if (
                                        m_totalFeeding > m_feedingLevelBaseRequirement &&
                                        m_totalInvestment > m_investmentLevelBaseRequirement &&
                                        m_totalTimeOutside > m_outsideBaseRequirement)
                                    {
                                        evolvedWell = true;
                                        m_thirdEpochScore = 1;
                                        petSprite.sprite = petSpriteSheet[8];
                                    }
                                    else
                                    {
                                        m_thirdEpochScore = 0;
                                        petSprite.sprite = petSpriteSheet[9];
                                    }
                                }
                                break;
                        }

                        var lifeExpectancyModifier = evolvedWell ? 3f : -3f;
                        m_lifeExpectancy = m_age + m_lifeExpectancy + lifeExpectancyModifier;
                    }


                    
                }
            }

            m_fullness += e.feedingAmount;
            m_totalFeeding += e.feedingAmount;
            m_lifeExpectancy += e.healthInvestment;
            m_totalInvestment += e.healthInvestment;
        }

        public void OnEvent(ContextChangedEvent e)
        {
            switch (e.newContext)
            {
                case Context.Inside:
                case Context.Outside:
                    petSprite.color = new Color(1f, 1f, 1f, 1f);
                    break;
                case Context.Shop:
                    petSprite.color = new Color(1f, 1f, 1f, 1f);
                    break;
            }
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
