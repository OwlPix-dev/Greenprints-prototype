using UnityEngine;

public class GreenprintsToogle : MonoBehaviour
{
    [SerializeField] private GreenprintsUIController _greenprintsUIController;

    [SerializeField] private Greenprints _greenprints;

    [SerializeField] private KeyCode _greenprintsToogleKey = KeyCode.G;

    private void Update()
    {
        if (Input.GetKeyDown(_greenprintsToogleKey) && !_greenprintsUIController.IsActive)
        {
            _greenprintsUIController.OpenGreenprints(_greenprints);
        }
    }
}
