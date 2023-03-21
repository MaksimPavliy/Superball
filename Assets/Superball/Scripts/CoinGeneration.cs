using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Superball
{
    public class CoinGeneration : MonoBehaviour
    {
        private int coinAmount;
        private Rigidbody2D rb;

        public Ball ball;

        private void Start()
        {
            coinAmount = PlayerPrefs.GetInt("Coins");
            rb = GetComponent<Rigidbody2D>();
            //text.text = coinAmount.ToString();

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (rb.velocity.y > 0 && collision.tag == "leftTube" || rb.velocity.y > 0 && collision.tag == "rightTube")
            {
                coinAmount = PlayerPrefs.GetInt("Coins");
                coinAmount = ball.jumpCounter % 5 == 0 ? coinAmount += 3 : coinAmount += 1;
                PlayerPrefs.SetInt("Coins", coinAmount);
                //text.text = coinAmount.ToString();
            }
        }
    }
}