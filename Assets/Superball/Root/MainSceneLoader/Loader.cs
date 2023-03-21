using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.LoadScene(1);
    }
}