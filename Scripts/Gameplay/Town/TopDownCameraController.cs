using UnityEngine;

namespace Gameplay.Town {
    public class TopDownCameraController : MonoBehaviour {
        [Header("Movement Settings")]
        [SerializeField] private float dragSensitivity = 1f;
        [SerializeField] private float smoothTime = 0.1f;
        [SerializeField] private bool enableSmoothing = true;
        
        [Header("Zoom Settings")]
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float minZoom = 5f;
        [SerializeField] private float maxZoom = 20f;
        [SerializeField] private bool invertZoom;
        
        [Header("Boundary Settings")]
        [SerializeField] private Vector2 minBounds = new(-10f, -10f); // X and Z bounds
        [SerializeField] private Vector2 maxBounds = new(10f, 10f);   // X and Z bounds
        [SerializeField] private bool showBoundsInGizmos = true;
        [SerializeField] private float groundLevel;
        
        [Header("Input Settings")]
        [SerializeField] private bool useRightClick = true;
        [SerializeField] private bool useMiddleClick;
        
        private Camera _mainCamera;
        private Vector3 _origin;
        private Vector3 _targetPosition;
        private Vector3 _velocity;
        private Plane _groundPlane;
        private float _targetZoom;
        private float _zoomVelocity;

        private void Start() {
            _mainCamera = Camera.main != null ? Camera.main : GetComponent<Camera>();
            
            if (_mainCamera == null) {
                Debug.LogError("TopDownCameraController: No camera found!");
                enabled = false;
                return;
            }
            
            _groundPlane = new Plane(Vector3.up, new Vector3(0, groundLevel, 0));
            _targetPosition = _mainCamera.transform.position;
            
            // Initialize zoom based on camera type
            _targetZoom = _mainCamera.orthographic ? _mainCamera.orthographicSize : _mainCamera.transform.position.y;
        }

        private void LateUpdate() {
            HandleInput();
            HandleZoom();
            UpdateCameraPosition();
        }
        
        private void HandleInput() {
            bool mouseButtonDown = useRightClick && Input.GetMouseButton(1) || useMiddleClick && Input.GetMouseButton(2);

            if (mouseButtonDown) {
                if (!IsDragging) {
                    IsDragging = true;
                    _origin = GetWorldPosition(Input.mousePosition);
                }

                if (!IsDragging) return;
                Vector3 currentMouseWorld = GetWorldPosition(Input.mousePosition);
                if (currentMouseWorld == Vector3.zero || _origin == Vector3.zero) return;
                Vector3 difference = (_origin - currentMouseWorld) * dragSensitivity;
                // Use current camera position as base, not target position
                Vector3 newTargetPosition = _mainCamera.transform.position + difference;
                        
                // Clamp the target position within bounds (X and Z only, preserve Y)
                newTargetPosition.x = Mathf.Clamp(newTargetPosition.x, minBounds.x, maxBounds.x);
                newTargetPosition.z = Mathf.Clamp(newTargetPosition.z, minBounds.y, maxBounds.y);
                newTargetPosition.y = _targetPosition.y; // Preserve height
                        
                // Apply immediately when dragging, no smoothing during drag
                _targetPosition = newTargetPosition;
                if (!IsDragging) return;
                Vector3 immediatePos = newTargetPosition;
                immediatePos.y = _mainCamera.transform.position.y; // Keep current Y
                _mainCamera.transform.position = immediatePos;
            } else {
                if(IsDragging) IsDragging = false;
            }
        }
        
        private void HandleZoom() {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f) {
                float zoomDirection = invertZoom ? -scroll : scroll;
                
                if (_mainCamera.orthographic) {
                    // Orthographic camera: adjust orthographic size
                    _targetZoom -= zoomDirection * zoomSpeed * Time.deltaTime;
                    _targetZoom = Mathf.Clamp(_targetZoom, minZoom, maxZoom);
                } else {
                    // Perspective camera: adjust Y position (height)
                    _targetZoom -= zoomDirection * zoomSpeed;
                    _targetZoom = Mathf.Clamp(_targetZoom, minZoom, maxZoom);
                }
            }
        }
        
