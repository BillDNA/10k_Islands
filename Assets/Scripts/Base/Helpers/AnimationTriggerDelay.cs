using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

//this class should eventually be extended out to all types of parameters
public class AnimationTriggerDelay : BaseMonoBehaviour
{
    [ValueDropdown("parameters")]
    public string animationParameterName;

    private List<string> parameters
    {
        get
        {
            if (animator == null) return new List<string>() { "No Animator" };
            return animator.parameters.Select(x => x.name).ToList();
        }
    }
    
    private Animator _animator;
    public Animator animator
    {
        get
        {
            if (_animator == null) _animator = GetComponent<Animator>();
            return _animator;
        }
    }

    public virtual void StartCountDown(float time)
    {
        timeRemaining = time;
        isCountingDown = true;
    }

    private bool isCountingDown = false;
    private float timeRemaining;
    public override void Update()
    {
        base.Update();
        if (isCountingDown)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                animator.SetTrigger(animationParameterName);
                isCountingDown = false;
            }
            
        }
    }

}
