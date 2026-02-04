CREATE TABLE Categories (
    IdCategory SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    IsObsolete BOOLEAN NOT NULL
);

CREATE TABLE Roles (
    IdRole SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL
);

CREATE TABLE Users (
    IdUser SERIAL PRIMARY KEY,
    RoleId INTEGER NOT NULL,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    TenantId INTEGER NOT NULL,
    StatusType SMALLINT NOT NULL,
    CONSTRAINT fk_users_role
        FOREIGN KEY (RoleId) REFERENCES Roles(IdRole)
);

CREATE TABLE Clients (
    IdClient SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Phone VARCHAR(15) NOT NULL,
    TenantId INTEGER NOT NULL
);

CREATE TABLE ClientAddress (
    IdAddress SERIAL PRIMARY KEY,
    ClientId INTEGER NOT NULL,
    City VARCHAR(75) NOT NULL,
    Street VARCHAR(100) NOT NULL,
    ZipCode VARCHAR(75) NOT NULL,
    NumberStreet VARCHAR(15) NOT NULL,
    TypeAddress SMALLINT NOT NULL,
    LocationNumber VARCHAR(15),
    IsObsolete BOOLEAN NOT NULL,
    CONSTRAINT fk_address_client
        FOREIGN KEY (ClientId) REFERENCES Clients(IdClient)
);

CREATE TABLE ElementGroup (
    IdElementGroup SERIAL PRIMARY KEY,
    CategoryId INTEGER NOT NULL,
    Name VARCHAR(100) NOT NULL,
    IsObsolete BOOLEAN NOT NULL,
    CONSTRAINT fk_elementgroup_category
        FOREIGN KEY (CategoryId) REFERENCES Categories(IdCategory)
);

CREATE TABLE Element (
    IdElement SERIAL PRIMARY KEY,
    ElementGroupId INTEGER NOT NULL,
    Name VARCHAR(100) NOT NULL,
    IsObsolete BOOLEAN NOT NULL,
    Description VARCHAR(255),
    CONSTRAINT fk_element_group
        FOREIGN KEY (ElementGroupId) REFERENCES ElementGroup(IdElementGroup)
);

CREATE TABLE Warehouse (
    IdWarehouse SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Location SMALLINT NOT NULL,
    IsObsolete BOOLEAN NOT NULL,
    JobId INTEGER
);

CREATE TABLE Stock (
    IdStock SERIAL PRIMARY KEY,
    ElementId INTEGER NOT NULL,
    Quantity INTEGER NOT NULL,
    LastUpdated TIMESTAMP NOT NULL,
    RowVersion BIGINT NOT NULL,
    WarehouseId INTEGER NOT NULL,
    CONSTRAINT fk_stock_element
        FOREIGN KEY (ElementId) REFERENCES Element(IdElement),
    CONSTRAINT fk_stock_warehouse
        FOREIGN KEY (WarehouseId) REFERENCES Warehouse(IdWarehouse)
);

CREATE TABLE Jobs (
    IdJob SERIAL PRIMARY KEY,
    ClientId INTEGER NOT NULL,
    AddressId INTEGER NOT NULL,
    StatusId SMALLINT NOT NULL,
    Date TIMESTAMP NOT NULL,
    CONSTRAINT fk_jobs_client
        FOREIGN KEY (ClientId) REFERENCES Clients(IdClient),
    CONSTRAINT fk_jobs_address
        FOREIGN KEY (AddressId) REFERENCES ClientAddress(IdAddress)
);

CREATE TABLE Tasks (
    IdTask SERIAL PRIMARY KEY,
    JobId INTEGER NOT NULL,
    Description VARCHAR(255) NOT NULL,
    StatusId SMALLINT NOT NULL,
    TypeTaskId SMALLINT NOT NULL,
    CONSTRAINT fk_tasks_job
        FOREIGN KEY (JobId) REFERENCES Jobs(IdJob)
);

