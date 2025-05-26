-- DROP old table if present
DROP TABLE IF EXISTS Knowledge;

-- CREATE table with columns Keyword, Answer, Category
CREATE TABLE Knowledge (
  Id       INTEGER PRIMARY KEY AUTOINCREMENT,
  Keyword  TEXT    NOT NULL,
  Answer   TEXT    NOT NULL,
  Category TEXT    NOT NULL
);

-- SEED data: a massive, in depth set of tips
INSERT INTO Knowledge (Keyword, Answer, Category) VALUES
-- PASSWORD SECURITY
('password', 'Use strong unique passwords at least 12 characters long', 'Password Security'),
('password manager', 'Use a password manager to generate and store complex passwords securely', 'Password Security'),
('password reuse', 'Never reuse passwords across different accounts', 'Password Security'),
('password update', 'Change passwords periodically and immediately after any breach', 'Password Security'),

-- PHISHING AWARENESS
('phishing', 'Do not click links in unsolicited emails without verifying the sender', 'Phishing Awareness'),
('spear phishing', 'Be wary of highly personalized emails asking for sensitive data', 'Phishing Awareness'),
('smishing', 'Never respond to unexpected text messages requesting credentials or codes', 'Phishing Awareness'),
('vishing', 'Verify phone calls asking for personal info by calling the official number', 'Phishing Awareness'),
('phishing links', 'Hover over links to inspect the actual URL before clicking', 'Phishing Awareness'),
('phishing attachments', 'Do not open email attachments from unknown or suspicious sources', 'Phishing Awareness'),

-- NETWORK SECURITY
('wifi', 'Avoid public wifi for sensitive tasks or use a trusted vpn', 'Network Security'),
('vpn', 'Use a vpn on untrusted networks to encrypt all your traffic', 'Network Security'),
('network segmentation', 'Separate critical devices on their own network segments', 'Network Security'),
('firewall', 'Enable and configure your firewall to block unwanted inbound connections', 'Network Security'),
('secure router', 'Change default router credentials and keep firmware up to date', 'Network Security'),

-- MALWARE DEFENSE
('antivirus', 'Install reputable antivirus software and keep it updated', 'Malware Defense'),
('malware scan', 'Run regular full system scans to detect hidden malware', 'Malware Defense'),
('ransomware', 'Back up your files offline to recover from ransomware without paying', 'Malware Defense'),
('malware removal', 'If infected use official removal tools or reinstall from clean media', 'Malware Defense'),

-- DATA PROTECTION
('backup', 'Back up important data at least weekly and store copies offsite', 'Data Protection'),
('encryption', 'Encrypt sensitive files and use full disk encryption on laptops', 'Data Protection'),
('secure delete', 'Use secure wipe tools to permanently remove confidential files', 'Data Protection'),
('data retention', 'Only keep data as long as necessary and then securely delete it', 'Data Protection'),

-- PRIVACY
('privacy settings', 'Review and tighten privacy settings on social media accounts', 'Privacy'),
('metadata', 'Remove metadata from documents and images before sharing', 'Privacy'),
('tracking', 'Install browser extensions to block unwanted trackers and ads', 'Privacy'),
('data sharing', 'Only share personal information with trusted and verified parties', 'Privacy'),

-- AUTHENTICATION
('two factor', 'Enable two factor authentication on every service that supports it', 'Authentication'),
('multi factor', 'Use multi factor with hardware keys or authenticator apps', 'Authentication'),
('sms 2fa risks', 'Avoid sms based two factor when possible due to sim swap attacks', 'Authentication'),

-- SOFTWARE UPDATES
('patch', 'Install software and operating system updates promptly', 'Software Updates'),
('auto update', 'Enable automatic updates to ensure you never miss a security patch', 'Software Updates'),
('third party updates', 'Update browsers plugins and third party apps regularly', 'Software Updates'),

-- SOCIAL ENGINEERING
('social engineering', 'Be skeptical of unsolicited requests for information or help', 'Social Engineering'),
('pretexting', 'Do not trust callers claiming to be from support without verification', 'Social Engineering'),
('baiting', 'Avoid tempting free offers on usb drives or links from unknown sources', 'Social Engineering'),

-- MOBILE SECURITY
('mobile lock', 'Use a strong screen lock pin or biometric on your phone', 'Mobile Security'),
('app store', 'Only install apps from official app store and review permissions', 'Mobile Security'),
('os updates', 'Keep your mobile operating system up to date', 'Mobile Security'),

-- CLOUD SECURITY
('cloud backup', 'Encrypt files before uploading to cloud services', 'Cloud Security'),
('iam', 'Use identity and access management to grant least privilege', 'Cloud Security'),
('cloud monitoring', 'Enable logging and alerts for unusual activity in cloud accounts', 'Cloud Security'),

-- DEVELOPER SECURITY
('sql injection', 'Use parameterized queries or prepared statements to prevent sql injection', 'Developer Security'),
('xss', 'Sanitize and encode all user input to protect against cross site scripting', 'Developer Security'),
('csrf', 'Implement anti csrf tokens to prevent cross site request forgery', 'Developer Security'),
('secure coding', 'Follow secure coding guidelines and perform code reviews', 'Developer Security'),

-- INCIDENT RESPONSE
('incident response', 'Develop and test an incident response plan regularly', 'Incident Response'),
('forensics', 'Preserve logs and forensic images when investigating a breach', 'Incident Response'),
('communication plan', 'Establish internal and external communication protocols for incidents', 'Incident Response'),

-- PHYSICAL SECURITY
('badge', 'Always wear your identification badge in secure facilities', 'Physical Security'),
('tailgating', 'Do not allow others to follow you into restricted areas', 'Physical Security'),
('secure desk', 'Lock your workstation when away and secure physical documents', 'Physical Security'),

-- GENERAL TIPS
('ask help', 'If in doubt contact your it or security team immediately', 'General'),
('stay aware', 'Stay vigilant and report any suspicious activity without delay', 'General'),
('exit', 'exit', 'General')
;

