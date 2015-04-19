%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
::TODO

[] background!
[] virtuální nos
http://www.engadget.com/2015/03/24/virtual-nose-could-reduce-simulator-sickness/?ncid=rss_truncated

[] použít pøímo èoèky na kamerách
a SharpDX.RawInput - nejrychlejší asi?
[] nìco o tìch kamerách
[] udìlat to jakože VR a že si nasadíš brýle
-> a uvidíš kamerama.. mùžeš se hýbat a vybrat si jiné brýle :OOOOOOOO
[] robotické rameno - 3d tisklé
[] databázi rozmìrù hlavy od lidí (aspoò 100)
a pak programek pro nastavení prumìrných hodnot
- podle vzrùstu, pohlaví, stáøí, zemì pùvodu?, váhy
[] udìlat taky headset position tracking - s kamerou. !!! ooooh goood
[] virtual desktop
[] ovìøit kecy o kortexu u nìjakyho medika
[] pohled vzhuru nohama - upside down glasses
http://www.cns.nyu.edu/~nava/courses/psych_and_brain/pdfs/Stratton_1896.pdf
http://en.wikipedia.org/wiki/Perceptual_adaptation
http://www.madsci.org/posts/archives/mar97/858984531.Ns.r.html
sensory and perception textbook - http://sites.sinauer.com/wolfe3e/chap1/sensoryareasF.htm
- optické klamy - perception of vision - teèky mizí okolo
 George Stratton 
http://en.wikipedia.org/wiki/Perceptual_adaptation
[] mozek to prohazuje
http://www.scienceiq.com/Facts/BrainFlips.cfm
- pinhole experiment
[] nystagmus - pøebíhání oèí - jerk-jolting zornièek
http://en.wikipedia.org/wiki/Nystagmus
[] microsaccades
aj dyž sem zaostøený tak kmitám - simulovat v helmì?
random in direction and about1 to 2 minutes of arc in amplitude. 
-> vymaže se dyž upe stojí
[] vidíme distortion pøi stoupajícím teplém vzduchu, ale pouze pokud máme hlavu v klidu.. jinak nic nevidíme - zmìny jsou pøíliš malé
[] simulovat oèní vady

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
::WRITE
[] XML - Extensible Markup Language
vs XAML - Extensible Application Markup Language - .NET 3.0, 4.0
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
::URLS
[] SDK
q:\DEV\OculusSDK_0_4_3
[x] main gdoc
http://goo.gl/XLESGc 
[x] latex school
http://latex.feec.vutbr.cz/
[x] main directory
Q:\__DIP\ = #!b
[x] IS diplo
https://www.vutbr.cz/studis/student.phtml?script_name=zav_prace&operation=detail&zav_prace_id=81726&vedouci_id=0&tematicky_okruh_id=0&nazev_prace=
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
:: game
[] vr dir cat
https://forums.oculus.com/viewtopic.php?f=42&t=18822
[] some more demos
vrwear.com ?
http://stv.re/category/app/game/
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
::DEV
[] oculus SDK 4.2  [ 2014_10_18 ]
docu:
https://developer.oculusvr.com/?action=doc
manual
http://static.oculusvr.com/sdk-downloads/documents/Oculus_Rift_Development_Kit_Instruction_Manual.pdf
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
:: HW
[] vyvážení kamer na robotickým rameni
[] 
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
[] tf2
There is only one way for me to get TF2 working with the Rift.
.) Make sure the Rift Service is running.
.) Make sure the Rift is in Extended Mode.
.) Make sure SteamVR is installed.
.) Open Steam
.) Set the Rift to your primary Display
.) Open Virtual Reality Mode (BETA) - (In Steam under "View")
.) Use the Rift to navigate through your Steam library in VRM and start TF2 in VR Mode
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
:: install
[x] visual studio 2012
- update 
[ne] vs 2013
nejde na win7
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
:: BIBLIO
[] Virtuality Continuum's State of the Art ?
http://www.sciencedirect.com/science/article/pii/S1877050913012374
____________________________________________________
::take
[] vision - neural paths
http://hubel.med.harvard.edu/book/b15.htm
[] OculusBestPractices.pdf
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
:: TeXstudio
[] mluvnice
nástroj pro jazyk?
C:\PROG\office\TeXstudio\dictionaries
[x] dictionary!!
[] odkazy biblio
[x] url
% \url{http:\\www.w.com}
[x] vkládání tabulek - aby byli odkazovatelny
% \label{} a \ref{}
[x] zkratky
% \zk{name} = \zkratka{name} = název zkratky
% \zkratkatext{name} = text zkratky
____________________________________________________
:: latex
[] ??
$ \hhline{~~~--} 
[] stupeò
$^\circ$
[] ovr unity integration
http://www.academia.edu/5169019/Oculus_VR_Inc_Oculus_Unity_Integration_Guide
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
::DONE
[x] 2014_11_21
osobní nastavení oculu
height = 180 -13 cm = 167cm
IPD = Inter Pupulary distance 
Eye to neck distance = E2N 
Eye Relief
[IPD, E2N-horiz, E2n-verti, ER] mm
measured /def = [ 72, 100.5, 120, 14 ] mm 
empiricky & measured = [ 69.3, 90.5, 100, 11 ] mm (LO 453,LI 19,RI 36, RO 471)
____________________________________________________
[] settings
Rift display mode
- [x] Direct HMD [x] DK1 Legacy App Support
 -- extended display - or mirroring = good colors
 - a funguje toscany demo directtorift 
 --> to bylo oboji bez optimusu
