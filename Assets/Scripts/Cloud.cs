using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField]
    private float m_startingX;

    [SerializeField]
    private float m_endingX;

    [SerializeField]
    private float m_speed;

    private bool m_floating;

    // Update is called once per frame
    void Update()
    {
        if (m_floating)
        {
            if (transform.position.x + m_speed >= m_endingX)
            {
                transform.position = new Vector3(m_startingX, transform.position.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x + m_speed, transform.position.y, transform.position.z);
            }
        }
    }

    public void SetFloating(bool floating)
    {
        m_floating = floating;
    }
}
