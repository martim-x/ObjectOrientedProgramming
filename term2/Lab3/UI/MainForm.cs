using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace Lab2
{
    public partial class MainForm : Form
    {
        private readonly List<Computer> _orders = new();
        private List<Computer> _currentDisplay = new();
        private int _navIndex = -1;
        private readonly List<int> _navHistory = new();
        private string _lastAction = "—";

        private static readonly Dictionary<string, (int memGB, decimal basePrice)> GpuSpec = new()
        {
            { "RTX 4050", (8,  35_000) }, { "RTX 4070", (12, 60_000) },
            { "RTX 4080", (16, 90_000) }, { "RTX 4090", (24,130_000) },
            { "RTX 5050", (8,  45_000) }, { "RTX 5070", (12, 75_000) },
            { "RTX 5080", (16,110_000) }, { "RTX 5090", (24,160_000) },
            { "RX 9060",  (8,  28_000) }, { "RX 9070",  (16, 55_000) },
        };

        public MainForm()
        {
            InitializeComponent();

            // ═══════════════════════════════════════════════════════
            // МЕНЮ И ТУЛБАР строятся ЗДЕСЬ, а не в Designer.cs
            // Это гарантирует что VS Designer не сотрёт их при
            // редактировании формы (изменении размеров, позиций и т.д.)
            // ═══════════════════════════════════════════════════════
            BuildMenuStrip();
            BuildToolStrip();

            lvOrders.Columns.Add("№",       42);
            lvOrders.Columns.Add("Тип",    115);
            lvOrders.Columns.Add("CPU",    175);
            lvOrders.Columns.Add("GPU",    135);
            lvOrders.Columns.Add("ОЗУ",     85);
            lvOrders.Columns.Add("Диск",    65);
            lvOrders.Columns.Add("Дата",    92);
            lvOrders.Columns.Add("Цена ₽", 115);

            var clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            clockTimer.Tick += (s, e) => tsslDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy  HH:mm:ss");
            clockTimer.Start();
            tsslDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy  HH:mm:ss");

            UpdateStatus("Приложение запущено.");
            lblCostValue.Text = "0 ₽";
            pbCost.Value      = 0;
            lblBreakdown.Text = "Выберите компоненты для расчёта цены...";
        }

        // ── МЕНЮ — строится в коде, не в Designer ─────────────────
        // VS Designer не тронет этот метод при редактировании формы.
        private void BuildMenuStrip()
        {
            menuStrip1.Items.Clear();

            var miFile    = new ToolStripMenuItem("📁 Файл");
            var miSave    = new ToolStripMenuItem("💾 Сохранить",               null, BtnSave_Click)       { ShortcutKeys = Keys.Control | Keys.S };
            var miLoad    = new ToolStripMenuItem("📂 Загрузить",               null, BtnLoad_Click)       { ShortcutKeys = Keys.Control | Keys.O };
            var miSaveRes = new ToolStripMenuItem("📤 Сохранить результаты...", null, MenuSaveResults_Click);
            var miExit    = new ToolStripMenuItem("🚪 Выход",                   null, MenuExit_Click)      { ShortcutKeys = Keys.Alt | Keys.F4 };
            miFile.DropDownItems.AddRange(new ToolStripItem[]
                { miSave, miLoad, new ToolStripSeparator(), miSaveRes, new ToolStripSeparator(), miExit });

            var miSearch     = new ToolStripMenuItem("🔍 Поиск");
            var miOpenSearch = new ToolStripMenuItem("🔎 Конструктор поиска...", null, MenuSearch_Click)  { ShortcutKeys = Keys.Control | Keys.F };
            var miShowAll    = new ToolStripMenuItem("📋 Показать все",          null, MenuShowAll_Click) { ShortcutKeys = Keys.Control | Keys.D0 };
            miSearch.DropDownItems.AddRange(new ToolStripItem[] { miOpenSearch, new ToolStripSeparator(), miShowAll });

            var miSort          = new ToolStripMenuItem("📊 Сортировка");
            var miSortPriceAsc  = new ToolStripMenuItem("По цене ↑",                   null, MenuSortByPriceAsc_Click);
            var miSortPriceDesc = new ToolStripMenuItem("По цене ↓",                   null, MenuSortByPriceDesc_Click);
            var miSortDateAsc   = new ToolStripMenuItem("По дате ↑ (старые)",          null, MenuSortByDateAsc_Click);
            var miSortDateDesc  = new ToolStripMenuItem("По дате ↓ (новые)",           null, MenuSortByDateDesc_Click);
            var miSortRAM       = new ToolStripMenuItem("По объёму ОЗУ ↓",             null, MenuSortByRAM_Click);
            var miSortCPU       = new ToolStripMenuItem("По производителю CPU + ядра", null, MenuSortByCPU_Click);
            miSort.DropDownItems.AddRange(new ToolStripItem[]
                { miSortPriceAsc, miSortPriceDesc, new ToolStripSeparator(),
                  miSortDateAsc,  miSortDateDesc,  new ToolStripSeparator(),
                  miSortRAM, miSortCPU });

            var miEdit   = new ToolStripMenuItem("✏️ Правка");
            var miClear  = new ToolStripMenuItem("🗑 Очистить все заказы", null, MenuClearAll_Click) { ShortcutKeys = Keys.Control | Keys.Delete };
            var miDelete = new ToolStripMenuItem("❌ Удалить выбранный",   null, MenuDelete_Click)   { ShortcutKeys = Keys.Delete };
            miEdit.DropDownItems.AddRange(new ToolStripItem[] { miClear, new ToolStripSeparator(), miDelete });

            var miView = new ToolStripMenuItem("👁 Вид");
            miToggleToolbar = new ToolStripMenuItem("Панель инструментов", null, MenuToggleToolbar_Click)
                { CheckOnClick = true, Checked = true };
            miView.DropDownItems.Add(miToggleToolbar);

            var miHelp  = new ToolStripMenuItem("❓ Справка");
            var miAbout = new ToolStripMenuItem("О программе...", null, MenuAbout_Click) { ShortcutKeys = Keys.F1 };
            miHelp.DropDownItems.Add(miAbout);

            menuStrip1.Items.AddRange(new ToolStripItem[]
                { miFile, miSearch, miSort, miEdit, miView, miHelp });
        }

        // ── ТУЛБАР — строится в коде, не в Designer ───────────────
        // VS Designer не тронет этот метод при редактировании формы.
        private void BuildToolStrip()
        {
            toolStrip1.Items.Clear();

            var tsBtnSearch  = MakeToolBtn("🔍", "Поиск (Ctrl+F)");
            var tsBtnSort    = MakeToolBtn("📊", "Сортировка по цене ↑");
            var tsBtnShowAll = MakeToolBtn("📋", "Показать все (Ctrl+0)");
            var tsBtnDelete  = MakeToolBtn("❌", "Удалить выбранный");
            var tsBtnClear   = MakeToolBtn("🗑", "Очистить все");
            var tsBtnBack    = MakeToolBtn("◀",  "Назад");
            var tsBtnForward = MakeToolBtn("▶",  "Вперёд");

            tsBtnSearch.Click  += TsBtnSearch_Click;
            tsBtnSort.Click    += TsBtnSort_Click;
            tsBtnShowAll.Click += TsBtnShowAll_Click;
            tsBtnDelete.Click  += TsBtnDelete_Click;
            tsBtnClear.Click   += TsBtnClear_Click;
            tsBtnBack.Click    += TsBtnBack_Click;
            tsBtnForward.Click += TsBtnForward_Click;

            toolStrip1.Items.AddRange(new ToolStripItem[]
            {
                tsBtnSearch, tsBtnSort, tsBtnShowAll,
                new ToolStripSeparator(),
                tsBtnDelete, tsBtnClear,
                new ToolStripSeparator(),
                tsBtnBack, tsBtnForward
            });
        }

        // ── StatusStrip ───────────────────────────────────────────
        private void UpdateStatus(string action)
        {
            _lastAction     = action;
            tsslCount.Text  = $"Заказов: {_orders.Count}";
            tsslAction.Text = $"Действие: {_lastAction}";
        }

        // ── Частичный объект — 1 источник правды для цены ────────
        private Computer BuildPartialComputer()
        {
            var cpu = new Processor { ArchBits = 64 };
            if (cbCPUMaker.SelectedIndex >= 0 &&
                cbCPUCore.SelectedIndex  >= 0 &&
                cbModelCPU.SelectedIndex >= 0)
            {
                string maker     = cbCPUMaker.Text;
                int    cores     = int.Parse(cbCPUCore.Text);
                int    gen       = int.Parse(cbModelCPU.Text);
                float  mult      = maker == "Intel" ? 1.7f : 2.3f;
                cpu.Manufacturer = maker;
                cpu.Cores        = cores;
                cpu.MaxFrequency = 3.5 + cores * 0.5 * gen * 0.3 * mult;
                cpu.CacheL3MB    = cores * 2;
                cpu.Model        = maker == "Intel"
                    ? $"Core i{gen} ({cores}C)"
                    : $"Ryzen {gen} ({cores}C)";
            }

            var gpu = cbGPU.SelectedIndex >= 0 && GpuSpec.TryGetValue(cbGPU.Text.Trim(), out var spec)
                ? new GPU
                  {
                      Manufacturer = cbGPU.Text.Trim().StartsWith("RX") ? "AMD" : "NVIDIA",
                      Model        = cbGPU.Text.Trim(),
                      MemoryGB     = spec.memGB,
                      Frequency    = 1800,
                      BasePrice    = spec.basePrice
                  }
                : new GPU { Frequency = 0, MemoryGB = 0, BasePrice = 0 };

            return new Computer
            {
                Type         = cbComputerType.SelectedIndex >= 0 ? cbComputerType.Text : "",
                CPU          = cpu,
                VideoCard    = gpu,
                RAMsizeGB    = cbRamSize.SelectedIndex >= 0 ? int.Parse(cbRamSize.Text) : 0,
                RAMtype      = cbRamType.SelectedIndex >= 0 ? cbRamType.Text            : "DDR4",
                HDDsizeGB    = cbHDDSize.SelectedIndex >= 0 ? int.Parse(cbHDDSize.Text) : 0,
                HDDtype      = rbSSD.Checked ? "SSD" : "HDD",
                PurchaseDate = calPurchase.SelectionStart,
                HasMonitor   = chkMonitor.Checked,
                HasKeyboard  = chkKeyboard.Checked,
                HasMouse     = chkMouse.Checked
            };
        }

        // ── Пересчёт цены ─────────────────────────────────────────
        private void RecalcCost()
        {
            bool anySelected = cbCPUMaker.SelectedIndex    >= 0
                            || cbGPU.SelectedIndex          >= 0
                            || cbRamSize.SelectedIndex      >= 0
                            || cbComputerType.SelectedIndex >= 0;

            if (!anySelected)
            {
                lblCostValue.Text = "0 ₽";
                pbCost.Value      = 0;
                lblBreakdown.Text = "Выберите компоненты для расчёта цены...";
                return;
            }

            decimal total     = BuildPartialComputer().Price();
            lblCostValue.Text = $"{total:N0} ₽";
            pbCost.Value      = (int)Math.Min(total, pbCost.Maximum);
            lblBreakdown.Text = "Нажмите «📊 Подробнее» чтобы увидеть из чего сложилась цена";
        }

        // ── Полный объект с валидацией ────────────────────────────
        private Computer CollectComputer()
        {
            errorProvider.Clear();
            bool valid = true;

            void Check(ComboBox cb, string name)
            {
                if (cb.SelectedIndex < 0) { errorProvider.SetError(cb, $"Выберите «{name}»"); valid = false; }
                else errorProvider.SetError(cb, "");
            }

            Check(cbComputerType, "Тип компьютера");
            Check(cbCPUMaker,     "Производитель CPU");
            Check(cbCPUCore,      "Количество ядер");
            Check(cbModelCPU,     "Поколение CPU");
            Check(cbRamSize,      "Объём ОЗУ");
            Check(cbRamType,      "Тип ОЗУ");
            Check(cbGPU,          "Видеокарта");
            Check(cbHDDSize,      "Размер диска");

            if (!valid)
                throw new InvalidOperationException("Заполните все поля, отмеченные значком ⚠");

            var computer = BuildPartialComputer();
            var ctx      = new ValidationContext(computer);
            var errors   = new List<ValidationResult>();
            if (!Validator.TryValidateObject(computer, ctx, errors, true))
                throw new ValidationException(
                    "Ошибки валидации:\n" + string.Join("\n", errors.Select(r => r.ErrorMessage)));

            return computer;
        }

        // ── Обработчики контролов ─────────────────────────────────
        private void AnyControl_Changed(object sender, EventArgs e)               => RecalcCost();
        private void cbCoreMaker_SelectedIndexChanged(object sender, EventArgs e)  => RecalcCost();
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)    => RecalcCost();
        private void cbRamSize_SelectedIndexChanged(object sender, EventArgs e)    => RecalcCost();
        private void cbRamType_SelectedIndexChanged(object sender, EventArgs e)    => RecalcCost();
        private void cbGPU_SelectedIndexChanged(object sender, EventArgs e)        => RecalcCost();

        private void gbMain_Enter(object sender, EventArgs e)          { }
        private void gbGPU_Enter(object sender, EventArgs e)           { }
        private void lblComputerType_Click(object sender, EventArgs e) { }
        private void lblCPUMaker_Click(object sender, EventArgs e)     { }
        private void lblCPUCores_Click(object sender, EventArgs e)     { }
        private void lblGPUMaker_Click(object sender, EventArgs e)     { }

        // ── Добавить заказ ────────────────────────────────────────
        private void BtnAddOrder_Click(object sender, EventArgs e)
        {
            try
            {
                var c = CollectComputer();
                _orders.Add(c);
                _navHistory.Add(_orders.Count - 1);
                _navIndex = _navHistory.Count - 1;
                RefreshOrdersList(_orders);
                TabCtrl.SelectedIndex = 1;
                UpdateStatus($"Добавлен заказ #{_orders.Count}");
                MessageBox.Show(
                    $"Заказ #{_orders.Count} добавлен!\n" +
                    $"Стоимость: {c.Price():N0} ₽\n" +
                    $"Итого по лаборатории: {PriceCalculator.GetLaboratoryTotal(_orders):N0} ₽",
                    "Заказ добавлен", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ── Обновление таблицы ────────────────────────────────────
        private void RefreshOrdersList(IEnumerable<Computer> source)
        {
            _currentDisplay = source.ToList();
            lvOrders.Items.Clear();

            for (int i = 0; i < _currentDisplay.Count; i++)
            {
                var c    = _currentDisplay[i];
                var item = new ListViewItem((i + 1).ToString());
                item.SubItems.Add(c.Type);
                item.SubItems.Add($"{c.CPU.Manufacturer} {c.CPU.Model}");
                item.SubItems.Add(c.VideoCard.Model);
                item.SubItems.Add($"{c.RAMsizeGB} ГБ {c.RAMtype}");
                item.SubItems.Add(c.HDDtype);
                item.SubItems.Add(c.PurchaseDate.ToString("dd.MM.yy"));
                item.SubItems.Add($"{c.Price():N0} ₽");
                item.BackColor = i % 2 == 0 ? Color.White : Color.AliceBlue;
                lvOrders.Items.Add(item);
            }

            decimal total = PriceCalculator.GetLaboratoryTotal(_currentDisplay);
            lblOrders.Text = _currentDisplay.Count == 0
                ? "Список заказов:"
                : $"Список заказов: {_currentDisplay.Count} шт.  |  Итого: {total:N0} ₽";

            UpdateStatus(_lastAction);
        }

        private void LvOrders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvOrders.SelectedIndices.Count > 0)
            {
                _navHistory.Add(lvOrders.SelectedIndices[0]);
                _navIndex = _navHistory.Count - 1;
            }
        }

        // ── Удалить заказ ─────────────────────────────────────────
        private void BtnDelete_Click(object sender, EventArgs e) => DeleteSelected();

        private void DeleteSelected()
        {
            if (lvOrders.SelectedIndices.Count == 0)
            {
                MessageBox.Show("Выберите строку заказа для удаления.",
                    "Ничего не выбрано", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int visualIdx = lvOrders.SelectedIndices[0];
            if (visualIdx >= _currentDisplay.Count) return;
            Computer target = _currentDisplay[visualIdx];

            if (MessageBox.Show($"Удалить заказ #{visualIdx + 1}?\n{target}",
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _orders.Remove(target);
                RefreshOrdersList(_orders);
                UpdateStatus($"Удалён: {target.Type} ({target.CPU.Model})");
            }
        }

        // ── Сохранить / Загрузить ─────────────────────────────────
        private void BtnSave_Click(object sender, EventArgs e) => SaveOrders();
        private void BtnLoad_Click(object sender, EventArgs e) => LoadOrders();

        private void SaveOrders()
        {
            if (_orders.Count == 0)
            {
                MessageBox.Show("Нет заказов для сохранения.", "Пусто",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using var dlg = new SaveFileDialog
                { Title = "Сохранить заказы", Filter = "JSON файл (*.json)|*.json", FileName = "lab_orders" };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                FileManager.Save(_orders, dlg.FileName);
                UpdateStatus($"Сохранено {_orders.Count} заказов → {Path.GetFileName(dlg.FileName)}");
                MessageBox.Show($"Сохранено {_orders.Count} заказов.", "Готово",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения:\n" + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadOrders()
        {
            using var dlg = new OpenFileDialog
                { Title = "Загрузить заказы", Filter = "JSON файл (*.json)|*.json" };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                var loaded = FileManager.Load(dlg.FileName);
                if (loaded.Count == 0)
                {
                    MessageBox.Show("Файл пуст или не содержит заказов.", "Нет данных",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (_orders.Count > 0)
                {
                    var ans = MessageBox.Show(
                        $"Добавить {loaded.Count} загруженных заказов к существующим {_orders.Count}?\n«Нет» — заменить.",
                        "Способ загрузки", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (ans == DialogResult.Cancel) return;
                    if (ans == DialogResult.No) _orders.Clear();
                }
                _orders.AddRange(loaded);
                RefreshOrdersList(_orders);
                TabCtrl.SelectedIndex = 1;
                UpdateStatus($"Загружено {loaded.Count} заказов");
                MessageBox.Show($"Загружено заказов: {loaded.Count}", "Готово",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки:\n" + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Меню: Поиск ───────────────────────────────────────────
        private void MenuSearch_Click(object sender, EventArgs e)
        {
            if (_orders.Count == 0)
            {
                MessageBox.Show("Список заказов пуст.", "Поиск",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using var sf = new SearchForm(_orders);
            sf.ShowDialog(this);
            if (sf.Results.Count > 0)
            {
                RefreshOrdersList(sf.Results);
                TabCtrl.SelectedIndex = 1;
                UpdateStatus($"Поиск: найдено {sf.Results.Count} из {_orders.Count}");
            }
        }

        private void MenuShowAll_Click(object sender, EventArgs e)
        {
            RefreshOrdersList(_orders);
            UpdateStatus("Показаны все заказы");
        }

        // ── Меню: Сортировка ──────────────────────────────────────
        private void MenuSortByPriceAsc_Click(object sender, EventArgs e)
        {
            RefreshOrdersList(_orders.OrderBy(c => c.Price()).ToList());
            TabCtrl.SelectedIndex = 1;
            UpdateStatus("Сортировка: по цене ↑");
        }

        private void MenuSortByPriceDesc_Click(object sender, EventArgs e)
        {
            RefreshOrdersList(_orders.OrderByDescending(c => c.Price()).ToList());
            TabCtrl.SelectedIndex = 1;
            UpdateStatus("Сортировка: по цене ↓");
        }

        private void MenuSortByDateAsc_Click(object sender, EventArgs e)
        {
            RefreshOrdersList(_orders.OrderBy(c => c.PurchaseDate).ToList());
            TabCtrl.SelectedIndex = 1;
            UpdateStatus("Сортировка: по дате ↑");
        }

        private void MenuSortByDateDesc_Click(object sender, EventArgs e)
        {
            RefreshOrdersList(_orders.OrderByDescending(c => c.PurchaseDate).ToList());
            TabCtrl.SelectedIndex = 1;
            UpdateStatus("Сортировка: по дате ↓");
        }

        private void MenuSortByRAM_Click(object sender, EventArgs e)
        {
            RefreshOrdersList(_orders.OrderByDescending(c => c.RAMsizeGB).ToList());
            TabCtrl.SelectedIndex = 1;
            UpdateStatus("Сортировка: по объёму ОЗУ ↓");
        }

        private void MenuSortByCPU_Click(object sender, EventArgs e)
        {
            RefreshOrdersList(_orders.OrderBy(c => c.CPU.Manufacturer)
                                     .ThenByDescending(c => c.CPU.Cores).ToList());
            TabCtrl.SelectedIndex = 1;
            UpdateStatus("Сортировка: CPU производитель + ядра ↓");
        }

        // ── Меню: Сохранить результаты ────────────────────────────
        private void MenuSaveResults_Click(object sender, EventArgs e)
        {
            if (_currentDisplay.Count == 0)
            {
                MessageBox.Show("Нет данных для сохранения.", "Экспорт",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using var dlg = new SaveFileDialog
            {
                Title    = "Сохранить текущий список",
                Filter   = "JSON файл (*.json)|*.json|XML файл (*.xml)|*.xml",
                FileName = "results"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            try
            {
                if (dlg.FilterIndex == 1)
                {
                    var opts = new JsonSerializerOptions { WriteIndented = true };
                    File.WriteAllText(dlg.FileName, JsonSerializer.Serialize(_currentDisplay, opts));
                }
                else
                {
                    var xml = new System.Xml.Serialization.XmlSerializer(typeof(List<Computer>));
                    using var fs = File.Create(dlg.FileName);
                    xml.Serialize(fs, _currentDisplay);
                }
                UpdateStatus($"Экспортировано {_currentDisplay.Count} записей");
                MessageBox.Show($"Сохранено {_currentDisplay.Count} записей.", "Готово",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:\n" + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Меню: О программе ─────────────────────────────────────
        private void MenuAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "IT Лаборатория — Лабораторная работа № 3\n\n" +
                "Версия: 3.0.0\n" +
                "Дисциплина: Объектно-ориентированное программирование\n\n" +
                "Функции: управление заказами компьютеров,\n" +
                "поиск с RegEx, сортировка LINQ,\n" +
                "валидация DataAnnotations.\n\n" +
                "Разработчик: студент группы XXX\n© 2026",
                "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ── Меню: Очистить всё ────────────────────────────────────
        private void MenuClearAll_Click(object sender, EventArgs e)
        {
            if (_orders.Count == 0) return;
            if (MessageBox.Show("Удалить все заказы?", "Очистить",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _orders.Clear();
                RefreshOrdersList(_orders);
                UpdateStatus("Список очищен");
            }
        }

        // ── Панель инструментов: скрыть/показать ─────────────────
        private void MenuToggleToolbar_Click(object sender, EventArgs e)
        {
            toolStrip1.Visible = !toolStrip1.Visible;
            if (sender is ToolStripMenuItem mi) mi.Checked = toolStrip1.Visible;
            UpdateStatus(toolStrip1.Visible ? "Панель инструментов показана" : "Панель инструментов скрыта");
        }

        // ── Навигация ─────────────────────────────────────────────
        private void NavigateBack()
        {
            if (_navIndex <= 0) return;
            _navIndex--;
            int idx = _navHistory[_navIndex];
            if (idx < lvOrders.Items.Count)
            {
                lvOrders.SelectedIndices.Clear();
                lvOrders.Items[idx].Selected = true;
                lvOrders.EnsureVisible(idx);
                TabCtrl.SelectedIndex = 1;
                UpdateStatus($"Назад → строка {idx + 1}");
            }
        }

        private void NavigateForward()
        {
            if (_navIndex >= _navHistory.Count - 1) return;
            _navIndex++;
            int idx = _navHistory[_navIndex];
            if (idx < lvOrders.Items.Count)
            {
                lvOrders.SelectedIndices.Clear();
                lvOrders.Items[idx].Selected = true;
                lvOrders.EnsureVisible(idx);
                TabCtrl.SelectedIndex = 1;
                UpdateStatus($"Вперёд → строка {idx + 1}");
            }
        }

        // ── Кнопка «Подробнее» — окно разбивки цены ──────────────
        private void BtnPriceDetails_Click(object sender, EventArgs e)
        {
            bool anySelected = cbCPUMaker.SelectedIndex    >= 0
                            || cbGPU.SelectedIndex          >= 0
                            || cbRamSize.SelectedIndex      >= 0
                            || cbComputerType.SelectedIndex >= 0;
            if (!anySelected)
            {
                MessageBox.Show("Выберите хотя бы один компонент для просмотра разбивки.",
                    "Нет данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using var dlg = new PriceDetailsForm(BuildPartialComputer());
            dlg.ShowDialog(this);
        }

        // ── Меню: Выход и Удалить ─────────────────────────────────
        private void MenuExit_Click(object sender, EventArgs e)   => Application.Exit();
        private void MenuDelete_Click(object sender, EventArgs e) => DeleteSelected();

        // ── Вспомогательный метод для кнопок ToolStrip ───────────
        private static ToolStripButton MakeToolBtn(string text, string tooltip) =>
            new ToolStripButton
            {
                Text         = text,
                ToolTipText  = tooltip,
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font         = new Font("Segoe UI", 11F)
            };

        // ── ToolStrip handlers ────────────────────────────────────
        private void TsBtnSearch_Click(object sender, EventArgs e)  => MenuSearch_Click(sender, e);
        private void TsBtnSort_Click(object sender, EventArgs e)     => MenuSortByPriceAsc_Click(sender, e);
        private void TsBtnClear_Click(object sender, EventArgs e)    => MenuClearAll_Click(sender, e);
        private void TsBtnDelete_Click(object sender, EventArgs e)   => DeleteSelected();
        private void TsBtnBack_Click(object sender, EventArgs e)     => NavigateBack();
        private void TsBtnForward_Click(object sender, EventArgs e)  => NavigateForward();
        private void TsBtnShowAll_Click(object sender, EventArgs e)  => MenuShowAll_Click(sender, e);
    }
}
