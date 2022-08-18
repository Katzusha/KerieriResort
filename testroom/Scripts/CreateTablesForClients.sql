
CREATE TABLE `ClassifficationEssentials`
(
 `Id`           serial NOT NULL ,
 `Name`         varchar(255) NOT NULL ,
 `Price`        decimal(5, 2) NOT NULL ,
 `Description`  varchar(255) NOT NULL ,
 `CreatedDate`  timestamp NOT NULL ,
 `ModifiedDate` timestamp NOT NULL ,
 `ModifiedBy`   varchar(255) NOT NULL ,

PRIMARY KEY (`Id`)
);

CREATE TABLE `ClassifficationOtherEssentials`
(
 `Id`                serial NOT NULL ,
 `OtherEssentialId`  int NOT NULL ,
 `ClassifficationId` int NOT NULL ,

PRIMARY KEY (`Id`)
);

CREATE TABLE `Classiffications`
(
 `Id`           serial NOT NULL ,
 `Name`         varchar(255) NOT NULL ,
 `SerialNumber` varchar(255) NOT NULL ,
 `Price`        varchar(255) NOT NULL ,
 `CreatedDate`  timestamp NOT NULL ,
 `ModifiedDate` timestamp NOT NULL ,
 `ModifiedBy`   varchar(255) NOT NULL ,

PRIMARY KEY (`Id`)
);

CREATE TABLE `OtherEssentials`
(
 `Id`             serial NOT NULL ,
 `Name`           varchar(255) NOT NULL ,
 `Price`          decimal(10, 2) NOT NULL ,
 `Description`    varchar(255) NOT NULL ,
 `AgeRequirement` decimal(3, 1) NOT NULL ,
 `CreatedDate`    timestamp NOT NULL ,
 `ModifiedDate`   timestamp NOT NULL ,
 `ModifiedBy`     varchar(255) NOT NULL ,

PRIMARY KEY (`Id`)
);

CREATE TABLE `Reservants`
(
 `Id`             serial NOT NULL ,
 `Firstname`      varchar(255) NOT NULL ,
 `Surname`        varchar(255) NOT NULL ,
 `Birth`          date NOT NULL ,
 `Email`          varchar(255) NOT NULL ,
 `PhoneNumber`    varchar(255) NOT NULL ,
 `Country`        varchar(255) NOT NULL ,
 `PostNumber`     varchar(255) NOT NULL ,
 `Address`        varchar(255) NOT NULL ,
 `Gender`         varchar(255) NOT NULL ,
 `Sexuality`      varchar(255) NOT NULL ,
 `ReservationsId` int NOT NULL ,
 `CreatedDate`    timestamp NOT NULL ,
 `ModifiedDate`   timestamp NOT NULL ,
 `ModifiedBy`     varchar(255) NOT NULL ,

PRIMARY KEY (`Id`)
);

CREATE TABLE `ReservationOtherEssentials`
(
 `Id`               serial NOT NULL ,
 `OtherEssentialId` int NOT NULL ,
 `ReservationId`    int NOT NULL ,

PRIMARY KEY (`Id`)
);

CREATE TABLE `Reservations`
(
 `Id`                serial NOT NULL ,
 `ClassifficationId` int NOT NULL ,
 `FromDate`          date NOT NULL ,
 `ToDate`            date NOT NULL ,
 `Comments`          text NOT NULL ,
 `CreatedDate`       timestamp NOT NULL ,
 `ModifiedDate`      timestamp NOT NULL ,
 `ModifiedBy`        varchar(255) NOT NULL ,

PRIMARY KEY (`Id`)
);