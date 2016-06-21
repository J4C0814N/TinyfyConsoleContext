# TinyfyConsoleContext
Windows console app tied to a context menu item to tinify images from within Windows Explorer

Simple Windows Console app that uses the [TinyPNG API](https://tinypng.com/developers) to compress images from a Windows Explorer context menu item.
Install:

1. Add your API Key to the `App.config`
2. Build/deploy the `.exe` to the directory you would like application installed to. e.g. `C:\Program Files\TinyfyConsoleContext\TinyfyConsoleContext.exe`
2. Run the `.exe` with the `install` argument as an Administrator. e.g. `C:\Program Files\TinyfyConsoleContext\TinyfyConsoleContext.exe install`
3. Ensure you get an install success message, otherwise debug any errors and install again.

Once installed Simply:

1. Right-click an image or image selection
2. Select the Tinify context menu item
3. An [ImageName]-Tinified.[ext] image will be created in the same directory

TODO:

1. Verify input is an image
2. Optimise code
3. ~~Show remaining API Calls count~~


