using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class LoadingWidget : MonoBehaviour
    {
        private RectTransform _rect;

        public float speed = 180f;
        // Start is called before the first frame update
        void Start()
        {
            _rect = GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {
            _rect.Rotate(Vector3.back, speed * Time.deltaTime);
            
        }
    }
}
