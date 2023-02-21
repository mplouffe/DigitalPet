namespace lvl0
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class AdoptButton : MonoBehaviour
    {
        public void OnAdoptButtonPressed()
        {
            EventBus<SceneChangeEvent>.Raise(new SceneChangeEvent()
            {
                nextScene = Scene.Adoption
            });
        }
    }
}
