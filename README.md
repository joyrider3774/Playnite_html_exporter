# Playnite html exporter
Export your [Playnite](https://www.playnite.link/) games library to (static) html using templates

[Latest Release](https://github.com/joyrider3774/Playnite_html_exporter/releases/latest)

## Release / Help Video

### V1.0
[![Playnite html exporter V1.0](http://img.youtube.com/vi/KR2R6ZWxbgM/0.jpg)](https://youtu.be/KR2R6ZWxbgM "Playnite html exporter V1.0")

### V2.0
[![Playnite html exporter V2.0](http://img.youtube.com/vi/6WqE6oj33Fo/0.jpg)](https://youtu.be/6WqE6oj33Fo "Playnite html exporter V2.0")

### V3.0
[![Playnite html exporter V2.0](http://img.youtube.com/vi/xdtpItr2iC4/0.jpg)](https://youtu.be/xdtpItr2iC4 "Playnite html exporter V3.0")

## Export Speed
The very first time when you export with images enabled it will take a longer time than the second time you will export. The very first time it has copy all images while the next times it only has to copy new images. Also generating the very first page takes longer than all other pages as the very first page also generates all detail pages of games. Also having playnite installed on a SSD and exporting to an SSD folder will be a lot faster than having playnite installed and exporting on platter disk or even worse an SMR platter disk. 

Here are some statistics from my pc where i have about 2250 games in my library all containing images for every game when exporting with the default grid template and copying images:
* 1st export on (non SMR) platter disk: 3 minutes 9 seconds
* 2nd export on (non SMR) platter disk: 1 minute 15 seconds
* 1st export on ssd disk: 50 seconds
* 2nd export on ssd disk: 23 seconds

## Export Size
Export size highly depends on library size (nr of games), and when exporting images, images resolution and the amount of games having all images (icons, cover, background). In my playnite all game images are optimzed according to playnite (not one game shows the optimazation warning for images). Here are the sizes for my current real export for about 2250 games:
* Grid + Images: 2,14 GB
* List + Images: 2,17 GB
* List Text only (no images): 113 MB

As of version 2.0 of html exporter you can now reduce the size of the export by converting icon files to png and cover / background images to jpeg as well as resize images. Here are some results of my real export of about 2600 games:

* Grid Copy only: 2,33 GB 
* Grid Resize + Force 100% Jpeg Quality: 1,20 GB
* Grid Resize + Force 85% Jpeg Quality: 554 MB
* Grid Resize + Convert + Force 100% Jpeg Quality: 1,17 GB
* **Grid Resize + Convert + Force 85% Jpeg Quality: 469 MB**
* Grid Convert 100% Jpeg Quality: 2,28 GB
* Grid Convert 85% Jpeg Quality: 2,18 GB
* Grid Convert + Force 100% Jpeg Quality: 1,92 GB
* Grid Convert + Force 85% Jpeg Quality: 895 MB

## Templates
I have provided 3 templates myself a GRID based view, a LIST View and a TEXT Only version. In theory you can create your own templates but to describe this i need to provide a lot of documentation on how todo this and i will first see if there is any need for this.

## Hosting the files
The text only version can easily be used to share your games library with friends or to host this on a website. If you use the images version you either must self host the html files using your own webserver (for example using [Wamp](https://www.wampserver.com/en/) on windows) and some dynamic dns provider (for example usin [NO-IP](https://www.noip.com/)). Another option is to upload your files to Onedrive or google drive and use https://drv.tw/ to make a website out of your shared files on those cloud services (this will be slower).

## Translation
The project is translatable on [Crowdin](https://crowdin.com/project/playnite-game-speak)

#### People who contributed to translations:
joyrider3774, slippy, MeatBoy, voi__Wood, JCraftPlay, zolini, pedroazevedo1999, atemporal_, Federico Pezzuolo 


## Credits
* Used Icon made by [Smartline](https://www.flaticon.com/authors/smartline) from [Flaticon](https://www.flaticon.com/)
* Original Localization file loader by [Lacro59](https://github.com/Lacro59)

## Donations
[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://paypal.me/joyrider3774)
