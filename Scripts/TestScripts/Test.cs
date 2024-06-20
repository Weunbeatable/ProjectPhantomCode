using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Test : MonoBehaviour
{
    [SerializeField]private Animator m_Animator;
    [SerializeField]private CinemachineVirtualCamera m_Camera;
    private bool m_Running;

    private void Start()
    {
        m_Running = true;
        m_Animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if(m_Running)
            {
                m_Running = false;
                m_Animator.speed = 0;
            }
            else
            {
                m_Running = true;
                m_Animator.speed = 1;
            }
           
        }
        Revolve();
    }
    public void Revolve()
    {
        if (m_Camera.isActiveAndEnabled)
        {
            m_Camera.transform.RotateAround(transform.position, Vector3.up, 90 * Time.deltaTime) ;
        }
    }
    private void OnEnable()
    {
        
    }
}
