# SI-server-grupa4

## Uvod
MonitorWebAPI je jedna od komponenti aplikacije Monitor koja služi za udaljeni nadzor, kontrolu i upravljanje industrijskih windows mašina. Njen osnovni zadatak je da omogući komunikaciju između Main Servera i MCWA/MMA  - Web i mobile aplikacija.

Da bi se aplikacija mogla koristiti potrebno je koristiti i rješenje koje je razvila grupa 6 koja je bila zadužena za dodjelu JWT tokena. Link na repozitorij: https://github.com/skujrakovic/SI-server-Grupa6

Naš projekat je podržan na svim platformama.

## Uputstvo za korisnika
Potrebno je skinuti projekat sa github repozitorija i otvoriti ga pomoću Visual Studija.

## User stories
Sprint 1:

    • Kao korisnik-administrator želim da imam mogućnost kreiranja i pregledanja mašina koje se servisiraju
    • Kao obični korisnik želim da se omogući pregledanje statusa mašina iz moje grupe
    
Sprint 2:

    • Kao obični korisnik želim da se omogući pretraga mašina po nazivu, statusu
    • Kao obični korisnik želim da se omogući paginacija mašina koje mi pripadaju i da se omogući sortiranje istih po nazivu, statusu i lokaciji
    Dodatni feature:
        • Omogućeno web/mobile aplikaciji hijerarhijski prikaz grupa, kao i dobavljanje svih uređaja koji pripadaju trenutno logovanom korisniku. Nad tim
        uređajima je moguće raditi gore navedene zahtjeve
      
Sprint 3:

    • Kao korisnik želim da se omogući mogućnost upload slika u folder koji je na mašini
    • Kao korisnik želim da se omogući pregled srednje vrijednosti korištenja hardvera mašina
    • Kao korisnik želim da se omogući mogućnost pregleda da li je mašina pod mojom ovlasti
    • Kao korisnik želim da se omogući pregled mašine po njenom instalacijskom kodu prvi put nakon čega taj instalacijski kod ističe i ne može se više koristiti


## Rute za zahtjeve

### Rute za korisnika

* [Route("api/user/Me")]
 
  [HttpGet]

  * Vraća osnovne informacije o loggovanom korisniku

* [Route("api/user/MeExtendedInfo")]

  [HttpGet]
  
  * Vraća dodatne informacije o loggovanom korisniku

### Rute za uređaje

* [Route("api/device/AllDevices")]

  [HttpGet]

  * Vraća sve uređaje koji su dostupni tom korisniku u zavisnosti od njegove role i grupe

* [Route("api/device/CreateDevice")]

  [HttpPost]

  * Ukoliko je korisnik SuperAdmin koji je zaduzen za grupu koja je poslana kao parametar onda može kreirati uređaj u toj grupi, a ako je korisnik
    MonitorSuperAdmin on moze svugdje kreirati uređaj, u suprotnom se vraća Unathorized().

* [Route("api/device/GetDeviceByInstallationCode/{code}")]

  [HttpGet]
  
  * Vraća korisniku uređaj pomoću poslatog instalacijskog koda,nakon čega taj kod postaje nevalidan za ponovno korištenje.

* [Route("api/device/CheckIfDeviceBelongsToUser/{deviceUid}")]
  
  [HttpGet]
  
  * Provjerava da li je uređaj, čiji je jedinstveni kod poslat, pod ovlasti trenutno loggovanog korisnika 

* [Route("api/device/GetAverageHardwareUsageForUser")]

  [HttpGet]
  
  * Vraća srednju vrijednost iskorištenosti hardvera u određenom vremenskom periodu trenutno loggovanom korisniku

* [Route("api/device/AllDevicesForGroup")]

  [HttpGet]

  * Ukoliko je korisnik MonitorSuperAdmin dobija za grupu poslanu kao parametar sve njene uređaje kao i uređaje njenih podgrupa, također isto to dobija ako je on     SuperAdmin grupe koja je poslana kao parametara, u suprotnom se vraća Unathorized().

* [Route("api/device/GetDeviceLogs")] 

  [HttpGet]
  
  * Vraća sve logove mašine za sve mašine kojim logovani korisnik ima pristup

### Rute za file upload

* [Route("api/upload/UploadFile")]
  
  [HttpPost]
  
  * Omogućuje upload file-a u bazu podataka

### Rute za grupe

* [Route("api/group/MyGroup")]

  [HttpGet]

   * Vraća grupu kojoj logovani korisnik pripada, ako korisnik nije logovan vraća se Unathorized()

* [Route("/api/group/MyAssignedGroups")]

  [HttpGet]

  * Ako je logovani korisnik MonitorSuperAdmin - vraća sve grupe koje postoje u bazi, u suprotnom vraća grupe kojim je logovani korisnik dodijeljen

* [Route("/api/group/GroupTreeById/{groupId}")]

  [HttpGet]
  
  * Omogućuje da se za određenu grupu vraća hijerarhija podgrupa

* [Route("/api/group/CreateGroup")]

  [HttpPost]
  
  * Omogućuje kreiranje nove grupe ukoliko je korisnik MonitorSuperAdmin ili ukoliko je nova grupa koja se dodaje podgrupa grupe kojoj je trenutni korisnik
    SuperAdmin

### Rute za izvještaj

* [Route("/api/report/AllReportsForUser")]

  [HttpGet]
  
  * Vraća sve izvještaje za logovanog korisnika

### Rute za role

* [Route("/api/role/GetRoles")]

  [HttpGet]
  
  * Daje sve role 

Za dodatne informacije o rutama posjetite: https://si-2021.167.99.244.168.nip.io/swagger/index.html

Odgovor je u JSON formatu.

## Korišteni framework
Korišten je .NET Core 3.1.
