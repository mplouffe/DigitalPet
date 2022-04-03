namespace lvl0
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class GameOverStarter : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            var coroutine = WaitToCloseDeathScreen();
            StartCoroutine(coroutine);
        }

        private IEnumerator WaitToCloseDeathScreen()
        {
            yield return new WaitForSeconds(3f);
            EventBus<SceneChangeEvent>.Raise(new SceneChangeEvent()
            {
                nextScene = Scene.Title
            });
        }
    }
}
