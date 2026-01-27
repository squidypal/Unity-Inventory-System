using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Items
{
    public class InventoryItemAnimator : MonoBehaviour
    {
        [Header("Animations")] 
        public AnimationClip equipAnimation;
        public AnimationClip unequipAnimation;
        
        private Animator animator;
        private bool isUnequipDone = false;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void StartEquipAnimation()
        {
            isUnequipDone = false;
        }

        public void OnUnequipAnimationFinish()
        {
            isUnequipDone = true;
        }

        public void StartUnequip()
        {
            animator.Play(unequipAnimation.name);
        }

        public bool IsUnequipDone()
        {
            return isUnequipDone;
        }

        public AnimationClip GetCurrentPlayingAnimationClip()
        {
            return animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        }
    }
}
