DELETE FROM OrderProduct;
DELETE FROM ComputerEmployee;
DELETE FROM EmployeeTraining;
DELETE FROM Employee;
DELETE FROM TrainingProgram;
DELETE FROM Computer;
DELETE FROM Department;
DELETE FROM [Order];
DELETE FROM PaymentType;
DELETE FROM Product;
DELETE FROM ProductType;
DELETE FROM Customer;


ALTER TABLE Employee DROP CONSTRAINT [FK_EmployeeDepartment];
ALTER TABLE ComputerEmployee DROP CONSTRAINT [FK_ComputerEmployee_Employee];
ALTER TABLE ComputerEmployee DROP CONSTRAINT [FK_ComputerEmployee_Computer];
ALTER TABLE EmployeeTraining DROP CONSTRAINT [FK_EmployeeTraining_Employee];
ALTER TABLE EmployeeTraining DROP CONSTRAINT [FK_EmployeeTraining_Training];
ALTER TABLE Product DROP CONSTRAINT [FK_Product_ProductType];
ALTER TABLE Product DROP CONSTRAINT [FK_Product_Customer];
ALTER TABLE PaymentType DROP CONSTRAINT [FK_PaymentType_Customer];
ALTER TABLE [Order] DROP CONSTRAINT [FK_Order_Customer];
ALTER TABLE [Order] DROP CONSTRAINT [FK_Order_Payment];
ALTER TABLE OrderProduct DROP CONSTRAINT [FK_OrderProduct_Product];
ALTER TABLE OrderProduct DROP CONSTRAINT [FK_OrderProduct_Order];


DROP TABLE IF EXISTS OrderProduct;
DROP TABLE IF EXISTS ComputerEmployee;
DROP TABLE IF EXISTS EmployeeTraining;
DROP TABLE IF EXISTS Employee;
DROP TABLE IF EXISTS TrainingProgram;
DROP TABLE IF EXISTS Computer;
DROP TABLE IF EXISTS Department;
DROP TABLE IF EXISTS [Order];
DROP TABLE IF EXISTS PaymentType;
DROP TABLE IF EXISTS Product;
DROP TABLE IF EXISTS ProductType;
DROP TABLE IF EXISTS Customer;


CREATE TABLE Department (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    [Name] VARCHAR(55) NOT NULL,
    Budget  INTEGER NOT NULL
);

CREATE TABLE Employee (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    FirstName VARCHAR(55) NOT NULL,
    LastName VARCHAR(55) NOT NULL,
    DepartmentId INTEGER NOT NULL,
    IsSuperVisor BIT NOT NULL DEFAULT(0),
    CONSTRAINT FK_EmployeeDepartment FOREIGN KEY(DepartmentId) REFERENCES Department(Id)
);

CREATE TABLE Computer (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    PurchaseDate DATETIME NOT NULL,
    DecomissionDate DATETIME,
    Make VARCHAR(55) NOT NULL,
    Manufacturer VARCHAR(55) NOT NULL
);

CREATE TABLE ComputerEmployee (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    EmployeeId INTEGER NOT NULL,
    ComputerId INTEGER NOT NULL,
    AssignDate DATETIME NOT NULL,
    UnassignDate DATETIME,
    CONSTRAINT FK_ComputerEmployee_Employee FOREIGN KEY(EmployeeId) REFERENCES Employee(Id),
    CONSTRAINT FK_ComputerEmployee_Computer FOREIGN KEY(ComputerId) REFERENCES Computer(Id)
);

CREATE TABLE TrainingProgram (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    [Name] VARCHAR(255) NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    MaxAttendees INTEGER NOT NULL
);

CREATE TABLE EmployeeTraining (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    EmployeeId INTEGER NOT NULL,
    TrainingProgramId INTEGER NOT NULL,
    CONSTRAINT FK_EmployeeTraining_Employee FOREIGN KEY(EmployeeId) REFERENCES Employee(Id),
    CONSTRAINT FK_EmployeeTraining_Training FOREIGN KEY(TrainingProgramId) REFERENCES TrainingProgram(Id)
);

CREATE TABLE ProductType (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    [Name] VARCHAR(55) NOT NULL
);

CREATE TABLE Customer (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    FirstName VARCHAR(55) NOT NULL,
    LastName VARCHAR(55) NOT NULL
);

CREATE TABLE Product (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    ProductTypeId INTEGER NOT NULL,
    CustomerId INTEGER NOT NULL,
    Price INTEGER NOT NULL,
    Title VARCHAR(255) NOT NULL,
    [Description] VARCHAR(255) NOT NULL,
    Quantity INTEGER NOT NULL,
    CONSTRAINT FK_Product_ProductType FOREIGN KEY(ProductTypeId) REFERENCES ProductType(Id),
    CONSTRAINT FK_Product_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id)
);

CREATE TABLE PaymentType (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    AcctNumber INTEGER NOT NULL,
    [Name] VARCHAR(55) NOT NULL,
    CustomerId INTEGER NOT NULL,
    CONSTRAINT FK_PaymentType_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id)
);

