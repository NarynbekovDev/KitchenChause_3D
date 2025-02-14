using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConteinerCounterVisual : MonoBehaviour
{
    private const string OPEN_CLOSE = "OpenClose";

    [SerializeField] private ConteinerCounter conteinerCounter;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        conteinerCounter.OnPlayerGrabbedObject += ConteinerCounter_OnPlayerGrabbedObject;
    }

    private void ConteinerCounter_OnPlayerGrabbedObject(object sender, System.EventArgs e)
    {
        _animator.SetTrigger(OPEN_CLOSE);
    }
}
