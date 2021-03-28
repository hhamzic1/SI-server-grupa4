# SI-server-grupa4

## Uvod
MonitorWebAPI je jedna od komponenti aplikacije Monitor koja služi za udaljeni nadzor, kontrolu i upravljanje industrijskih windows mašina. Njen osnovni zadatak je da omogući komunikaciju između Main Servera i MCWA/MMA  - Web i mobile aplikacija.

Da bi se aplikacija mogla koristiti potrebno je koristiti i rješenje koje je razvila grupa 6 koja je bila zadužena za dodjelu JWT tokena. Link na repozitorij: https://github.com/skujrakovic/SI-server-Grupa6

Naš projekat je podržan na svim platformama.

## Uputstvo za korisnika
Potrebno je skinuti projekat sa github repozitorija i otvoriti ga pomoću Visual Studija.

## User stories
    • Kao korisnik-administrator želim da imam mogućnost kreiranja i pregledanja mašina koje se servisiraju
    • Kao obični korisnik želim da se omogući pregledanje statusa mašina iz moje grupe

## Rute za zahtjeve
* [Route("api/user/Me")]
 
  [HttpGet]

  * Daje osnovne informacije o loggovanom korisniku

* [Route("api/user/MeExtendedInfo")]

  [HttpGet]
  
  * Daje dodatne informacije o loggovanom korisniku

* [Route("api/device/AllDevicesForGroup")]

  [HttpGet]

  * Ukoliko je korisnik MonitorSuperAdmin dobija za grupu poslanu kao parametar sve njene uređaje kao i uređaje njenih podgrupa, također isto to dobija ako je on SuperAdmin grupe koja je poslana kao parametara, u suprotnom se vraća Unathorized().

* [Route("api/device/CreateDevice")]

  [HttpPost]

  * Ukoliko je korisnik SuperAdmin koji je zaduzen za grupu koja je poslana kao parametar onda može kreirati uređaj u toj grupi, a ako je korisnik MonitorSuperAdmin on moze svugdje kreirati uređaj, u suprotnom se vraća Unathorized().

* [Route("api/device/AllDevices")]

  [HttpGet]

  * Daje sve uređaje koji su dostupni tom korisniku u zavisnosti od njegove role i grupe

* [Route("api/device/GetDeviceLogs")] 

  [HttpGet]
  
  * Daje sve logove mašine za sve mašine kojim logovani korisnik ima pristup

* [Route("api/group/MyGroup")]

  [HttpGet]

   * Vraća grupu kojoj logovani korisnik pripada, ako korisnik nije logovan vraća se Unathorized()

* [Route("/api/group/MyAssignedGroups")]

  [HttpGet]

  * Ako je logovani korisnik MonitorSuperAdmin - vraća sve grupe koje postoje u bazi, u suprotnom vraća grupe kojim je logovani korisnik dodijeljen

* [Route("/api/group/GroupTreeById/{groupId}")]

  [HttpGet]
  
  * Za određenu grupu vraća se hijerarhija podgrupa

* [Route("/api/report/AllReportsForUser")]

  [HttpGet]
  
  * Daje sve izvještaje za logovanog korisnika

* [Route("/api/role/GetRoles")]

  [HttpGet]
  
  * Daje sve role 

Za dodatne informacije o rutama posjetite: https://si-2021.167.99.244.168.nip.io/swagger/index.html

Odgovor je u JSON formatu.

## Korišteni framework
Korišten je .NET Core 3.1.
