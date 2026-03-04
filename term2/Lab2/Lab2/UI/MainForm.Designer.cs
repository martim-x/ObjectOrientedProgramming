using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab2
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // ── MenuStrip ─────────────────────────────────────────
            menuStrip1 = new MenuStrip { Font = new Font("Segoe UI", 10F) };

            var miFile    = new ToolStripMenuItem("📁 Файл");
            var miSave    = new ToolStripMenuItem("💾 Сохранить",          null, BtnSave_Click)       { ShortcutKeys = Keys.Control | Keys.S };
            var miLoad    = new ToolStripMenuItem("📂 Загрузить",           null, BtnLoad_Click)       { ShortcutKeys = Keys.Control | Keys.O };
            var miSaveRes = new ToolStripMenuItem("📤 Сохранить результаты...", null, MenuSaveResults_Click);
            var miExit    = new ToolStripMenuItem("🚪 Выход",               null, MenuExit_Click) { ShortcutKeys = Keys.Alt | Keys.F4 };
            miFile.DropDownItems.AddRange(new ToolStripItem[]
                { miSave, miLoad, new ToolStripSeparator(), miSaveRes, new ToolStripSeparator(), miExit });

            var miSearch     = new ToolStripMenuItem("🔍 Поиск");
            var miOpenSearch = new ToolStripMenuItem("🔎 Конструктор поиска...", null, MenuSearch_Click) { ShortcutKeys = Keys.Control | Keys.F };
            var miShowAll    = new ToolStripMenuItem("📋 Показать все",         null, MenuShowAll_Click) { ShortcutKeys = Keys.Control | Keys.D0 };
            miSearch.DropDownItems.AddRange(new ToolStripItem[] { miOpenSearch, new ToolStripSeparator(), miShowAll });

            var miSort          = new ToolStripMenuItem("📊 Сортировка");
            var miSortPriceAsc  = new ToolStripMenuItem("По цене ↑",                  null, MenuSortByPriceAsc_Click);
            var miSortPriceDesc = new ToolStripMenuItem("По цене ↓",                  null, MenuSortByPriceDesc_Click);
            var miSortDateAsc   = new ToolStripMenuItem("По дате ↑ (старые)",         null, MenuSortByDateAsc_Click);
            var miSortDateDesc  = new ToolStripMenuItem("По дате ↓ (новые)",          null, MenuSortByDateDesc_Click);
            var miSortRAM       = new ToolStripMenuItem("По объёму ОЗУ ↓",            null, MenuSortByRAM_Click);
            var miSortCPU       = new ToolStripMenuItem("По производителю CPU + ядра",null, MenuSortByCPU_Click);
            miSort.DropDownItems.AddRange(new ToolStripItem[]
                { miSortPriceAsc, miSortPriceDesc, new ToolStripSeparator(),
                  miSortDateAsc,  miSortDateDesc,  new ToolStripSeparator(),
                  miSortRAM, miSortCPU });

            var miEdit   = new ToolStripMenuItem("✏️ Правка");
            var miClear  = new ToolStripMenuItem("🗑 Очистить все заказы", null, MenuClearAll_Click)  { ShortcutKeys = Keys.Control | Keys.Delete };
            var miDelete = new ToolStripMenuItem("❌ Удалить выбранный",   null, MenuDelete_Click) { ShortcutKeys = Keys.Delete };
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

            // ── ToolStrip ─────────────────────────────────────────
            toolStrip1 = new ToolStrip
            {
                GripStyle = ToolStripGripStyle.Visible,   // можно перетаскивать
                AllowItemReorder = true
            };

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

            // ── StatusStrip ───────────────────────────────────────
            statusStrip1 = new StatusStrip { Font = new Font("Segoe UI", 9F) };

            tsslCount    = new ToolStripStatusLabel { Text = "Заказов: 0", AutoSize = true,
                BorderSides = ToolStripStatusLabelBorderSides.Right };
            tsslAction   = new ToolStripStatusLabel { Text = "Действие: —", Spring = true,
                TextAlign = ContentAlignment.MiddleLeft };
            tsslDateTime = new ToolStripStatusLabel { Text = "", AutoSize = true,
                BorderSides = ToolStripStatusLabelBorderSides.Left };

            statusStrip1.Items.AddRange(new ToolStripItem[] { tsslCount, tsslAction, tsslDateTime });

            // ── Основные контролы ─────────────────────────────────
            gbMain          = new GroupBox();
            cbModelCPU      = new ComboBox();
            cbCPUCore       = new ComboBox();
            lblHardDrive    = new Label();
            lblOrderMaker   = new Label();
            lblComputerType = new Label();
            cbComputerType  = new ComboBox();
            rbSSD           = new RadioButton();
            lblCPUMaker     = new Label();
            rbHDD           = new RadioButton();
            cbCPUMaker      = new ComboBox();
            lblCPUModel     = new Label();
            lblCPUCores     = new Label();
            calPurchase     = new MonthCalendar();
            gbGPU           = new GroupBox();
            cbGPU           = new ComboBox();
            lblGPUMaker     = new Label();
            chkMonitor      = new CheckBox();
            chkKeyboard     = new CheckBox();
            chkMouse        = new CheckBox();
            gbRAM           = new GroupBox();
            cbRamType       = new ComboBox();
            cbRamSize       = new ComboBox();
            lblRAMSize      = new Label();
            lblRAMType      = new Label();
            gbCost          = new GroupBox();
            pbCost          = new ProgressBar();
            lblCostValue    = new Label();
            TabCtrl         = new TabControl();
            CatalogTab      = new TabPage();
            OrderTab        = new TabPage();
            btnLoad         = new Button();
            btnAddOrder     = new Button();
            btnSave         = new Button();
            lblOrders       = new Label();
            lvOrders        = new ListView();
            btnDelete       = new Button();
            errorProvider   = new ErrorProvider(components);

            gbMain.SuspendLayout();
            gbGPU.SuspendLayout();
            gbRAM.SuspendLayout();
            gbCost.SuspendLayout();
            TabCtrl.SuspendLayout();
            CatalogTab.SuspendLayout();
            OrderTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
            SuspendLayout();

            // ── gbMain ────────────────────────────────────────────
            gbMain.Controls.Add(cbModelCPU); gbMain.Controls.Add(cbCPUCore);
            gbMain.Controls.Add(lblHardDrive); gbMain.Controls.Add(lblOrderMaker);
            gbMain.Controls.Add(cbHDDSize); gbMain.Controls.Add(lblHDDSize);
            gbMain.Controls.Add(lblComputerType); gbMain.Controls.Add(cbComputerType);
            gbMain.Controls.Add(rbSSD); gbMain.Controls.Add(lblCPUMaker);
            gbMain.Controls.Add(rbHDD); gbMain.Controls.Add(cbCPUMaker);
            gbMain.Controls.Add(lblCPUModel); gbMain.Controls.Add(lblCPUCores);
            gbMain.Controls.Add(calPurchase);
            gbMain.Font = new Font("Segoe UI Semibold", 14F);
            gbMain.Location = new Point(5, 4); gbMain.Size = new Size(885, 278);
            gbMain.TabStop = false; gbMain.Text = "Основные компоненты";
            gbMain.Enter += gbMain_Enter;

            cbModelCPU.Cursor = Cursors.Hand; cbModelCPU.DropDownStyle = ComboBoxStyle.DropDownList;
            cbModelCPU.Font = new Font("Segoe UI", 12F);
            cbModelCPU.Items.AddRange(new object[] { "4","5","6","7","8","9","10" });
            cbModelCPU.Location = new Point(272, 163); cbModelCPU.Name = "cbModelCPU";
            cbModelCPU.Size = new Size(206, 29);
            cbModelCPU.SelectedIndexChanged += comboBox1_SelectedIndexChanged;

            cbCPUCore.Cursor = Cursors.Hand; cbCPUCore.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCPUCore.Font = new Font("Segoe UI", 12F);
            cbCPUCore.Items.AddRange(new object[] { "6","8","10","12","14","16" });
            cbCPUCore.Location = new Point(272, 119); cbCPUCore.Name = "cbCPUCore";
            cbCPUCore.Size = new Size(206, 29);
            cbCPUCore.SelectedIndexChanged += cbCoreMaker_SelectedIndexChanged;

            lblHardDrive.AutoSize = true; lblHardDrive.Font = new Font("Segoe UI", 12F);
            lblHardDrive.Location = new Point(23, 207); lblHardDrive.Text = "Жесткий диск:";

            lblHDDSize = new Label();
            lblHDDSize.AutoSize = true; lblHDDSize.Font = new Font("Segoe UI", 12F);
            lblHDDSize.Location = new Point(23, 245); lblHDDSize.Text = "Размер диска (ГБ):";

            cbHDDSize = new ComboBox();
            cbHDDSize.Cursor = Cursors.Hand; cbHDDSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cbHDDSize.Font = new Font("Segoe UI", 12F);
            cbHDDSize.Items.AddRange(new object[] { "256", "512", "1000", "2000", "4000" });
            cbHDDSize.Location = new Point(272, 242); cbHDDSize.Name = "cbHDDSize";
            cbHDDSize.Size = new Size(206, 29);
            cbHDDSize.SelectedIndexChanged += AnyControl_Changed;

            lblOrderMaker.AutoSize = true; lblOrderMaker.Font = new Font("Segoe UI", 12F);
            lblOrderMaker.Location = new Point(566, 43); lblOrderMaker.Text = "Дата оформления заказа";

            lblComputerType.AutoSize = true; lblComputerType.Font = new Font("Segoe UI", 12F);
            lblComputerType.Location = new Point(23, 43); lblComputerType.Text = "Тип компьютера:";
            lblComputerType.Click += lblComputerType_Click;

            cbComputerType.Cursor = Cursors.Hand; cbComputerType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbComputerType.Font = new Font("Segoe UI", 12F);
            cbComputerType.Items.AddRange(new object[] { "Рабочая станция","Сервер","Ноутбук","Моноблок","Мини-ПК" });
            cbComputerType.Location = new Point(272, 40); cbComputerType.Name = "cbComputerType";
            cbComputerType.Size = new Size(206, 29);
            cbComputerType.SelectedIndexChanged += AnyControl_Changed;

            rbSSD.AutoSize = true; rbSSD.Checked = true; rbSSD.Font = new Font("Segoe UI", 12F);
            rbSSD.Location = new Point(365, 205); rbSSD.Name = "rbSSD"; rbSSD.Text = "SSD";
            rbSSD.TabStop = true; rbSSD.CheckedChanged += AnyControl_Changed;

            lblCPUMaker.AutoSize = true; lblCPUMaker.Font = new Font("Segoe UI", 12F);
            lblCPUMaker.Location = new Point(23, 84); lblCPUMaker.Text = "Производитель CPU:";
            lblCPUMaker.Click += lblCPUMaker_Click;

            rbHDD.AutoSize = true; rbHDD.Font = new Font("Segoe UI", 12F);
            rbHDD.Location = new Point(272, 205); rbHDD.Name = "rbHDD"; rbHDD.Text = "HDD";
            rbHDD.CheckedChanged += AnyControl_Changed;

            cbCPUMaker.Cursor = Cursors.Hand; cbCPUMaker.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCPUMaker.Font = new Font("Segoe UI", 12F);
            cbCPUMaker.Items.AddRange(new object[] { "Intel","AMD" });
            cbCPUMaker.Location = new Point(272, 81); cbCPUMaker.Name = "cbCPUMaker";
            cbCPUMaker.Size = new Size(206, 29);
            cbCPUMaker.SelectedIndexChanged += AnyControl_Changed;

            lblCPUModel.AutoSize = true; lblCPUModel.Font = new Font("Segoe UI", 12F);
            lblCPUModel.Location = new Point(23, 166); lblCPUModel.Text = "Модель CPU:";

            lblCPUCores.AutoSize = true; lblCPUCores.Font = new Font("Segoe UI", 12F);
            lblCPUCores.Location = new Point(23, 125); lblCPUCores.Text = "Ядер CPU:";
            lblCPUCores.Click += lblCPUCores_Click;

            calPurchase.Location = new Point(566, 73); calPurchase.MaxSelectionCount = 1;
            calPurchase.Name = "calPurchase";

            // ── gbGPU ─────────────────────────────────────────────
            gbGPU.Controls.Add(cbGPU); gbGPU.Controls.Add(lblGPUMaker);
            gbGPU.Controls.Add(chkMonitor); gbGPU.Controls.Add(chkKeyboard); gbGPU.Controls.Add(chkMouse);
            gbGPU.Font = new Font("Segoe UI Semibold", 14F);
            gbGPU.Location = new Point(5, 426); gbGPU.Size = new Size(885, 213);
            gbGPU.TabStop = false; gbGPU.Text = "Видеокарта и периферия";
            gbGPU.Enter += gbGPU_Enter;

            cbGPU.Cursor = Cursors.Hand; cbGPU.DropDownStyle = ComboBoxStyle.DropDownList;
            cbGPU.Font = new Font("Segoe UI", 12F);
            cbGPU.Items.AddRange(new object[]
                { "RTX 4050","RTX 4070","RTX 4080","RTX 4090",
                  "RTX 5050","RTX 5070","RTX 5080","RTX 5090",
                  "RX 9060","RX 9070" });
            cbGPU.Location = new Point(272, 48); cbGPU.Name = "cbGPU"; cbGPU.Size = new Size(206, 29);
            cbGPU.SelectedIndexChanged += cbGPU_SelectedIndexChanged;

            lblGPUMaker.AutoSize = true; lblGPUMaker.Font = new Font("Segoe UI", 12F);
            lblGPUMaker.Location = new Point(23, 48); lblGPUMaker.Text = "GPU:";
            lblGPUMaker.Click += lblGPUMaker_Click;

            chkMonitor.AutoSize = true; chkMonitor.Font = new Font("Segoe UI", 12F);
            chkMonitor.Location = new Point(23, 88); chkMonitor.Text = "Монитор (+9 000₽)";
            chkMonitor.CheckedChanged += AnyControl_Changed;

            chkKeyboard.AutoSize = true; chkKeyboard.Font = new Font("Segoe UI", 12F);
            chkKeyboard.Location = new Point(23, 129); chkKeyboard.Text = "Клавиатура (+1 500₽)";
            chkKeyboard.CheckedChanged += AnyControl_Changed;

            chkMouse.AutoSize = true; chkMouse.Font = new Font("Segoe UI", 12F);
            chkMouse.Location = new Point(23, 169); chkMouse.Text = "Мышь (+1 000₽)";
            chkMouse.CheckedChanged += AnyControl_Changed;

            // ── gbRAM ─────────────────────────────────────────────
            gbRAM.Controls.Add(cbRamType); gbRAM.Controls.Add(cbRamSize);
            gbRAM.Controls.Add(lblRAMSize); gbRAM.Controls.Add(lblRAMType);
            gbRAM.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold);
            gbRAM.Location = new Point(5, 288); gbRAM.Size = new Size(885, 132);
            gbRAM.TabStop = false; gbRAM.Text = "Оперативная память";

            cbRamType.Cursor = Cursors.Hand; cbRamType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRamType.Font = new Font("Segoe UI", 12F);
            cbRamType.Items.AddRange(new object[] { "DDR3","DDR4","DDR5" });
            cbRamType.Location = new Point(272, 71); cbRamType.Name = "cbRamType"; cbRamType.Size = new Size(206, 29);
            cbRamType.SelectedIndexChanged += cbRamType_SelectedIndexChanged;

            cbRamSize.Cursor = Cursors.Hand; cbRamSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRamSize.Font = new Font("Segoe UI", 12F);
            cbRamSize.Items.AddRange(new object[] { "8","16","32","64" });
            cbRamSize.Location = new Point(272, 36); cbRamSize.Name = "cbRamSize"; cbRamSize.Size = new Size(206, 29);
            cbRamSize.SelectedIndexChanged += cbRamSize_SelectedIndexChanged;

            lblRAMSize.AutoSize = true; lblRAMSize.Font = new Font("Segoe UI", 12F);
            lblRAMSize.Location = new Point(23, 39); lblRAMSize.Text = "Объём (ГБ):";

            lblRAMType.AutoSize = true; lblRAMType.Font = new Font("Segoe UI", 12F);
            lblRAMType.Location = new Point(23, 74); lblRAMType.Text = "Тип:";

            // ── gbCost ────────────────────────────────────────────
            gbCost.Controls.Add(pbCost);
            gbCost.Controls.Add(lblCostValue);
            gbCost.Controls.Add(btnSaveOrder);
            gbCost.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold);
            gbCost.Location = new Point(5, 645); gbCost.Size = new Size(885, 95);
            gbCost.TabStop = false; gbCost.Text = "Расчёт стоимости";

            pbCost.Location = new Point(10, 30); pbCost.Maximum = 500000;
            pbCost.Name = "pbCost"; pbCost.Size = new Size(468, 22);

            lblCostValue.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblCostValue.ForeColor = Color.DarkGreen;
            lblCostValue.Location = new Point(492, 28); lblCostValue.Size = new Size(220, 22);
            lblCostValue.Text = "0 ₽";

            btnSaveOrder = new Button();
            btnSaveOrder.Text = "💾 Сохранить заказ";
            btnSaveOrder.Font = new Font("Segoe UI Semibold", 10F);
            btnSaveOrder.Location = new Point(720, 24);
            btnSaveOrder.Size = new Size(152, 30);
            btnSaveOrder.FlatStyle = FlatStyle.Flat;
            btnSaveOrder.BackColor = Color.FromArgb(30, 160, 100);
            btnSaveOrder.ForeColor = Color.White;
            btnSaveOrder.Name = "btnSaveOrder";
            btnSaveOrder.Click += BtnSaveOrder_Click;

            // ── TabControl ────────────────────────────────────────
            TabCtrl.Controls.Add(CatalogTab); TabCtrl.Controls.Add(OrderTab);
            TabCtrl.Font = new Font("Segoe UI Semibold", 14F);
            // FIX: Dock.Fill — автоматически занимает пространство ниже MenuStrip и ToolStrip,
            // выше StatusStrip. Устраняет перекрытие контролов.
            TabCtrl.Dock = DockStyle.Fill;
            TabCtrl.SelectedIndex = 0;

            CatalogTab.AutoScroll = true;
            CatalogTab.Controls.Add(gbMain); CatalogTab.Controls.Add(gbCost);
            CatalogTab.Controls.Add(btnLoad); CatalogTab.Controls.Add(btnAddOrder);
            CatalogTab.Controls.Add(gbGPU); CatalogTab.Controls.Add(gbRAM);
            CatalogTab.Controls.Add(btnSave);
            CatalogTab.Location = new Point(4, 34); CatalogTab.Size = new Size(899, 761);
            CatalogTab.Text = "📋 Каталог";

            btnLoad.BackColor = Color.FromArgb(150, 80, 200); btnLoad.FlatStyle = FlatStyle.Flat;
            btnLoad.Font = new Font("Segoe UI Semibold", 14F); btnLoad.ForeColor = Color.White;
            btnLoad.Location = new Point(750, 719); btnLoad.Size = new Size(140, 36);
            btnLoad.Text = "📂 Загрузить"; btnLoad.Click += BtnLoad_Click;

            btnAddOrder.BackColor = Color.FromArgb(0, 120, 215); btnAddOrder.FlatStyle = FlatStyle.Flat;
            btnAddOrder.Font = new Font("Segoe UI Semibold", 14F); btnAddOrder.ForeColor = Color.White;
            btnAddOrder.Location = new Point(370, 719); btnAddOrder.Size = new Size(192, 36);
            btnAddOrder.Text = "✅ Добавить заказ"; btnAddOrder.Click += BtnAddOrder_Click;

            btnSave.BackColor = Color.FromArgb(30, 160, 100); btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Segoe UI Semibold", 14F); btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(15, 719); btnSave.Size = new Size(167, 36);
            btnSave.Text = "💾 Сохранить"; btnSave.Click += BtnSave_Click;

            OrderTab.Controls.Add(lblOrders); OrderTab.Controls.Add(lvOrders); OrderTab.Controls.Add(btnDelete);
            OrderTab.Location = new Point(4, 34); OrderTab.Size = new Size(899, 761);
            OrderTab.Text = "📦 Мои заказы";

            lblOrders.AutoSize = true; lblOrders.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblOrders.Location = new Point(5, 8); lblOrders.Text = "Список заказов:";

            lvOrders.FullRowSelect = true; lvOrders.GridLines = true;
            lvOrders.Location = new Point(5, 30); lvOrders.Size = new Size(891, 456);
            lvOrders.View = View.Details; lvOrders.UseCompatibleStateImageBehavior = false;
            lvOrders.SelectedIndexChanged += LvOrders_SelectedIndexChanged;

            btnDelete.BackColor = Color.FromArgb(200, 50, 50); btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold);
            btnDelete.ForeColor = Color.White; btnDelete.Location = new Point(712, 508);
            btnDelete.Size = new Size(164, 47); btnDelete.Text = "🗑 Удалить";
            btnDelete.Click += BtnDelete_Click;

            errorProvider.ContainerControl = this;

            // ── Form ─────────────────────────────────────────────
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 245, 255);
            ClientSize = new Size(931, 870);
            MainMenuStrip = menuStrip1;

            // Порядок добавления важен: MenuStrip и ToolStrip докируются сверху,
            // StatusStrip — снизу, TabCtrl (Dock.Fill) занимает остаток.
            Controls.Add(TabCtrl);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);

            Text = "IT Лаборатория";

            menuStrip1.ResumeLayout(false); menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false); toolStrip1.PerformLayout();

            gbMain.ResumeLayout(false); gbMain.PerformLayout();
            gbGPU.ResumeLayout(false);  gbGPU.PerformLayout();
            gbRAM.ResumeLayout(false);  gbRAM.PerformLayout();
            gbCost.ResumeLayout(false);
            TabCtrl.ResumeLayout(false);
            CatalogTab.ResumeLayout(false);
            OrderTab.ResumeLayout(false); OrderTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        // ── Объявления полей ─────────────────────────────────────
        private MenuStrip menuStrip1 = null!;
        private ToolStrip toolStrip1 = null!;
        private StatusStrip statusStrip1 = null!;
        private ToolStripStatusLabel tsslCount = null!;
        private ToolStripStatusLabel tsslAction = null!;
        private ToolStripStatusLabel tsslDateTime = null!;
        private ToolStripMenuItem miToggleToolbar = null!;

        private GroupBox gbMain, gbGPU, gbRAM, gbCost;
        private TabControl TabCtrl;
        private TabPage CatalogTab, OrderTab;
        private ComboBox cbComputerType, cbCPUMaker, cbModelCPU, cbCPUCore, cbRamType, cbRamSize, cbGPU;
        private Label lblComputerType, lblCPUMaker, lblCPUModel, lblCPUCores;
        private Label lblRAMSize, lblRAMType, lblGPUMaker, lblCostValue, lblOrders, lblOrderMaker, lblHardDrive;
        private ProgressBar pbCost;
        private ErrorProvider errorProvider;
        private CheckBox chkMonitor, chkKeyboard, chkMouse;
        private RadioButton rbSSD, rbHDD;
        private ComboBox cbHDDSize;
        private Label lblHDDSize;
        private Button btnAddOrder, btnSave, btnLoad, btnDelete, btnSaveOrder;
        private MonthCalendar calPurchase;
        private ListView lvOrders;
    }
}
