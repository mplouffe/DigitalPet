namespace lvl0
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class DeathStarter : MonoBehaviour
    {
        [SerializeField]
        private string[] m_texts;

        private void Awake()
        {
            var coroutine = TriggerTextWindow();
            StartCoroutine(coroutine);
        }

        private IEnumerator TriggerTextWindow()
        {
            var petLevel = GameManagerSystem.Instance.petLevel;
            switch (petLevel)
            {
                case 6:
                    m_texts[3] = "'I was given everything I ever wanted. Just like I deserved.'";
                    m_texts[4] = "Looks like your raised quite the spoiled, entitled, pet!";
                    break;
                case 5:
                    m_texts[3] = "'I lived a blessed life. You worked hard to give me everything and I appreciate it so much.'";
                    m_texts[4] = "You should be proud of the fully realized pet you cared for.";
                    break;
                case 4:
                    m_texts[3] = "'My life started out so good. But then you didn't seem to care as much anymore...'";
                    m_texts[4] = "What a shame. So much wasted potential...";
                    break;
                case 3:
                    m_texts[3] = "'My childhood was rough, but you tried your best to make me happy and fullfilled.'";
                    m_texts[4] = "You overcame your mistakes and raised a descent, respectable pet.";
                    break;
                case 2:
                    m_texts[3] = "'You did the bare minimum, and never gave me a chance to be a success.'";
                    m_texts[4] = "You could use some practice caring and rasing digital pets.";
                    break;
                case 1:
                    m_texts[3] = "'It would have been better off if I had never been made...'";
                    m_texts[4] = "ProTip: Try to have more empathy for your pet next time.";
                    break;
                case 0:
                    m_texts[3] = "'You never cared for me. My short life was terrible because of you!!!'";
                    m_texts[4] = "Maybe you should be playing a different game...";
                    break;
            }
            yield return new WaitForSeconds(2.0f);
            EventBus<TextWindowEvent>.Raise(new TextWindowEvent()
            {
                texts = m_texts,
            });
        }

        public void OnDeathTextCloseClicked()
        {
            var coroutine = WaitToCloseDeathScreen();
            StartCoroutine(coroutine);
        }

        private IEnumerator WaitToCloseDeathScreen()
        {
            yield return new WaitForSeconds(1f);
            EventBus<SceneChangeEvent>.Raise(new SceneChangeEvent()
            {
                nextScene = Scene.GameOver
            });
        }
    }
}
