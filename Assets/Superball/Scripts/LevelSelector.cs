using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Superball
{
    public class LevelSelector : MonoBehaviour
    {
        [SerializeField] GameObject[] backgrounds;
        [SerializeField] Material[] splineMaterials;
        [SerializeField] Ball ball;
        [SerializeField] GameObject background;
        private bool changed = true;

        private int counter;

        private void Update()
        {
         //   ChangeBackground();
        }

        public void OnThemeChanged(int index)
        {
            counter = index % backgrounds.Length;

            Vector3 position = background ? background.transform.position : Vector3.zero;
            Destroy(background);
            background = Instantiate(backgrounds[counter], position, Quaternion.identity,transform.parent);
        }
        //private void ChangeBackground()
        //{

        //    if (ball.jumpCounter % 3 != 0)
        //    {
        //        changed = false;
        //    }

        //    if (ball.jumpCounter % 3 == 0 && changed == false)
        //    {
        //        counter++;
        //        counter = counter > backgrounds.Length - 1 ? 0 : counter;

        //        changed = true;
        //        Vector3 position = background?background.transform.position:Vector3.zero;
        //        Destroy(background);
        //        background = Instantiate(backgrounds[counter], position, Quaternion.identity);
        //    }
        //}
    }
}