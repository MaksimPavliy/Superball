using FriendsGamesTools;
using UnityEngine;

namespace HcUtils
{
    public class ObjectSelector : MonoBehaviour
    {
        [SerializeField] int index = -1;
        [SerializeField] GameObject[] objects;

        private void Awake()
        {
            if (index < 0)
            {
                index = Utils.Random(0, objects.Length - 1);
            }

            index = index % objects.Length;

            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(i == index);
            }
        }
    }
}