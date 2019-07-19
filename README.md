# QuadtreeUnity

## Previous note
This project is being developed in Unity 2019.1.5f1, in Linux. However, the quadtree's source code is independent from Unity.

## Currently implemented quadtrees
- [x] Region quadtree (added in version 1.0).
- [ ] Point quadtree.
- [ ] Point-region quadtree.
- [ ] Edge quadtree.
- [ ] Polygonal map quadtree.
- [ ] Compressed quadtree.

## Contents of the unity package
Latest realease: [Version 1.0](https://github.com/Jormii/QuadtreeUnity/releases/tag/v1.0).

The unity package provides:
- Quadtree's source code.
- A scene and a script used for testing the quadtrees and their time consumption. The scene is composed of a camera and an empty object with the testing script attached. This scripts spawns as many instances as requested and later adds every instance to the quadtree and measures its performance.
