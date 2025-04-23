# ST10166526_PROG2A_PART1
# 🛡️ Cybersecurity Awareness Bot

A beginner-friendly **C# console application** that uses **audio**, **ASCII art**, and **interactive chat** to teach essential cybersecurity tips in a fun and engaging way.

---

## 🎯 Project Overview

This chatbot helps users learn about common cyber threats like phishing, weak passwords, unsafe Wi-Fi, and more — using simple language and instant feedback.

---

## ✨ Features

- 🔊 Plays a **voice greeting** (`greeting.wav`)
- 🎨 Displays a colorful **ASCII art logo**
- 🧑‍💻 Accepts name input for personalized welcome
- 🤖 Responds to common **cybersecurity questions**
- ⚠️ Warns about invalid or unclear questions
- 🎨 Uses **colored console text** for clarity
- 🧪 Integrated with **GitHub Actions** for CI
- 🗃️ Clean file structure and `.gitignore` setup

---

## 🚀 To Run

```bash
cd CyberSecurityBot
dotnet restore
dotnet run


📂 File Structure
CyberSecurityBot/
├── Program.cs                 # Main bot logic
├── greeting.wav              # Audio greeting
├── ascii_logo.txt            # Logo displayed at launch
├── .gitignore                # Git exclusions
├── .github/workflows/        # CI setup
│   └── dotnet.yml


💡 Example Questions
You can ask:

"How do I create a strong password?"

"What is phishing?"

"Should I use 2FA?"

"Can I trust public Wi-Fi?"

Or type "exit" to leave the chatbot.

✅ GitHub Actions CI
This project uses GitHub Actions to run:

🔧 Build and restore packages

✅ Compile and verify with .NET SDK

🔄 Ensures reliability and code quality

🙌 Made With
💻 C# (.NET)

🔉 System.Media

🎨 Console color formatting

❤️ Passion for teaching cybersecurity

🔐 Author
Aidan Causton
Cybersecurity Enthusiast • Developer • AI Explorer
GitHub

Stay safe. Stay smart. And never click that suspicious link. 😉