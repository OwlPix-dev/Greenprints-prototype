using UnityEngine;

public class NodeAffiliation : MonoBehaviour
{
    [SerializeField] private string _affiliationName;
    [SerializeField] private Color _affiliationColor;

    public string AffiliationName => _affiliationName;
    public Color AffiliationColor => _affiliationColor;
}
