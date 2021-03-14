using System;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

namespace View
{
    public class PlayerView : MonoBehaviour, IPlayerView
    {
        private static readonly int IsWalk = Animator.StringToHash("is_walk");
        private static readonly int IsCarry = Animator.StringToHash("is_carry");
        private static readonly int StanTrigger = Animator.StringToHash("stan_trigger");

        [SerializeField]
        private float top;

        [SerializeField]
        private float bottom;

        [SerializeField]
        private float speed = 1f;

        [SerializeField]
        private Collider2D catchTrigger;

        [SerializeField]
        private Transform holder;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private Collider2D footCollider;

        [SerializeField]
        private CinemachineImpulseSource impulseSource;

        private Rigidbody2D rigidbody;

        public Transform CarryingHolder => holder;

        public Collider2D CatchTrigger => catchTrigger;

        public Collider2D FootTrigger => footCollider;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }

        public void SetDirection(int direction)
            => transform.DOScaleX(direction, 0.2f);

        public void SetWalk(bool isWalk)
            => animator.SetBool(IsWalk, isWalk);

        public void SetCarry(bool isCarry)
            => animator.SetBool(IsCarry, isCarry);

        public void SetStan()
            => animator.SetTrigger(StanTrigger);

        public Vector3 Translate(Vector3 vector3)
        {
            if (vector3 == Vector3.zero)
            {
                rigidbody.velocity = Vector2.zero;
            }
            else
            {
                rigidbody.velocity = vector3 * speed;
            }
            return transform.position;
        }

        public void StanImpulse()
        {
            impulseSource.GenerateImpulse();
        }
    }
}