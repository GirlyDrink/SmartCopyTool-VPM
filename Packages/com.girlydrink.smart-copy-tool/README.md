# Smart Copy Tool

A Unity Editor tool for VRChat creators to copy folders and automatically update all asset references in components. Ideal for avatar development, it updates references to textures, meshes, animations, prefabs, and VRCFury components. Includes a preview feature with a table view to review reference changes before copying.

## Installation
1. Add this package to your VRChat project via the VRChat Creator Companion (VCC):
   - Visit [https://GirlyDrink.github.io/SmartCopyTool-VPM](https://GirlyDrink.github.io/SmartCopyTool-VPM) to easily copy the repository URL.
   - In VCC, go to **Settings > Repos** and add the repository URL: `https://GirlyDrink.github.io/SmartCopyTool-VPM/index.json`.
   - In your project, add the **Smart Copy Tool** package (version 1.0.3) from the repository.
2. The tool will appear in the Unity Editor under **Tools > GirlyDrink's Tools > Smart Copy Tool**.

## Usage
1. Open the tool via **Tools > GirlyDrink's Tools > Smart Copy Tool**.
2. Drag a source folder (e.g., `Assets/_Canis/Avatar/Yeen`) into the **Source Folder** field.
3. Enter a new folder name (e.g., `New_Yeen`) in the **New Folder Name** field.
4. Click **Preview Changes** to see a table view of all reference updates:
   - Displays columns for Asset, Property, Old Reference, New Reference, and Status (Valid or Missing).
   - Review the table to ensure all references are correct.
5. Click **Confirm and Copy** to:
   - Copy the folder to the new destination (e.g., `Assets/_Canis/Avatar/New_Yeen`).
   - Update all asset references in the new folder’s components to point to the new assets.
   - Or click **Cancel** to abort without changes.
6. Check the Unity Console for success messages or warnings.

## Features
- Updates references in materials, prefabs, animations, and VRCFury components.
- Recursively processes prefab hierarchies for comprehensive reference updates.
- Preserves folder structure using relative paths (e.g., `Yeen/Textures/Albedo.png` → `New_Yeen/Textures/Albedo.png`).
- Previews all reference changes in a table view, highlighting missing assets.
- Logs warnings for missing assets.

## Compatibility
- Unity 2022.3 (VRChat-compatible version).
- Supports VRCFury components (e.g., `FullController`, `Toggle`, `SPS`).
- Works with VRChat SDK 3.5.0+ and VRCFury v1.1234.0+.

## License
MIT License

## Support
For issues or feature requests, create an issue on the [GitHub repository](https://github.com/GirlyDrink/SmartCopyTool-VPM).