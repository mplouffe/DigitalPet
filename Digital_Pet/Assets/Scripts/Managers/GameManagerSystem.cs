#define GAME_MANAGER_SYSTEM

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace lvl_0
{
    public enum Scene
    {
        Title,
        Adoption,
        Game,
        Death,
        GameOver,
    }

    public struct SceneChangeEvent : IEvent
    {
        public Scene nextScene;
    }

    public class GameManagerSystem : SingletonBase<GameManagerSystem>, IEventReceiver<SceneChangeEvent>
    {

        private Scene m_currentScene;

        public int petLevel;

        [SerializeField]
        private bool m_escIsDown = false;

        public void OnEvent(SceneChangeEvent e)
        {
            if (e.nextScene != m_currentScene)
            {
                string nextSceneName;
                switch (e.nextScene)
                {
                    case Scene.Title:
                        nextSceneName = "0_TitleScreen";
                        break;
                    case Scene.Adoption:
                        nextSceneName = "1_AdoptionScreen";
                        break;
                    case Scene.Game:
                        nextSceneName = "2_GameScreen";
                        break;
                    case Scene.Death:
                        nextSceneName = "3_DeathScreen";
                        break;
                    case Scene.GameOver:
                        nextSceneName = "4_GameOverScreen";
                        break;
                    default:
                        nextSceneName = "";
                        break;
                }

                m_currentScene = e.nextScene;
                SceneManager.LoadScene(nextSceneName);
            }
        }

        private void Awake()
        {
            m_currentScene = Scene.Title;
            EventBus<SceneChangeEvent>.Register(this);
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("GameManager");
            if (objs.Length == 0)
            {
                EventBus<SceneChangeEvent>.UnRegister(this);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !m_escIsDown)
            {
                m_escIsDown = true;
                switch (m_currentScene)
                {
                    case Scene.Title:
                        Application.Quit();
                        break;
                    default:
                        EventBus<QuitWindowEvent>.Raise(new QuitWindowEvent());
                        break;
                }
            }

            if (Input.GetKeyUp(KeyCode.Escape) && m_escIsDown)
            {
                m_escIsDown = false;
            }
        }
    }
}
