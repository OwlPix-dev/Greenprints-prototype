using UnityEngine;

public class GreenprintsToogle : MonoBehaviour
{
    [SerializeField] private KeyCode _greenprintsToogleKey = KeyCode.G;

    private void Update()
    {
        if (Input.GetKeyDown(_greenprintsToogleKey))
        {

        }
    }
}
