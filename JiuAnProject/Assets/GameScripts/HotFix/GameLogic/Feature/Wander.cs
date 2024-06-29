using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using TEngine;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace GameLogic
{
    public class Wander : MonoBehaviour
    {
        public float mouseSensitivity = 75f; //鼠标灵敏度
        public float moveSpeed = 30f; //移动速度
        public float lerpDuration = 0.5f; //插值时间
        
        private Camera mainCamera;
        private float mouseX, mouseY; //获取鼠标移动的值
        private float moveX, moveY; //获取移动值
        private float moveUp; //上升下降值

        private Vector3 _moveDir;
        [HideInInspector]
        public SubCameraMgr curSubMgr;

        private bool _inputLock;
        // Start is called before the first frame update
        void Start()
        {
            if (Camera.main == null)
            {
                Log.Error("没有找到MainCamera!");
                return;
            }
            mainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            if (!_inputLock)
            {
                //按下左键点击
                if (Input.GetMouseButton(0))
                {
                    // Transform camTrans = mainCamera.transform;
                    // 将鼠标位置转换为世界射线
                    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
     
                    // 进行射线检测
                    if (Physics.Raycast(ray, out hit))
                    {
                        _inputLock = true;
                        // 如果射线碰到物体，输出物体的名字
                        // Debug.Log("Clicked object: " + hit.transform.name);
                        if (hit.transform.CompareTag("Billboard"))
                        {
                            curSubMgr = hit.transform.GetComponentInParent<SubCameraMgr>();
                            curSubMgr.Show();
                            Vector3 destPos = hit.transform.parent.position;
                            mainCamera.transform.DOMove(destPos, lerpDuration).onComplete += () =>
                            {
                                curSubMgr.LaunchCheckHide();
                                _inputLock = false;
                            };
                            mainCamera.transform.DOLookAt(destPos + hit.transform.forward * mainCamera.farClipPlane, lerpDuration);
                        }
                    }
                }
                //按下右键漫游
                else if (Input.GetMouseButton(1))
                {
                    Transform camTrans = mainCamera.transform;
                    mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                    mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
                    camTrans.Rotate(Vector3.up * mouseX, Space.World);
                    camTrans.Rotate(Vector3.left * mouseY, Space.Self);
                    moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
                    moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
                    moveUp = Input.GetAxis("UpDown") * moveSpeed * Time.deltaTime;
                    _moveDir = camTrans.forward * moveY + camTrans.right * moveX + camTrans.up * moveUp;
                    camTrans.Translate(_moveDir, Space.World);
                }
            }
        }
    }
}
