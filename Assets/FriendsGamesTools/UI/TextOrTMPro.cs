using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    [Serializable]
    public class TextOrTMPro
    {
        [SerializeField] Text _text;
        [SerializeField] TextMeshProUGUI _textPro;
        public string text
        {
            get => _text != null ? _text.text : _textPro.text;
            set
            {
                if (_text != null)
                    _text.text = value;
                else
                    _textPro.text = value;
            }
        }
    }
}
