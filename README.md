# 🎓 OOP Laboratory Works (Об'єктно-орієнтоване програмування)

Репозиторій для виконання, тестування та здачі університетських лабораторних робіт з дисципліни **"Об'єктно-орієнтоване програмування" (C# / .NET)** через систему **GitHub Pull Requests**.

---

## 🏛️ Архітектура репозиторію (Multi-Solution Structure)

Для кожної лабораторної роботи виділяється **окреме незалежне рішення (`.sln`)** всередині каталогу `labs/LabXX/`.

```text
OOP_Lab/
├── .github/
│   ├── workflows/
│   │   └── ci.yml                     # Автоматичний запуск тестів і збірки в Pull Requests
│   └── pull_request_template.md       # Шаблон оформлення Pull Request для здачі роботи
├── labs/
│   ├── README.md                      # Гайд зі створення нового лабораторного рішення
│   ├── Lab01/                         # Лабораторна робота 1
│   │   ├── Lab01.sln                  # Рішення для Lab 1
│   │   ├── src/
│   │   │   ├── Lab01.Core/            # Бібліотека класів: моделі, бізнес-логіка, сервіси
│   │   │   └── Lab01.App/             # Консольний інтерфейс / клієнтська частина
│   │   ├── tests/
│   │   │   └── Lab01.Tests/           # Unit-тести (xUnit / NUnit)
│   │   └── README.md                  # Звіт по лабораторній роботі (варіант, опис, висновки)
│   ├── Lab02/                         # Лабораторна робота 2
│   │   ├── Lab02.sln
│   │   └── ...
├── .editorconfig                      # Єдині стандарти коду та форматування (C#)
├── .gitignore                         # Фільтрація артефактів .NET, IDE (Rider, VS) та ОС
└── README.md                          # Головна документація репозиторію
```

### Чому саме така структура?
1. **Повна ізоляція**: Кожна лабораторна робота відкривається в IDE (JetBrains Rider, Visual Studio, VS Code) як окремий автономний проєкт.
2. **Чиста історія та PR**: Викладач перевіряє виключно код поточної лабораторної роботи без конфліктів з попередніми чи наступними роботами.
3. **Автоматична перевірка (CI)**: GitHub Actions автоматично білдить та тестує зміни при створенні кожного Pull Request.

---

## 🔄 Процес виконання та здачі робіт (Pull Request Workflow)

Здача кожної лабораторної роботи відбувається через гілку (branch) та Pull Request у гілку `main`.

### Крок 1. Перехід на актуальну гілку `main`
Перед початком нової роботи завжди оновлюйте локальну гілку `main`:
```bash
git checkout main
git pull origin main
```

### Крок 2. Створення робочої гілки для нової лабораторної
Створіть нову гілку для відповідної лабораторної роботи:
```bash
# Формат назви: lab-01, lab-02, lab-03 і т.д.
git checkout -b lab-01
```

### Крок 3. Створення структури лабораторної роботи
Перейдіть у папку нової лаби та згенеруйте проєкти:
```bash
mkdir -p labs/Lab01 && cd labs/Lab01

# Створення solution
dotnet new sln -n Lab01

# Створення проєктів
dotnet new classlib -o src/Lab01.Core -f net8.0
dotnet new console -o src/Lab01.App -f net8.0
dotnet new xunit -o tests/Lab01.Tests -f net8.0

# Додавання проєктів до solution
dotnet sln Lab01.sln add src/Lab01.Core/Lab01.Core.csproj
dotnet sln Lab01.sln add src/Lab01.App/Lab01.App.csproj
dotnet sln Lab01.sln add tests/Lab01.Tests/Lab01.Tests.csproj

# Налаштування зв'язків між проєктами
dotnet add src/Lab01.App/Lab01.App.csproj reference src/Lab01.Core/Lab01.Core.csproj
dotnet add tests/Lab01.Tests/Lab01.Tests.csproj reference src/Lab01.Core/Lab01.Core.csproj

# Повернення в корінь репозиторію
cd ../..
```

### Крок 4. Розробка, тестування та фіксація змін
Реалізуйте необхідну бізнес-логіку відповідно до вашого варіанту. Перевірте, що код збирається та тести проходять:
```bash
dotnet build labs/Lab01/Lab01.sln
dotnet test labs/Lab01/Lab01.sln
```

Зробіть коміти змін (рекомендується використовувати зрозумілі префікси, наприклад Conventional Commits):
```bash
git add labs/Lab01/
git commit -m "feat(lab-01): implement models, streams, and unit tests"
```

### Крок 5. Відправка гілки на GitHub
```bash
git push -u origin lab-01
```

### Крок 6. Створення Pull Request
1. Перейдіть у репозиторій на GitHub.
2. Натисніть **Compare & pull request** для гілки `lab-01` у напрямку до `main` (`base: main` ← `compare: lab-01`).
3. Заповніть автоматично згенерований шаблон PR (`.github/pull_request_template.md`): вкажіть тему, варіант, виконані пункти.
4. Надішліть Pull Request на перевірку викладачу.

### Крок 7. Внесення правок (Review) та злиття (Merge)
- Якщо викладач залишив зауваження, внесіть виправлення у цій же локальній гілці `lab-01`, зробіть `git commit` та `git push` — Pull Request оновиться автоматично.
- Після успішної перевірки та схвалення (Approved) злийте PR у гілку `main` (Merge Pull Request).
- Для наступної лабораторної повторіть кроки, починаючи з **Кроку 1** (`git checkout main`, `git checkout -b lab-02`).

---

## 🛠️ Вимоги до оточення

- **.NET SDK:** 8.0 або новіший (`dotnet --version`)
- **IDE:** JetBrains Rider / Visual Studio 2022 / VS Code з C# Dev Kit
- **Git:** 2.x+
