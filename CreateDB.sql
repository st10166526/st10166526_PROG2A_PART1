DROP TABLE IF EXISTS Knowledge;

CREATE TABLE Knowledge (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Question TEXT NOT NULL,
    Answer TEXT NOT NULL
);

INSERT INTO Knowledge (Question, Answer) VALUES 
-- Passwords
('password', 'Use strong, unique passwords for each account. A password manager helps.'),
('how do I make a strong password', 'Use a mix of upper/lowercase, numbers, and symbols. Avoid personal info.'),
('is my password safe', 'Try testing it at haveibeenpwned.com. Use long and unpredictable phrases.'),
-- Phishing
('phishing', 'Never click suspicious links. Verify sender details and spelling.'),
('what is phishing', 'Phishing is when attackers trick you into giving up sensitive info through fake emails or websites.'),
('how do I spot a phishing email', 'Look for urgent tone, unknown senders, and weird links or attachments.'),
-- 2FA
('2fa', 'Two-Factor Authentication adds a layer of security. Always enable it!'),
('should I use 2fa', 'Yes, it makes your account much harder to hack.'),
-- Malware
('malware', 'Malware includes viruses and spyware. Use antivirus software and avoid sketchy downloads.'),
('i think i have malware', 'Disconnect from internet, scan with antivirus, and change passwords.'),
-- Updates
('update', 'Keep your software and OS updated to patch vulnerabilities.'),
('should i update my apps', 'Yes. Updates often fix bugs and security holes.'),
-- Wi-Fi
('wifi', 'Avoid using public Wi-Fi for private info. Use a VPN if you have to.'),
('is public wifi dangerous', 'Yes. Hackers can intercept your traffic.'),
-- Hacking
('i think i got hacked', 'Change your passwords immediately, run antivirus, and enable 2FA.'),
('how do i know if i was hacked', 'Check for unknown devices, strange activity, and unexpected login attempts.'),
-- VPN
('vpn', 'A VPN encrypts your data. Use it on public Wi-Fi or when traveling.'),
('what does a vpn do', 'It hides your IP and secures your traffic.'),
-- Social Media
('facebook security', 'Enable 2FA, check active sessions, and be cautious of friend requests.'),
('instagram scam', 'Don’t click on “brand deals” or giveaways from unknown accounts.');


INSERT INTO Knowledge (Question, Answer) VALUES
-- Account Security
('How do I secure my email?', 'Use a strong password, enable two-factor authentication, and review login activity regularly.'),
('Is it safe to reuse passwords?', 'No, never reuse passwords. Use a unique one for every account to prevent credential stuffing.'),
('What is credential stuffing?', 'Its when attackers use stolen credentials from one site to try and access accounts on other sites.'),

-- Scams & Social Engineering
('What is social engineering?', 'Its the act of manipulating people into giving up confidential information or performing unsafe actions.'),
('How do I recognize a scam call?', 'Scam calls often demand urgent action or payment. Always verify through official sources before responding.'),
('What is vishing?', 'Voice phishing. A phone scam where attackers impersonate trusted entities to steal personal information.'),

-- Secure Browsing
('Is incognito mode safe?', 'Incognito mode hides your local history, but does not protect you from online tracking or malware.'),
('How do I know if a website is safe?', 'Check for HTTPS, a padlock icon, and verify the website address is correct.'),
('Can I trust browser extensions?', 'Only install extensions from trusted developers and review the permissions they request.'),

-- Mobile Security
('Are public charging stations safe?', 'Avoid them. Use your own power bank or a USB data blocker to prevent juice jacking.'),
('How do I secure my phone?', 'Use a screen lock, keep software updated, only download from official app stores, and enable remote wipe features.'),

-- Malware & Threats
('What is ransomware?', 'A type of malware that locks your files and demands payment. Don’t pay—recover from backups if possible.'),
('How do I know if I have a virus?', 'Symptoms include slow performance, pop-ups, and unknown programs. Run antivirus software to confirm.'),
('What is spyware?', 'Spyware secretly collects data from your device, often bundled with suspicious downloads or fake apps.'),

-- Updates & Backups
('How often should I back up my files?', 'Back up important data at least once a week. Use secure cloud storage or offline external drives.'),
('Why are software updates important?', 'Updates fix vulnerabilities that hackers can exploit to gain access to your system.'),

-- Wi-Fi Security
('Is my home Wi-Fi secure?', 'Use WPA2 or WPA3 encryption, change the default router password, and update firmware regularly.'),
('What is WPA3?', 'It is the latest Wi-Fi security protocol offering stronger encryption and better protection against attacks.'),

-- Workplace Security
('How can I stay secure at work?', 'Lock your screen when away, avoid unknown USB devices, report suspicious emails, and follow IT policies.'),
('What is data classification?', 'It is the process of labeling data by sensitivity (e.g., public, internal, confidential) to protect it appropriately.'),

