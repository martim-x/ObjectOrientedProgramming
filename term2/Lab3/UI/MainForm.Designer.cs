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
            menuStrip1 = new MenuStrip();
            miFile = new ToolStripMenuItem();
            miSearch = new ToolStripMenuItem();
            miSort = new ToolStripMenuItem();
            miEdit = new ToolStripMenuItem();
            miView = new ToolStripMenuItem();
            miHelp = new ToolStripMenuItem();
            toolStrip1 = new ToolStrip();
            statusStrip1 = new StatusStrip();
            tsslCount = new ToolStripStatusLabel();
            tsslAction = new ToolStripStatusLabel();
            tsslDateTime = new ToolStripStatusLabel();
            gbMain = new GroupBox();
            cbModelCPU = new ComboBox();
            cbCPUCore = new ComboBox();
            lblHardDrive = new Label();
            lblOrderMaker = new Label();
            cbHDDSize = new ComboBox();
            lblHDDSize = new Label();
            lblComputerType = new Label();
            cbComputerType = new ComboBox();
            rbSSD = new RadioButton();
            lblCPUMaker = new Label();
            rbHDD = new RadioButton();
            cbCPUMaker = new ComboBox();
            lblCPUModel = new Label();
            lblCPUCores = new Label();
            calPurchase = new MonthCalendar();
            gbGPU = new GroupBox();
            cbGPU = new ComboBox();
            lblGPUMaker = new Label();
            chkMonitor = new CheckBox();
            chkKeyboard = new CheckBox();
            chkMouse = new CheckBox();
            gbRAM = new GroupBox();
            cbRamType = new ComboBox();
            cbRamSize = new ComboBox();
            lblRAMSize = new Label();
            lblRAMType = new Label();
            gbCost = new GroupBox();
            pbCost = new ProgressBar();
            lblCostValue = new Label();
            btnAddOrder = new Button();
            btnLoad = new Button();
            btnSave = new Button();
            lblBreakdown = new Label();
            btnPriceDetails = new Button();
            TabCtrl = new TabControl();
            CatalogTab = new TabPage();
            OrderTab = new TabPage();
            lblOrders = new Label();
            lvOrders = new ListView();
            btnDelete = new Button();
            errorProvider = new ErrorProvider(components);
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            gbMain.SuspendLayout();
            gbGPU.SuspendLayout();
            gbRAM.SuspendLayout();
            gbCost.SuspendLayout();
            TabCtrl.SuspendLayout();
            CatalogTab.SuspendLayout();
            OrderTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { miFile, miSearch, miSort, miEdit, miView, miHelp });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(907, 24);
            menuStrip1.TabIndex = 3;
            // 
            // miFile
            // 
            miFile.Name = "miFile";
            miFile.Size = new Size(63, 20);
            miFile.Text = "📁 Файл";
            // 
            // miSearch
            // 
            miSearch.Name = "miSearch";
            miSearch.Size = new Size(69, 20);
            miSearch.Text = "🔍 Поиск";
            // 
            // miSort
            // 
            miSort.Name = "miSort";
            miSort.Size = new Size(100, 20);
            miSort.Text = "📊 Сортировка";
            // 
            // miEdit
            // 
            miEdit.Name = "miEdit";
            miEdit.Size = new Size(74, 20);
            miEdit.Text = "✏️ Правка";
            // 
            // miView
            // 
            miView.Name = "miView";
            miView.Size = new Size(54, 20);
            miView.Text = "👁 Вид";
            // 
            // miHelp
            // 
            miHelp.Name = "miHelp";
            miHelp.Size = new Size(80, 20);
            miHelp.Text = "❓ Справка";
            // 
            // toolStrip1
            // 
            toolStrip1.Location = new Point(0, 24);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(907, 25);
            toolStrip1.TabIndex = 2;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { tsslCount, tsslAction, tsslDateTime });
            statusStrip1.Location = new Point(0, 915);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(907, 22);
            statusStrip1.TabIndex = 1;
            // 
            // tsslCount
            // 
            tsslCount.Name = "tsslCount";
            tsslCount.Size = new Size(0, 17);
            // 
            // tsslAction
            // 
            tsslAction.Name = "tsslAction";
            tsslAction.Size = new Size(0, 17);
            // 
            // tsslDateTime
            // 
            tsslDateTime.Name = "tsslDateTime";
            tsslDateTime.Size = new Size(0, 17);
            // 
            // gbMain
            // 
            gbMain.Controls.Add(cbModelCPU);
            gbMain.Controls.Add(cbCPUCore);
            gbMain.Controls.Add(lblHardDrive);
            gbMain.Controls.Add(lblOrderMaker);
            gbMain.Controls.Add(cbHDDSize);
            gbMain.Controls.Add(lblHDDSize);
            gbMain.Controls.Add(lblComputerType);
            gbMain.Controls.Add(cbComputerType);
            gbMain.Controls.Add(rbSSD);
            gbMain.Controls.Add(lblCPUMaker);
            gbMain.Controls.Add(rbHDD);
            gbMain.Controls.Add(cbCPUMaker);
            gbMain.Controls.Add(lblCPUModel);
            gbMain.Controls.Add(lblCPUCores);
            gbMain.Controls.Add(calPurchase);
            gbMain.Font = new Font("Segoe UI Semibold", 14F);
            gbMain.Location = new Point(5, 4);
            gbMain.Name = "gbMain";
            gbMain.Size = new Size(885, 278);
            gbMain.TabIndex = 0;
            gbMain.TabStop = false;
            gbMain.Text = "Основные компоненты";
            gbMain.Enter += gbMain_Enter;
            // 
            // cbModelCPU
            // 
            cbModelCPU.Cursor = Cursors.Hand;
            cbModelCPU.DropDownStyle = ComboBoxStyle.DropDownList;
            cbModelCPU.Font = new Font("Segoe UI", 12F);
            cbModelCPU.Items.AddRange(new object[] { "4", "5", "6", "7", "8", "9", "10" });
            cbModelCPU.Location = new Point(272, 163);
            cbModelCPU.Name = "cbModelCPU";
            cbModelCPU.Size = new Size(206, 29);
            cbModelCPU.TabIndex = 0;
            cbModelCPU.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // cbCPUCore
            // 
            cbCPUCore.Cursor = Cursors.Hand;
            cbCPUCore.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCPUCore.Font = new Font("Segoe UI", 12F);
            cbCPUCore.Items.AddRange(new object[] { "6", "8", "10", "12", "14", "16" });
            cbCPUCore.Location = new Point(272, 119);
            cbCPUCore.Name = "cbCPUCore";
            cbCPUCore.Size = new Size(206, 29);
            cbCPUCore.TabIndex = 1;
            cbCPUCore.SelectedIndexChanged += cbCoreMaker_SelectedIndexChanged;
            // 
            // lblHardDrive
            // 
            lblHardDrive.AutoSize = true;
            lblHardDrive.Font = new Font("Segoe UI", 12F);
            lblHardDrive.Location = new Point(23, 207);
            lblHardDrive.Name = "lblHardDrive";
            lblHardDrive.Size = new Size(112, 21);
            lblHardDrive.TabIndex = 2;
            lblHardDrive.Text = "Жесткий диск:";
            // 
            // lblOrderMaker
            // 
            lblOrderMaker.AutoSize = true;
            lblOrderMaker.Font = new Font("Segoe UI", 12F);
            lblOrderMaker.Location = new Point(566, 43);
            lblOrderMaker.Name = "lblOrderMaker";
            lblOrderMaker.Size = new Size(189, 21);
            lblOrderMaker.TabIndex = 3;
            lblOrderMaker.Text = "Дата оформления заказа";
            // 
            // cbHDDSize
            // 
            cbHDDSize.Cursor = Cursors.Hand;
            cbHDDSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cbHDDSize.Font = new Font("Segoe UI", 12F);
            cbHDDSize.Items.AddRange(new object[] { "256", "512", "1000", "2000", "4000" });
            cbHDDSize.Location = new Point(272, 242);
            cbHDDSize.Name = "cbHDDSize";
            cbHDDSize.Size = new Size(206, 29);
            cbHDDSize.TabIndex = 4;
            cbHDDSize.SelectedIndexChanged += AnyControl_Changed;
            // 
            // lblHDDSize
            // 
            lblHDDSize.AutoSize = true;
            lblHDDSize.Font = new Font("Segoe UI", 12F);
            lblHDDSize.Location = new Point(23, 245);
            lblHDDSize.Name = "lblHDDSize";
            lblHDDSize.Size = new Size(141, 21);
            lblHDDSize.TabIndex = 5;
            lblHDDSize.Text = "Размер диска (ГБ):";
            // 
            // lblComputerType
            // 
            lblComputerType.AutoSize = true;
            lblComputerType.Font = new Font("Segoe UI", 12F);
            lblComputerType.Location = new Point(23, 43);
            lblComputerType.Name = "lblComputerType";
            lblComputerType.Size = new Size(133, 21);
            lblComputerType.TabIndex = 6;
            lblComputerType.Text = "Тип компьютера:";
            lblComputerType.Click += lblComputerType_Click;
            // 
            // cbComputerType
            // 
            cbComputerType.Cursor = Cursors.Hand;
            cbComputerType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbComputerType.Font = new Font("Segoe UI", 12F);
            cbComputerType.Items.AddRange(new object[] { "Рабочая станция", "Сервер", "Ноутбук", "Моноблок", "Мини-ПК" });
            cbComputerType.Location = new Point(272, 40);
            cbComputerType.Name = "cbComputerType";
            cbComputerType.Size = new Size(206, 29);
            cbComputerType.TabIndex = 7;
            cbComputerType.SelectedIndexChanged += AnyControl_Changed;
            // 
            // rbSSD
            // 
            rbSSD.AutoSize = true;
            rbSSD.Checked = true;
            rbSSD.Font = new Font("Segoe UI", 12F);
            rbSSD.Location = new Point(365, 205);
            rbSSD.Name = "rbSSD";
            rbSSD.Size = new Size(57, 25);
            rbSSD.TabIndex = 8;
            rbSSD.TabStop = true;
            rbSSD.Text = "SSD";
            rbSSD.CheckedChanged += AnyControl_Changed;
            // 
            // lblCPUMaker
            // 
            lblCPUMaker.AutoSize = true;
            lblCPUMaker.Font = new Font("Segoe UI", 12F);
            lblCPUMaker.Location = new Point(23, 84);
            lblCPUMaker.Name = "lblCPUMaker";
            lblCPUMaker.Size = new Size(158, 21);
            lblCPUMaker.TabIndex = 9;
            lblCPUMaker.Text = "Производитель CPU:";
            lblCPUMaker.Click += lblCPUMaker_Click;
            // 
            // rbHDD
            // 
            rbHDD.AutoSize = true;
            rbHDD.Font = new Font("Segoe UI", 12F);
            rbHDD.Location = new Point(272, 205);
            rbHDD.Name = "rbHDD";
            rbHDD.Size = new Size(61, 25);
            rbHDD.TabIndex = 10;
            rbHDD.Text = "HDD";
            rbHDD.CheckedChanged += AnyControl_Changed;
            // 
            // cbCPUMaker
            // 
            cbCPUMaker.Cursor = Cursors.Hand;
            cbCPUMaker.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCPUMaker.Font = new Font("Segoe UI", 12F);
            cbCPUMaker.Items.AddRange(new object[] { "Intel", "AMD" });
            cbCPUMaker.Location = new Point(272, 81);
            cbCPUMaker.Name = "cbCPUMaker";
            cbCPUMaker.Size = new Size(206, 29);
            cbCPUMaker.TabIndex = 11;
            cbCPUMaker.SelectedIndexChanged += AnyControl_Changed;
            // 
            // lblCPUModel
            // 
            lblCPUModel.AutoSize = true;
            lblCPUModel.Font = new Font("Segoe UI", 12F);
            lblCPUModel.Location = new Point(23, 166);
            lblCPUModel.Name = "lblCPUModel";
            lblCPUModel.Size = new Size(103, 21);
            lblCPUModel.TabIndex = 12;
            lblCPUModel.Text = "Модель CPU:";
            // 
            // lblCPUCores
            // 
            lblCPUCores.AutoSize = true;
            lblCPUCores.Font = new Font("Segoe UI", 12F);
            lblCPUCores.Location = new Point(23, 125);
            lblCPUCores.Name = "lblCPUCores";
            lblCPUCores.Size = new Size(82, 21);
            lblCPUCores.TabIndex = 13;
            lblCPUCores.Text = "Ядер CPU:";
            lblCPUCores.Click += lblCPUCores_Click;
            // 
            // calPurchase
            // 
            calPurchase.Location = new Point(566, 73);
            calPurchase.MaxSelectionCount = 1;
            calPurchase.Name = "calPurchase";
            calPurchase.TabIndex = 14;
            // 
            // gbGPU
            // 
            gbGPU.Controls.Add(cbGPU);
            gbGPU.Controls.Add(lblGPUMaker);
            gbGPU.Controls.Add(chkMonitor);
            gbGPU.Controls.Add(chkKeyboard);
            gbGPU.Controls.Add(chkMouse);
            gbGPU.Font = new Font("Segoe UI Semibold", 14F);
            gbGPU.Location = new Point(5, 426);
            gbGPU.Name = "gbGPU";
            gbGPU.Size = new Size(885, 213);
            gbGPU.TabIndex = 4;
            gbGPU.TabStop = false;
            gbGPU.Text = "Видеокарта и периферия";
            gbGPU.Enter += gbGPU_Enter;
            // 
            // cbGPU
            // 
            cbGPU.Cursor = Cursors.Hand;
            cbGPU.DropDownStyle = ComboBoxStyle.DropDownList;
            cbGPU.Font = new Font("Segoe UI", 12F);
            cbGPU.Items.AddRange(new object[] { "RTX 4050", "RTX 4070", "RTX 4080", "RTX 4090", "RTX 5050", "RTX 5070", "RTX 5080", "RTX 5090", "RX 9060", "RX 9070" });
            cbGPU.Location = new Point(272, 48);
            cbGPU.Name = "cbGPU";
            cbGPU.Size = new Size(206, 29);
            cbGPU.TabIndex = 0;
            cbGPU.SelectedIndexChanged += cbGPU_SelectedIndexChanged;
            // 
            // lblGPUMaker
            // 
            lblGPUMaker.AutoSize = true;
            lblGPUMaker.Font = new Font("Segoe UI", 12F);
            lblGPUMaker.Location = new Point(23, 48);
            lblGPUMaker.Name = "lblGPUMaker";
            lblGPUMaker.Size = new Size(44, 21);
            lblGPUMaker.TabIndex = 1;
            lblGPUMaker.Text = "GPU:";
            lblGPUMaker.Click += lblGPUMaker_Click;
            // 
            // chkMonitor
            // 
            chkMonitor.AutoSize = true;
            chkMonitor.Font = new Font("Segoe UI", 12F);
            chkMonitor.Location = new Point(23, 88);
            chkMonitor.Name = "chkMonitor";
            chkMonitor.Size = new Size(169, 25);
            chkMonitor.TabIndex = 2;
            chkMonitor.Text = "Монитор (+9 000₽)";
            chkMonitor.CheckedChanged += AnyControl_Changed;
            // 
            // chkKeyboard
            // 
            chkKeyboard.AutoSize = true;
            chkKeyboard.Font = new Font("Segoe UI", 12F);
            chkKeyboard.Location = new Point(23, 129);
            chkKeyboard.Name = "chkKeyboard";
            chkKeyboard.Size = new Size(185, 25);
            chkKeyboard.TabIndex = 3;
            chkKeyboard.Text = "Клавиатура (+1 500₽)";
            chkKeyboard.CheckedChanged += AnyControl_Changed;
            // 
            // chkMouse
            // 
            chkMouse.AutoSize = true;
            chkMouse.Font = new Font("Segoe UI", 12F);
            chkMouse.Location = new Point(23, 169);
            chkMouse.Name = "chkMouse";
            chkMouse.Size = new Size(149, 25);
            chkMouse.TabIndex = 4;
            chkMouse.Text = "Мышь (+1 000₽)";
            chkMouse.CheckedChanged += AnyControl_Changed;
            // 
            // gbRAM
            // 
            gbRAM.Controls.Add(cbRamType);
            gbRAM.Controls.Add(cbRamSize);
            gbRAM.Controls.Add(lblRAMSize);
            gbRAM.Controls.Add(lblRAMType);
            gbRAM.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold);
            gbRAM.Location = new Point(5, 288);
            gbRAM.Name = "gbRAM";
            gbRAM.Size = new Size(885, 132);
            gbRAM.TabIndex = 5;
            gbRAM.TabStop = false;
            gbRAM.Text = "Оперативная память";
            // 
            // cbRamType
            // 
            cbRamType.Cursor = Cursors.Hand;
            cbRamType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRamType.Font = new Font("Segoe UI", 12F);
            cbRamType.Items.AddRange(new object[] { "DDR3", "DDR4", "DDR5" });
            cbRamType.Location = new Point(272, 71);
            cbRamType.Name = "cbRamType";
            cbRamType.Size = new Size(206, 29);
            cbRamType.TabIndex = 0;
            cbRamType.SelectedIndexChanged += cbRamType_SelectedIndexChanged;
            // 
            // cbRamSize
            // 
            cbRamSize.Cursor = Cursors.Hand;
            cbRamSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRamSize.Font = new Font("Segoe UI", 12F);
            cbRamSize.Items.AddRange(new object[] { "8", "16", "32", "64" });
            cbRamSize.Location = new Point(272, 36);
            cbRamSize.Name = "cbRamSize";
            cbRamSize.Size = new Size(206, 29);
            cbRamSize.TabIndex = 1;
            cbRamSize.SelectedIndexChanged += cbRamSize_SelectedIndexChanged;
            // 
            // lblRAMSize
            // 
            lblRAMSize.AutoSize = true;
            lblRAMSize.Font = new Font("Segoe UI", 12F);
            lblRAMSize.Location = new Point(23, 39);
            lblRAMSize.Name = "lblRAMSize";
            lblRAMSize.Size = new Size(93, 21);
            lblRAMSize.TabIndex = 2;
            lblRAMSize.Text = "Объём (ГБ):";
            // 
            // lblRAMType
            // 
            lblRAMType.AutoSize = true;
            lblRAMType.Font = new Font("Segoe UI", 12F);
            lblRAMType.Location = new Point(23, 74);
            lblRAMType.Name = "lblRAMType";
            lblRAMType.Size = new Size(39, 21);
            lblRAMType.TabIndex = 3;
            lblRAMType.Text = "Тип:";
            // 
            // gbCost
            // 
            gbCost.Controls.Add(pbCost);
            gbCost.Controls.Add(lblCostValue);
            gbCost.Controls.Add(btnAddOrder);
            gbCost.Controls.Add(btnLoad);
            gbCost.Controls.Add(btnSave);
            gbCost.Controls.Add(lblBreakdown);
            gbCost.Controls.Add(btnPriceDetails);
            gbCost.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold);
            gbCost.Location = new Point(5, 645);
            gbCost.Name = "gbCost";
            gbCost.Size = new Size(885, 175);
            gbCost.TabIndex = 1;
            gbCost.TabStop = false;
            gbCost.Text = "Расчёт стоимости";
            // 
            // pbCost
            // 
            pbCost.Location = new Point(10, 28);
            pbCost.Maximum = 500000;
            pbCost.Name = "pbCost";
            pbCost.Size = new Size(468, 22);
            pbCost.TabIndex = 0;
            // 
            // lblCostValue
            // 
            lblCostValue.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblCostValue.ForeColor = Color.DarkGreen;
            lblCostValue.Location = new Point(492, 28);
            lblCostValue.Name = "lblCostValue";
            lblCostValue.Size = new Size(200, 22);
            lblCostValue.TabIndex = 1;
            lblCostValue.Text = "0 ₽";
            // 
            // btnAddOrder
            // 
            btnAddOrder.BackColor = Color.FromArgb(0, 120, 215);
            btnAddOrder.FlatStyle = FlatStyle.Flat;
            btnAddOrder.Font = new Font("Segoe UI Semibold", 14F);
            btnAddOrder.ForeColor = Color.White;
            btnAddOrder.Location = new Point(365, 112);
            btnAddOrder.Name = "btnAddOrder";
            btnAddOrder.Size = new Size(192, 36);
            btnAddOrder.TabIndex = 3;
            btnAddOrder.Text = "✅ Добавить заказ";
            btnAddOrder.UseVisualStyleBackColor = false;
            btnAddOrder.Click += BtnAddOrder_Click;
            // 
            // btnLoad
            // 
            btnLoad.BackColor = Color.FromArgb(150, 80, 200);
            btnLoad.FlatStyle = FlatStyle.Flat;
            btnLoad.Font = new Font("Segoe UI Semibold", 14F);
            btnLoad.ForeColor = Color.White;
            btnLoad.Location = new Point(707, 112);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(140, 36);
            btnLoad.TabIndex = 2;
            btnLoad.Text = "📂 Загрузить";
            btnLoad.UseVisualStyleBackColor = false;
            btnLoad.Click += BtnLoad_Click;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.FromArgb(30, 160, 100);
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Segoe UI Semibold", 14F);
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(10, 112);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(167, 36);
            btnSave.TabIndex = 6;
            btnSave.Text = "💾 Сохранить";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += BtnSave_Click;
            // 
            // lblBreakdown
            // 
            lblBreakdown.Font = new Font("Segoe UI", 9F);
            lblBreakdown.ForeColor = Color.FromArgb(60, 60, 60);
            lblBreakdown.Location = new Point(10, 65);
            lblBreakdown.Name = "lblBreakdown";
            lblBreakdown.Size = new Size(468, 44);
            lblBreakdown.TabIndex = 2;
            lblBreakdown.Text = "Выберите компоненты для расчёта цены...";
            // 
            // btnPriceDetails
            // 
            btnPriceDetails.BackColor = Color.FromArgb(0, 120, 215);
            btnPriceDetails.FlatStyle = FlatStyle.Flat;
            btnPriceDetails.Font = new Font("Segoe UI Semibold", 14F);
            btnPriceDetails.ForeColor = Color.White;
            btnPriceDetails.Location = new Point(707, 28);
            btnPriceDetails.Name = "btnPriceDetails";
            btnPriceDetails.Size = new Size(140, 36);
            btnPriceDetails.TabIndex = 3;
            btnPriceDetails.Text = "📊 Подробнее";
            btnPriceDetails.UseVisualStyleBackColor = false;
            btnPriceDetails.Click += BtnPriceDetails_Click;
            // 
            // TabCtrl
            // 
            TabCtrl.Controls.Add(CatalogTab);
            TabCtrl.Controls.Add(OrderTab);
            TabCtrl.Dock = DockStyle.Fill;
            TabCtrl.Font = new Font("Segoe UI Semibold", 14F);
            TabCtrl.Location = new Point(0, 49);
            TabCtrl.Name = "TabCtrl";
            TabCtrl.SelectedIndex = 0;
            TabCtrl.Size = new Size(907, 866);
            TabCtrl.TabIndex = 0;
            // 
            // CatalogTab
            // 
            CatalogTab.AutoScroll = true;
            CatalogTab.Controls.Add(gbMain);
            CatalogTab.Controls.Add(gbCost);
            CatalogTab.Controls.Add(gbGPU);
            CatalogTab.Controls.Add(gbRAM);
            CatalogTab.Location = new Point(4, 34);
            CatalogTab.Name = "CatalogTab";
            CatalogTab.Size = new Size(899, 828);
            CatalogTab.TabIndex = 0;
            CatalogTab.Text = "📋 Каталог";
            // 
            // OrderTab
            // 
            OrderTab.Controls.Add(lblOrders);
            OrderTab.Controls.Add(lvOrders);
            OrderTab.Controls.Add(btnDelete);
            OrderTab.Location = new Point(4, 34);
            OrderTab.Name = "OrderTab";
            OrderTab.Size = new Size(899, 828);
            OrderTab.TabIndex = 1;
            OrderTab.Text = "📦 Мои заказы";
            // 
            // lblOrders
            // 
            lblOrders.AutoSize = true;
            lblOrders.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblOrders.Location = new Point(5, 8);
            lblOrders.Name = "lblOrders";
            lblOrders.Size = new Size(123, 19);
            lblOrders.TabIndex = 0;
            lblOrders.Text = "Список заказов:";
            // 
            // lvOrders
            // 
            lvOrders.FullRowSelect = true;
            lvOrders.GridLines = true;
            lvOrders.Location = new Point(5, 30);
            lvOrders.Name = "lvOrders";
            lvOrders.Size = new Size(891, 456);
            lvOrders.TabIndex = 1;
            lvOrders.UseCompatibleStateImageBehavior = false;
            lvOrders.View = View.Details;
            lvOrders.SelectedIndexChanged += LvOrders_SelectedIndexChanged;
            // 
            // btnDelete
            // 
            btnDelete.BackColor = Color.FromArgb(200, 50, 50);
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold);
            btnDelete.ForeColor = Color.White;
            btnDelete.Location = new Point(712, 508);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(164, 47);
            btnDelete.TabIndex = 2;
            btnDelete.Text = "🗑 Удалить";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += BtnDelete_Click;
            // 
            // errorProvider
            // 
            errorProvider.ContainerControl = this;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 245, 255);
            ClientSize = new Size(907, 937);
            Controls.Add(TabCtrl);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "IT Лаборатория";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            gbMain.ResumeLayout(false);
            gbMain.PerformLayout();
            gbGPU.ResumeLayout(false);
            gbGPU.PerformLayout();
            gbRAM.ResumeLayout(false);
            gbRAM.PerformLayout();
            gbCost.ResumeLayout(false);
            TabCtrl.ResumeLayout(false);
            CatalogTab.ResumeLayout(false);
            OrderTab.ResumeLayout(false);
            OrderTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1 = null!;
        private ToolStrip toolStrip1 = null!;
        private StatusStrip statusStrip1 = null!;
        private ToolStripStatusLabel tsslCount = null!, tsslAction = null!, tsslDateTime = null!;
        private ToolStripMenuItem miToggleToolbar = null!;

        private GroupBox gbMain, gbGPU, gbRAM, gbCost;
        private TabControl TabCtrl;
        private TabPage CatalogTab, OrderTab;
        private ComboBox cbComputerType, cbCPUMaker, cbModelCPU, cbCPUCore, cbRamType, cbRamSize, cbGPU;
        private Label lblComputerType, lblCPUMaker, lblCPUModel, lblCPUCores;
        private Label lblRAMSize, lblRAMType, lblGPUMaker, lblCostValue, lblOrders, lblOrderMaker, lblHardDrive;
        private ProgressBar pbCost;
        private Label lblBreakdown;
        private ErrorProvider errorProvider;
        private CheckBox chkMonitor, chkKeyboard, chkMouse;
        private RadioButton rbSSD, rbHDD;
        private ComboBox cbHDDSize;
        private Label lblHDDSize;
        private Button btnAddOrder, btnSave, btnLoad, btnDelete, btnPriceDetails;
        private MonthCalendar calPurchase;
        private ListView lvOrders;
        private ToolStripMenuItem miFile;
        private ToolStripMenuItem miSearch;
        private ToolStripMenuItem miSort;
        private ToolStripMenuItem miEdit;
        private ToolStripMenuItem miView;
        private ToolStripMenuItem miHelp;
    }
}
