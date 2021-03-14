using Cysharp.Threading.Tasks;
using DG.Tweening;
using Domain;
using EPOOutline;
using Master;
using UniRx;
using UnityEngine;
using UseCase;
using VContainer;

namespace View
{
    public class DustView : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer renderer;

        [SerializeField]
        private Collider2D collider2D;

        [SerializeField]
        private Outlinable outline;

        [SerializeField]
        private AudioClip dropSe;

        [SerializeField]
        private AudioClip dustInSe;

        public DustModel Model { get; private set; }

        [Inject]
        public void Initialize(DustEntity data, ISeController seController)
        {
            Model = new DustModel(transform, data);

            Model.Selected
                .Subscribe(SetSelected)
                .AddTo(this);

            Model.OnTrash
                .Subscribe(async _ =>
                {
                    await renderer.DOFade(0, 0.35f);
                    Destroy(gameObject);
                })
                .AddTo(this);

            Model.Carrying
                .Subscribe(isCarrying => collider2D.enabled = !isCarrying)
                .AddTo(this);

            Model.Carrying
                .Pairwise()
                .Subscribe(pair =>
                {
                    if (pair.Previous && !pair.Current)
                    {
                        seController.Play(dropSe);
                        Model.Position = transform.position;
                    }
                })
                .AddTo(this);

            Model.Position = transform.position;
        }

        private void SetSelected(bool isSelected)
        {
            outline.enabled = isSelected;
        }
    }
}
