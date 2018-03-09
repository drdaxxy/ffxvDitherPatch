# ffxvDitherPatch

This utility allows you to narrow that nasty terrain blending (dithering) effect in *FINAL FANTASY XV*

![sbsbig](https://user-images.githubusercontent.com/40321/37212845-9cafc756-23b0-11e8-8a1d-502bdf80130e.png)

![sidebyside](https://user-images.githubusercontent.com/40321/37212892-bb56e554-23b0-11e8-8a16-95f29127eda0.png)

Full screenshots: [Original (16x)](http://i.cubeupload.com/ekOLq6.png) vs [Medium (40x)](http://i.cubeupload.com/qMc1GA.png)

Screenshots taken with anti-aliasing disabled for clarity. This is still beneficial with temporal AA on.

---

### Release history

* **1.0** *(March 9, 2018)*
  * Initial release

### Compatibility

*ffxvDitherPatch* has been tested with the benchmark, Steam demo, Origin demo, and initial Steam full version.

**Windows Store versions are currently not supported** [for the reasons outlined in this post](https://www.reddit.com/r/FFXV/comments/824sk3/modding_on_windows_store_version_may_be/).

### Usage

This program **requires .NET Framework 4.6.2**. You likely already have it installed, but [download and install](https://www.microsoft.com/net/download/dotnet-framework-runtime/net462) it if you're having trouble running the program.

* **[Download](https://github.com/drdaxxy/ffxvDitherPatch/releases/latest)** the current version
  * As a user, you do not need the source code.
* **Extract** the archive to your game installation folder
  * **Steam**: right click the game in your library, click *Properties*, *Local Files*, *Browse Local Files...* to find it
* **Make sure** your game is closed before continuing
* **Run** `ffxvDitherPatch.exe`, wait for it to fully load, select one of the options, click *Process*
* **Wait** for the process to finish ("Status: Done")
* **Close** the program

You can delete *ffxvDitherPatch.exe* after this, but **you will likely have to reapply the patch after game updates**, so you might want to keep it around.

### Changing settings / Undoing patch

To undo the patch, run `ffxvDitherPatch.exe` in the game folder again. It will prompt you if you want to restore a backup.

After restoring a backup, you can apply the patch again with a different option.

You can also manually restore the backup by replacing `datas/shader/shadergen/autoexternal.earc` with `datas/shader/shadergen/autoexternal.preDitherPatch.earc`.

Note that if after you apply the patch, a game update replaces the shaders, the backup will be of the old version. Therefore, **do not restore** the backup if `ffxvDitherPatch.exe` simply doesn't prompt you to and the game shows no signs of the patch being applied anymore.

---

#### Sharing

If you'd like to share this mod with other people, please link directly to this page, not the Releases page (so people see this Readme first), and please do not mirror releases so there's always exactly one place to get the latest version.

#### Developer information

This repository contains the following projects:

* *ffxvDitherPatch*, the GUI tool for applying this patch
* *DXBCChecksum*, a C++/CLI version of [AMD's code](https://github.com/GPUOpen-Tools/common-src-ShaderUtils/blob/master/DX10/DXBCChecksum.cpp) for calculating DXBC file checksums (a slight modification of MD5)
* *ChecksumFix*, a commandline tool for patching the correct checksum into a DXBC binary. Useful for testing out or disassembling (`fxc.exe /Fc`) hand-modified shader bytecode.
* *Craf*, my (somewhat application-specific) implementation of the .earc file format
* *CrappyCrafCrafter*, a basic commandline tool for unpacking and repacking .earc archives using the above

While the license allows you to do so, I'd like to ask that you do not distribute *CrappyCrafCrafter* binaries because it's crappy and I'm aware of other people working on (hopefully better) tools.