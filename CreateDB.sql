CREATE TABLE IF NOT EXISTS Knowledge (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Keyword TEXT NOT NULL,
    Response TEXT NOT NULL
);

INSERT INTO Knowledge (Keyword, Response) VALUES
('password', 'Use strong, unique passwords and a password manager.'),
('phishing', 'Phishing tricks users into giving up private info. Never click suspicious links.'),
('vpn', 'A VPN encrypts your internet traffic, hiding it from snoopers.'),
('malware', 'Malware is software designed to harm your device. Always use antivirus.'),
('2fa', 'Two-factor authentication adds extra protection to your logins.');
