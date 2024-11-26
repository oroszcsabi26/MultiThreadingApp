A projekt a többszálú környezet tesztelését és megvalósitását hivatott bemutatni.  
A felületet Windows MAUI-ra tervezzük MVVM minta alapján. 

Egy PropertObject objektum példányait jelenítjük meg a felületen, ami  
- 2 db bool 
- 2 db int  
- 2 db double  
- 1db String  (30.000 karakter tárolása, random adatokkal valo feltöltés - és ezekből az adatokból random helyekről GUID képzése) 
- 1db Color lista aminek a maximum mérete 5 db lehet (Random szinekkel való feltöltes a mérete is random 1-5 között változhat)
- 1db Datetime
  property-kel rendelkezik. 

Létrehoztunk egy szálat, ami azért felelős hogy ilyen objektumokat kreáljon random kezdeti adatokkal. Az objektumokat ObservableCollection-el kezeljük. A lista maximális méretét kézzel be lehet állítani a felületen amit futás közben módositani is lehet. Ha lista feltöltődött akkor 2 másodpercenként elveszünk egy random elemet a listából, majd 2 másodperc múlva random helyre beszúrunk egy újat. A listából kézzel is lehet törölni a felületen keresztül.

Az app olyan taskokat tartalmaz, ami megvalósítja az adatok módositását.
- Bool negálása 
- Integerek inkrementálása 
- Double változók randomizálása 
- Hosszu idejű algoritmus létrehozása a string módositása
- Szinek randomizálása
- A dátum a módosítást jelöli

Egy osztályt ezekből a taskokbol előállit egy feladat sort amit aztán végrehajt. A feladatsor maximalis merete 20-30 db. Minden feladat masik random PropertObject-en hajtódik végre, de úgy hogy egy PropertObject-en akár több feladat is dolgozhat egyszerre. Ha a feladatokból elfogy egy akkor pótoljuk azt egy másik random feladattal. A felületen lehessen megadni hány darab ilyen feldolgozó osztály futhat egyszerre. 
A folyamok kezelését le lehet állitani és el lehet inditani. Egy külső számlálóban jegyezzük fel a már végrehajtott feladatok számát.
