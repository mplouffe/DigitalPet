namespace lvl0
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class AdoptionStarter : MonoBehaviour
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
            yield return new WaitForSeconds(2.0f);
            EventBus<TextWindowEvent>.Raise(new TextWindowEvent()
            {
                texts = m_texts,
            });
        }

        public void OnAdoptionTextCloseClicked()
        {
            EventBus<CreateMessageEvent>.Raise(new CreateMessageEvent());
            var coroutine = WaitToCloseAdoption();
            StartCoroutine(coroutine);
        }

        private IEnumerator WaitToCloseAdoption()
        {
            yield return new WaitForSeconds(2f);
            EventBus<SceneChangeEvent>.Raise(new SceneChangeEvent()
            {
                nextScene = Scene.Game
            });
        }
    }
}
