using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAnimationController : MonoBehaviour
{
    Animator _animator;

    AudioClip hurtSound;
    AudioClip dodgeSound;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void playDamage()
    {
        _animator.SetTrigger("Damaged");
        SoundDelegation.PlaySoundEffect(hurtSound);
    }

    public void playDodge()
    {
        _animator.SetTrigger("Dodged");
        SoundDelegation.PlaySoundEffect(dodgeSound);
    }
}
