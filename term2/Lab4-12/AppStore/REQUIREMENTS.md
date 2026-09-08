# Требования к ПС «Магазин приложений»

Сформировано по фактической реализации проекта `term2/Lab4-12/AppStore` (commit `23711d2`). Каждое требование привязано к коду; реализованного здесь — только то, что реально присутствует в проекте.

---

## 1. Функциональные требования (FR)

### 1.1. Аутентификация и роли

| № | Требование | Реализация |
|---|------------|-----------|
| FR-1 | Вход в систему по логину и паролю; при неверных данных — сообщение об ошибке и подсветка полей; вход по клавише Enter | `Views/LoginWindow.xaml.cs` (`TryLogin`), `Services/AuthService.Login` |
| FR-2 | Пароли не хранятся в открытом виде — хеширование SHA-256 | `Services/AuthService.HashPassword`, `Data/BaseRepository.HashPassword` |
| FR-3 | Две роли: Администратор и Пользователь (`UserRole.Admin` / `UserRole.User`); роль определяет видимость административных функций | `Data/IModels.cs`, `AuthService.IsAdmin`, `Visibility="{Binding IsAdmin}"` |
| FR-4 | Выход из системы с возвратом к окну входа; закрытие окна каталога завершает приложение | `ApplicationApp.OpenCatalog`, `MainViewModel.LogoutRequested`, `ProfilePopup.OnLogout` |
| FR-5 | Учётные записи разделяют данные: список установленных приложений и история отмен привязаны к текущему пользователю | `UsersApps`, `JsonRep.SetCurrentUser`, `UndoRedoService.Clear()` при выходе |

### 1.2. Функции клиента (пользователя)

| № | Требование | Реализация |
|---|------------|-----------|
| FR-6 | Просмотр каталога приложений с карточками (название, разработчик, рейтинг, цена, иконка-цвет) | `Views/AppRowView`, `MainWindow.xaml` |
| FR-7 | Блок «Выбор дня» (featured-приложения) — баннеры на главной | `MainViewModel.FeaturedApps`, `Views/FeaturedBannerView` |
| FR-8 | Навигация по разделам: Обзор, Создать (Design), Работа (Productivity), Игры (Games), Разработка (Development), Установленные, Категории | `MainViewModel.NavCategoryMap`, боковое меню `MainWindow.xaml` |
| FR-9 | Просмотр полной карточки приложения: описание, версия, разработчик, страна, возрастной рейтинг, размер, теги, дата релиза, скидка | `Views/AppDetailView.xaml` |
| FR-10 | Поиск по названию (краткому и полному), разработчику, категории и тегам | `MainViewModel.BuildQuery` (поиск приоритетнее фильтров) |
| FR-11 | Фильтрация: по цене (мин/макс, с учётом скидочной цены), по минимальному рейтингу, только установленные | `MinPrice`, `MaxPrice`, `MinRating`, `DownloadedOnly`, popup фильтров `OnFilterClick` |
| FR-12 | Сортировка: рекомендуемые, по названию, по цене, по рейтингу, по дате релиза | `MainViewModel.BuildQuery` (switch по `SortBy`) |
| FR-13 | Фильтрация по тегам (облако тегов, собирается из всех приложений) | `AllTags`, `SelectTagCommand` |
| FR-14 | «Показать все» — раскрытие списка с 6 позиций до полного и обратно | `ToggleSeeAllCommand`, `_visibleCount` |
| FR-15 | Установка / удаление приложения; кнопка меняет подпись: «Установить» / «Открыть» / цена | `MainViewModel.DownloadCommand`, `IRepository.DownloadApp/UninstallApp`, `ButtonLabelConverter` |
| FR-16 | Форматирование цены в валюте текущего языка (₽, €, $, BYN); бесплатные — «Бесплатно» | `Converters.PriceConverter`, `Converters.ButtonLabelConverter` |
| FR-17 | Отмена и повтор действий установки/удаления (Undo/Redo), глубина истории — 10 операций | `Services/UndoRedoService` (`MaxActions = 10`), кнопки ← → в тулбаре |
| FR-18 | Управление профилем: просмотр (логин, аватар, роль), редактирование имени, фамилии, e-mail | `Views/ProfilePopup`, `ProfileViewModel`, `AuthService.UpdateProfile` |
| FR-19 | Смена пароля с проверкой старого пароля | `AuthService.ChangePassword`, `ProfileViewModel` |
| FR-20 | Переключение светлой / тёмной темы | `Services/ThemeService`, `ProfileViewModel.ToggleTheme` |
| FR-21 | Переключение языка интерфейса (ru / en) без перезапуска | `Services/LocalizationService`, `SwitchLanguageCommand` |

### 1.3. Функции администратора