-- Developer Tips
('How do I secure a website?', 'Use HTTPS, validate user input, sanitize data, and keep software dependencies updated.'),
('What is SQL injection?', 'An attack that manipulates database queries via input fields. Use parameterized queries to prevent it.'),
('What is XSS?', 'Cross-Site Scripting allows attackers to inject malicious scripts into websites. Always sanitize user input.'),

-- General Advice
('How do I report a cybercrime?', 'In South Africa, contact the SAPS Cybercrime Unit or visit www.cybersecurity.gov.za for official reporting channels.'),
('Is antivirus software enough?', 'It is helpful, but not enough. Safe browsing habits and regular updates are just as important.'),
('What is a zero-day vulnerability?', 'A security flaw that is exploited before the software developer releases a fix. These are highly dangerous.'),
('How can I learn cybersecurity?', 'Try platforms like TryHackMe, Hack The Box, and Cybrary. Practice ethical hacking in legal environments.');

DROP TABLE IF EXISTS Knowledge;
CREATE TABLE Knowledge (
    Id       INTEGER PRIMARY KEY AUTOINCREMENT,
    Keyword  TEXT    NOT NULL,
    Answer   TEXT    NOT NULL,
    Category TEXT    NOT NULL    -- helps us distinguish General vs Security topics
);

-- General chit-chat
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
('how are you',            'I am doing great, thanks for asking! How can I help you stay safe today?', 'General'),
('what is your purpose',   'I answer your cybersecurity questions and help keep you safe online.',       'General'),
('what can i ask you about','You can ask me about passwords, phishing, secure browsing, VPNs, malware, and more.', 'General');

-- Password security
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
('password',               'Use strong, unique passwords of at least 12 characters, and store them in a manager.', 'Password'),
('how do i make a strong password','Mix upper/lowercase, numbers & symbols in an unpredictable phrase.',   'Password'),
('is it safe to reuse passwords','Never reuse passwords—if one site leaks, attackers will try them everywhere!','Password');

-- Phishing
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
('phishing',               'Phishing tricks you into giving up info via fake emails/sites—always verify links.', 'Phishing'),
('how do i spot a phishing email','Look for urgent tone, typos, mismatched URLs, and unexpected attachments.','Phishing');

-- Secure browsing & Wi-Fi
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
('wifi',                   'Use WPA2/WPA3 encryption, change default router passwords, and update firmware.', 'Network'),
('is public wifi safe',    'Not without a VPN—public hotspots can snoop on your traffic.',              'Network'),
('what is wpa3',           'WPA3 is the latest Wi-Fi security standard, offering stronger encryption.',  'Network');

-- Malware & Antivirus
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
('malware',                'Malware includes viruses & spyware—run up-to-date antivirus and avoid shady downloads.', 'Malware'),
('how do i remove malware', 'Disconnect, boot in safe mode, run a full antivirus scan, and restore from clean backup if needed.', 'Malware'),
('ransomware',             'Ransomware encrypts your files and demands payment. Never pay—restore from backups.',        'Malware');

-- VPN
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
('vpn',                    'A VPN encrypts your internet traffic, keeping your data private on public networks.',       'Network'),
('what does a vpn do',     'It hides your IP and encrypts traffic so others can not snoop on your activity.',             'Network');


-- Malware Removal & Prevention
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
('how do i remove malware', 
 '1) Disconnect from the Internet.  
 2) Boot into Safe Mode.  
 3) Run a full scan with up-to-date antivirus/anti-malware software.  
 4) Quarantine or delete any threats found.  
 5) Reboot normally and check system performance.  
 6) Restore any lost files from a clean backup.', 
 'Malware'),

('malware removal steps', 
 'Safe Mode ➞ Full antivirus scan ➞ Quarantine/Delete threats ➞ Reboot ➞ Verify system health ➞ Restore from backup if needed.', 
 'Malware'),

('how to prevent malware', 
 '1) Keep OS & apps updated.  
 2) Use reputable antivirus.  
 3) Don not open unknown email attachments.  
 4) Only download software from official sources.  
 5) Enable your firewall.', 
 'Malware'),

-- Signs & Detection
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
('signs of malware', 
 'Slow performance, unexpected pop-ups, CPU spikes, new toolbars, programs opening by themselves, and corrupted files are common malware symptoms.', 
 'Malware'),

('how to detect malware', 
 'Use your antivirus to scan regularly, monitor Task Manager for odd CPU/RAM usage, and check for unknown startup entries (msconfig or Task Manager > Startup).', 
 'Malware'),

-- Hacked / Breach Recovery
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
('i think i got hacked', 
 '1) Disconnect from the Internet.  
 2) Change passwords from a clean device.  
 3) Scan your system with antivirus.  
 4) Enable 2FA on all accounts.  
 5) Check account activity/logs for unauthorized actions.', 
 'Breach'),

('what to do if hacked', 
 'Immediately change all passwords, run security scans, enable 2FA, review account logs, and notify affected contacts if needed.', 
 'Breach'),