- [x] Direct HMD [ ] DK1 Legacy App Support
 -- small preview on main = switched colors RGB -> BGR
 - ale je to doporuèený tak co dpc
 --> to bylo oboji bez optimusu
[x] toscany easteregg
c v f t - zasekne display
7 8 - x speed
9 0 - x mouse speed
f11 - fullscreen
[] cuda VS2012 settings
http://ramblingsofagamedevstudent.blogspot.co.uk/search/label/Other%20Work
____________________________________________________
[x] rozchodit Oc SDK
[x] vybrat kamery
- objednat
- vybral a objednal za mì
[x] vybrat serva
- objednat
- vybral a objednal za mì
[x] proèíst parametry Oculusu - zapsat
[x] C# kniha
____________________________________________________
:: nuget
[x] vs2012 nuget installed
[x] moving package files
Install-Package NuGetPackageFolderOverride
[x] takže abych mìl jednu složku s packagema pro více projektù
As the note says - There may be dragons. You have been warned!
http://www.gr4viton.cz/2015/01/nuget-vs2012-multiple-solutions-project-with-only-one-package-folder/
____________________________________________________
:: SharpOVR
[x] install
https://www.nuget.org/packages/SharpOVR/
[x] first hint
https://forums.oculus.com/viewtopic.php?f=20&t=9044&p=120544&hilit=SharpOVR#p120544
1.35x performance penalty compared to native C++
[x] thread
https://forums.oculus.com/viewtopic.php?f=20&t=8464&hilit=Sharpovr+sample
[x] sample 
http://1drv.ms/1hDMLvs
=
https://onedrive.live.com/?cid=8ae5a4e24a2f8960&id=8AE5A4E24A2F8960%21149320&ithint=file,.zip&authkey=!AFOLyLvcnSgLcqk
[x] tools->NuGet packager -> powershell
Install-Package SharpOVR
Install-Package SharpOVR -Version 0.4.2
SharpDX 2.6.0 - se nainstaloval sam..
pak restore - downloading missing packages - ale stejnì už nic nenainstalova
- build run = SAMPLE BÌŽÍÍÍÍ JOOOO
error
--> select .NET Framework 4 Client Profile
[..] sharpovr without sharpdx toolkit 
- only lib dependency
http://riftdev.com/oculus-general-development-re-sharpovr-0-4-2-nuget-update-4/
proj = 2- wiggle
[x] changing to 4.0 client in RiftGame prop.. 
better no errors :O
http://stackoverflow.com/questions/4764978/the-type-or-namespace-name-could-not-be-found
[x] working settings:
duplicate displays, [x] Extend, [x] DK1
[] sharp dx doc
http://sharpdx.org/documentation
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

[] try different wrappers
[] co to je c# wrapper
jak funguje
s514

