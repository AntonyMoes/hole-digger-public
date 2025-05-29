using System.Collections.Generic;
using _Game.Scripts.Data.Configs;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Level.Digging;
using _Game.Scripts.Game.Level.DynamicTerrain;
using _Game.Scripts.Utils;
using UnityEngine;

namespace _Game.Scripts.Game.Level {
    public class LevelView : MonoBehaviour, ILevelDiggingView {
        [SerializeField] private DynamicMeshView _digZone;
        [SerializeField] private LevelCamera _camera;
        [SerializeField] private LevelScroll _scroll;
        [SerializeField] private LevelEntranceLight _light;

        public Camera Camera => _camera.Camera;

        [SerializeField] private Transform _dropParent;
        public Transform DropParent => _dropParent;

        [SerializeField] private Transform _toolParent;
        public Transform ToolParent => _toolParent;

        [SerializeField] private LevelEffectController _levelEffectController;
        public LevelEffectController LevelEffectController => _levelEffectController;

        [SerializeField] private SoundConfig _dropPickupSound;
        public SoundConfig DropPickupSound => _dropPickupSound;

        [Header("Settings")]
        [Range(0.05f, 1f)] [SerializeField] private float _tapZone = 0.1f;
        [SerializeField] private LayerMask _castMask;

        public int MaxDigDepth => _digZone.DepthCells - 1;
        public DynamicMeshView DigZone => _digZone;

        private LevelDiggingController _diggingController;
        public LevelDiggingController DiggingController => _diggingController;

        private const float CastDistance = 50f;
        private readonly RaycastHit[] _hitBuffer = new RaycastHit[25];

        private Vector3 _initialCameraPosition;

        public IEnumerable<ILevelController.LevelCell> LevelCells => _diggingController.Cells;

        public void Init(int initialDepth, IReadOnlyList<ILevelController.LevelCell> levelCells, IContainer container) {
            _diggingController = container.Create<LevelDiggingController>(_digZone, levelCells, _camera.Movement, this);
            _digZone.Init(_diggingController, _diggingController, initialDepth);
            _camera.MoveCamera(_digZone.BackBoundary, true);
            _initialCameraPosition = _camera.Position.Value;
            _camera.Position.Subscribe(_scroll.UpdateScrollTargetPosition, true);
            _camera.Position.Subscribe(_light.UpdateTargetPosition, true);
            _diggingController.AnimateNextRow(_digZone.FirstRow.Index);
            _diggingController.ClearSavedCells();
        }

        public void AnimateRowFinished() {
            _diggingController.AnimateNextRow(_digZone.FirstRow.Index);
            _camera.MoveCamera(_digZone.BackBoundary);
        }

        public bool TryGetTapTarget(Vector2 screenPosition, bool prioritizeOres, out Transform result) {
            // _hitCells.Clear();
            // _targetHitCell = null;

            var ray = _camera.Camera.ScreenPointToRay(screenPosition);
            // _lastRay = ray;

            RaycastHit targetHit = default;
            Vector3Int? targetPosition = null;
            var targetOre = false;
            var hits = Physics.SphereCastNonAlloc(ray, _tapZone, _hitBuffer, CastDistance, _castMask);
            for (var i = 0; i < hits; i++) {
                var hit = _hitBuffer[i];

                if (hit.transform.TryGetComponent(out Drop _)) {
                    result = hit.transform;
                    return true;
                }

                if (hit.transform.TryGetComponent(out CellView cell)) {
                    var cellPosition = cell.View.Cell.RelativePosition;
                    // _hitCells.Add(cell.transform.position);
                    // TODO REFACTOR
                    var isOre = hit.transform.TryGetComponent<OreCellView>(out _);
                    if (targetPosition is not { } position
                        || prioritizeOres && !targetOre && isOre
                        || ((position.z > cellPosition.z
                             || (position.z == cellPosition.z
                                 && ray.DistanceToPoint(hit.point) <
                                 ray.DistanceToPoint(targetHit.point)))
                            && ((prioritizeOres && !(targetOre && !isOre)) || !prioritizeOres))) {
                        targetHit = hit;
                        targetPosition = cellPosition;
                        targetOre = isOre;
                        // _targetHitCell = cell.transform.position;
                    }
                }
            }

            if (targetPosition is { } pos) {
                result = _digZone.GetFrontCellInLine(pos).transform;
                return true;
            }

            result = default;
            return false;
        }

        private void OnDestroy() {
            _diggingController.Dispose();
        }

        //     private Vector3? _targetHitCell;
        //     private readonly List<Vector3> _hitCells = new List<Vector3>();
        //     private Ray? _lastRay;
        //
        //     private void OnDrawGizmos() {
        //         if (_lastRay is not { } ray) {
        //             return;
        //         }
        //
        //         var oldColor = Gizmos.color;
        //
        //         Gizmos.color = Color.blue;
        //         for (var distance = CastDistance; distance >= 0; distance -= 1) {
        //             var point = ray.GetPoint(distance);
        //             Gizmos.DrawSphere(point, _tapZone);
        //         }
        //
        //         foreach (var hitCell in _hitCells) {
        //             Gizmos.color = hitCell == _targetHitCell ? Color.red : Color.magenta;
        //             Gizmos.DrawCube(hitCell, _digZone.CellSize);
        //         }
        //
        //         Gizmos.color = oldColor;
        //     }
    }
}