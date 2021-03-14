using Domain;
using Master;
using UniRx;
using UnityEngine;
using UseCase;
using VContainer;

namespace View
{
    public class GenerateDustView : MonoBehaviour
    {
        [SerializeField]
        private DustView dustPrefab;

        [SerializeField]
        private int num = 10;

        [SerializeField]
        private Vector2 to;

        [SerializeField]
        private Vector2 generateArea = Vector2.one;

        private ISeController seController;

        public GenerateDustModel Model { get; private set; }
        
        [Inject]
        public async void Setup(IDustMaster database, AssetLoader<DustView> assetLoader, ISeController seController)
        {
            this.seController = seController;
            Model = new GenerateDustModel(database, assetLoader);
            Model.OnGenerateDust
                .Subscribe(data => Create(data.Data, data.Prefab, data.Position))
                .AddTo(this);

            var from = new Vector3(transform.position.x, transform.position.y, 0);
            var to = new Vector3(transform.position.x + this.to.x, transform.position.y + this.to.y, 0);
            await Model.StartGenerate(num, from, to, generateArea);
        }

        public void Create(DustEntity data, DustView prefab, Vector3 position)
        {
            var instance = Instantiate(prefab, position, Quaternion.identity);
            instance.Initialize(data, seController);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.0f, 1f, 0.0f, 0.4f);
            Gizmos.DrawCube(transform.position, generateArea);
            
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, to);
        }
    }
}