using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Superball
{
    public class TutorialFinger : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            gameObject.SetActive(SuperballRoot.instance.levels.currLocationInd == 0);
        }

    }
}