# ST10166526_PROG2A_PART1
# CyberSecurityBot

![.NET Build & Test](https://github.com/st10166526/ST10166526_PROG2A_PART1/actions/workflows/dotnet.yml/badge.svg)

A **console-based** cybersecurity assistant built in C# and .NET, featuring:

- **Voice & Typing Greeting**: Plays `greeting.wav` while typing a welcome message
- **ASCII-Art Banner**: Displays `ascii_logo.txt` on startup
- **Typing Effect**: All responses are typed out character-by-character
- **SQLite Knowledge Base**: Q&A stored in `knowledge.db` seeded from `CreateDB.sql`
- **Intelligent Matching**: Keyword-based category routing + fuzzy text similarity
- **Contextual Memory**: Multi-step follow-ups (e.g., “What else can I do?”)
- **Slash Commands**:
  - `/list`: Show all available topics
  - `/add`: Add new Q&A entries interactively
  - `/history`: View previous inputs
  - `/reset`: Clear conversation memory

---

## 📸 Screenshots

> ![Checks Passed](C:\Users\Client\Documents\CyberSecurityBot\docs\screenshots\All_checks_passed.png)
> ![Confirm Checks Passed](C:\Users\Client\Documents\CyberSecurityBot\docs\screenshots\CheckedRefresh.png)


---

## 🛠️ Setup & Run Locally

1. **Clone the repository**

   ```bash
   git clone https://github.com/st10166526/ST10166526_PROG2A_PART1.git
   cd ST10166526_PROG2A_PART1
   ```

2. **Ensure required files exist**:
   - `greeting.wav`
   - `ascii_logo.txt`
   - `CreateDB.sql` (database schema & seed)

3. **Build and run** (first run auto-creates `knowledge.db`):

   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```

4. **Interact**:
   - Answer the name prompt
   - Ask questions like:
     - `What is phishing?`
     - `How do I remove malware?`
   - Use slash commands:
     - `/list`, `/add`, `/history`, `/reset`

---

## 🔍 Demonstration Scenarios

1. **Basic Q&A**: Ask `What is phishing?` or `How are you?`
2. **Database Topics**: `What is WPA3?`, `What is spear phishing?`
3. **Add New Entry**: `/add` and verify via `/list`
4. **Contextual Follow-up**:
   ```
   You: I think I got hacked
   Bot: [step1 recovery advice]
   You: What else can I do?
   Bot: [step2 recovery advice]
   ```
5. **History & Reset**: `/history` then `/reset`

---

## 📂 Project Structure

```
├── CyberSecurityBot.sln
├── Program.cs
├── KnowledgeBase.cs
├── DatabaseSetup.cs
├── CreateDB.sql
├── ascii_logo.txt
├── greeting.wav
├── README.md
└── docs
    └── screenshots
        ├── ci_green_check.png
        ├── greeting_typing.gif
        └── console_run.png
```