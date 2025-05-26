-- drop any old version
DROP TABLE IF EXISTS Knowledge;

-- note: column is now Keyword, not Question
CREATE TABLE Knowledge (
    Id       INTEGER PRIMARY KEY AUTOINCREMENT,
    Keyword  TEXT    NOT NULL,
    Answer   TEXT    NOT NULL,
    Category TEXT    NOT NULL
);

-- seed your data; NO trailing comma on the last row
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
('password', 'Use strong, unique passwords for each account. A password manager helps.', 'Password'),
('how do i make a strong password', 'Use a mix of upper/lowercase, numbers, and symbols. Avoid personal info.', 'Password'),
('is my password safe', 'Try testing it at haveibeenpwned.com. Use long and unpredictable phrases.', 'Password'),
('phishing', 'Never click suspicious links. Verify sender details and spelling.', 'Phishing'),
('what is phishing', 'Phishing is when attackers trick you into giving up sensitive info via fake emails or websites.', 'Phishing'),
('how do i spot a phishing email', 'Look for urgent tone, unknown senders, and weird links or attachments.', 'Phishing'),
('2fa', 'Two-Factor Authentication adds a layer of security. Always enable it!', '2FA'),
('should i use 2fa', 'Yes, it makes your account much harder to hack.', '2FA'),
('malware', 'Malware includes viruses and spyware. Use antivirus software and avoid sketchy downloads.', 'Malware'),
('i think i have malware', 'Disconnect from internet, scan with antivirus, and change passwords.', 'Malware'),
('update', 'Keep your software and OS updated to patch vulnerabilities.', 'Updates'),
('should i update my apps', 'Yes. Updates often fix bugs and security holes.', 'Updates'),
('wifi', 'Avoid using public Wi-Fi for private info. Use a VPN if you have to.', 'Wi-Fi'),
('is public wifi dangerous', 'Yes. Hackers can intercept your traffic.', 'Wi-Fi'),
('i think i got hacked', 'Change your passwords immediately, run antivirus, and enable 2FA.', 'Breach'),
('how do i know if i was hacked', 'Check for unknown devices, strange activity, and unexpected login attempts.', 'Breach'),
('vpn', 'A VPN encrypts your data. Use it on public Wi-Fi or when traveling.', 'Network'),
('what does a vpn do', 'It hides your IP and secures your traffic.', 'Network'),
('facebook security', 'Enable 2FA, check active sessions, and be cautious of friend requests.', 'Social Media'),
('instagram scam', 'Dont click on “brand deals” or giveaways from unknown accounts.', 'Social Media'),
('how do i secure my email', 'Use a strong password, enable two factor authentication, and review login activity regularly.', 'Account Security'),
('is it safe to reuse passwords', 'No, never reuse passwords. Use a unique one for every account to prevent credential stuffing.', 'Account Security'),
('what is credential stuffing', 'When attackers use stolen credentials from one site to try and access accounts on other sites.', 'Account Security'),
('what is social engineering', 'The act of manipulating people into giving up confidential information.', 'Scams & Social Engineering'),
('how do i recognize a scam call', 'Scam calls often demand urgent action or payment. Always verify through official sources before responding.', 'Scams & Social Engineering'),
('what is vishing', 'Voice phishing. A phone scam where attackers impersonate trusted entities to steal personal information.', 'Scams & Social Engineering'),
('is incognito mode safe', 'Incognito mode hides your local history but doesnt protect you from online tracking or malware.', 'Secure Browsing'),
('how do i know if a website is safe', 'Check for HTTPS, a padlock icon, and verify the website address is correct.', 'Secure Browsing'),
('are public charging stations safe', 'Avoid them. Use your own power bank or a USB data blocker to prevent juice jacking.', 'Mobile Security'),
('how do i secure my phone', 'Use a screen lock, keep software updated, only download from official app stores, and enable remote wipe features.', 'Mobile Security'),
('what is ransomware', 'A type of malware that locks your files and demands payment. Recover from backups if possible—never pay.', 'Malware & Threats'),
('how do i know if i have a virus', 'Symptoms include slow performance, pop-ups, and unknown programs. Run antivirus software to confirm.', 'Malware & Threats'),
('what is spyware', 'Spyware secretly collects data from your device, often bundled with suspicious downloads or fake apps.', 'Malware & Threats'),
('how often should i back up my files', 'Back up important data at least once a week. Use secure cloud storage or offline external drives.', 'Updates & Backups'),
('why are software updates important', 'Updates fix vulnerabilities that hackers can exploit to gain access to your system.', 'Updates & Backups'),
('how can i stay secure at work', 'Lock your screen when away, avoid unknown USB devices, report suspicious emails, and follow IT policies.', 'Workplace Security'),
('what is data classification', 'Label data by sensitivity (e.g., public, internal, confidential) to protect it correctly.', 'Workplace Security'),
('how do i secure a website', 'Use HTTPS, validate user input, sanitize data, and keep dependencies updated.', 'Developer Tips'),
('what is sql injection', 'An attack that manipulates database queries via input fields. Use parameterized queries to prevent it.', 'Developer Tips'),
('what is xss', 'Cross-Site Scripting allows attackers to inject malicious scripts into websites. Always sanitize user input.', 'Developer Tips'),
('how do i report a cybercrime', 'In South Africa, contact the SAPS Cybercrime Unit or visit www.cybersecurity.gov.za.', 'General'),
('is antivirus software enough', 'Helpful but not enough. Safe browsing habits and regular updates are just as important.', 'General'),
('what is a zero-day vulnerability', 'A flaw exploited before a fix is released. Highly dangerous—patch ASAP.', 'General'),
('how can i learn cybersecurity', 'Try platforms like TryHackMe, Hack The Box, and Cybrary. Practice ethical hacking legally.', 'General')
;