[] directx programing in c# by tomd123
http://www.codersource.net/2010/02/09/directx-programming-in-c/

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
:: HOWTO :: na vyzkoušení..
[x] add reference
C:\WINDOWS\Microsoft.NET\DirectX for Managed Code\1.0.2902.0\
Microsoft.DirectX.dll
Microsoft.DirectX.Direct3D.dll
- už je..
[x] Project properties -> build -> Target platform - x86     - musím
[x] if using .NETv4.5 => - nemusím = chyba
"Mixed mode assembly is built against version 'v1.1.4322'"
app.config
    <startup useLegacyV2RuntimeActivationPolicy="true">
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5"/>
    </startup>
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
/:: .NET c# wrapper - DIRECTX + C# + OCULUS

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
/:: DIRECTX + C# + OCULUS + VIDEO

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
/:: DIRECTX + C#

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
/:: VIDEO + DIRECTX

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
/:: VIDEO + C# + (DirectX)

____________________________________________________
/:: C# code samples

____________________________________________________
[] good .. aj seeking -> video playback
/B:\DEV\SDK\Windows SDK v6.1\Samples\WPFSamples\GraphicsMM_Media\MediaGallery\csharp
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
/:: VIDEO CAPTURE = C#
____________________________________________________
[] ozeki camera sdk - looks good
http://www.camera-sdk.com/p_261-how-to-implement-frame-capture-from-an-usb-camera-in-c-onvif.html
____________________________________________________
[] 2camera
http://gamedev.stackexchange.com/questions/83515/how-do-i-output-a-webcam-stream-to-a-2d-canvas-with-sharpdx-in-c
____________________________________________________
[] Convert live camera feed to byte = Visual C# -> Desktop -> C# -> search: video
____________________________________________________
[] avi DirectX capture class
http://www.codeproject.com/Articles/3566/DirectX-Capture-Class-Library
____________________________________________________
[] recomended video formats for video rendering
https://msdn.microsoft.com/en-us/library/windows/desktop/dd206750(v=vs.85).aspx
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
/:: KNÍŽKA
[] Introduction to 3D Game Engine Design Using DirectX 9 and C#
s 281 - video clas ->DirectDraw AudioVideo class in Managed DirectX
Video class stream video to screen / texture
https://books.google.cz/books?id=DDFV5UhmbMYC&printsec=frontcover&dq=Introduction+to+3D+Game+Engine+Design+Using+DirectX+9+and+C%23&hl=en&sa=X&ei=EJ3KVJWwCYmrU4CwgPgC&redir_esc=y#v=onepage&q=Introduction%20to%203D%20Game%20Engine%20Design%20Using%20DirectX%209%20and%20C%23&f=false
[] Kick Start
- memory pools
https://books.google.de/books?id=8Y4VrGBtGM8C&pg=PA392&lpg=PA392&dq=directx+video+playback+texture&source=bl&ots=pV0DaWpgm4&sig=tlnicf3RT8TisTmxclEriRBJ1Q0&hl=cs&sa=X&ei=YI3KVKvwC4O_PIuDgYgC&ved=0CEUQ6AEwBA#v=onepage&q=directx%20video%20playback%20texture&f=false
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
[] vylepšení
http://forums.guru3d.com/showthread.php?t=327922
[] camery
2048^2 80Hz
basler ac a2040 - 900c
[]serva mx-64ar
naštudovat co potøebuju
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
[] 1 recherche mobilni robotika
[] 2 rozhyby hlavy - tabulka - a ovìøit 14 dní
rozsahy pohyby ve všech 3
rychlosti
zrychlení
rozlišení 
pøesnosti - jestli mají smysl- opakovaòi
FOV, rozlišení osvìtlení
spousty vìcí - nepodcenit
[] 3 dynyamixel
sou na 14 V takže ATX 
sw nesmysl - knihovnovna kera není celá 
desku rs485 s frantou - esi nema tak napájet
puèený aj studentka takže ne nacelou domu
[] camera
pleora - franta dostal za úkol aby z toho vyèítal co nejrychlej
- rovnou na basler - drivery !
fora
[] x3 ac - na 
harmonic drive na orpheus 3
[] 4 vhodný
50kKè
- obraz s nízkou latencí
[] 5 param
latence toho hw - 2 ruzný vìci
[] text!
že byse dalo použít
____________________________________________________
[] více na rychlost
obhájíme že nekompenzuje
- podle toho jestli se to stihne
latenci
dk 2
- modul latence - mìøení ! !!!!!!!!!!!!!!
- jesi od úrovnì z pamìtì a vymìøí
-- z blesku kameru
____________________________________________________
[] SDK 4.4 ? jestli zapùjèit poèítaè tak vyøíše ??
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
[] vylepšení rychlost
http://forums.guru3d.com/showthread.php?t=327922
____________________________________________________
[] visual .net 4.5.1
https://msdn.microsoft.com/query/dev11.query?appId=Dev11IDEF1&l=EN-US&k=k(VS.ReviewProjectAndSolutionChangesDialog.Upgrade)&rd=true
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
:: požadavky PC
Je to tedy tak, že budu muset poprosit o zapùjèení poèítaèe, jestli se tedy najdou dané komponenty.
Zejména kvùli tomu, že mùj souèasný notebook:
- nemá USB3.0 vstup jenž je nutný pro vstup z kamer
- nepodporuje DirectX 11 a výš (pouze verzi 10.1) 
Nejlépe by bylo kdyby sestava mìla:
[x] CPU - nejlépe dvoujádro 


