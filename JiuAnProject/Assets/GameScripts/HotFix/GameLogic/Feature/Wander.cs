using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using LibVLCSharp;
using TEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector3 = UnityEngine.Vector3;

namespace GameLogic
{
    /// <summary>
    /// 在MainScene下测试记得关闭啊
    /// </summary>
    public class Wander : MonoBehaviour
    {
        public float mouseSensitivity = 50f; //鼠标灵敏度
        public float moveSpeed = 15f; //移动速度
        public float lerpDuration = 0.5f; //插值时间
        
        private Camera mainCamera;
        private float mouseX, mouseY; //获取鼠标移动的值
        private float moveX, moveY; //获取移动值
        private float moveUp; //上升下降值

        private Vector3 _moveDir;
        private float _moveSpeed; //内部速度
        [HideInInspector]
        public BillboardScript scirpt;

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
            _moveSpeed = moveSpeed;
        }

        // Update is called once per frame
        void Update()
        {
            if (!_inputLock)
            {
                try
                {
                    //按下左Shift
                    if (Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        _moveSpeed = moveSpeed * 3;
                    }
                    else if (Input.GetKeyUp(KeyCode.LeftShift))
                    {
                        _moveSpeed = moveSpeed;
                    }

                    //按下左键点击
                    if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
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
                                scirpt = hit.transform.GetComponent<BillboardScript>();
                                scirpt.subMgr.Show();
                                Transform target = hit.transform.parent.GetChild(0);
                                Vector3 destPos = target.position; //拿兄弟节点Transform，其本质是实际的SubCamera
                                float subFov = target.GetComponent<Camera>().fieldOfView;
                                destPos -= target.forward * 7.0f * Mathf.Lerp(0, 1, (subFov - mainCamera.fieldOfView) / 45.0f);
                                mainCamera.transform.DOMove(destPos, lerpDuration).onComplete += () =>
                                {
                                    scirpt.subMgr.LaunchCheckHide();
                                    _inputLock = false;
                                };
                                // mainCamera.transform.DOLookAt(destPos + target.forward * mainCamera.farClipPlane, lerpDuration);
                                mainCamera.transform.DORotateQuaternion(target.rotation, lerpDuration);
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
                        moveX = Input.GetAxis("Horizontal") * _moveSpeed * Time.deltaTime;
                        moveY = Input.GetAxis("Vertical") * _moveSpeed * Time.deltaTime;
                        moveUp = Input.GetAxis("UpDown") * _moveSpeed * Time.deltaTime;
                        _moveDir = camTrans.forward * moveY + camTrans.right * moveX + camTrans.up * moveUp;
                        camTrans.Translate(_moveDir, Space.World);
                    }
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
        }
    }
}
