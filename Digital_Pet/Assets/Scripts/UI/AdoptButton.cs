using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lvl_0
{
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
