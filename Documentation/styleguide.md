# Documentation Style Guide
This document should be used as a reference for documenting the repository.

## Key information
Every system/asset/*thing* that is documented should have at least the following information:
- Authors / contributors
  > Who should be given credit / bothered when things stop working?
- Where in the project to find it
  > Larger systems may have components in multiple folders, so it is important to record where each part can be found.
  > As the project gets bigger, even small systems may be hard to find.
  > 
  > For example, water caustics contains a script (in the scripts/ folder), and 2D textures (in the resources/caustics/ folder)
- How to use the asset
  > What does a developer hoping to use the asset need to know?
  >
  > Do any objects need to be placed in the scene, or in a specific part of the hierarchy?
  >
  > Are there variables that must be set in the inspector? What do these variables mean?

- How the asset works
  > Scripts should be commented for implementation details which do not need to be written in this documentation.
  >
  > Interactions between scripts and other assets, however, should be detailed in this documentation.