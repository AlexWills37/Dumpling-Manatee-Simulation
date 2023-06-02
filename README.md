# Dumpling-Manatee-Simulation

If you are a developer or plan on opening the project with Unity, [read this section on setting up the project for the first time](#setting-up-the-project-for-developmentbuilding).

# Setting up the project for development/building

In order to work with this project, you will need:
- Unity 2021.3.23f1
  - Android Build Support module
  - Android SDK & NDK Tools module
  - OpenJDK module
- Oculus Integration Package v50.0
    > If you have Git Large File Storage installed, this version of the package will be downloaded with the repository.


Setting up the Oculus Integration Package will take a substantial amount of time, but you will only have to set it up once.

This setup is adapted from [the official getting-started guide from Meta](https://developer.oculus.com/documentation/unity/unity-tutorial-hello-vr/) (some steps on the official guide can be skipped, since they are already part of the project files).


## Why is the Oculus Integration Package here?
In short: the Oculus Integration Package is huge. In its compressed `.unitypackage` form, it is over 500 MB. If I were to include the unpacked assets in the repository, it would include tons of small files that add up to make cloning the repository take a long time.

The downside to this approach is that it requires developers to take these extra steps before working with the project and possibly use Git LFS, and the `.unitypackage` will continue to take up 500 MB (if you have Git LFS) even after it has been imported. 

The upside is that the repository clones much faster, and this is a one-time setup, which I believe justifies the extra work. Also, we will always have the same version of the Oculus Integration Package, in case the official download is ever lost.

## Step 1: Download the Oculus Integration Package
If you do not have [Git Large File Storage (LFS) installed](https://docs.github.com/en/repositories/working-with-files/managing-large-files/installing-git-large-file-storage), I recommend [trying to download the package from the official page](#official-download). 

If you already have Git LFS installed, then the integration package should [download with the repository](#git-large-file-storage-unofficial).

### Official download
The official download can be obtained from [this link](https://developer.oculus.com/downloads/package/unity-integration/), then by selecting **Version 50.0** from the dropdown before clicking **Download**.

### Git Large File storage (unofficial)
If the official download is unavailable, or if you already have Git LFS installed, you can use the package file included in this repository. First, you must install Git LFS. Instructions on that can be found at [this link](https://docs.github.com/en/repositories/working-with-files/managing-large-files/installing-git-large-file-storage). With Git LFS, when you clone this repository, it will download the package file into the OculusIntegrationPackage folder.

## Step 2: Open the project
With Unity 2021.3.23f1, open the `./Dumpling Manatee Simulation/` folder (this folder is the project).

## Step 3: Import the Oculus Integration Package
In the editor, go to **Assets > Import Package > Custom Package**.

Then locate and select `OculusIntegration_v50.0.unitypackage`.

After it loads, a window will allow you to choose what parts of the package to import. In the future, we may want to optimize the project by excluding some of the package. For now, we should just import all.

From the Oculus Integration Package, this project uses:
- VR

If you are prompted to update the Oculus Utilities plugin, click **Yes**.

If you are prompted to use the OpenXR backend, click **Yes**.

When prompted to clean up assets, click **Show Assets** and then **Clean Up**.
