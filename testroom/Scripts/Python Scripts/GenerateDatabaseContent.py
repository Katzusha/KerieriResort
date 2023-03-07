import random
import datetime

names = [
    "Ana",
    "Matej",
    "Eva",
    "Bernard",
    "Andraž",
    "Anej",
    "Jakob",
    "Maja",
    "Filip",
    "Tina",
    "Marko",
    "Barbara",
    "Luka",
    "Lea",
    "Jan",
    "Kaja",
    "Borut",
    "Nina",
    "Andrej",
    "Maša",
    "Tadej",
    "Vid",
    "Ines",
    "Miha",
    "Živa",
    "Peter",
    "Lana",
    "David",
    "Aljaž",
    "Nikolina",
    "Marjan",
    "Larisa",
    "Anton",
]

surnames = [
    "Novak",
    "Horvat",
    "Kovačič",
    "Zupančič",
    "Schmidt",
    "Müller",
    "Cverle",
    "Kužnik",
    "Košak",
    "Melanšek",
    "Jezovšek",
    "Florjan",
    "Kos",
    "Hrovat",
    "Vidmar",
    "Rozman",
    "Krajnc",
    "Golob",
    "Kotnik",
    "Kovač",
    "Pintar",
    "Koren",
    "Kramar",
    "Babič",
    "Bergant",
    "Zupanc",
    "Žagar",
    "Medved",
    "Kocjančič",
    "Kotar",
    "Cimerman",
    "Perko",
    "Lah",
    "Vidic",
    "Rus",
    "Klemenčič",
]

date = datetime.date.today()
todate = date + datetime.timedelta(days=random.randint(2, 14))

i = 1

for x in range (1, 45):
    print(f"INSERT INTO Classiffications (Id, Name, SerialNumber, Price, Size, MaxReservants) VALUES ({x}, 'Parcel {x}', 'SN-{x}-22', '{random.randint(14, 150)}.{random.randint(0, 99)}', '{random.randint(10, 60)}', '{random.randint(1, 6)}');")

for x in range(1, 45):
    date = datetime.date.today()
    for y in range (1, 11):
        todate = date + datetime.timedelta(days=random.randint(2, 14))
        print(f"INSERT INTO Reservations (Id, ClassifficationId, FromDate, ToDate, Price) VALUES ({i}, {x}, '{str(date)}', '{str(todate)}', '00');")
        print(f"INSERT INTO MainReservants (Firstname, Surname, Birth, Email, PhoneNumber, Country, PostNumber, City, Address, Gender, CertifiedNumber, ReservationsId) VALUES ('{random.choice(first_names)}', '{random.choice(surnames)}', '1990-01-01', 'example@example.com', '000 000 000', 'Country', '0000', 'City', 'Address', 'Gender', '00000000', {i});")

        if (random.randint(0, 2) == 1):
            todate = todate + datetime.timedelta(days=random.randint(1, 4))

        date = todate
        i = i + 1