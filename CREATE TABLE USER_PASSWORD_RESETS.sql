CREATE TABLE USER_PASSWORD_RESETS (
    IdReset           SERIAL PRIMARY KEY,
    UserId            INTEGER NOT NULL,
    TokenHash         VARCHAR(255) NOT NULL, -- Przechowujemy hash tokena, nie czysty tekst
    ExpiresAt         TIMESTAMP WITH TIME ZONE NOT NULL,
    CreatedAt         TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UsedAt            TIMESTAMP WITH TIME ZONE, -- NULL dopóki token nie zostanie użyty
    IpAddress         VARCHAR(45), -- Opcjonalnie dla celów audytowych i bezpieczeństwa
    
    -- Powiązanie z Twoją istniejącą tabelą USERS
    CONSTRAINT FK_PasswordResets_User FOREIGN KEY (UserId) 
        REFERENCES USERS(IdUser) ON DELETE CASCADE
);

-- Indeks przyspieszający wyszukiwanie tokena podczas weryfikacji
CREATE INDEX IX_PasswordResets_TokenHash ON USER_PASSWORD_RESETS(TokenHash);