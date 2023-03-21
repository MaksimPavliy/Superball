using System.Collections.Generic;
using UnityEngine;

namespace Superball
{
    public class SuperballSkinView : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> skins;
      public void SetActiveSkin(int index)
        {
            for (int i = 0; i < skins.Count; i++)
            {
                skins[i].SetActive(i == index);
            }
        }
        private void Start()
        {
            SuperballSelectorView.AddSkin(this);
        }
    }
}