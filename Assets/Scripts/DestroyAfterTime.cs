using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 0.35f;

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }
}
