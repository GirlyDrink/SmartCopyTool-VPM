# Changelog

All notable changes to the **Smart Copy Tool** will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.5] - 2025-06-13

### Added
- Option to limit the number of rows displayed in the preview table (default: 100 rows).
- "Show More" button to incrementally load additional rows in the preview.

### Changed
- Optimized table view rendering with cached `GUIContent` objects to improve performance for large previews.
- Added a field to set the maximum number of rows in the preview UI.

### Fixed
- Performance issues with the preview table when handling large numbers of reference changes.

## [1.0.4] - 2025-06-13

### Added
- Adjustable column widths in the preview table, resizable by dragging header edges.
- Tooltips for table entries, showing full text on hover for truncated paths.

### Changed
- Updated table view to use precise width control with `GUI.Label` for better layout.
- Incremented version to `1.0.4` to reflect UI enhancements.

## [1.0.3] - 2025-06-13

### Added
- Table view for the preview of reference changes, with columns for Asset, Property, Old Reference, New Reference, and Status (Valid or Missing).

### Changed
- Replaced horizontal layout with a structured table view in the preview.
- Incremented version to `1.0.3` to reflect UI improvement.

## [1.0.2] - 2025-06-13

### Changed
- Moved the tool to the Unity Editor menu path `Tools > GirlyDrink's Tools > Smart Copy Tool` for better organization.
- Incremented version to `1.0.2` to reflect menu path update.

## [1.0.1] - 2025-06-13

### Changed
- Updated package metadata to reflect new username (`GirlyDrink`) and repository (`https://github.com/GirlyDrink/SmartCopyTool-VPM`).
- Incremented version to `1.0.1` for release with updated metadata.

## [1.0.0] - 2025-06-13

### Added
- Website link (`https://GirlyDrink.github.io/SmartCopyTool-VPM`) for easy VCC repository addition.
- Preview feature to review reference changes before copying, listing asset, property, old reference, new reference, and missing asset warnings.

### Changed
- Incremented version to `1.0.0` for first stable release, replacing alpha `0.0.1`.
- Updated for Unity 2022.3 compatibility.

## [0.0.1] - 2025-06-13

### Added
- Initial alpha release of Smart Copy Tool.
- Core functionality to copy folders and update asset references in components (e.g., materials, prefabs, animations, VRCFury components).
- Support for recursive prefab hierarchy processing and relative path preservation.
- Basic UI under `Tools > Smart Copy Tool` for selecting source folder and destination name.
- MIT License with Copyright 2025 GirlyDrink.