        private void UpdateCameraPosition() {
            _mainCamera.transform.position = IsDragging switch {
                // Only apply smoothing when NOT dragging to avoid conflicts
                false when enableSmoothing => Vector3.SmoothDamp(_mainCamera.transform.position, _targetPosition,
                    ref _velocity, smoothTime),
                false => _targetPosition,
                _ => _mainCamera.transform.position
            };

            // Handle zoom separately - always smooth if enabled
            if (_mainCamera.orthographic) {
                if (enableSmoothing) {
                    _mainCamera.orthographicSize = Mathf.SmoothDamp(
                        _mainCamera.orthographicSize,
                        _targetZoom,
                        ref _zoomVelocity,
                        smoothTime
                    );
                } else {
                    _mainCamera.orthographicSize = _targetZoom;
                }
            } else {
                // For perspective camera, adjust Y position for zoom
                Vector3 currentPos = _mainCamera.transform.position;
                
                currentPos.y = enableSmoothing ? Mathf.SmoothDamp(currentPos.y, _targetZoom, ref _zoomVelocity, smoothTime) : _targetZoom;
                
                _mainCamera.transform.position = currentPos;
                _targetPosition.y = currentPos.y; // Keep target position in sync
            }
        }
        
        private Vector3 GetWorldPosition(Vector3 screenPosition) {
            // Cast a ray from camera through the mouse position
            Ray ray = _mainCamera.ScreenPointToRay(screenPosition);
            
            // Find where the ray hits the ground plane
            if (_groundPlane.Raycast(ray, out float distance) && distance > 0) {
                return ray.GetPoint(distance);
            }
            
            return Vector3.zero;
        }
        
        // Update ground plane when ground level changes
        public void SetGroundLevel(float newGroundLevel) {
            groundLevel = newGroundLevel;
            _groundPlane = new Plane(Vector3.up, new Vector3(0, groundLevel, 0));
        }
        
        // Public methods to set bounds dynamically (X and Z axes)
        private void SetBounds(Vector2 min, Vector2 max)
        {
            minBounds = min; // min.x = minX, min.y = minZ
            maxBounds = max; // max.x = maxX, max.y = maxZ
        }
        
        public void SetBounds(Bounds townBounds)
        {
            Vector2 min = new Vector2(townBounds.min.x, townBounds.min.z); // X and Z bounds
            Vector2 max = new Vector2(townBounds.max.x, townBounds.max.z); // X and Z bounds
            SetBounds(min, max);
        }
        
        // Method to instantly move camera to a position (useful for initialization)
        private void SetCameraPosition(Vector3 position)
        {
            position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
            position.z = Mathf.Clamp(position.z, minBounds.y, maxBounds.y);
            
            _targetPosition = position;
            if (!enableSmoothing) {
                _mainCamera.transform.position = position;
            }
        }
        
        // Reset camera to center of bounds
        public void ResetToCenter() {
            Vector3 center = new Vector3(
                (minBounds.x + maxBounds.x) / 2f,
                _mainCamera.transform.position.y,
                (minBounds.y + maxBounds.y) / 2f
            );
            SetCameraPosition(center);
        }
        
        // Check if camera is currently being dragged
        private bool IsDragging { get; set; }

        // Set zoom level directly
        public void SetZoom(float zoom) {
            _targetZoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        }
        
        // Get current zoom level
        public float GetZoom() {
            return _mainCamera.orthographic ? _mainCamera.orthographicSize : _mainCamera.transform.position.y;
        }
        
        // Reset to default zoom
        public void ResetZoom() {
            _targetZoom = (minZoom + maxZoom) / 2f;
        }
        
        // Gizmos for visualizing bounds in Scene view (X and Z plane)
        private void OnDrawGizmosSelected() {
            if (!showBoundsInGizmos) return;
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2f, groundLevel, (minBounds.y + maxBounds.y) / 2f);
            Vector3 size = new Vector3(maxBounds.x - minBounds.x, 0.1f, maxBounds.y - minBounds.y);
            Gizmos.DrawWireCube(center, size);
        }
    }
}