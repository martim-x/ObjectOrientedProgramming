# AppStore — WPF Desktop Application

A desktop app-catalogue built with WPF / .NET 8, styled after macOS App Store.  
Theme: **software marketplace** (apps sold or distributed for free).

---

## Table of Contents

1. [Quick start](#quick-start)
2. [Features](#features)
3. [Project structure](#project-structure)
4. [Architecture](#architecture)
5. [Data storage](#data-storage)
6. [Localisation](#localisation)
7. [Theming and styles](#theming-and-styles)
8. [Commands pattern](#commands-pattern)
9. [Compliance with the assignment](#compliance-with-the-assignment)
10. [WPF Theory — Full Answers](#wpf-theory--full-answers)

---

## Quick start

**Requirements:** .NET 8 SDK, Windows 10/11.

```bash
cd AppStoreRefactor
dotnet restore
dotnet run
```

Data file is created automatically on first launch:
`%AppData%\AppStore\apps.json`

---

## Features

| Feature | Details |
|---|---|
| App catalogue | 12 seed apps across 6 categories |
| Full-text search | Name, description, developer, tags |
| Section navigation | Discover · Create · Work · Play · Develop · Installed · Categories |
| Category grid | Tag-based 2-column grid view |
| Featured banner | Hero banner on Discover page |
| Filters popup | Min rating, max price, sort order, downloaded-only toggle |
| See All / collapse | List limited to 6, expandable |
| Install / Uninstall | Persisted to JSON, counter incremented |
| Edit app | Full form in separate modal window |
| Delete app | Confirmation dialog before removal |
| Restore defaults | Rewrites JSON with original 12-app seed |
| Add new app | Full form with colour preview |
| EN / RU localisation | Dynamic switch without restart, BYN currency at x2.97 |
| Custom cursor | `.cur` loaded at startup |
| Custom icon | 32x32 ICO embedded in executable |
| Light macOS theme | White background, `#0072F7` accent |
| Responsive layout | DockPanel + Grid proportional scaling |

---

## Project structure

```
AppStoreRefactor/
├── App.xaml / App.xaml.cs          # Application entry, global resources
├── AssemblyInfo.cs                 # ThemeInfo attribute
├── AppStore.csproj
│
├── Models/
│   └── AppItem.cs                  # Domain model (21 properties + 2 computed)
│
├── Data/
│   ├── JsonStorageService.cs       # Newtonsoft.Json persistence + 12-app seed
│   └── AppRepository.cs           # Write-through in-memory cache
│
├── Services/
│   ├── AppService.cs               # Business logic facade
│   └── LocalizationService.cs     # ResourceDictionary swap + currency switch
│
├── Commands/
│   ├── RelayCommand.cs             # ICommand (generic + non-generic)
│   └── AppCommands.cs             # DownloadAppCommand, DeleteAppCommand
│
├── ViewModels/
│   ├── ViewModelBase.cs            # INotifyPropertyChanged with SetField<T>
│   ├── MainViewModel.cs            # All UI state, filter/search/navigation
│   └── AddEditAppViewModel.cs     # Form state for add/edit modal
│
├── Views/
│   ├── MainWindow.xaml/cs          # Shell: sidebar + main panel + filter popup
│   ├── AppRowView.xaml/cs          # Single list row (UserControl)
│   ├── AppDetailView.xaml/cs       # Detail panel replacing the list
│   ├── FeaturedBannerView.xaml/cs  # Hero banner
│   ├── AddEditWindow.xaml/cs       # Add / Edit modal
│   ├── EditDeleteDialog.xaml/cs    # Action sheet: Edit | Delete | Cancel
│   └── ConfirmDialog.xaml/cs       # Generic yes/no confirmation
│
├── Converters/
│   └── Converters.cs               # 14 IValueConverter / IMultiValueConverter
│
└── Resources/
    ├── Styles/AppStyles.xaml       # All control styles and brushes
    ├── Localization/en.xaml        # English strings (~60 keys)
    ├── Localization/ru.xaml        # Russian strings (~60 keys)
    ├── Cursors/arrow.cur           # Custom cursor
    └── Icons/app.ico               # Application icon
```

---

## Architecture

Three-layer + MVVM separation:

```
Presentation  (Views / ViewModels)
     |
Business      (AppService, LocalizationService)
     |
Data          (AppRepository -> JsonStorageService -> apps.json)
```

Views bind exclusively to ViewModel properties.
Code-behind only wires events that XAML cannot handle directly
(GotFocus, MouseLeftButtonDown on Canvas).
All data mutations go through ICommand implementations.

---

## Data storage

AppItem objects are serialised with Newtonsoft.Json to `%AppData%\AppStore\apps.json`.
The file is created on first launch from 12 built-in seed apps.
"Restore Defaults" in the Filter popup rewrites the file from the same seed.

AppRepository keeps a write-through in-memory cache: every mutation calls
the storage service then immediately reloads the cache, so reads are O(1)
without a disk hit on every query.

### AppItem fields

| Field | Type | Notes |
|---|---|---|
| Id | Guid | Auto-generated |
| ShortName / FullName | string | Short used in icon, Full in list |
| Description | string | Shown in detail panel |
| Developer / Category | string | |
| Rating / RatingCount | double / int | |
| Price | double | 0 = Free |
| DiscountPercent | double? | null = no discount; badge hidden for free apps |
| FinalPrice | double (computed) | Price x (1 - discount/100) |
| Version / SizeMB / Country / AgeRating | various | |
| Color | string hex | Icon background |
| IsFeatured | bool | Shown in hero banner |
| IsDownloaded / DownloadCount | bool / int | Toggled by DownloadCommand |
| IsInStock | bool | |
| ReleaseDate | DateTime | |
| Tags | List string | Used in search and Categories grid |
| RelatedAppIds | List Guid | Stored, reserved for future UI |

---

## Localisation

Dynamic ResourceDictionary swap — no restart needed:

```csharp
var dict = new ResourceDictionary { Source = new Uri(".../ru.xaml") };
Application.Current.Resources.MergedDictionaries.Remove(oldDict);
Application.Current.Resources.MergedDictionaries.Add(dict);
// Also update converter labels:
AppGetLabelConverter.GetLabel  = Get("BtnGet");
AppGetLabelConverter.OpenLabel = Get("BtnOpen");
```

All UI strings use `{DynamicResource Key}` and update instantly.

Currency rates:

| Language | Symbol | Rate |
|---|---|---|
| EN | $ (USD) | x1.0 |
| RU | BYN | x2.97 |

---

## Theming and styles

All styles live in `Resources/Styles/AppStyles.xaml`.
No raw hex colours appear in any View XAML — all references use named brushes.

Key brushes:

| Key | Hex | Usage |
|---|---|---|
| WindowBgBrush | #FFFFFF | Main background |
| SidebarBgBrush | #F5F5F7 | Left panel |
| AccentBlueBrush | #0072F7 | Buttons, links, focus |
| TitleTextBrush | #272727 | Headings |
| SecondaryTextBrush | #828282 | Subtitles |
| GetBtnBgBrush | #F2F2F7 | Get/Open pill background |
| GetBtnTextBrush | #3478F6 | Get/Open pill text |
| SeparatorBrush | #F3F3F3 | Row dividers |
| DividerBrush | #E6E6E6 | Section dividers |

---

## Commands pattern

| Command | Type | Triggered from |
|---|---|---|
| Install / Uninstall | DownloadAppCommand | Row button, detail button |
| Delete | DeleteAppCommand | EditDeleteDialog -> ConfirmDialog |
| Add / Update | RelayCommand (inline in VM) | AddEditWindow Save button |
| Clear search | RelayCommand | X button in search bubble |
| Select category | RelayCommand string | Sidebar nav items |
| Select tag | RelayCommand string | Tag sub-list |
| Switch language | RelayCommand string | EN / RU buttons |
| Toggle See All | RelayCommand | Section header |
| Restore defaults | RelayCommand | Filter popup |
| Refresh | RelayCommand | Internal reload after CRUD |

---

## Compliance with the assignment

| Requirement | Status | Where |
|---|---|---|
| Short + full name | OK | AppItem.ShortName, .FullName |
| Description | OK | AppItem.Description |
| Category | OK | AppItem.Category |
| Rating | OK | AppItem.Rating, .RatingCount |
| Price + discounts | OK | AppItem.Price, .DiscountPercent, FinalPrice |
| Quantity / in-stock | OK | AppItem.DownloadCount, .IsInStock |
| Country | OK | AppItem.Country |
| Related products | OK | AppItem.RelatedAppIds (stored) |
| JSON storage | OK | %AppData%\AppStore\apps.json |
| List view | OK | AppRowView in ItemsControl |
| Detail + edit + delete | OK | AppDetailView + EditDeleteDialog |
| Add new item window | OK | AddEditWindow |
| Filter by criteria | OK | Filter popup (rating, price, sort) |
| Search | OK | Search bubble |
| Filter by category | OK | Sidebar nav + Categories grid |
| Price range filter | OK | Max-price slider |
| Split panel layout | OK | Sidebar + main Grid |
| All actions via Command | OK | See Commands table |
| Data bindings | OK | Throughout all Views |
| Responsive design | OK | DockPanel, Grid *, WrapPanel, UniformGrid |
| Two languages | OK | EN + RU |
| Dynamic localisation | OK | DynamicResource throughout |
| Custom cursor | OK | Resources/Cursors/arrow.cur |
| Custom icon | OK | Resources/Icons/app.ico |
| Application style | OK | Resources/Styles/AppStyles.xaml |

---

## WPF Theory — Full Answers

---

### 1. Преимущества и недостатки WPF

**Преимущества:**

**Декларативный UI через XAML.** Разметка полностью отделена от логики. Дизайнер работает с XAML, разработчик с C#, инструменты синхронизируют оба мира через code-behind и биндинги.

**Мощная система биндингов.** `Binding`, `MultiBinding`, `IValueConverter`, `INotifyPropertyChanged` позволяют полностью синхронизировать данные и интерфейс без ручного обновления элементов.

**Аппаратное ускорение (DirectX).** WPF рисует через DirectX, а не GDI/GDI+. Сложные анимации, 3D, видео, прозрачность — без потери производительности.

**Векторная графика и DPI-независимость.** Все размеры задаются в логических единицах. Приложение корректно выглядит на 4K без дополнительных усилий.

**Стили и шаблоны.** `Style`, `ControlTemplate`, `DataTemplate` позволяют кардинально переопределить внешний вид любого контрола без наследования.

**MVVM-первый.** Архитектура естественно ложится на MVVM: биндинги, INotifyPropertyChanged, ICommand.

**ResourceDictionary.** Глобальные ресурсы вынесены в словари и могут меняться в рантайме (динамическая тема, локализация).

**Недостатки:**

**Windows-only.** WPF работает исключительно на Windows.

**Высокий порог входа.** XAML, Dependency Properties, Routed Events, Binding Path — объём концепций велик.

**Производительность при больших данных.** ItemsControl без виртуализации рендерит все элементы разом.

**Сложная отладка биндингов.** Ошибки биндинга не бросают исключений — молча пишутся в Output.

**Нет mobile/web.** WPF только desktop.

---

### 2. Язык XAML и его роль в WPF

XAML (eXtensible Application Markup Language) — диалект XML, специализированный для создания объектных графов .NET.

```xml
<Button Width="100" Content="Click me" Background="DodgerBlue" Click="OnClick"/>
```

Эквивалент C#:
```csharp
var btn = new Button { Width = 100, Content = "Click me",
                        Background = Brushes.DodgerBlue };
btn.Click += OnClick;
```

XAML компилируется в BAML (Binary Application Markup Language) и встраивается в сборку. `InitializeComponent()` материализует объекты из BAML при загрузке окна.

**Зачем нужен:**
1. Разделение труда дизайнер/разработчик.
2. Иерархия вложенных элементов выражается компактно.
3. Маркап-расширения `{Binding}`, `{StaticResource}`, `{DynamicResource}` встраивают сложную логику прямо в разметку.

**Ключевые конструкции:**

| Конструкция | Пример |
|---|---|
| Property element | `<Button.Background><SolidColorBrush/></Button.Background>` |
| Markup extension | `{Binding Path=Name}`, `{StaticResource Style}` |
| Attached property | `Grid.Row="1"`, `DockPanel.Dock="Top"` |
| x:Key | Имя ресурса в словаре |
| x:Name | Имя для доступа из code-behind |
| DataTemplate | Шаблон отображения объекта данных |

---

### 3. Контейнеры компоновки

| Панель | Стратегия | Типичное применение |
|---|---|---|
| StackPanel | Линейно по горизонтали или вертикали | Вертикальный список кнопок |
| WrapPanel | Линейно с переносом на новую строку | Галерея, чипы тегов |
| DockPanel | Элементы пристыковываются к краям | Шапка + боковая панель + центр |
| Grid | Явная сетка строк и столбцов | Формы, сложные интерфейсы |
| UniformGrid | Равные ячейки без явного задания | Информационные плитки |
| Canvas | Абсолютные координаты | Векторная графика |
| VirtualizingStackPanel | Как StackPanel, только видимые элементы | Большие списки |

Адаптивность: `Width="*"` и `Width="Auto"`, `MinWidth`/`MaxWidth`, `HorizontalAlignment="Stretch"`, вложенные панели.

В проекте: `DockPanel` (сайдбар), `Grid` (основное окно, плитки, форма), `UniformGrid` (5 плиток деталей), `WrapPanel` (категории, теги), `StackPanel` (вертикальные списки).

---

### 4. Элементы управления и объектная модель WPF

**Иерархия классов:**

```
Object
└── DispatcherObject         # доступ через UI-поток
    └── DependencyObject     # поддержка DependencyProperty
        └── Visual           # участие в визуальном дереве
            └── UIElement    # ввод, hit-testing, layout
                └── FrameworkElement  # DataContext, Style, Name
                    ├── Control       # Template, Foreground, FontFamily
                    │   ├── ContentControl    # Button, Label, Window
                    │   └── ItemsControl      # ListBox, ComboBox
                    └── Panel         # Grid, StackPanel, Canvas
```

**Основные группы:**

| Группа | Примеры |
|---|---|
| Контейнеры | Grid, StackPanel, DockPanel, Canvas, WrapPanel |
| Кнопки | Button, RadioButton, CheckBox, ToggleButton |
| Текст | TextBlock, TextBox, PasswordBox |
| Выбор | ListBox, ComboBox, Slider, ProgressBar |
| Меню | Menu, ContextMenu, ToolBar |
| Данные | DataGrid, ListView, TreeView |
| Диалоги | Window, Popup, ToolTip |
| Медиа | Image, MediaElement |

**Два дерева WPF:**
- **Logical Tree** — дерево объектов из XAML. С ним работает программист.
- **Visual Tree** — полное дерево после применения ControlTemplate. С ним работает система рендеринга.

---

### 5. DependencyProperty

Специальный механизм хранения значений в глобальной хэш-таблице вместо приватного поля.

**Зачем нужен:**
1. Биндинг работает только с DependencyProperty.
2. Анимация умеет менять только DependencyProperty.
3. Setter в Style может задавать только DependencyProperty.
4. Наследование по дереву (DataContext, FontSize).
5. Приоритет значений из 8 источников (локальное, стиль, анимация и т.д.).
6. PropertyChangedCallback при изменении.

**Создание:**

```csharp
public class AppRowView : UserControl
{
    public static readonly DependencyProperty DownloadCommandProperty =
        DependencyProperty.Register(
            nameof(DownloadCommand),
            typeof(ICommand),
            typeof(AppRowView),
            new FrameworkPropertyMetadata(null));

    public ICommand? DownloadCommand
    {
        get => (ICommand?)GetValue(DownloadCommandProperty);
        set => SetValue(DownloadCommandProperty, value);
    }
}
```

**Когда создавать:** свойство кастомного UserControl должно поддерживать биндинг снаружи, анимацию, работать в Style/Trigger, или наследоваться по визуальному дереву.

---

### 6. Обработка событий. Маршрутизированные события

**Три стратегии маршрутизации:**

| Стратегия | Направление | Пример |
|---|---|---|
| Tunnel (Preview) | Вниз от корня к источнику | PreviewKeyDown, PreviewMouseDown |
| Bubble | Вверх от источника к корню | Click, KeyDown, MouseMove |
| Direct | Только на источнике | Loaded, GotFocus |

```
Порядок при клике на кнопку в Grid:
PreviewMouseDown(Grid) -> PreviewMouseDown(Button)
-> MouseDown(Button) -> Click(Button) -> MouseDown(Grid)
```

Остановка: `e.Handled = true` — событие не поднимается выше.

В MVVM предпочтительны ICommand вместо event handlers. Маршрутизированные события используются для инфраструктурных задач (drag-and-drop, навигация клавиатурой).

**Основные группы:**

| Группа | События |
|---|---|
| Мышь | MouseMove, MouseDown, MouseUp, MouseWheel, MouseEnter |
| Клавиатура | KeyDown, KeyUp, TextInput |
| Фокус | GotFocus, LostFocus, GotKeyboardFocus |
| Lifetime | Loaded, Unloaded, Initialized |
| Drag-and-drop | DragEnter, DragOver, Drop |

---

### 7. Resource Dictionary

Словарь "ключ -> объект", доступный всем элементам дерева через {StaticResource} или {DynamicResource}.

```xml
<ResourceDictionary>
    <SolidColorBrush x:Key="AccentBrush" Color="#0072F7"/>
    <Style x:Key="PrimaryBtn" TargetType="Button">
        <Setter Property="Background" Value="{StaticResource AccentBrush}"/>
    </Style>
</ResourceDictionary>
```

**Области действия:** элемент -> окно -> приложение -> внешний файл (MergedDictionaries).

**StaticResource vs DynamicResource:**

| | StaticResource | DynamicResource |
|---|---|---|
| Разрешение | При загрузке XAML (один раз) | При каждом обращении |
| Динамическая замена | Не отражается | Отражается мгновенно |
| Применение | Кисти, числа, иконки | Строки локализации, темы |

В проекте DynamicResource для всех строк локализации, StaticResource для кистей и стилей.

---

### 8. Стиль в WPF

Style — набор Setter-ов, применяемых к целевому типу. Эквивалент CSS-класса.

```xml
<Style x:Key="PrimaryButtonStyle" TargetType="Button">
    <Setter Property="Background" Value="#0072F7"/>
    <Setter Property="Foreground" Value="White"/>
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="Button">
                <Border Background="{TemplateBinding Background}" CornerRadius="14"
                        Padding="{TemplateBinding Padding}">
                    <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
                </Border>
                <ControlTemplate.Triggers>
                    <Trigger Property="IsMouseOver" Value="True">
                        <Setter Property="Opacity" Value="0.85"/>
                    </Trigger>
                </ControlTemplate.Triggers>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>
```

**Типы триггеров:**

| Тип | Условие |
|---|---|
| Trigger | Значение DependencyProperty |
| DataTrigger | Значение через Binding |
| MultiTrigger | Несколько условий AND |
| EventTrigger | Событие (запуск анимации) |

**Преимущества:**
1. DRY — изменение в одном месте обновляет все контролы.
2. Разделение — дизайн вынесен из code-behind.
3. ControlTemplate — полная замена визуального дерева при сохранении поведения.
4. Наследование — `BasedOn="{StaticResource Base}"`.

---

### 9. Command — паттерн и применение в WPF

Паттерн Command (GoF) инкапсулирует действие как объект.

Интерфейс `System.Windows.Input.ICommand`:

```csharp
public interface ICommand
{
    event EventHandler? CanExecuteChanged;
    bool CanExecute(object? parameter);
    void Execute(object? parameter);
}
```

`CanExecute` возвращает false — кнопка автоматически IsEnabled = false.
WPF вызывает CanExecute при CommandManager.RequerySuggested.

**RelayCommand:**

```csharp
public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    public event EventHandler? CanExecuteChanged
    {
        add    => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? p) => _canExecute?.Invoke(p) ?? true;
    public void Execute(object? p)    => _execute(p);
}
```

**Биндинг в XAML:**

```xml
<Button Command="{Binding DownloadCommand}"
        CommandParameter="{Binding SelectedApp}"/>
```

**Зачем Command в MVVM:**
1. Тестируемость — ViewModel не зависит от UI.
2. Нет code-behind — логика в ViewModel.
3. CanExecute — блокировка кнопки синхронизируется с данными автоматически.
4. Переиспользование — одна команда на кнопку, меню, горячую клавишу.

---

*Documentation generated for AppStore WPF, .NET 8.*
