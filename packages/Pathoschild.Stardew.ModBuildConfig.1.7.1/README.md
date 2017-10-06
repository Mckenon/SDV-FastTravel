**Stardew.ModBuildConfig** is an open-source NuGet package which automates the build configuration
for [Stardew Valley](http://stardewvalley.net/) [SMAPI](https://github.com/Pathoschild/SMAPI) mods.

The package...

* lets you write your mod once, and compile it on any computer. It detects the current platform
  (Linux, Mac, or Windows) and game install path, and injects the right references automatically.
* configures Visual Studio so you can debug into the mod code when the game is running (_Windows
  only_).
* packages the mod automatically into the game's mod folder when you build the code (_optional_).

## Contents
* [Install](#install)
* [Simplify mod development](#simplify-mod-development)
* [Troubleshoot](#troubleshoot)
* [Versions](#versions)

## Install
**When creating a new mod:**

1. Create an empty library project.
2. Reference the [`Pathoschild.Stardew.ModBuildConfig` NuGet package](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig).
3. [Write your code](http://canimod.com/guides/creating-a-smapi-mod).
4. Compile on any platform.

**When migrating an existing mod:**

1. Remove any project references to `Microsoft.Xna.*`, `MonoGame`, Stardew Valley,
   `StardewModdingAPI`, and `xTile`.
2. Reference the [`Pathoschild.Stardew.ModBuildConfig` NuGet package](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig).
3. Compile on any platform.

## Simplify mod development
### Package your mod into the game folder automatically
You can copy your mod files into the `Mods` folder automatically each time you build, so you don't
need to do it manually:

1. Edit your mod's `.csproj` file.
2. Add this block above the first `</PropertyGroup>` line:

     ```xml
     <DeployModFolderName>$(MSBuildProjectName)</DeployModFolderName>
     ```

That's it! Each time you build, the files in `<game path>\Mods\<mod name>` will be updated with
your `manifest.json`, build output, and any `i18n` files.

Notes:
* To add custom files, just [add them to the build output](https://stackoverflow.com/a/10828462/262123).
* To customise the folder name, just replace `$(MSBuildProjectName)` with the folder name you want.
* If your project references another mod, make sure the reference is [_not_ marked 'copy local'](https://msdn.microsoft.com/en-us/library/t1zz5y8c(v=vs.100).aspx).

### Debug into the mod code (Windows-only)
Stepping into your mod code when the game is running is straightforward, since this package injects
the configuration automatically. To do it:

1. [Package your mod into the game folder automatically](#package-your-mod-into-the-game-folder-automatically).
2. Launch the project with debugging in Visual Studio or MonoDevelop.

This will deploy your mod files into the game folder, launch SMAPI, and attach a debugger
automatically. Now you can step through your code, set breakpoints, etc.

### Create release zips automatically (Windows-only)
You can create the mod package automatically when you build:

1. Edit your mod's `.csproj` file.
2. Add this block above the first `</PropertyGroup>` line:

     ```xml
     <DeployModZipTo>$(SolutionDir)\_releases</DeployModZipTo>
     ```

That's it! Each time you build, the mod files will be zipped into `_releases\<mod name>.zip`. (You
can change the value to save the zips somewhere else.)

## Troubleshoot
### "Failed to find the game install path"
That error means the package couldn't figure out where the game is installed. You need to specify
the game location yourself. There's two ways to do that:

* **Option 1: set the path globally.**  
  _This will apply to every project that uses version 1.5+ of package._

  1. Get the full folder path containing the Stardew Valley executable.
  2. Create this file path:
  
     platform  | path
     --------- | ----
     Linux/Mac | `~/stardewvalley.targets`
     Windows   | `%USERPROFILE%\stardewvalley.targets`

  3. Save the file with this content:

     ```xml
     <Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
        <PropertyGroup>
          <GamePath>PATH_HERE</GamePath>
        </PropertyGroup>
     </Project>
     ```

  4. Replace `PATH_HERE` with your custom game install path.

* **Option 2: set the path in the project file.**  
  _(You'll need to do it for every project that uses the package.)_
  1. Get the folder path containing the Stardew Valley `.exe` file.
  2. Add this to your `.csproj` file under the `<Project` line:

     ```xml
     <PropertyGroup>
       <GamePath>PATH_HERE</GamePath>
     </PropertyGroup>
     ```

  3. Replace `PATH_HERE` with your custom game install path.

The configuration will check your custom path first, then fall back to the default paths (so it'll
still compile on a different computer).

## Versions
See [release notes](release-notes.md).
