# QuadtreeUnity

## Previous note
This project is being developed in Unity 2019.1.5f1, in Linux. However, the quadtree's source code is independent from Unity. If you only want the source files, you can find them [here](https://github.com/Jormii/QuadtreesUnity/tree/master/Assets/Quadtree).

## Currently implemented quadtrees
- [x] Region quadtree (added in version 1.1).
- [x] Point quadtree (added in version 1.1).
- [x] Point-region quadtree (added in version 1.0).
- [ ] Edge quadtree.
- [ ] Polygonal map quadtree.
- [ ] Compressed quadtree.

## Contents of the unity package
Latest release: [Version 1.1](https://github.com/Jormii/QuadtreesUnity/releases/tag/v1.1).

The unity package provides:
- Quadtree's source code.
- A scene and a script used for testing the quadtrees and their time consumption. The scene is composed of a camera, a directional light and an empty object with the testing script attached. This scripts spawns as many instances as requested and later adds every instance to the quadtree and measures its performance. Every instance is assigned a value, either a 0 or 1, which defines its color (0: red, 1: yellow).
