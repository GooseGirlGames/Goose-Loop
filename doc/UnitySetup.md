# Unity Setup (Linux)

## Installing Unity Hub

First, install `unityhub`
([AppImage for any distro][hubappimg],
[AUR for Arch][hubaur])

Sign into Unity Hub, then
under *Settings > License Management*,
add your serial key (can be found [here][key]).

Currently, _Steve's Job_ is built on Unity version
**2020.3.22f1**.
You must install this specific version to work with the project.

## Installing Unity Editor...

Note:
If you wish to use VS Code as an editor, check [this guide][vscodeguide]
to properly set everything up after the Unity Editor is installed.

### ...the easy way

Clone theis repo. In Unity Hub, under *Projects* click *Add* and select
the `G-Loop` directory.  Unity Hub will inform you that the correct
version of the Editor is not installed and offer to let you install it.

That's it!  Have fun messing around with Steve's Job!

If you are unable to add the project, continue reading:

### ...the slightly more convoluted way

To workaround the issue you'll need to create a project before
attempting to add the existing `steves-job` project.

To create a project, you must install the Unity Editor.
You could install any version, create a new project and then add
`steves-job` as described above.  Then, you'll also get a prompt to
install the correct version.  You might wish to install the correct
version straightaway though:

The usual way of installing the Unity Editor is under the
*Installs* tab.
If the correct version is not available, head to the
[download archive][archive] to find the correct version.

If clicking the green *Unity Hub* button does not launch Unity Hub with
an install prompt for you:
Right-click the green *Unity Hub* button and copy the URL.
Launch Unity Hub from command line with the URL as a parameter, like:
```
./UnityHub.AppImage unityhub://2020.3.6f1/338bb68529b2
```
if using the AppImage, or, if Unity Hub is installed from AUR:
```
unityhub unityhub://2020.3.6f1/338bb68529b2
```

The correct version should now install.  You might still be unable to
add the project.  In that case, create a new project with the Unity
version you just installed and let the Editor open that project.
Close the Editor (perhaps restart Unity Hub too), and now you should be
able to add the existing project.

### ...if Unity Hub just absolutely hates you

If Unity Hub throws a "Not enough space" error during installation,
it's most likely due to Unity Hub not liking your `/tmp` directory.

To work around this, create a directory on your hard drive and use
that for installation by passing Unity Hub an environment variable:
```
mkdir ~/cool_directory
TEMP=$HOME/cool_directory unityhub
```
Note that the environment variable only needs to be passed for
installation.  After that, you can launch Unity Hub without it.

### some more useful hints:

If Unity Editor just keeps on crashing, esp. when opening Package
Manager: If you are on Intel graphics, revert Mesa from 21 to 20, e.g.
like this:
```
pacman -U /var/cache/pacman/pkg/mesa-20.3.4-3-x86_64.pkg.tar.zst /var/cache/pacman/pkg/lib32-mesa-20.3.4-3-x86_64.pkg.tar.zst
```

[hubaur]: https://aur.archlinux.org/packages/unityhub/
[hubappimg]: https://docs.unity3d.com/Manual/GettingStartedInstallingHub.html
[key]: https://id.unity.com/en/subscriptions
[archive]: https://unity3d.com/get-unity/download/archive
[vscodeguide]: https://gist.github.com/jakobbbb/a15d2505a37ca632601d147fd5d91836