('how to recover from hack', 
 'Clean your device with anti-malware, update all passwords (use a manager), alert your email/payment providers, and monitor accounts for 30 days.', 
 'Breach'),

-- Proactive Measures
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
('how to secure my router', 
 'Change default admin credentials, update firmware, disable WPS, use WPA3/WPA2 encryption, and place it in a secure location.', 
 'Network'),

('should i use a firewall', 
 'Yes—enable the built-in OS firewall or a hardware firewall on your router to block unsolicited inbound connections.', 
 'Network'),

-- Account Security
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
('what to do after password leak', 
 'If your password appears in a breach, immediately change it everywhere, enable 2FA, and consider a password manager for unique credentials.', 
 'Password'),

('how to safely reset passwords', 
 'Use a clean device, navigate directly to the official site (no email links), choose a long unique password, and enable 2FA.', 
 'Password'),

-- Phishing Specifics
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
('what is spear phishing', 
 'A targeted phishing attack where criminals personalize messages using your info to trick you into revealing data.', 
 'Phishing'),

('how to report phishing', 
 'Forward phishing emails to your email provider (e.g. phishing@domain.com), report to anti-phishing orgs, and inform your IT department.', 
 'Phishing');

DROP TABLE IF EXISTS Knowledge;
CREATE TABLE Knowledge (
    Id       INTEGER PRIMARY KEY AUTOINCREMENT,
    Keyword  TEXT    NOT NULL,
    Answer   TEXT    NOT NULL,
    Category TEXT    NOT NULL    -- helps us distinguish General vs Security topics
);
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
  ('how do i remove malware', 
   '1) Disconnect from the Internet.
    2) Boot into Safe Mode.
    3) Run a full scan with up-to-date antivirus/anti-malware software.
    4) Quarantine or delete any threats found.
    5) Reboot normally and check system performance.
    6) Restore any lost files from a clean backup.',
   'Malware'),
  ('malware removal steps', 
   'Safe Mode ➞ Full antivirus scan ➞ Quarantine/Delete threats ➞ Reboot ➞ Verify system health ➞ Restore from backup if needed.',
   'Malware'),
  ('how to prevent malware', 
   '1) Keep OS & apps updated.
    2) Use reputable antivirus.
    3) Don not open unknown email attachments.
    4) Only download software from official sources.
    5) Enable your firewall.',
   'Malware');

-- Signs & Detection
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
  ('signs of malware', 
   'Slow performance, unexpected pop-ups, CPU spikes, new toolbars, programs opening by themselves, and corrupted files are common malware symptoms.',
   'Malware'),
  ('how to detect malware', 
   'Use your antivirus to scan regularly, monitor Task Manager for odd CPU/RAM usage, and check for unknown startup entries (msconfig or Task Manager > Startup).',
   'Malware');

-- Hacked / Breach Recovery
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
  ('i think i got hacked', 
   '1) Disconnect from the Internet.
    2) Change passwords from a clean device.
    3) Scan your system with antivirus.
    4) Enable 2FA on all accounts.
    5) Check account activity/logs for unauthorized actions.',
   'Breach'),
  ('what to do if hacked', 
   'Immediately change all passwords, run security scans, enable 2FA, review account logs, and notify affected contacts if needed.',
   'Breach'),
  ('how to recover from hack', 
   'Clean your device with anti-malware, update all passwords (use a manager), alert your email/payment providers, and monitor accounts for 30 days.',
   'Breach');

-- Proactive Measures
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
  ('how to secure my router', 
   'Change default admin credentials, update firmware, disable WPS, use WPA3/WPA2 encryption, and place it in a secure location.',
   'Network'),
  ('should i use a firewall', 
   'Yes—enable the built-in OS firewall or a hardware firewall on your router to block unsolicited inbound connections.',
   'Network');

-- Account Security
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
  ('what to do after password leak', 
   'If your password appears in a breach, immediately change it everywhere, enable 2FA, and consider a password manager for unique credentials.',
   'Password'),
  ('how to safely reset passwords', 
   'Use a clean device, navigate directly to the official site (no email links), choose a long unique password, and enable 2FA.',
   'Password');

-- Phishing Specifics
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
  ('what is spear phishing', 
   'A targeted phishing attack where criminals personalize messages using your info to trick you into revealing data.',
   'Phishing'),
  ('how to report phishing', 
   'Forward phishing emails to your email provider (e.g. phishing@domain.com), report to anti-phishing orgs, and inform your IT department.',
   'Phishing');


   -- Breach Recovery Steps
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
  ('breach-step1',
   '1) Immediately change all passwords, run security scans, enable 2FA, review account logs, and notify affected contacts if needed.',
   'Breach'),
  ('breach-step2',
   '2) Monitor your credit reports and bank statements for the next 90 days.',
   'Breach'),
  ('breach-step3',
   '3) Check for unknown devices in your account security settings and remove them.',
   'Breach'),
  ('breach-step4',
   '4) Update your security questions and answers to something no one else knows.',
   'Breach');