| № | Требование | Реализация |
|---|------------|-----------|
| FR-22 | Добавление приложения: все атрибуты (название, разработчик, описание, категория из списка, цена, рейтинг, версия, размер, страна, возрастной рейтинг, скидка, цвет, теги) | `Views/AddEditWindow`, `AddEditAppViewModel.Save` → `AddApp` |
| FR-23 | Редактирование приложения (клонирование объекта в диалог — оригинал не меняется до сохранения) | `AddEditAppViewModel.Clone`, `UpdateApp` |
| FR-24 | Удаление приложения с диалогом подтверждения (опасное действие) | `EditDeleteDialog.OnDeleteClick`, `ConfirmDialog` (isDanger), `DeleteCommand` |
| FR-25 | Восстановление исходных данных (seed) с диалогом подтверждения | `RestoreDefaultsCommand`, `IRepository.RestoreDefaults`, `OnRestoreData` |
| FR-26 | Кнопки «Добавить» и «Изменить/Удалить» видны только администратору | `Visibility="{Binding IsAdmin, BoolToVis}"` в `MainWindow.xaml`, `AppDetailView.xaml` |

---

## 2. Нефункциональные требования (NFR)

| № | Требование | Реализация |
|---|------------|-----------|
| NFR-1 | Desktop-приложение для ОС Windows, WPF, C#, платформа .NET 8 | `Project.csproj`: `net8.0-windows`, `UseWPF` |
| NFR-2 | Архитектура MVVM: отображение, бизнес-логика и данные независимы; code-behind окон содержит только UI-логику (фокус, popup, позиционирование) | `ViewModels/`, `Views/`, `Data/`, `Services/`; `ViewModelBase`, `RelayCommand` |
| NFR-3 | Паттерны проектирования (задание: ≥ 2): Repository (интерфейс + абстрактный базовый класс + 3 реализации), Command (RelayCommand + RoutedUICommand), Undo/Redo (Command History / Memento), Dependency Injection через конструкторы, Observer (INotifyPropertyChanged) | `IRepository`, `BaseRepository`, `Commands/`, `Services/UndoRedoService` |
| NFR-4 | Подсистема данных заменяемая без изменения UI и ViewModel — три взаимозаменяемых хранилища: JSON-файлы, PostgreSQL через ADO.NET, PostgreSQL через EF Core | `JsonRep`, `PostgreSQLRepADO`, `PostgreSQLRepEF`; выбор в `ApplicationApp.ShowLogin` |
| NFR-5 | Хранение данных: PostgreSQL (docker, конфигурация в `.env`: имя БД, хост, порт, volume); БД поставляется с seed-данными (12 приложений, 2 пользователя) | `Data/PostgreSQLRepEF`, `Data/BaseRepository.SeedApps/SeedUsers`, `.env` |
| NFR-6 | Локализация интерфейса: русский и английский (89 строк ресурсов каждый), словари ресурсов ru.xaml / en.xaml | `Resources/Localization/` |
| NFR-7 | Два визуальных стиля (светлая и тёмная темы) на ресурсных словарях | `Resources/Styles/LightAppTheme.xaml`, `DarkAppTheme.xaml` |
| NFR-8 | Безопасность: пароли только в виде SHA-256-хешей; сессия пользователя изолирована (установки и undo-история не переходят другому пользователю) | `AuthService`, `UndoRedoService.Clear()` |
| NFR-9 | Удобство управления: клавиша Enter для входа, сброс красной подсветки при фокусе, tooltips, диалоги подтверждения для необратимых действий (удаление, восстановление), undo для обратимых | `LoginWindow.xaml.cs`, `ConfirmDialog`, тулбар `MainWindow.xaml` |
| NFR-10 | Роутинг событий WPF: туннелирование, всплытие и прямые routed-события на баннере и карточках; команды уровня приложения `RoutedUICommand` | `Views/MainWindow.xaml.cs`, `Commands/AppCommands.cs`, `FeaturedBannerView.xaml.cs` |
| NFR-11 | Мягкая деградация: кастомный курсор приложения подхватывается, если ресурс доступен, без падения при его отсутствии | `MainWindow()` ctor, `ApplicationApp.OnStartup` (try/catch) |

---

## 3. Что есть в задании, но НЕ реализовано в коде

Перечислено, чтобы не приписывать проекту несуществующий функционал:

- Регистрация новых пользователей в UI (в коде есть только `IRepository.AddUser` для seed);
- Просмотр списка всех пользователей, блокировка/разблокировка учётных записей;
- Системный журнал действий пользователей (есть только Debug-лог routed-событий в `MainWindow.EventLog`);
- CRUD категорий администратором (категории — фиксированный список в `AddEditAppViewModel.AvailableCategories`).
