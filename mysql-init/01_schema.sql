-- ============================================================
-- DigitalLoanSystem — MySQL Schema
-- ============================================================

CREATE DATABASE IF NOT EXISTS `DigitalLoanDb` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `DigitalLoanDb`;

-- ── Customers ────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS `Customers` (
    `Id`             INT             NOT NULL AUTO_INCREMENT,
    `FirstName`      VARCHAR(100)    NOT NULL,
    `LastName`       VARCHAR(100)    NOT NULL,
    `Email`          VARCHAR(200)    NOT NULL,
    `Phone`          VARCHAR(20)     NOT NULL,
    `Address`        VARCHAR(500)    NOT NULL,
    `IdentityNumber` VARCHAR(50)     NOT NULL,
    `CreditScore`    INT             NOT NULL DEFAULT 0,
    `Balance`        DECIMAL(18,2)   NOT NULL DEFAULT 0.00,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ── Loans ────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS `Loans` (
    `Id`              INT           NOT NULL AUTO_INCREMENT,
    `CustomerId`      INT           NOT NULL,
    `Type`            INT           NOT NULL,   -- 1=Personal,2=Education,3=Vehicle,4=Housing,5=Business
    `PrincipalAmount` DECIMAL(18,2) NOT NULL,
    `InterestRate`    DECIMAL(5,2)  NOT NULL,
    `TermInMonths`    INT           NOT NULL,
    `StartDate`       DATE          NOT NULL,
    `Status`          INT           NOT NULL DEFAULT 1,  -- 1=Active, 2=Closed
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Loans_Customers`
        FOREIGN KEY (`CustomerId`) REFERENCES `Customers` (`Id`)
        ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ── Installments ─────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS `Installments` (
    `Id`                 INT           NOT NULL AUTO_INCREMENT,
    `LoanId`             INT           NOT NULL,
    `InstallmentNumber`  INT           NOT NULL,
    `Amount`             DECIMAL(18,2) NOT NULL,
    `DueDate`            DATE          NOT NULL,
    `Status`             INT           NOT NULL DEFAULT 2,  -- 1=Paid, 2=Unpaid, 3=Delayed
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Installments_Loans`
        FOREIGN KEY (`LoanId`) REFERENCES `Loans` (`Id`)
        ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ── Payments ─────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS `Payments` (
    `Id`             INT           NOT NULL AUTO_INCREMENT,
    `InstallmentId`  INT           NOT NULL,
    `Amount`         DECIMAL(18,2) NOT NULL,
    `PaymentDate`    DATE          NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Payments_Installments`
        FOREIGN KEY (`InstallmentId`) REFERENCES `Installments` (`Id`)
        ON DELETE RESTRICT,
    CONSTRAINT `UQ_Payments_InstallmentId`
        UNIQUE (`InstallmentId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
