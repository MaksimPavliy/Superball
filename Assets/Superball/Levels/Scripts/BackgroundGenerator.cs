using FriendsGamesTools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Superball
{


    public class BackgroundItem
    {
        public Transform transform;
        public Vector3 startOffset;
    }
    [Serializable]
    public class BackgroundLayer
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private Transform _layerParent;
        [SerializeField] private float _offsetMultiplier = 1;
        [SerializeField] private float _minDelta = 5;
        [SerializeField] private float _maxDelta = 10;
        [SerializeField] private int _count = 30;
        List<BackgroundItem> _layerObjects;
        
          
        public void Create()
        {
            _layerObjects=new List<BackgroundItem>();
            Vector3 _lastPos = Vector3.zero+Vector3.left * Utils.Random(_minDelta, _maxDelta)*3f ;
            for (int i = 0; i < _count; i++)
            {
                var g = GameObject.Instantiate(_prefab, _layerParent);
                g.transform.localPosition = _lastPos;
                g.transform.localScale *= Utils.Random(0.9f, 1.3f);
                _lastPos += Vector3.right * Utils.Random(_minDelta, _maxDelta);
                _layerObjects.Add(new BackgroundItem { startOffset = g.transform.localPosition, transform = g.transform });
                
            }
        }
        public void Update(Vector3 offset)
        {
            // _layerParent.transform.position = Vector3.right * offset.x * _offsetMultiplier;
            float threshold = 10;
            foreach (var item in _layerObjects)
            {
                item.transform.localPosition = Vector3.right * offset.x * _offsetMultiplier+item.startOffset;
                if (item.transform.position.x > threshold)
                {
                    item.startOffset += Vector3.left * threshold*2;
                } else if (item.transform.position.x < -threshold)
                {
                    item.startOffset += Vector3.right * threshold*2;
                }

            }
        }
    }
    public class BackgroundGenerator: MonoBehaviour
    {
        [SerializeField] private List<BackgroundLayer> _layers;
        [SerializeField] private Transform _bgCamera;
        private void Start()
        {
            foreach (var layer in _layers)
            {
                layer.Create();
            }
        }
        private void Update()
        {
            foreach (var item in _layers)
            {
                item.Update(_bgCamera.position);
            }
        }
    }
}