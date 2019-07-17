using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Quadtree;
using UnityEngine;

public class QuadtreeTest : MonoBehaviour {

    public BoxCollider testingPrefab;
    public uint numberOfInstances = 10000;
    public uint quadtreeMaxDepth = 5;
    public uint quadtreeBucketSize = 4;
    public Vector2 quadtreeOrigin = new Vector2 (0, 0);
    public Vector2 originalHalfSize = new Vector2 (100, 100);

    private IQuadtree<Vector2> quadtree;

    public readonly Color quadtreeFirstCenterColor = Color.red;
    public readonly Color quadtreeCenterColor = Color.magenta;
    public readonly Color gridColor = Color.green;
    public readonly Color prefabCenterColor = Color.white;
    public const float radius = 0.15f;

    private void OnDrawGizmosSelected () {
        if (quadtree == null) {
            return;
        }

        List<IQuadtree<Vector2>> leafNodes = new List<IQuadtree<Vector2>> ();
        quadtree.GetLeafNodes (leafNodes);
        PaintQuadtree (quadtree);
    }

    private void PaintQuadtree<T> (IQuadtree<T> tree) {
        Gizmos.color = (tree.Depth == 0) ? quadtreeFirstCenterColor : quadtreeCenterColor;
        Vector2 center = new Vector2 (tree.Region.center.x, tree.Region.center.y);

        if (tree.IsLeaf) {
            Gizmos.color = gridColor;
            Vector2 size = new Vector2 (
                tree.Region.halfRegionSize.x * 2f,
                tree.Region.halfRegionSize.y * 2f
            );
            Gizmos.DrawWireCube (center, size);
        } else {
            for (QuadtreeQuadrant quadrant = QuadtreeQuadrant.NorthEast; quadrant < QuadtreeQuadrant.NumberOfQuadrants; ++quadrant) {
                PaintQuadtree (tree.GetChild (quadrant));
            }
        }
    }

    void Start () {
        BoxCollider[] colliders = new BoxCollider[numberOfInstances];
        for (int i = 0; i < numberOfInstances; ++i) {
            Vector2 position = GetRandomPosition ();
            colliders[i] = Instantiate<BoxCollider> (testingPrefab, position, Quaternion.identity);
        }

        BuildQuadtree (colliders);
        QuadtreeCollisionChecking ();
        BruteForceCollisionChecking (colliders);

        // UnityEngine.Debug.LogWarning (quadtree);
    }

    private Vector2 GetRandomPosition () {
        return Random.insideUnitCircle * System.Math.Min (originalHalfSize.x, originalHalfSize.y);
    }

    private Vector2 GetPositionInDiagonal (int index) {
        return new Vector2 (
            Mathf.Lerp (-originalHalfSize.x, originalHalfSize.x, (float) index / numberOfInstances),
            Mathf.Lerp (-originalHalfSize.y, originalHalfSize.y, (float) index / numberOfInstances)
        );
    }

    private void BuildQuadtree (BoxCollider[] colliders) {
        Vector2D origin = new Vector2D (quadtreeOrigin.x, quadtreeOrigin.y);
        Vector2D halfSize = new Vector2D (originalHalfSize.x, originalHalfSize.y);
        QuadtreeRegion region = new QuadtreeRegion (origin, halfSize);
        quadtree = new RegionQuadtree<Vector2> (quadtreeMaxDepth, quadtreeBucketSize, region);

        Stopwatch treeBuildingStopwatch = Stopwatch.StartNew ();
        foreach (BoxCollider bc in colliders) {
            Vector2D point = new Vector2D (bc.transform.position.x, bc.transform.position.y);
            bool inserted = quadtree.InsertPoint (point, new Vector2 (point.x, point.y));
            if (!inserted) {
                // UnityEngine.Debug.LogWarning ("");
            }
        }
        treeBuildingStopwatch.Stop ();
        UnityEngine.Debug.Log ("Time spent building the quadtree: " + treeBuildingStopwatch.Elapsed);
    }

    private void QuadtreeCollisionChecking () {
        Stopwatch collisionCheckingStopwatch = Stopwatch.StartNew ();
        List<IQuadtree<Vector2>> leafNodes = new List<IQuadtree<Vector2>> ();
        quadtree.GetLeafNodes (leafNodes);
        foreach (IQuadtree<Vector2> leaf in leafNodes) {
            List<Vector2> leafData = new List<Vector2> (leaf.Data.Values); // Too slow?
            for (int i = 0; i < leafData.Count; ++i) {
                for (int j = i + 1; j < leafData.Count; ++j) {
                    // Collision checking
                }
            }
        }
        collisionCheckingStopwatch.Stop ();
        UnityEngine.Debug.Log ("Time spent checking collisions (with quadtree): " + collisionCheckingStopwatch.Elapsed);
    }

    private void BruteForceCollisionChecking (BoxCollider[] colliders) {
        Stopwatch bruteForceCollisionCheckingStopwatch = Stopwatch.StartNew ();
        for (int i = 0; i < colliders.Length; ++i) {
            for (int j = i + 1; j < colliders.Length; ++j) {
                // Collision checking
            }
        }
        bruteForceCollisionCheckingStopwatch.Stop ();
        UnityEngine.Debug.Log ("Time spent checking collisions (brute force): " + bruteForceCollisionCheckingStopwatch.Elapsed);
    }

}