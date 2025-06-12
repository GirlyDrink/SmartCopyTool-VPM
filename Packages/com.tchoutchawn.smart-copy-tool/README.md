# Smart Copy Tool

A Unity Editor tool for VRChat creators to copy folders and automatically update all asset references in components. Ideal for avatar development, it updates references to textures, meshes, animations, prefabs, and VRCFury components when duplicating asset folders.

## Installation
1. Add this package to your VRChat project via the VRChat Creator Companion (VCC):
   - Go to **Settings > Repos** in VCC.
   - Add the repository URL: `https://tchoutchawn.github.io/SmartCopyTool-VPM/index.json`.
   - In your project, add the **Smart Copy Tool** package from the repository.
2. The tool will appear in the Unity Editor under **Tools > Smart Copy Tool**.

## Usage
1. Open the tool via **Tools > Smart Copy Tool**.
2. Drag a source folder (e.g., `Assets/_Canis/Avatar/Yeen`) into the **Source Folder** field.
3. Enter a new folder name (e.g., `New_Yeen`) in the **New Folder Name** field.
4. Click **Smart Copy** to:
   - Copy the folder to the new destination (e.g., `Assets/_Canis/Avatar/New_Yeen`).
   - Update all asset references in the new folder’s components (e.g., materials, prefabs, VRCFury components) to point to the new assets.
5. Check the Unity Console for success messages or warnings.

## Features
- Updates references in materials, prefabs, animations, and VRCFury components.
- Recursively processes prefab hierarchies for comprehensive reference updates.
- Preserves folder structure using relative paths (e.g., `Yeen/Textures/Albedo.png` → `New_Yeen/Textures/Albedo.png`).
- Logs warnings for missing assets.

## Compatibility
- Unity 2019.4+ (VRChat-compatible versions).
- Supports VRCFury components (e.g., `FullController`, `Toggle`, `SPS`).
- Works with VRChat SDK 3.5.0+ and VRCFury v1.1234.0+.

## License
MIT License

## Support
For issues or feature requests, create an issue on the [GitHub repository](https://github.com/tchoutchawn/SmartCopyTool-VPM).