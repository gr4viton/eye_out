použitý jazyk : C#, spolu s gui ve prostøedí WPF
obsahuje aplikaci pro nastavení a vyzkoušení jednotlivých motorù a kamery + logovací systém zpráv
poté se zapne teleprezenèní mód kdy se do hmd promítá scena

ve scenì se dá zapnout èi vypnout vykreslování rùzných modelù
- skybox pro orientaci v prostoru (obloha a scenérie v dalce)
- skelet polohy robotického ramene (pozdìj možná vložím i pøimo modely motorù, zatím jsou zobrazeny teèky / válce / koule na místech jednotlivých propojù robotického ramene)
 -- podložka
 -- osa spodního motoru (roll)
 -- osa støedního motoru (pitch)
 -- osa horního motoru (yaw)
 -- støed CMOS èipu v kameøe
 -- støed obrazové roviny (ètverec do kterého je vykreslována kamera)

- vykreslování help menu ve kterém jsou vidìt:
 -- úhly natoèení motoru : chtìné (vypoètené ale nezaslane), zaslané motoru (nastavené), vyètené z motoru (jistý delay)
 -- FPS vykreslování scény
 -- latence spoèítáná vnitøním latency testerem v Oculusu

pohyb ve scénì:
 - volný - WASD + natoèení hlavy
 - fixovaný na bod 
  -- støed CMOS chipu kamery 
  -- další pohledy na rameno z vnìjšku

model robotického ramene
 - nastavení pozice podle úhlu motorù:
  -- chtìné (vypoètené ale nezaslane), zaslané motoru (nastavené), vyètené z motoru (jistý delay)

hlavní smyèka teleprezenèního programu - zhruba

loop
(

            SETUP_eyeRender(); // vyète data aktuálního natoèení z Oculu a další parametry dùležité pro vykreslení sceny (fov .. rozlišení)
            Update_PlayerFromHmd(); // nastaví hodnotu polohy playera (místo odkud se budu helmou dívat) - dle natoèení HMD a pozice ve scénì (viz volný vs fixní pohyb ve scenì)
            Update_RobotArmWantedAnglesFromPlayer(); // nastaví do modelu robotického ramene úhly tak aby se kamera na ramenu dívala stejným smìrem jako hlava uživatele
            Update_MotorWantedAnglesFromRobotArmWantedAngles(); // nastaví do modelù motorù (v PC) úhly vypoètené v pøedchozím kroku
            Update_RoboticArmDrawnPosture(); // nastavi model robotického ramene (úhly) dle zvoleného nastavení (viz model robotického ramene)
            
            Update_ScoutPosition(); // nastaví polohu scouta = relativní poloha od poèátku (fixního) když je zvolen volný pohyb ve scenì

            Update_helpText(); // updatuje text v help menu

            ControlMotors(); // zašle motorùm chtìné úhly z poèítaèových modelù motoru (jež byli vypoèteny výše)
            ReadFromMotors(); // zašle motorùm požadavek na vyètení aktuální polohy - pokud je zapnuto
            CaptureCameraImage(); // vyète aktuální snímek z kamery (ta je zapnuta na nepøetržité snímkování - ale vyèítá se pouze on demand)
    
            Draw(); // znovu vyète data aktuálního natoèení z Oculu, nastaví dle nich scénu pro vykreslení a vykreslí postupnì pokud je vykreslovaní nastaveno
// model robotického ramene, povrch obrazu kamery (ètvereèek s videem), scenérii (skybox)

)




// pozdìji možná budu ještì dodìlávat, ale to tam zatím ani nepiš 
 -- vypoèet polohy motorù i se zpoždìním (abych nemusel zahlcovat linku žádostmi o ètení polohy) - zpomaluje to 
 -- obrazu z kamery jež je vykreslován bude vyèten døíve - aby bylo zabránìno latency pøi vyèítání (než se vyète chvíli to trvá)
