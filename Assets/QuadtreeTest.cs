using Quadtree;
using UnityEngine;

public class QuadtreeTest : MonoBehaviour {

    // public uint quadtreeMaxDepth = 5; // Yet to implement
    public uint quadtreeBucketSize = 4;
    public Vector2 quadtreeOrigin = new Vector2 (0, 0);
    public Vector2 originalHalfSize = new Vector2 (100, 100);

    private IQuadtree<Vector2> quadtree;

    void Start () {
        Vector2D origin = new Vector2D (quadtreeOrigin.x, quadtreeOrigin.y);
        Vector2D halfSize = new Vector2D (originalHalfSize.x, originalHalfSize.y);
        QuadtreeRegion region = new QuadtreeRegion (origin, halfSize);
        quadtree = new RegionQuadtree<Vector2> (quadtreeBucketSize, region);

        Object[] gameObjectsWithColliders = GameObject.FindObjectsOfType<BoxCollider> ();
        foreach (Object o in gameObjectsWithColliders) {
            BoxCollider bc = (BoxCollider) o;
            Vector2D point = new Vector2D (bc.transform.position.x, bc.transform.position.y);
            quadtree.InsertPoint (point, new Vector2 (point.X, point.Y));

            // Debug.Log (quadtree);
        }

        Debug.Log (quadtree);
    }

}