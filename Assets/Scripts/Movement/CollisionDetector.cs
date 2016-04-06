/*
public class CollisionDetector : MonoBehaviour {

    private Vector2[] dirs = {Vector2.right, Vector2.up, Vector2.left, Vector2.down};
    private Vector2[,] bounds = null;

    private new Collider2D collider;

    public int NoOfRays = 3;
    private float RayLength = 15f;

    void Start() {
        collider = GetComponent<Collider2D>();
        bounds = new Vector2[,] {
            {
                collider.bounds.center + collider.bounds.extents.x*Vector3.right,
                collider.bounds.center + collider.bounds.extents.x*Vector3.right + collider.bounds.extents.y*Vector3.up,
                collider.bounds.center + collider.bounds.extents.x*Vector3.right + collider.bounds.extents.y*Vector3.down
            },
            {
                collider.bounds.center + collider.bounds.extents.y*Vector3.up,
                collider.bounds.center + collider.bounds.extents.y*Vector3.up + collider.bounds.extents.x*Vector3.left,
                collider.bounds.center + collider.bounds.extents.y*Vector3.up + collider.bounds.extents.x*Vector3.right
            },

        };
    }

	void FixedUpdate() {
        int _layer = gameObject.layer;
        gameObject.layer = GameLayer.IGNORERAYCAST;
        HashSet<Collider2D> collisions = new HashSet<Collider2D>();
	    for(int di = 0; di < dirs.Length; di++) {
	        var dir = dirs[di];

            var pos = collider.bounds.center;
            var p = Mathf.Abs(Vector3.Dot(collider.bounds.extents, dirs[(di + 1) % 4])) * dirs[(di + 1) % 4].To3DXY();
            for (int i = 0; i < NoOfRays; ++i) {
                var origin = pos + p*(1 - i/NoOfRays*2);

                Debug.DrawRay(origin, dir, Color.red);
	            RaycastHit2D hit = Physics2D.Raycast(origin, dir, 1f);
	            if(hit.collider != null && hit.distance < 0.1f) {
	                collisions.Add(hit.collider);
	            }
	        }
	    }
	    gameObject.layer = _layer;
	    foreach(var collision in collisions) {
            SendMessage("CustomCollision", collision);
        }

    }
}
*/