# GentleTouch

Gentle Touch is a Dalamud plugin adding functionality for the Buttplug.io framework.
Buttplug.io is not only limited to Buttplugs but it will propably support any remote controllable vibrator device that exists.
Its windows only yet. (You could make some simple edits to Connection.cs to make it run on your non-windows system)

## How To Use

### Getting Started

After starting the plugin it will automatically connect either to the Intiface(R) Central app (If available)
or it will start the service by itself when not available yet.

Any device in reach will be automatically detected.
You can see the list of detected devices in the main window (/gentle)
If your device doesnt show up in the list after 1-2 seconds:
- Make sure your device is in reach and you dont block the signal somehow (Having it covered up by ur leg or like that)
If this isnt the case then the options divert now on what service you are using:

#### Intiface(R) Central App
- Open the app and go to settings.
- Here you now select what kind of connection your device uses.
- After selecting the correct protocols you can start the server again and it should detect your device.

#### Only using the plugin
- Open the Gentle Touch Configuration (/gentleconf or /gentlec)
- Select the protocols that your device is using
- Click Save and it will restart the plugin service with the new protocols
- It now should find your device

### Functions

1. You can let any device listed run a test vibration. This test will make the device vibrate for 5 seconds at 50% intensity.
2. When you land a critical or direct hit the device will start vibrating.

I maybe plan to add more functionality for more immersive ERP and others.
To name some:
1. Detecting selected characters motion changes to transfer the motion that the character did / expierinced to your device.
2. Customizable Events with extended triggers, like not dropping any GCD, running a good rotation and so on.
3. Parsing chat commands so other players can controll ur device with a secret trigger word you choose
4. Adding pattern functionality

### Protocols

1. Bluetooth LE
	Propably the most common protocol. 
	Lovens devices (Lush and Nora) should find your device with this.
	Requires you having either a build-in Bluetooth feature or a Bluetooth dongle.
2. Lovense Dongle
	The official dongle from their store.
	It has a slightly different protocol than common bluetooth dongles, thou its based on the same technology.
	It should work with all Lovense products that have the compatibility listed on their product page.
3. Serial Port
	Honestly im not sure why it exists but i guess some devices are wired and this is for them?
4. XInput
	Well if you know if your device needs this or not then you know more than me.

### Building

This section is only relevant if you want to compile the Plugin yourself.
Its not relevant for you if you just want to use the Plugin or dont know how to programm.

The plugin itself is simply build using any IDE with c# capacity.

The Buttplug.io framework requires the use of their server. 
Wich is run localy on your pc, it wont need any acc creation nor will it permanently store any information from you.
You can either use their official server called Intiface(R) Central or use the embeeded feature.
To use embeeded you have to build the server from their server files, its written in Rust. (I did this allready and shipping the .exe with the plugin)
For any case that you want to build it yourself (Im to slow updating, you just dont trust me, and so on...)
and if you are not familiar with Rust then heres a simple step by step guide to build a Rust project:
1. Download and Install Rust from their official website.
2. Clone the Intiface Engine GitHub
3. (Optional, for debuging) Download VS Code (You maybe can use Visual Studio allready but im not sure)
3. b. Open VS Code get the Rust Analyzer Extension
3. c. Get also any compiler of your choise that supports Rust.
3. d. Open the Project with VS Code (Open the directory of the project) and VS Code will ask you if they should configure the Rust project for you; say yes.
4. Open the Terminal (Terminal shortcut in VSCode: "CTRL+Shift+`")
5. Run the Rust build command: "cargo build"
It now should have created ur .exe file.
To validate it, run it from console with the argument "--version".
