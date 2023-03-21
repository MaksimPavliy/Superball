using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Superball
{
    public class SizeAdapter : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] public Faction _faction;

        void Start()
        {
            _camera = Camera.main;
            float height = _camera.orthographicSize * 2;
            float width = height * _camera.aspect;
            float halfWidth = width * 0.5f;
            float halfHeight = height * 0.5f;



            switch (_faction)
            {
                case Faction.Width:
                    transform.localScale = new Vector2(width, transform.localScale.y);
                    break;
            }
        }
        public enum Faction
        {
            Width
        }
    }
}