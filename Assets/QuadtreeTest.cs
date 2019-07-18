using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Quadtree;
using UnityEngine;

public class QuadtreeTest : MonoBehaviour {

    public enum SpawnType
    {
        RandomSquare,
        RandomCircle,
        Diagonal
    }

    public BoxCollider testingPrefab;
    public uint numberOfInstances = 10000;
    public SpawnType spawnType = SpawnType.RandomSquare;

    public uint quadtreeMaxDepth = 5;
    public uint quadtreeBucketSize = 4;
    public Vector2 quadtreeOrigin = new Vector2 (0, 0);
    public Vector2 quadtreeHalfSize = new Vector2 (100, 100);

    public bool printQuadtree = false;

    private IQuadtree<QVector2D> quadtree;

    private void OnDrawGizmosSelected () {
        if (quadtree == null) {
            return;
        }

        List<IQuadtree<QVector2D>> leafNodes = new List<IQuadtree<QVector2D>> ();
        quadtree.GetLeafNodes (leafNodes);
        PaintQuadtree (quadtree);
    }

    private void PaintQuadtree<T> (IQuadtree<T> tree) {
        Vector2 center = new Vector2 (tree.Region.center.x, tree.Region.center.y);

        if (tree.IsLeaf) {
            Gizmos.color = Color.green;
            Vector2 size = new Vector2 (
                tree.Region.halfRegionSize.x * 2f,
                tree.Region.halfRegionSize.y * 2f
            );
            Gizmos.DrawWireCube (center, size);
        } else {
            for (QQuadrant quadrant = QQuadrant.NorthEast; quadrant < QQuadrant.NumberOfQuadrants; ++quadrant) {
                PaintQuadtree (tree.GetChild (quadrant));
            }
        }
    }

    void Start () {
        BoxCollider[] colliders = new BoxCollider[numberOfInstances];
        for (int i = 0; i < numberOfInstances; ++i) {
            Vector2 position = new Vector2();
            switch (spawnType)
            {
                case SpawnType.RandomSquare:
                    position = GetRandomPositionInSquare();
                    break;
                case SpawnType.RandomCircle:
                    position = GetRandomPositionInCircle();
                    break;
                case SpawnType.Diagonal:
                    position = GetPositionInDiagonal(i);
                    break;
            }
            colliders[i] = Instantiate<BoxCollider> (testingPrefab, position, Quaternion.identity);
        }

        BuildQuadtree (colliders);
        QuadtreeCollisionChecking ();
        BruteForceCollisionChecking (colliders);
        PrintDebugMessages ();

        if (printQuadtree)
        {
            UnityEngine.Debug.LogWarning(quadtree);
        }
    }

    private Vector2 GetRandomPositionInSquare () {
        return new Vector2 (
            Random.Range (quadtreeOrigin.x - quadtreeHalfSize.x, quadtreeOrigin.x + quadtreeHalfSize.x),
            Random.Range (quadtreeOrigin.y - quadtreeHalfSize.y, quadtreeOrigin.y + quadtreeHalfSize.y)
        );
    }

    private Vector2 GetRandomPositionInCircle () {
        return Random.insideUnitCircle * System.Math.Min(quadtreeHalfSize.x, quadtreeHalfSize.y);
    }

    private Vector2 GetPositionInDiagonal (int index) {
        return new Vector2 (
            Mathf.Lerp (-quadtreeHalfSize.x, quadtreeHalfSize.x, (float) index / numberOfInstances),
            Mathf.Lerp (-quadtreeHalfSize.y, quadtreeHalfSize.y, (float) index / numberOfInstances)
        );
    }

    private void BuildQuadtree (BoxCollider[] colliders) {
        QVector2D origin = new QVector2D (quadtreeOrigin.x, quadtreeOrigin.y);
        QVector2D halfSize = new QVector2D (quadtreeHalfSize.x, quadtreeHalfSize.y);
        quadtree = new RegionQuadtree<QVector2D> (quadtreeMaxDepth, quadtreeBucketSize, new QRegion(origin, halfSize));

        Stopwatch treeBuildingStopwatch = Stopwatch.StartNew ();
        foreach (BoxCollider bc in colliders) {
            QVector2D point = new QVector2D (bc.transform.position.x, bc.transform.position.y);
            quadtree.InsertPoint (point, point);
        }
        treeBuildingStopwatch.Stop ();
        UnityEngine.Debug.Log ("Time spent building the quadtree: " + treeBuildingStopwatch.Elapsed);
    }

    private void QuadtreeCollisionChecking () {
        Stopwatch collisionCheckingStopwatch = Stopwatch.StartNew ();
        List<IQuadtree<QVector2D>> leafNodes = new List<IQuadtree<QVector2D>> ();
        quadtree.GetLeafNodes (leafNodes);
        foreach (IQuadtree<QVector2D> leaf in leafNodes) {
            List<QVector2D> leafData = new List<QVector2D> (leaf.Data.Values);
            for (int i = 0; i < leafData.Count; ++i) {
                for (int j = i + 1; j < leafData.Count; ++j) {
                    // Collision checking
                }
            }
        }
        collisionCheckingStopwatch.Stop ();
        UnityEngine.Debug.Log ("Time spent checking collisions (using quadtree): " + collisionCheckingStopwatch.Elapsed);
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

    // Remove
    private static Dictionary<string, uint> debugs = new Dictionary<string, uint> ();

    public static void AddDebugMessage (string message) {
        if (debugs.ContainsKey (message)) {
            debugs[message] += 1;
        } else {
            debugs.Add (message, 1);
        }
    }
    private static void PrintDebugMessages () {
        foreach (KeyValuePair<string, uint> entry in debugs.OrderByDescending (key => key.Value)) {
            UnityEngine.Debug.LogFormat ("[ {0} => {1} ]", entry.Key, entry.Value);
        }
    }

}