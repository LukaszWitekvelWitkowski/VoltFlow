INSERT INTO "Email_Templates" ("EmailType", "Name", "Subject", "BodyHtml", "LastUpdated")
VALUES 
(1, 'Promocja CRM', 'Mamy dla Ciebie specjalną ofertę!', 
 '<h1>Witaj {{ClientName}}!</h1><p>Tylko w tym miesiącu przygotowaliśmy dla Ciebie 20% rabatu na wszystkie nasze usługi.</p><p>Użyj kodu: <strong>VOLT2026</strong></p>', 
 NOW()),

(2, 'Ponowienie pomiarów / Ubezpieczenie', 'Termin Twojego ubezpieczenia dobiega końca', 
 '<h1>Cześć {{ClientName}}</h1><p>Przypominamy, że ubezpieczenie dla Twojego obiektu wygasa wkrótce.</p><p>Prosimy o umówienie terminu ponownych pomiarów technicznych, aby zachować ciągłość ochrony.</p><p>Pozdrawiamy, Zespół VoltFlow.</p>', 
 NOW()),

(3, 'Zaległa płatność', 'Przypomnienie o zaległej fakturze', 
 '<h1 style="color: red;">Uwaga: Zaległość w płatności</h1><p>Szanowny Kliencie ({{ClientName}}),</p><p>Informujemy, że na Twoim koncie widnieje zaległość w kwocie <strong>{{Amount}}</strong>.</p><p>Termin płatności upłynął: {{DueDate}}. Prosimy o niezwłoczne uregulowanie należności.</p>', 
 NOW()),

(4, 'Reset hasła', 'Instrukcja resetowania hasła', 
 '<h1>Reset hasła</h1><p>Otrzymaliśmy prośbę o reset hasła dla konta powiązanego z tym adresem e-mail.</p><p>Kliknij w poniższy link, aby ustawić nowe hasło (ważny 1 godzinę):</p><a href="{{ResetLink}}">Resetuj hasło</a>', 
 NOW());