- Oculus SDK 0.4.4 

Oculus SDK - dropping DX9

____________________________________________________
-> media foundation bìží i na win7
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
[] kde
 google.cz
springerlink
ebsco
IEEE.ORG
primo.vutbr.cz
Springerlink a ScienceDirect 
] lekarske:
http://www.ncbi.nlm.nih.gov/pubmed/990235
wikiskripta
wikipedie
http://theses.cz/


[] dynamixel
] double drivers
] firmware update
http://forums.trossenrobotics.com/showthread.php?6025-Dynamixel-AX-12a-lights-up-and-searches-but-can-t-be-found
[] ping
http://forums.trossenrobotics.com/showthread.php?7302-AX-12A-Ping-issue


[] serial port monitor
http://www.serial-port-monitor.com/


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
/::KNIHY check
910128EMQA
[] oko - 
HRAZDIRA, Ivo a Vojtìch MORNSTEIN. Lékaøská biofyzika a pøístrojová technika. 1. vydání. Brno : Neptun, 2001. ISBN 80-902896-1-4.

[] Studovna pøít. a technických vìd - 6.patro  
[] Elektroretinografie
[] AUTRATA, R. Nauka o zraku. 1. vydání. Brno. 2006. 

[] do 3.3
https://vufind.mzk.cz/Record/MZK01-001437406
Fyziologie oka a vidìní /
Hlavní autor:	Synek, Svatopluk, 1951-
Další autoøi:	Skorkovská, Šárka, 1953-
Jazyk:	Èeština
Vydáno:	Praha : Grada 2014


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
:: TODO NOW
[] Video Capture using OpenCV with C#
http://www.codeproject.com/Articles/722569/Video-Capture-using-OpenCV-with-Csharp
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
:: sharpDX rasterTek examples
-> to run you need to
[x] project - properties - build - Platform Target - x86
[x] readd resource sharpdx and etc libraries!
[x] if it uses Models defined in txt (tut12 is the first) and you are using number digit comma instead of point, you must readd the txts with comma instead of point in float number
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
:: emugCV
[x] howto
http://fewtutorials.bravesites.com/entries/announcements/-level-0-emgucv-installation-guide-for-64-bit-windows-users

[x] install libemgucv
= 32bit 
libemgucv-windows-x86-2.4.0.1717.exe
[x] install   MSVCRT 9.0 SP1 x86 or MSVCRT 9.0 SP1 x64 
= x86
[x] copied dlls
[x] references
[x] setup preferences build for x86
[x] path
C:\Emgu\emgucv-windows-universal-gpu 2.4.9.1847\bin;
C:\Emgu\emgucv-windows-universal-gpu 2.4.9.1847\bin\x64;
C:\Emgu\emgucv-windows-universal-gpu 2.4.9.1847\bin\x86;

