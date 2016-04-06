using UnityEngine;

public class CSharpSmoothFollow2D : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f;
    private Transform thisTransform;
    private Vector2 velocity;

    private const float tilesize = 128;
    private const float smoothness = 1/tilesize;

    private void Start()
    {
        thisTransform = transform;
    }

    private void Update(){
        Vector3 vec = thisTransform.position;
        vec.x = Mathf.SmoothDamp(thisTransform.position.x,
            target.position.x, ref velocity.x, smoothTime);
        vec.y = Mathf.SmoothDamp(thisTransform.position.y,
            target.position.y, ref velocity.y, smoothTime);
        thisTransform.position = vec;
    }

    private void LateUpdate() {
        transform.position = new Vector3(
            Mathf.Round(transform.position.x * tilesize) * smoothness,
            Mathf.Round(transform.position.y * tilesize) * smoothness,
            Mathf.Round(transform.position.z * tilesize) * smoothness
            );
    }
}

