#if CAMERA
using UnityEngine;
using UnityEngine.Serialization;

namespace FriendsGamesTools
{
    [ExecuteAlways]
    public class CameraMoverBounds : MonoBehaviour
    {
        protected CameraMover mover;
        protected Camera cam => mover?.cam;
        public bool enableStartSizePos = true;
        public Vector2 startPos;
        public float startOrthographicSize = 1000;
        public bool local = false;
        private Transform camParent => mover.transform.parent;
        float camZ => local ? cam.transform.localPosition.z : cam.transform.position.z;
        Vector2 camPos
        {
            get => local ? cam.transform.localPosition : cam.transform.position;
            set
            {
                var val = new Vector3(value.x, value.y, camZ);
                if (local)
                    cam.transform.localPosition = val;
                else
                    cam.transform.position = val;
            }
        }
        private void OnEnable()
        {
            if (!Application.isPlaying)
                InitInEditor();
        }
        void InitInEditor()
        {
#if UNITY_EDITOR
            EditorTools.AssetsIterator.IterateOpenedScene(go => {
                var mover = go.GetComponent<CameraMover>();
                if (mover == null) return;
                Init(mover);
            });
#endif
        }
        Quaternion cameraRotation => mover.transform.rotation;

        public virtual void Init(CameraMover mover)
        {
            this.mover = mover;
            if (!enableStartSizePos)
                return;
            if (cam == null) return;
            if (!Application.isPlaying) return;
            cam.orthographicSize = ClampSize(startOrthographicSize);
            camPos = startPos;
        }

        public bool enableSizeMinMax = true;
        public float minOrthographicSize = 500f;
        public float maxOrthographicSize = 2000f;
        public float ClampSize(float size)
            => enableSizeMinMax ? Mathf.Clamp(size, minOrthographicSize, maxOrthographicSize) : size;

        public bool enableWordPosRestrictions = false;
        [FormerlySerializedAs("minWorldPos")]
        public Vector2 minVisiblePos = -Vector2.one * 100;
        [FormerlySerializedAs("maxWorldPos")]
        public Vector2 maxVisiblePos = Vector2.one * 100;
        protected Vector2 halfScreenInWorld => new Vector2(cam.aspect, 1) * cam.orthographicSize;
        public virtual void CameraPosRestrictions()
        {
            if (!enableWordPosRestrictions)
                return;

            // World pos clamp.
            var halfScreenMax = halfScreenInWorld;
            var halfScreenMin = halfScreenMax;
            // Camera cna have own screen bounds if some ui is there.
            if (CameraScreenBounds.instance!=null)
            {
                halfScreenMax.x -= CameraScreenBounds.instance.rightPadding;
                halfScreenMin.x -= CameraScreenBounds.instance.leftPadding;
                halfScreenMax.y -= CameraScreenBounds.instance.topPadding;
                halfScreenMin.y -= CameraScreenBounds.instance.bottomPadding;
            }
            var currMinWorldPos = minVisiblePos + halfScreenMin;
            var currMaxWorldPos = maxVisiblePos - halfScreenMax;
            // Just center screen if camera sees more than whole location.
            if (currMinWorldPos.x > currMaxWorldPos.x)
                currMinWorldPos.x = currMaxWorldPos.x = Mathf.Lerp(currMinWorldPos.x,currMaxWorldPos.x, 0.5f);
            if (currMinWorldPos.y > currMaxWorldPos.y)
                currMinWorldPos.y = currMaxWorldPos.y = Mathf.Lerp(currMinWorldPos.y, currMaxWorldPos.y, 0.5f);
            // Clamp world position.
            camPos = camPos.Clamp(currMinWorldPos, currMaxWorldPos);

            // Scale clamp.
            cam.orthographicSize = ClampSize(cam.orthographicSize);
        }
        [SerializeField] Color col = Color.gray;
        public virtual void DrawRestrictions()
        {
            if (cam == null) return;
            if (!enableWordPosRestrictions)
                return;

            var min = minVisiblePos.ZTo(camZ);
            var max = maxVisiblePos.ZTo(camZ);
            var bl = new Vector3(min.x, min.y, min.z);
            var br = new Vector3(max.x, min.y, min.z);
            var tl = new Vector3(min.x, max.y, min.z);
            var tr = new Vector3(max.x, max.y, min.z);
            if (local)
            {
                bl = camParent.TransformPoint(bl);
                br = camParent.TransformPoint(br);
                tl = camParent.TransformPoint(tl);
                tr = camParent.TransformPoint(tr);
            }
            Debug.DrawLine(bl, br, col);
            Debug.DrawLine(bl, tl, col);
            Debug.DrawLine(tr, br, col);
            Debug.DrawLine(tr, tl, col);
        }

        protected virtual void OnDrawGizmos() => DrawRestrictions();
    }
}
#endif