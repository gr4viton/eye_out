%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
logDip
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
/:: TODO
[x] zjistit co za po��ta� po�adavky

[] domluvit po��ta�

[x] camery
2048^2 80Hz
basler ac a2040 - 900c
http://www.baslerweb.com/en/products/area-scan-cameras/ace/aca2040-180km

[x]serva mx-64ar
na�tudovat co pot�ebuju
http://support.robotis.com/en/product/dynamixel/mx_series/mx-64at_ar.htm

[x] p�jet rs485

[] 1 recherche mobilni robotika
[] 2 rozhyby hlavy - tabulka 
-> p�j�it kn�ku o anatomii??
-> kontaktovat n�jak�ho medika :O

[] camera basler - drivery co maj� - na��tat co nejrychleji!

[] DK2 modul latence - m��en� !
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
/:: DONE
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
[x] mail 2015_02_16:
Je to tedy tak, �e budu muset poprosit o zap�j�en� po��ta�e, jestli se tedy najdou dan� komponenty. Kdyby n�co ne�lo, mohl bych zkusit poprosit kamar�da od P�erova (m� hern� pc snad s usb3)

Zejm�na pot�ebuji sestavu kv�li tomu, �e m�j sou�asn� notebook:
- nem� USB3.0 vstup jen� je nutn� pro vstup z kamer 
- nepodporuje DirectX 11 a v�� (pouze verzi 10.1) - vhodn�j�� pro lep�� video-dekod�r

____________________________________________________
Nejl�pe by bylo kdyby sestava m�la:
[x] CPU - nejl�pe dvouj�dro 

[x] grafick� karta podporuj�c� DirectX 11 
hdmi v�stup - propojen� s Oculem
doporu�eno nepou��vat Nvidia Optimus technologii 
kv�li vy��� latence a mo�n� nekompabilit� s Oculus drivery
http://www.reddit.com/r/oculus/comments/2bn62s/oculus_sdk_040_is_now_available/

[x] usb vstupy (minimum)
-- 2x USB 3.0 = 2 kamery  
-- minimal 2x USB 2.0 (l�pe 4x jinak m�m hub)
= oculus board 
= oculus camera 
= rs485 
= usb-hub (mouse, keyb, external hdd) 
(mo�n� m�m n�kde doma PCI roz�i�uj�c� 2porty usb2.0)

[x] s�ov� karta
minimaln� 1 LAN port, ale pokud je n�kde voln� wifi karta �i usb port tak by byla v�hodou nemusel bych tahat kabel z pokoje do pokoje p�es dvoje dve�e.
[x] hdd alespo� 50GB = OS, VS, SDK..


D�le se rozhoduji co za OS, (jestli u� tedy nebude nainstalovan�):

[x] OS: 
        | directx   | VisualStu | 
____________________________________________________
win 7   | 10,11     | 2012      |
win 8.1 | 10,11,12? | 2012,2013 |
win 10  | ?,12

Respektive, nainstaloval bych do win7, ale �etl jsem z r�zn�ch zdroj�, �e win8 m�:
- lep�� podporu usb3.0 (nebylo vyj�d�eno jak "lep��")
- teoreticky rychlej�� prov�zanost s DirectX 11 
..je�t� mus�m si o tom v�ce p�e�tu

____________________________________________________

Pop�em��lejte co ze sestavy je re�ln� a co ne a napi�te mi pros�m kdy se bude nejv�ce hodit abych p�i�el, jestli hned a vy�e��me to spolu, nebo jestli je pot�eba se zeptat ostatn�ch a zbyte�n� bych tam p�ek�el.
J� zat�m budu pokra�ovat v psan� k prvn�m 2ma bod�m a nap�j�m si rs485 to usb modul.

Op�t m�m �as v pond�l� a �ter� do 15ti hodin
ve st�edu pouze v lich� t�dny (16-20.2 je sud�)
a �tvrtek p�tek cel� den.

P�eji p�kn� den!
Dav�dek D.

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
::::::::::::::::::: NE

Sp� jenom n�co jako : no se mi zd� �e to jedno servo p�i vn�j��m pohybu rukou (to��m velk�m kolem proti mal�mu - do rychla) tak�e to trochu rachot�.. ale p�i regulaci z motoru (mal� kolo motoru to�� velk�m do pomala) je to v pohod�.
Pozval sem si kamar�da kter� studuje na FSI a ten ��kal �e je to norm�ln� a b�v� to v p�evodovk�ch auta..

nebo mu to v�bec nepsat a �ict mu to osobn�.. potom co odprezentuje� �e ti to funguje..

TODLE NE!:

Zdrav�m,
m�m dobrou a bohu�el i �patnou zpr�vu. V ned�li kdy� jsem cht�l vyzkou�et ovl�d�n� v�ech serv nar�z jsem zjistil, �e u jednoho serva doch�z� k z�sek�m. Je to zp�sobeno miniaturn�m vyk�iven�m p�lky zubu na posledn�m stupni p�evodovky 1:4 (viz p��loha). Styd�m se, �e k tomu do�lo. Asi kdy� jsem rameno p�en�el v batohu (byl vystlan� oble�en�m a zach�zel jsem s n�m opatrn�), ale asi jak nebylo rameno rozebr�no do�lo k p��li�n�mu tlaku na st�edn� servo. D�val jsem se, �e lze od Dynamixelu zakoupit novou p�evodovku (1600K�), ale zejm�na m� mrz�, �e mou vinou zdr�uji onu druhou studentku, kter� m� na rameni tak� d�lat. Na vy�e�en� placen� �kody se samoz�ejm� domluv�me.
Respektive dan� servo je pln� funk�n�, ale doch�z� p�i ka�d� �tvrt-oto�ce k "zasekrnut�.
Dobr� zpr�va je, �e program jen� obsluhuje serva u� m�m prakticky hotov�, tak�e u� m��u rameno p�edat (bohu�el s 1 servem s �patn�m p�evodem) aby i druh� strana mohla pokro�it v pr�ci. S t�m �e a� budu m�t rozchozenu kameru spolu s HMD bych se servem je�t� provedl m��en� rychlosti odezvy atp.

M� ot�zka tedy zn�, kdy bych se mohl

�e s kamar�dem stroja�em cosi ..zde�ou??

S velkou omluvou a p��n�m p�ijemn�ho dne,
Daniel Dav�dek

____________________________________________________
[x] csproj for content option
[x] multiple references
[x] nuget packages