[x] Rendering webcam images in the Rift using OpenCV and OpenGL in c++
https://forums.oculus.com/viewtopic.php?f=20&t=15906
https://www.youtube.com/watch?v=cPi3HgSciAw
emguCV -> ale není freee
[x] checkbox for safety warning, I hereby declare
[x] REGISTER_move ORDER_ACTION
[ne] piHalf naopak?
[x] motor sendToAll
- background worker = SMOOTH
[x] pitch and roll angle - wrong! why?
[x] packet reconstructor
[x] convertery motorDataRow namísto refresh funkce ? nebude to pomalejší?
možná ne bo se budou updatovat dycky jenom ty daný a ne všechny v daný èas
[x] resolved - C_Packet holds it all
echo - not sure if the echo 
is only echoLast without returnStatusPacket
or echoLast & returnStatusPacket
[x] serial dle Davida
x] No ak to mas threadsafe, tak pred kazdou read/write dvojicou vycisti buffer pre istotu
x] Dal by som normalne port read do cyklu kolkto dat ocakavas

ne]Nad to hodis try catch na timeoutexception
ne]Ak mas response, metoda vrati true, ak exceptiom false
____________________________________________________v commitech
[x] snižit rychlost getPosition - miò trhany
[x] zvyšit rychlost linky 1M    
[x] 3 mista aby šlo vidìt
bìžící refreshing teho dg bindingu
C_Motor.value vytvoøená k tomu
C_Motor         | public void ACTUALIZE_registerBinding(byte addressByte) | = CONVERTOR z mot.reg do mot.value
C_MotorDataRow  | public void GET_data(e_motorDataType _type) | = CONVERTOR z mot.value do string bindovaneho do dg
[x] C_motor angleGoal = C_VALUE() - nebralo v potaz rozsahy
[x] communication speed 1MHz ?
[x] rewrite list<C_motor> into C_motorControl : List<C_motor> - add Ms.Yaw property

[x] c_log counter
[x] apply angles button
[x] set RTL on start of motor
[x] on register write the echou should come if RTL is 2
[x] motorDataType - obsolete - i can find out about anything from instructionBytes

[x] SPI
vyèítání jenom jedné - blbý
vyšší rychlost read - spomaluje move
vyšší rychlost pøenosu.. asi lepší
echo? nevim
co to posralo? nevim!
discard buffer - probably not
[x] 0 motor ORDER_GET_position cmd
ORDER_GET_POSE() - from all motors at once? nebo postupnì
[ne] log settings - buffer - binding      BooleanToVisibilityConverter 
http://stackoverflow.com/questions/7000819/binding-a-button-visibility-to-bool-value-in-viewmodel - použil IsEnabled
[x] do tut 15 dodìlat vykresleni otaèející se krychle. z tut 7 a z 13tky cosi.. text

[x] audio
[x] geometry primitives
[x] rozdìlit frameworkovì..

[x] naimportovat a rozjet v eyeout RiftGame -started porting

[x] pøepsat to do stylu frameworku sharpdx tutorialu
- font už v tom pojede..
____________________________________________________ hmm?
[] delete logCMD 
[] c_log export
[] lost packets
[] garbage collector for c_packets in queue with big age
[] timeoutExceptionPeriod to counter
____________________________________________________ measuring
http://static.oculus.com/connect/slides/OculusConnect_Mastering_the_SDK.pdf
[] latency testing in DK1

____________________________________________________ polishing
[] strip tabs in about
[] another layer
C_MotorSet
[] uncheck send slider to motors when tp starts
[] vypnout kameru po initu.. aby nesvítila
[] tlaèítko teleprezence ze STOP dyž dam alt+f4
[] Lock  motors to not be able to change i.e. angle limits after settings
[] motor new settings

