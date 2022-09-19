Name = int(input("Name >>>  "))
x = int(input("Number of classiffications >>"))
GeneratedString = ""

for i in range(x):
    GeneratedString = GeneratedString +  "INSERT INTO Classiffications (Name, SerialNumber, Price, Size) VALUES ('" + str(Name)+ "', 'SN-" + str(Name) + "', 0.00, 0);"
    Name = Name + 1

    print(GeneratedString)



with open('D:/KerieriResort/testroom/Scripts/GeneratedScripts/GenerateClassiffications.txt', 'w') as file:
    file.write(str(GeneratedString))