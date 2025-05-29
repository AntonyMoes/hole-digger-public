using _Game.Scripts.Data.Configs.Level;
using _Game.Scripts.Game.Level.DynamicTerrain;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Game.Level.Digging {
    public class OreCellView : MonoBehaviour {
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private CellView _cellView;
        public CellView CellView => _cellView;
        [SerializeField] private PrebuiltMeshConfig _meshConfig;
        public PrebuiltMeshConfig MeshConfig => _meshConfig;

        [Header("Settings")]
        [SerializeField] private bool _displayPrebuiltMesh;

        private static readonly (Vector3 meshRotation, Vector3 faceRotationAxis)[] Rotations = new[] {
            (Vector3.zero, Vector3.forward),
            (Vector3.up * 90, Vector3.right),
            (Vector3.up * 180, Vector3.forward),
            (Vector3.up * 270, Vector3.right),
            (new Vector3(0, 90, 90), Vector3.right),
            (new Vector3(0, -90, 90), Vector3.right),
        };

        private static readonly int ShinePeriod = Shader.PropertyToID("_ShinePeriod");
        private static readonly int PeriodOffset = Shader.PropertyToID("_PeriodOffset");

        public void Init(Rng rng, Vector3 scale) {
            const int oreMaterialIndex = 1;
            var material = _renderer.sharedMaterials[oreMaterialIndex];
            var materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetFloat(PeriodOffset, rng.NextFloat(0, material.GetFloat(ShinePeriod)));
            _renderer.SetPropertyBlock(materialPropertyBlock, oreMaterialIndex);

            var (meshRotation, faceRotationAxis) = rng.NextChoice(Rotations);
            var faceRotation = /*0;//*/rng.NextInt(0, 4) * 90;
            var rotation = meshRotation + faceRotationAxis * faceRotation;
            transform.localRotation = Quaternion.Euler(rotation);

            var rotatedScale = Quaternion.Inverse(transform.localRotation) * scale;
            transform.localScale = new Vector3(
                Mathf.Abs(rotatedScale.x),
                Mathf.Abs(rotatedScale.y),
                Mathf.Abs(rotatedScale.z)
            );
        }

        private void OnDrawGizmos() {
            if (MeshConfig == null || !_displayPrebuiltMesh) {
                return;
            }

            foreach (var point in MeshConfig.PointData) {
                Gizmos.color = point.offset == Vector3.zero ? Color.gray : Color.magenta;
                var position = transform.position + (point.distance * ((Vector3) point.toPoint - point.fromPoint) + point.fromPoint) / IDynamicMeshCell.RelativeSize + point.offset;
                Gizmos.DrawSphere(position, 0.03f);
                Gizmos.DrawLine(position, position - point.offset);
            }

            // Gizmos.DrawSphere(new Vector3(-0.0807869434f, 0.353265822f, -0.5f), 0.05f);
            // Gizmos.DrawSphere(new Vector3(0.341402769f, 0.451596439f, -0.244617358f), 0.05f);
            // Gizmos.DrawSphere(new Vector3(0.449999988f, 0.451596439f, -0.0840660334f), 0.05f);
            // Gizmos.DrawSphere(new Vector3(0.449999988f, 0.451596439f, -0.199906886f), 0.05f);
        }
    }
}