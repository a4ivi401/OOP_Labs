# Лабораторні роботи з ООП (OOP Labs)

Ця директорія містить окремі лабораторні роботи. Кожна лабораторна робота є **повністю ізольованим рішенням (.sln)** зі своїми власними проектами (бібліотека класів, консольний додаток, тести).

---

## 📁 Структура директорії

```text
labs/
├── Lab01/                  # Лабораторна робота №1
│   ├── Lab01.sln           # Власний файл рішення для Lab 1
│   ├── src/
│   │   ├── Lab01.Core/     # Бізнес-логіка, моделі, інтерфейси
│   │   └── Lab01.App/      # Консольний інтерфейс користувача (UI)
│   ├── tests/
│   │   └── Lab01.Tests/    # Модульні тести (xUnit / NUnit)
│   └── README.md           # Опис варіанту, завдання та звіт
│
├── Lab02/                  # Лабораторна робота №2
│   ├── Lab02.sln
│   ├── ...
│
└── ...
```

---

## 🚀 Створення нової лабораторної роботи

Щоб створити нову лабораторну роботу через .NET CLI:

```bash
# 1. Перейдіть у папку нової лаби
mkdir -p labs/Lab01 && cd labs/Lab01

# 2. Створіть файл рішення (.sln)
dotnet new sln -n Lab01

# 3. Створіть бібліотеку класів (Core)
dotnet new classlib -o src/Lab01.Core -f net8.0

# 4. Створіть консольний застосунок (App / Presentation)
dotnet new console -o src/Lab01.App -f net8.0

# 5. Створіть проект модульних тестів (xUnit)
dotnet new xunit -o tests/Lab01.Tests -f net8.0

# 6. Додайте проекти до рішення
dotnet sln Lab01.sln add src/Lab01.Core/Lab01.Core.csproj
dotnet sln Lab01.sln add src/Lab01.App/Lab01.App.csproj
dotnet sln Lab01.sln add tests/Lab01.Tests/Lab01.Tests.csproj

# 7. Налаштуйте зв'язки між проектами
dotnet add src/Lab01.App/Lab01.App.csproj reference src/Lab01.Core/Lab01.Core.csproj
dotnet add tests/Lab01.Tests/Lab01.Tests.csproj reference src/Lab01.Core/Lab01.Core.csproj

# 8. Перевірте збірку
dotnet build Lab01.sln
dotnet test Lab01.sln
```

---

## 🎯 Переваги окремих рішень (.sln)

1. **Ізоляція залежностей**: Помилки компіляції в Лабораторній 2 не блокують запуск Лабораторної 1.
2. **Чистий Pull Request**: У PR потрапляють зміни лише конкретної лаби.
3. **Швидкість IDE**: JetBrains Rider або Visual Studio відкриває лише поточну лабу без навантаження від інших робіт.
