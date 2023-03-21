using UnityEngine;

namespace FriendsGamesTools
{
    public class CollectionElementTitleAttribute : PropertyAttribute
    {
        public string elementFieldName;
        public CollectionElementTitleAttribute(string elementFieldName)
        {
            this.elementFieldName = elementFieldName;
        }
    }
}