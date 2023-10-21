using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lvl_0
{
    public class EditorGameManagerSystem : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_gameManagerSystem;

        private void Awake()
        {
#if !GAME_MANAGER_SYSTEM
            Instantiate(m_gameManagerSystem, gameObject.transform.parent);
#endif
        }
    }
}