[] rewrite C_Value as 
C_ValueSpeed : C_ValueWithTime : C_Value
C_ValueAngle : C_ValueWithTime : C_Value
[] c_value.decTostring + overriding on different value types
[] c_value.e_regByteType
[] C_value.addresses low and high
____________________________________________________sound
[] from here http://www.soundjay.com/phone-sounds-4.html
____________________________________________________graphics
[] text hud
[] render to texture
[] original thought of camera
http://rifty-business.blogspot.cz/search?updated-min=2014-01-01T00:00:00-08:00&updated-max=2015-01-01T00:00:00-08:00&max-results=35
____________________________________________________ better telepresence
[] COMPLIANCE_CW_SLOPE - nastavit netrhací
- dynamicky - vìtší punch pøi delších skocích 
- smooth pøi pomalých
[] kontinuální otáèení dokola u yawu 
- nachvilu zapnout endles turn
[] zaseky dyž to pøeletím aby se to nevracelo - singularita
[] 0.2 motor allaround
[] detect DirectX version on creation of DirectX device -> if lower than 11.. msessage box
[] vykreslovat graf - (nezpracovaných èi pøerušených packages) / sekunduv závislosti na èasetimeru vyèítání - do závìru
-> proc úspìšnost = [ sum(statusOK) / (sum(statusOK) - sum(statusBroken) ] v èase
-> v závislosti na tom intervalu vyèítání, na :
SPI.ReadTimeout = 200; SPI.WriteTimeout ; Read Return Time (v MOtoru)
[x] pøedpovìdi dopøedu, podle toho jaky posledni command sem zadal
- aby se nemuselo vyèítat tak èasto?
____________________________________________________ must-have


[] rozchodit vyètení z kamery
[] zaslání sync write pozice a rychlost všem motorùm 
[] menší rychlost když je úhel mezi starou a novou pozicí malý menší

[] pøikurtování 1 motoru na nìjakou desku
[] svìtelná závora - cny70 - 1x + pøidìlat na desku s motorem aby to fachalo

[] square on position of camera pos



[] skysphere

[] compass of oculus 
compass of motors - heading
compass reseting button

[] cursor visibility on exit

[] working sample but inverted colors

[] Angle Limit Error
When Goal Position is written with the value that is not
between CW Angle Limit and CCW Angle Limit
[] packetstart and C_DynAdd bytes rewrite.
struct
C_Dyn.Packet.ReceivingBufferVolume
C_Dyn.InstructionPacket.IdByte.IndexOf
C_Dyn.InstructionPacket.IdByte.SizeOf
C_Dyn.Speed.Value.NoControl
C_Dyn.Postion.Value.Max
C_Dyn.Postion.Value.Min
C_Dyn.Postion.SizeOf
C_Dyn.Postion.Address.First = low
C_Dyn.Postion.Address.First = low
C_Dyn.Postion.Address.Low
C_Dyn.Postion.Address.High
[] cam in new thread loading
[] 0.5 text
[] 1 tile draw sharpDX 
[] 2 tile draw on motor pose
[] 3 motor pose set by oculus pose logic
- background worker of draw thread?
[] 4 
[] camera loading in xaml
https://msdn.microsoft.com/en-us/library/windows/apps/xaml/hh465144.aspx
[] pozadí se severem - skyball!
[] ASYNCHRONOUS
https://www.youtube.com/watch?v=-t-ALnqYV3c
[] rychlost motoru -> lag -> v závislosti na napìtí na motorech
[] lag na kameøe - zmìøit
[] lag oculu zmìøit tim lagometrem èi co to tam maj?
[] na zaèátku proskenovat scénu -> naplnit tím ten skyball! ale musí být dobøe ten lag urèenej
[] - další stupnì volnosti

[] 
To reduce the juddering on the camera view, you could crop (or fade the edges of) the distortion-corrected image slightly according to the current head pose.

This would keep the edges of the camera stable in the view, whilst still allow the camera to have latency/lower frame rate. If the user turns their head too quickly the same judder problem would re-occur, but I think it would be more pleasant most of the time.?


[] TOOLKIT SAMPLES CAN DO IT ALL
B:\__DIP\dev\2015_03_29 - sharpDX examples\SharpDX-Samples


____________________________________________________
:: OTHER drawer than sharpDX

[] https://forums.oculus.com/viewtopic.php?f=20&t=8464&start=40
This is maybe a dumb question but would it be possible to use sharpovr and render to the rift without the sharpdx game class?
Absolutely, you don't have to use the SharpDX Toolkit, the only dependency is with the base SharpDX library. In Virtual Desktop for example I'm using my own game class.


[x] 0.4.2 installed when working 0.4.1 sharpOVR
male 180
Horiz,vertical eye to neck distance, 90.5, 100 mm
IPD 69,3 mm
eyeRelief 11mm

- https://answers.oculus.com/questions/533/no-device-attached-dk1-runtime-044.html - extended when dk1 support probably

[x] 0.4.4 installed when working with 0.4.4 sharpOVR
demo Scene not functional?
x] Direct HMD access from Apps
no] DK1 Legacy App support


