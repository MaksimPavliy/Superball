using UnityEngine;

namespace Superball
{
    public class PipeEntrance: MonoBehaviour
    {
        [SerializeField] private Pipe _parentPipe;
        public Pipe Pipe => _parentPipe;
        [SerializeField] private int _directionSign = 1;
        public int DirectionSign => _directionSign;
      //  public void SetEnabled() { }
        private void OnValidate()
        {
            if (_parentPipe ==null) _parentPipe= GetComponentInParent<Pipe>();
        }
    }
}