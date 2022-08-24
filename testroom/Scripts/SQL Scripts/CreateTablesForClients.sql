CREATE TABLE `Classiffications`
(
 `Id`           serial NOT NULL ,
 `Name`         varchar(255) NOT NULL ,
 `SerialNumber` varchar(255) NOT NULL ,
 `Price`        varchar(255) NOT NULL ,
 `Size`         varchar(255) NOT NULL ,
 `CreatedDate`  timestamp NOT NULL ,
 `ModifiedDate` timestamp NOT NULL ,
 `ModifiedBy`   varchar(255) NOT NULL ,

PRIMARY KEY (`Id`)
);

ALTER TABLE `Classiffications` CHANGE `ModifiedDate` `ModifiedDate` TIMESTAMP on update CURRENT_TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE `Classiffications` CHANGE `CreatedDate` `CreatedDate` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;

CREATE TABLE `Essentials`
(
 `Id`           serial NOT NULL ,
 `Name`         varchar(255) NOT NULL ,
 `Description`  varchar(255) NOT NULL ,
 `CreatedDate`  timestamp NOT NULL ,
 `ModifiedDate` timestamp NOT NULL ,
 `ModifiedBy`   varchar(255) NOT NULL ,

PRIMARY KEY (`Id`)
);

ALTER TABLE `Essentials` CHANGE `ModifiedDate` `ModifiedDate` TIMESTAMP on update CURRENT_TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE `Essentials` CHANGE `CreatedDate` `CreatedDate` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;

CREATE TABLE `IncludedClassifficationEssentials`
(
 `Id`                serial NOT NULL ,
 `EssentialId`       int NOT NULL ,
 `ClassifficationId` int NOT NULL ,
 `CreatedDate`       timestamp NOT NULL ,
 `ModifiedDate`      timestamp NOT NULL ,
 `ModifiedBy`        varchar(255) NOT NULL ,

PRIMARY KEY (`Id`)
);

ALTER TABLE `IncludedClassifficationEssentials` CHANGE `ModifiedDate` `ModifiedDate` TIMESTAMP on update CURRENT_TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE `IncludedClassifficationEssentials` CHANGE `CreatedDate` `CreatedDate` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;

CREATE TABLE `MainReservants`
(
 `Id`              serial NOT NULL ,
 `Firstname`       varchar(255) NOT NULL ,
 `Surname`         varchar(255) NOT NULL ,
 `Birth`           date NOT NULL ,
 `Email`           varchar(255) NOT NULL ,
 `PhoneNumber`     varchar(255) NOT NULL ,
 `Country`         varchar(255) NOT NULL ,
 `PostNumber`      varchar(255) NOT NULL ,
 `Address`         varchar(255) NOT NULL ,
 `Gender`          varchar(255) NOT NULL ,
 `CertifiedNumber` varchar(255) NOT NULL ,
 `PersonalNumber`  varchar(255) NOT NULL ,
 `ReservationsId`  int NOT NULL ,
 `CreatedDate`     timestamp NOT NULL ,
 `ModifiedDate`    timestamp NOT NULL ,
 `ModifiedBy`      varchar(255) NOT NULL ,

PRIMARY KEY (`Id`)
);

ALTER TABLE `MainReservants` CHANGE `ModifiedDate` `ModifiedDate` TIMESTAMP on update CURRENT_TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE `MainReservants` CHANGE `CreatedDate` `CreatedDate` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;

CREATE TABLE `NotIncluddClassifficaitonEssentials`
(
 `Id`                serial NOT NULL ,
 `EssentialId`       int NOT NULL ,
 `ClassifficationId` int NOT NULL ,
 `Price`             decimal(10,2) NOT NULL ,
 `CreatedDate`       timestamp NOT NULL ,
 `ModifiedDate`      timestamp NOT NULL ,
 `ModifiedBy`        varchar(266) NOT NULL ,

PRIMARY KEY (`Id`)
);

ALTER TABLE `NotIncluddClassifficaitonEssentials` CHANGE `ModifiedDate` `ModifiedDate` TIMESTAMP on update CURRENT_TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE `NotIncluddClassifficaitonEssentials` CHANGE `CreatedDate` `CreatedDate` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;

CREATE TABLE `PaymentInfo`
(
 `Id`                      serial NOT NULL ,
 `CreditCardNumber`        varchar(255) NOT NULL ,
 `CreditCardHolderName`    varchar(255) NOT NULL ,
 `CreditCardHolderSurname` varchar(255) NOT NULL ,
 `CreditCardCvv`           varchar(255) NOT NULL ,
 `PaymentType`             int NOT NULL ,
 `ReservationId`           int NOT NULL ,
 `CreatedDate`             timestamp NOT NULL ,
 `ModifiedDate`            timestamp NOT NULL ,
 `ModifiedBy`              varchar(255) NOT NULL ,

PRIMARY KEY (`Id`)
);

ALTER TABLE `PaymentInfo` CHANGE `ModifiedDate` `ModifiedDate` TIMESTAMP on update CURRENT_TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE `PaymentInfo` CHANGE `CreatedDate` `CreatedDate` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;

CREATE TABLE `PaymentTypes`
(
 `Id`                         serial NOT NULL ,
 `Name`                       varchar(255) NOT NULL ,
 `PlannedPaymentDurationDays` int NOT NULL ,
 `CreatedDate`                timestamp NOT NULL ,
 `ModifiedDate`               timestamp NOT NULL ,
 `ModifiedBy`                 varchar(255) NOT NULL ,

PRIMARY KEY (`Id`)
);

ALTER TABLE `PaymentTypes` CHANGE `ModifiedDate` `ModifiedDate` TIMESTAMP on update CURRENT_TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE `PaymentTypes` CHANGE `CreatedDate` `CreatedDate` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;

CREATE TABLE `ReservationEssential`
(
 `Id`            serial NOT NULL ,
 `EssentialId`   int NOT NULL ,
 `ReservationId` int NOT NULL ,
 `CreatedDate`   timestamp NOT NULL ,
 `ModifiedDate`  timestamp NOT NULL ,
 `ModifiedBy`    varchar(255) NOT NULL ,

PRIMARY KEY (`Id`)
);

ALTER TABLE `ReservationEssential` CHANGE `ModifiedDate` `ModifiedDate` TIMESTAMP on update CURRENT_TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE `ReservationEssential` CHANGE `CreatedDate` `CreatedDate` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;

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

ALTER TABLE `Reservations` CHANGE `ModifiedDate` `ModifiedDate` TIMESTAMP on update CURRENT_TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE `Reservations` CHANGE `CreatedDate` `CreatedDate` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;

CREATE TABLE `SideReservants`
(
 `Id`            serial NOT NULL ,
 `Firstname`     varchar(255) NOT NULL ,
 `Surname`       varchar(255) NOT NULL ,
 `Birth`         date NOT NULL ,
 `Email`         varchar(255) NOT NULL ,
 `PhoneNumber`   varchar(255) NOT NULL ,
 `ReservationId` int NOT NULL ,
 `CreatedDate`   timestamp NOT NULL ,
 `ModifiedDate`  timestamp NOT NULL ,
 `ModifiedBy`    varchar(255) NOT NULL ,

PRIMARY KEY (`Id`)
);

ALTER TABLE `SideReservants` CHANGE `ModifiedDate` `ModifiedDate` TIMESTAMP on update CURRENT_TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE `SideReservants` CHANGE `CreatedDate` `CreatedDate` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;