CREATE TABLE [Order] (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    CustomerId INTEGER NOT NULL,
    PaymentTypeId INTEGER,
    CONSTRAINT FK_Order_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id),
    CONSTRAINT FK_Order_Payment FOREIGN KEY(PaymentTypeId) REFERENCES PaymentType(Id)
);

CREATE TABLE OrderProduct (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    OrderId INTEGER NOT NULL,
    ProductId INTEGER NOT NULL,
    CONSTRAINT FK_OrderProduct_Product FOREIGN KEY(ProductId) REFERENCES Product(Id),
    CONSTRAINT FK_OrderProduct_Order FOREIGN KEY(OrderId) REFERENCES [Order](Id)
);

INSERT INTO Customer
(FirstName, LastName)
VALUES
('Madison', 'Peper');

INSERT INTO Customer
(FirstName, LastName)
VALUES
('Matt', 'Hall');

INSERT INTO Customer
(FirstName, LastName)
VALUES
('Taylor', 'Gulley');

INSERT INTO ProductType
([Name])
VALUES
('Electronics');

INSERT INTO ProductType
([Name])
VALUES
('Alcohol');

INSERT INTO ProductType
([Name])
VALUES
('Books');

INSERT INTO PaymentType
(AcctNumber, [Name], CustomerId)
VALUES
(0123, 'Visa', 1);

INSERT INTO PaymentType
(AcctNumber, [Name], CustomerId)
VALUES
(1234, 'Discover', 2);

INSERT INTO PaymentType
(AcctNumber, [Name], CustomerId)
VALUES
(0987, 'MasterCard', 3);

INSERT INTO Product
(Price, Title, [Description], Quantity, CustomerId, ProductTypeId)
VALUES
(49, 'Pixel XL', 'We the best phone', 234, 1, 1);

INSERT INTO Product
(Price, Title, [Description], Quantity, CustomerId, ProductTypeId)
VALUES
(10, 'Yuengling', 'Americas Oldest Brewery', 24, 2, 2);

INSERT INTO Product
(Price, Title, [Description], Quantity, CustomerId, ProductTypeId)
VALUES
(24, 'The Hobbit', 'A short dude goes on an adventure.', 500, 3, 3);

INSERT INTO [Order]
(CustomerId, PaymentTypeId)
VALUES
(1, 1);

INSERT INTO [Order]
(CustomerId, PaymentTypeId)
VALUES
(2, 2);

INSERT INTO [Order]
(CustomerId, PaymentTypeId)
VALUES
(3, 3);

INSERT INTO Department
([Name], Budget)
VALUES
('Marketing', 50000);

INSERT INTO Department
([Name], Budget)
VALUES
('IT', 25000);

INSERT INTO Department
([Name], Budget)
VALUES
('Sales', 100000);

INSERT INTO Employee
(FirstName, LastName, DepartmentId, isSupervisor)
VALUES
('Madison', 'Peper', 2, 1);

INSERT INTO Employee
(FirstName, LastName, DepartmentId, IsSuperVisor)
VALUES
('Taylor', 'Gulley', 1, 1);

INSERT INTO Employee
(FirstName, LastName, DepartmentId, IsSuperVisor)
VALUES
('Matt', 'Hall', 3, 0);

INSERT INTO TrainingProgram
([Name], StartDate, EndDate, MaxAttendees)
VALUES
('BS Training', '20180704 09:00:00 AM', '20190108 12:00:00 PM', 30);

INSERT INTO TrainingProgram
([Name], StartDate, EndDate, MaxAttendees)
VALUES
('POS Training', '20180530 09:00:00 AM', '20180908 13:30:00 PM', 25);

INSERT INTO TrainingProgram
([Name], StartDate, EndDate, MaxAttendees)
VALUES
('How to Be Nice to Others', '20190319 09:00:00 AM', '20190610 20:00:00 PM', 50);

INSERT INTO Computer
(PurchaseDate, DecomissionDate, Make, Manufacturer)
VALUES
('20111010 07:00:00 AM', '20121010 15:00:00 PM', 'Schmapple III', 'Schmapple');

INSERT INTO Computer
(PurchaseDate, DecomissionDate, Make, Manufacturer)
VALUES
('20121010 07:00:00 AM', '20141210 17:00:00 PM', 'Schmurface Pro', 'Schmicrosoft');

INSERT INTO Computer
(PurchaseDate, DecomissionDate, Make, Manufacturer)
VALUES
('20170305 07:00:00 AM', '20181110 05:00:00 AM', 'Schmixel Book', 'Schmoogle');

INSERT INTO OrderProduct
(OrderId, ProductId)
VALUES
(3,3);

INSERT INTO OrderProduct
(OrderId, ProductId)
VALUES
(3,1);

INSERT INTO OrderProduct
(OrderId, ProductId)
VALUES
(2,2);