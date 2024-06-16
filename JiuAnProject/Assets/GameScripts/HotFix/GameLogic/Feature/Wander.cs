using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class Wander : MonoBehaviour
    {
        public float mouseSensitivity = 75f; //鼠标灵敏度
        public float moveSpeed = 30f; //移动速度
        
        private Transform camera;
        private float mouseX, mouseY; //获取鼠标移动的值
        private float moveX, moveY; //获取移动值
        private float moveUp; //上升下降值

        private Vector3 moveDir;
        // Start is called before the first frame update
        void Start()
        {
            if (Camera.main == null)
            {
                Log.Error("没有找到MainCamera!");
                return;
            }
            camera = Camera.main.transform;
        }

        // Update is called once per frame
        void Update()
        {
            //按下右键操作才有效
            if (Input.GetMouseButton(1))
            {
                mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
                camera.Rotate(Vector3.up * mouseX, Space.World);
                camera.Rotate(Vector3.left * mouseY, Space.Self);
                moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
                moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
                moveUp = Input.GetAxis("UpDown") * moveSpeed * Time.deltaTime;
                moveDir = camera.forward * moveY + camera.right * moveX + camera.up * moveUp;
                camera.Translate(moveDir, Space.World);
            }
        }
    }
}
