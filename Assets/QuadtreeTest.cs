using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Quadtree;
using UnityEditor;
using UnityEngine;

public class QuadtreeTest : MonoBehaviour {

    public enum QuadtreeType {
        RegionQuadtree,
        PointQuadtree,
        PointRegionQuadtree
    }

    public enum SpawnType {
        RandomSquare,
        RandomCircle,
        Diagonal
    }

    public BoxCollider testingPrefab;
    public uint numberOfInstances = 2500;
    public SpawnType spawnType = SpawnType.RandomSquare;

    public QuadtreeType quadtreeType = QuadtreeType.RegionQuadtree;
    public uint quadtreeMaxDepth = 5;
    public uint quadtreeBucketSize = 4;
    public Vector2 quadtreeOrigin = new Vector2 (0, 0);
    public Vector2 quadtreeHalfSize = new Vector2 (100, 100);

    public bool printQuadtree = false;
    public bool paintQuadtreeDepths = false;

    private IQuadtree<byte> quadtree;

    private void OnDrawGizmosSelected () {
        if (quadtree == null) {
            return;
        }

        List<IQuadtree<byte>> leafNodes = new List<IQuadtree<byte>> ();
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
            if (paintQuadtreeDepths) {
                Handles.Label (center, tree.Depth.ToString ());
            }
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
            Vector2 position = new Vector2 ();
            switch (spawnType) {
                case SpawnType.RandomSquare:
                    position = GetRandomPositionInSquare ();
                    break;
                case SpawnType.RandomCircle:
                    position = GetRandomPositionInCircle ();
                    break;
                case SpawnType.Diagonal:
                    position = GetPositionInDiagonal (i);
                    break;
            }
            colliders[i] = Instantiate<BoxCollider> (testingPrefab, position, Quaternion.identity);
        }

        BuildQuadtree (colliders);
        QuadtreeCollisionChecking ();
        BruteForceCollisionChecking (colliders);
        PrintDebugMessages ();

        if (printQuadtree) {
            UnityEngine.Debug.LogWarning (quadtree);
        }
    }

    private Vector2 GetRandomPositionInSquare () {
        return new Vector2 (
            Random.Range (quadtreeOrigin.x - quadtreeHalfSize.x, quadtreeOrigin.x + quadtreeHalfSize.x),
            Random.Range (quadtreeOrigin.y - quadtreeHalfSize.y, quadtreeOrigin.y + quadtreeHalfSize.y)
        );
    }

    private Vector2 GetRandomPositionInCircle () {
        return Random.insideUnitCircle * System.Math.Min (quadtreeHalfSize.x, quadtreeHalfSize.y);
    }

    private Vector2 GetPositionInDiagonal (int index) {
        return new Vector2 (
            Mathf.Lerp (-quadtreeHalfSize.x, quadtreeHalfSize.x, (float) index / numberOfInstances),
            Mathf.Lerp (-quadtreeHalfSize.y, quadtreeHalfSize.y, (float) index / numberOfInstances)
        );
    }

    private void BuildQuadtree (BoxCollider[] colliders) {
        CreateQuadtree ();

        Stopwatch treeBuildingStopwatch = Stopwatch.StartNew ();
        foreach (BoxCollider bc in colliders) {
            QVector2D point = new QVector2D (bc.transform.position.x, bc.transform.position.y);
            byte data = GetZeroOrOne ();
            quadtree.InsertPoint (point, data);
            bc.GetComponent<Renderer> ().material.color = (data == 1) ? Color.red : Color.yellow;
        }
        treeBuildingStopwatch.Stop ();
        UnityEngine.Debug.Log ("Time spent building the quadtree: " + treeBuildingStopwatch.Elapsed);
    }

    private void CreateQuadtree () {
        QVector2D origin = new QVector2D (quadtreeOrigin.x, quadtreeOrigin.y);
        QVector2D halfSize = new QVector2D (quadtreeHalfSize.x, quadtreeHalfSize.y);
        QRegion region = new QRegion (origin, halfSize);

        switch (quadtreeType) {
            case QuadtreeType.RegionQuadtree:
                quadtree = new RegionQuadtree<byte> (quadtreeMaxDepth, region, GetZeroOrOne ());
                break;
            case QuadtreeType.PointQuadtree:
                quadtree = new PointQuadtree<byte> (quadtreeMaxDepth, region);
                break;
            case QuadtreeType.PointRegionQuadtree:
                quadtree = new PointRegionQuadtree<byte> (quadtreeMaxDepth, quadtreeBucketSize, region);
                break;
        }
    }

    private byte GetZeroOrOne () {
        return (byte) Random.Range (0, 2);
    }

    private void QuadtreeCollisionChecking () {
        Stopwatch collisionCheckingStopwatch = Stopwatch.StartNew ();
        List<IQuadtree<byte>> leafNodes = new List<IQuadtree<byte>> ();
        quadtree.GetLeafNodes (leafNodes);
        foreach (IQuadtree<byte> leaf in leafNodes) {
            List<byte> leafData = new List<byte> (leaf.Data.Values);
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