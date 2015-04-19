%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
logDip
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
/:: TODO
[x] zjistit co za poèítaè poadavky

[] domluvit poèítaè

[x] camery
2048^2 80Hz
basler ac a2040 - 900c
http://www.baslerweb.com/en/products/area-scan-cameras/ace/aca2040-180km

[x]serva mx-64ar
naštudovat co potøebuju
http://support.robotis.com/en/product/dynamixel/mx_series/mx-64at_ar.htm

[x] pájet rs485

[] 1 recherche mobilni robotika
[] 2 rozhyby hlavy - tabulka 
-> pùjèit kníku o anatomii??
-> kontaktovat nìjakıho medika :O

[] camera basler - drivery co mají - naèítat co nejrychleji!

[] DK2 modul latence - mìøení !
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
/:: DONE
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
[x] mail 2015_02_16:
Je to tedy tak, e budu muset poprosit o zapùjèení poèítaèe, jestli se tedy najdou dané komponenty. Kdyby nìco nešlo, mohl bych zkusit poprosit kamaráda od Pøerova (má herní pc snad s usb3)

Zejména potøebuji sestavu kvùli tomu, e mùj souèasnı notebook:
- nemá USB3.0 vstup jen je nutnı pro vstup z kamer 
- nepodporuje DirectX 11 a vıš (pouze verzi 10.1) - vhodnìjší pro lepší video-dekodér

____________________________________________________
Nejlépe by bylo kdyby sestava mìla:
[x] CPU - nejlépe dvoujádro 

[x] grafická karta podporující DirectX 11 
hdmi vıstup - propojení s Oculem
doporuèeno nepouívat Nvidia Optimus technologii 
kvùli vyšší latence a moné nekompabilitì s Oculus drivery
http://www.reddit.com/r/oculus/comments/2bn62s/oculus_sdk_040_is_now_available/

[x] usb vstupy (minimum)
-- 2x USB 3.0 = 2 kamery  
-- minimal 2x USB 2.0 (lépe 4x jinak mám hub)
= oculus board 
= oculus camera 
= rs485 
= usb-hub (mouse, keyb, external hdd) 
(moná mám nìkde doma PCI rozšiøující 2porty usb2.0)

[x] síová karta
minimalnì 1 LAN port, ale pokud je nìkde volná wifi karta èi usb port tak by byla vıhodou nemusel bych tahat kabel z pokoje do pokoje pøes dvoje dveøe.
[x] hdd alespoò 50GB = OS, VS, SDK..


Dále se rozhoduji co za OS, (jestli u tedy nebude nainstalovanı):

[x] OS: 
        | directx   | VisualStu | 
____________________________________________________
win 7   | 10,11     | 2012      |
win 8.1 | 10,11,12? | 2012,2013 |
win 10  | ?,12

Respektive, nainstaloval bych do win7, ale èetl jsem z rùznıch zdrojù, e win8 má:
- lepší podporu usb3.0 (nebylo vyjádøeno jak "lepší")
- teoreticky rychlejší provázanost s DirectX 11 
..ještì musím si o tom více pøeètu

____________________________________________________

Popøemıšlejte co ze sestavy je reálné a co ne a napište mi prosím kdy se bude nejvíce hodit abych pøišel, jestli hned a vyøešíme to spolu, nebo jestli je potøeba se zeptat ostatních a zbyteènì bych tam pøekáel.
Já zatím budu pokraèovat v psaní k prvním 2ma bodùm a napájím si rs485 to usb modul.

Opìt mám èas v pondìlí a úterı do 15ti hodin
ve støedu pouze v liché tıdny (16-20.2 je sudı)
a ètvrtek pátek celı den.

Pøeji pìknı den!
Davídek D.

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
::::::::::::::::::: NE

Spíš jenom nìco jako : no se mi zdá e to jedno servo pøi vnìjším pohybu rukou (toèím velkım kolem proti malému - do rychla) take to trochu rachotí.. ale pøi regulaci z motoru (malé kolo motoru toèí velkım do pomala) je to v pohodì.
Pozval sem si kamaráda kterı studuje na FSI a ten øíkal e je to normální a bıvá to v pøevodovkách auta..

nebo mu to vùbec nepsat a øict mu to osobnì.. potom co odprezentuješ e ti to funguje..

TODLE NE!:

Zdravím,
mám dobrou a bohuel i špatnou zprávu. V nedìli kdy jsem chtìl vyzkoušet ovládání všech serv naráz jsem zjistil, e u jednoho serva dochází k zásekùm. Je to zpùsobeno miniaturním vykøivením pùlky zubu na posledním stupni pøevodovky 1:4 (viz pøíloha). Stydím se, e k tomu došlo. Asi kdy jsem rameno pøenášel v batohu (byl vystlanı obleèením a zacházel jsem s ním opatrnì), ale asi jak nebylo rameno rozebráno došlo k pøílišnému tlaku na støední servo. Díval jsem se, e lze od Dynamixelu zakoupit novou pøevodovku (1600Kè), ale zejména mì mrzí, e mou vinou zdruji onu druhou studentku, která má na rameni také dìlat. Na vyøešení placení škody se samozøejmì domluvíme.
Respektive dané servo je plnì funkèní, ale dochází pøi kadé ètvrt-otoèce k "zasekrnutí.
Dobrá zpráva je, e program jen obsluhuje serva u mám prakticky hotovı, take u mùu rameno pøedat (bohuel s 1 servem s špatnım pøevodem) aby i druhá strana mohla pokroèit v práci. S tím e a budu mít rozchozenu kameru spolu s HMD bych se servem ještì provedl mìøení rychlosti odezvy atp.

Má otázka tedy zní, kdy bych se mohl

e s kamarádem strojaøem cosi ..zdeòou??

S velkou omluvou a pøáním pøijemného dne,
Daniel Davídek

____________________________________________________
[x] csproj for content option
[x] multiple references
[x] nuget packages
