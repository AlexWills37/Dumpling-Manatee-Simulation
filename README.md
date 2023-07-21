# Dumpling-Manatee-Simulation
#### Documentation Guide
If you want to play through this simulation without setting up Unity, 
please read this section:
- [Installing a release](#installing-a-release)
  
If you are a developer or plan on opening the project with Unity, 
please read the following sections:
- [First time setup](#setting-up-the-project-for-developmentbuilding)
- [Project documentation (folder in project)](./Documentation/)

If you want to work with the project's backend for data collection, please
see the following \[work in progress\]:
- Data collection (section in documentation)
- Backend server (maybe another repository)

If you want general information about the project, please see the following:
- Project website (external site)
- [About this project](#about-this-project)

# About this project
This project is a VR game desinged to raise environmental awareness, specifically for
the Florida Manatee and some of the issues they face.

In this game, you become a manatee! Learn about the effects of pollution and what
indiviuals can do to help out. See firsthand how manatees live and how they are
impacted by humans in this short educational game! 


### Background (sources needed)
Manatees are large marine mammals (often called Sea Cows) that are a key part of
Florida's marine ecosystems. The primary issues they face are caused by humans, so
by raising awareness for these issues and what can be done to help, we hope to
work towards minimizing the harm manatees face.

In this simulation, we explore manatee starvation that results from a chain reaction
that starts with human pollution. When phosphates and nitrates (chemicals frequently
found in soaps, detergents, and fertilizers) make their way to manatee habitats
(via nonpoint-source pollution such as runoff from rain), they promote the rapid growth
of algae. This creates algae blooms that prevent sunlight from reaching the ground where
seagrass, manatees' main source of food, grows. This process is called **cultural eutrophication**, and it leads to manatees struggling to find food, becoming malnourished,
and starving.

This issue has been especially dangerous at the Indian River Lagoon, where manatees
tend to group together during the winter time for the Lagoon's usual warm waters
and plentiful seagrass. If we raise enough awareness and collectively take action
to reduce the amount of pollution we produce, we can help restore the Indian River
Lagoon's previous status as a wonderland for manatees!

### Credits
This project would not be possible without all of the people who have contributed (internally: feel free to change titles/roles if you want and add anyone I missed):
- Dr. Tania Roy - Project Leader - Assistant Professor of Human Centered Computing
- Dr. Athena Rycyk - Manatee Expert - Assistant Professor of Biology and Marine Science
- Riley Wood - Simulation Developer
- Alex Wills - Simulation Developer
- Alexis Merker - Marine Biologist
- Devin Gregg - 3D Modeler
- Ender Fluegge - Backend Developer
- Sami Cemek - Original Simulation Developer
- The Environmental Discovery Awards Program (EDAP) and New College of Florida (NCF) - Funding/Support

This project builds off of [**Twizzlers Manatee Simulation**](https://sites.google.com/ncf.edu/edap2022/home), a similar project built to raise awareness for high speed boat
accidents with manatees, another significant human-caused threat to manatees.

# Installing a release
This section explains how to install this simulation to the Meta Quest 2 headset without
setting up Unity and building to the headset from there. These instructions are
adapted from [Meta's guide on setting up the development environment to install APK files](https://developer.oculus.com/documentation/unity/unity-env-device-setup/).

Requirements:
- Meta Quest 2 Headset
- Development account with Meta/Oculus (for enabling developer mode)
- Android Debug Bridge (ADB)
  > You can run `$ adb devices` in a terminal to check if you have ADB set up.
  >
  > ADB comes with the Android SDK. If you have the Android SDK from
  > Android Studio,
  > you can add `/Android/SDK/platform-tools/` (starting from wherever the SDK is installed) to your system's PATH variable, or you can run the terminal
  > from that folder.
  > 
  > If you have the Android SDK & NDK Tools module from the Unity Editor,
  > the platform-tools folder can be found
  > in `/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/`, starting
  > from where your Unity Editor is located (to find this, open the Unity Hub and go
  > to **Preferences** > **Installs** > **Installs Location**).
  >
  > If you do not have the Android SDK, you can install the platform tools without
  > the entire SDK [from here](https://developer.android.com/tools/releases/platform-tools).
- (Windows Only) [OEM USB Driver (from Meta)](https://developer.oculus.com/downloads/package/oculus-adb-drivers/)
  > On macOS, this driver is not needed

## Step 1: Enable developer mode on the headset
Official instructions on enabling developer mode [can be found here (Developer Device Setup)](https://developer.oculus.com/documentation/native/android/mobile-device-setup/).

## Step 2: Confirm ADB is working
Detailed official instructions for using ADB [can be found here (ADB with Meta Quest)](https://developer.oculus.com/documentation/native/android/ts-adb/). ADB is part of the
Android SDK, so if you have a version of the SDK, either through Android Studio or through the Unity Editor's 
Android SDK & NDK Tools module, you should have ADB. 

- If you do not have the Android SDK,
  you can [dowload the standalone platform tools from here](https://developer.android.com/tools/releases/platform-tools).
- To run ADB commands, you can either:
  - Open a terminal in the folder with the `adb` executable (`/Android/SDK/platform-tools/`)
  - Add the folder with the `adb` executable to your system's PATH variable, and open
    a terminal anywhere on your computer
  > Examples of where to find the folder with the `adb` executable file can be found in 
  > the [Requirements section](#installing-a-release).

If your terminal recognizes `$ adb help` as a command, ADB is working.

If you are on Windows, you will need to install the [OEM USB Driver (from Meta)](https://developer.oculus.com/downloads/package/oculus-adb-drivers/) if you haven't already.

With your headset connected to your computer (after clicking "yes" to trust the 
computer from within the headset), run `$ adb devices`. You should get a list that contains
the headset's ID. If there are no devices in the list, verify that the headset is
connected and developer mode is enabled.

Once you are able to run `$ adb devices` and see the headset's ID, you are ready to
install the APK.

## Step 3: Download the APK from this repository
In the **Releases** section of the repository, download the latest release's `.apk` file.

## Step 4: Install the APK to the headset
For this step, you need the path of the `.apk` file and a terminal that can run ADB commands.

Running `$ abd devices` should list one device, which should be the headset.
  
Example:
  ```
    D:\> adb devices
    List of devices attached
        ce0551e7                device
  ```

To install the APK, run `$ abd install -r <path-of-the-.apk-file>`
> `-r` overwrites any APK with the same name, if you already installed a different
> version of this project.

If everything works, you should see a `Success` message.

## Step 5: Running the application
The application is now on the headset, but it is from an "Unrecognized Source".

On the headset, go to the **App Library**.

At the top of the library, click on **Search Apps** to go to a list form of the library.

At the top of this screen, click the dropdown that says **All**.

Select **Unknown Sources**.

Choose this app, **Dumpling Manatee Simulation**.

## Troubleshooting
For help with ADB, please see [Meta's section on troubleshooting ADB](https://developer.oculus.com/documentation/unity/unity-env-device-setup/#adb-troubleshooting)
or [Meta's page on setting up the development environment](https://developer.oculus.com/documentation/unity/unity-env-device-setup/).

# Setting up the project for development/building
This section explains how to set up the project in Unity from the code in this
repository. Since this repository currently ignores the very large Oculus folder,
you must download the Oculus Integration Package before the project cloned from
this repository will be functional.

>Total estimated time: 30+ minutes (lots of waiting)



In order to work with this project, you will need:
- [Unity 2021.3.23f1](https://unity.com/releases/editor/qa/lts-releases?version=2021.3)
  - Android Build Support module
  - Android SDK & NDK Tools module
  - OpenJDK module
- [Oculus Integration Package v50.0](https://developer.oculus.com/downloads/package/unity-integration/50.0)
    > This must be downloaded from Meta's website. [Instructions are below](#step-1-download-the-oculus-integration-package).


Setting up the Oculus Integration Package will take a substantial amount of time, but you will only have to set it up once after cloning the repository.

This setup is adapted from [the official getting-started guide from Meta](https://developer.oculus.com/documentation/unity/unity-tutorial-hello-vr/) (some steps on the official guide can be skipped, since they are saved in the repository as
project settings).


## Why do I need to download the Oculus Integration Package?
The entire Oculus Integration Package takes up 875 MB when imported into the project,
and 500 MB if left in its compressed `.unitypackage` form.

In the future, it would be best to keep only the necessary files from the Oculus Integration Package
tracked in this repository, but for development we have the entire package in our project. 



## Step 1: Download the Oculus Integration Package
>Estimated time: 5-15 minutes (large download)

This project uses v50.0 of the Oculus Integration Package for Unity, which is available
on Meta's website.


The official download can be obtained from [this link to Meta's download](https://developer.oculus.com/downloads/package/unity-integration/50.0).

Make sure that **Version 50.0** is selected in the dropdown before clicking **Download**.

The file should be a `.unitypackage` file that is around 500 MB. 
Save this file somewhere you can easily access later.


## Step 2: Open the project
>Estimated time: 5 minutes for first time

With Unity 2021.3.23f1, open the `./Dumpling Manatee Simulation/` project folder.

**There will be many errors in the project when it is opened, but this is okay.**

Do not enter the project in safe mode (or, if you do, exit safe mode). These errors occur because the project is missing the Oculus Integration Package, which
will be added in the next step (assets cannot be imported in safe mode).


## Step 3: Import the Oculus Integration Package
>Estimated time: 5-10 minutes

In the editor, go to **Assets > Import Package > Custom Package**.

Then locate and select `OculusIntegration_v50.0.unitypackage` from where you saved it earlier.

After it loads, a window will allow you to choose what parts of the package to import. In the future, we may want to optimize the project by excluding some of the package. For now, we should just import all.

From the Oculus Integration Package, this project uses the following parts of the package:
- VR
- SampleFramework

When you are prompted to update the Oculus Utilities plugin, click **Yes**. Then it will ask you to restart Unity, so click **Restart**.

If you are prompted to use the OpenXR backend, click **Yes**.

When prompted to clean up assets, click **Show Assets** and then **Clean Up**, then **Clean Up Package**.

When prompted to update the Spatializer plugins, click **Upgrade**. This will prompt another restart, so click **Restart**.

## Step 4: Configuring the settings
>Estimated time: 1 minute

Most, if not all, of the project settings should already be set up when you clone the repository. There are only a few changes to make that come from the Oculus Integration Package.

- Go to **Edit > Project Settings**. 
- In the left sidebar, click **Oculus**. 
- Under **Outstanding Issues**, click **Fix All**. 
- Under **Recommended Items**, click **Apply All**.

Now there should be no major errors, and the project should be setup correctly for
development.

## Building to the headset
To build the project to the headset:
1. Click on **File > Build Settings**
2. Under *Platform*, click on **Android**, and click **Switch Platform** if necessary
3. Next to *Run Device*, click **Refresh** and select your headset from the dropdown
   > At this step, your headset should be in developer mode and connected to your computer.
   > For official instructions, [read this guide from Meta](https://developer.oculus.com/documentation/native/android/mobile-device-setup/).
4. Click **Build and Run**
5. Choose a location to save the `.apk` file
- When the build is complete, it will start running on the headset.

Alternatively, for a faster build, you may be able to use **OVR Build APK**

1. Click on **Oculus > OVR Build APK**
2. Choose a location for *Built APK Path*
3. Check *Install & Run on Device?*
4. Click **Refresh** to ensure your headset is found
5. Click **Build**

# Other
Please make sure that you are always developing on a branch that is not `main`, then make a pull request to `main` when it is working. Afterwards, delete the branch you made, and create a new branch for the next feature.

I will update this README in the future with information on the tools for making development and building faster, but for now, that information can be found in [Meta's documentation here](https://developer.oculus.com/documentation/unity/unity-build/).

For development, it is recommended to use OVR Quick Scene Preview, and for full builds it is recommended to use OVR Build APK.