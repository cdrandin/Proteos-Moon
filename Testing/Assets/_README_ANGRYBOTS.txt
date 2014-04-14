For PUN instructions see the PUN readme at: "Photon Unity Networking/readme.txt"

The Unity Asset Store does not properly save build settings (the selected scenes) therefor you will have to correct these.

To get this demo working:

1) Create a fresh project and download the package from the asset store.
2) Add the following scenes to your buildsettings:
	Scenes/0_Preloader
	Scenes/1_MainMenu
	Scenes/2_AngryBotsMP
	Scenes/3_Endscene
3) Setup PUN (see the PUN wizard or the readme). You can either host your own server or use the free Photon Cloud trial.
4) Run the preloader or mainmenu.