CREATE TABLE Transaction (
    IdTransaction SERIAL PRIMARY KEY,
    StockId INTEGER NOT NULL,
    JobId INTEGER,
    SourceId INTEGER,
    TargetId INTEGER,
    Quantity INTEGER NOT NULL,
    TransactionTypeId SMALLINT NOT NULL,
    Date TIMESTAMP NOT NULL,
    CreateById SMALLINT NOT NULL,
    CONSTRAINT fk_transaction_stock
        FOREIGN KEY (StockId) REFERENCES Stock(IdStock),
    CONSTRAINT fk_transaction_job
        FOREIGN KEY (JobId) REFERENCES Jobs(IdJob),
    CONSTRAINT fk_transaction_source
        FOREIGN KEY (SourceId) REFERENCES Warehouse(IdWarehouse),
    CONSTRAINT fk_transaction_target
        FOREIGN KEY (TargetId) REFERENCES Warehouse(IdWarehouse)
);

CREATE TABLE TransactionLog (
    IdTransactionLog SERIAL PRIMARY KEY,
    TransactionId INTEGER NOT NULL,
    Timestamp TIMESTAMP NOT NULL,
    CreateById SMALLINT NOT NULL,
    Details VARCHAR(255),
    TransactionTypeId SMALLINT NOT NULL,
    CONSTRAINT fk_transactionlog_transaction
        FOREIGN KEY (TransactionId) REFERENCES Transaction(IdTransaction)
);

CREATE TABLE EventJob (
    IdEventJob SERIAL PRIMARY KEY,
    JobId INTEGER NOT NULL,
    StatusId SMALLINT NOT NULL,
    EventDetails VARCHAR(255) NOT NULL,
    TypeEventId SMALLINT NOT NULL,
    CONSTRAINT fk_eventjob_job
        FOREIGN KEY (JobId) REFERENCES Jobs(IdJob)
);

CREATE TABLE EventJobLog (
    IdEventJobLog SERIAL PRIMARY KEY,
    EventJobId INTEGER NOT NULL,
    Status VARCHAR(50) NOT NULL,
    Timestamp TIMESTAMP NOT NULL,
    NotificationStatusId SMALLINT NOT NULL,
    Processed SMALLINT NOT NULL,
    TypeEventId SMALLINT NOT NULL,
    CONSTRAINT fk_eventjoblog_eventjob
        FOREIGN KEY (EventJobId) REFERENCES EventJob(IdEventJob)
);

CREATE TABLE ErrorLog (
    IdErrorLog SERIAL PRIMARY KEY,
    ProcessId SMALLINT NOT NULL,
    Level SMALLINT NOT NULL,
    Name VARCHAR(255) NOT NULL,
    Message VARCHAR(500) NOT NULL,
    Timestamp TIMESTAMP NOT NULL,
    ContextTypeId SMALLINT NOT NULL,
    ContextId INTEGER NOT NULL
);


INSERT INTO categorie ("IdCategory", "Name", "IsObsolete") VALUES
(1, 'Elektronika', false),
(2, 'Podzespoły Mechaniczne', false),
(3, 'Materiały Eksploatacyjne', true);

INSERT INTO ElementGroup (IdElementGroup, Name, IsObsolete, CategoryId)
VALUES 
(1, 'Rezystory', false, 1),
(2, 'Łożyska Kulkowe', false, 2),
(3, 'Kable i Przewody', false, 1);



-- 2. Tabela ELEMENTGROUP (powiązana z Category)
INSERT INTO "ElementGroup" ("IdElementGroup", "Name", "IsObsolete", "CategoryId") VALUES
(1, 'Rezystory', false, 1),
(2, 'Łożyska Kulkowe', false, 2),
(3, 'Kable i Przewody', false, 1);

-- 3. Tabela ELEMENT (powiązana z ElementGroup)
INSERT INTO Element (IdElement, Name, Description, IsObsolete, ElementGroupId) VALUES
(1, 'Rezystor 10k Ohm', 'Rezystor węglowy 0.25W', false, 1),
(2, 'Łożysko 608-ZZ', 'Łożysko do silników elektrycznych', false, 2),
(3, 'Przewód miedziany 1.5mm', 'Przewód instalacyjny jednożyłowy', false, 3);