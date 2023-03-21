using UnityEngine;

namespace HcUtils
{
    //Handles a simple process with progress from 0 to 1
    public struct Process
    {
        public float elapsed { private set; get; }
        public float duration { private set; get; }
        public bool isGoing;
        public bool isFinished => elapsed > duration;

        public float progress => elapsed / (duration == 0 ? 1 : duration);
        public float smoothProgress => Mathf.SmoothStep(0f, 1f, progress);
        public float easeOutProgress => Mathf.Sin(progress * Mathf.PI * 0.5f);

        public Process(float duration = -1)
        {
            this.duration = duration;
            isGoing = duration >= 0;
            elapsed = 0;
        }

        public bool StopIsFinished()
        {
            if (isFinished)
            {
                StopProcess();
                return true;
            }
        return  false;
        }

        public void Start(float duration = 1f)
        {
            elapsed = 0;
            this.duration = duration;
            isGoing = true;
        }

        public void StopProcess()
        {
            elapsed = 0;
            isGoing = false;
        }

        public void OnUpdate(float deltaTime)
        {
            if (isFinished) return;
            elapsed += deltaTime;
        }
    }
}