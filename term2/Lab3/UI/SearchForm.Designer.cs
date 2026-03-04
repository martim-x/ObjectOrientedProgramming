using System.Drawing;
using System.Windows.Forms;

namespace Lab2
{
    partial class SearchForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // ── Controls ─────────────────────────────────────────
            var lblTitle = new Label
            {
                Text = "Конструктор поискового запроса",
                Font = new Font("Segoe UI Semibold", 13F),
                Location = new Point(10, 8),
                AutoSize = true
            };

            var gbCriteria = new GroupBox
            {
                Text = "Критерии поиска",
                Font = new Font("Segoe UI Semibold", 11F),
                Location = new Point(8, 35),
                Size = new Size(810, 230)
            };

            flowCriteria = new FlowLayoutPanel
            {
                Location = new Point(8, 22),
                Size = new Size(792, 200),
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                WrapContents = false
            };
            gbCriteria.Controls.Add(flowCriteria);

            var btnAddCriteria = new Button
            {
                Text = "➕ Добавить критерий",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(8, 272),
                Size = new Size(180, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White
            };
            btnAddCriteria.Click += BtnAddCriteria_Click;

            var btnSearch = new Button
            {
                Text = "🔍 Поиск",
                Font = new Font("Segoe UI Semibold", 11F),
                Location = new Point(196, 272),
                Size = new Size(130, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 160, 100),
                ForeColor = Color.White
            };
            btnSearch.Click += BtnSearch_Click;

            var btnClear = new Button
            {
                Text = "🗑 Очистить",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(334, 272),
                Size = new Size(120, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(200, 80, 50),
                ForeColor = Color.White
            };
            btnClear.Click += BtnClear_Click;

            var btnSaveResults = new Button
            {
                Text = "💾 Сохранить результаты",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(462, 272),
                Size = new Size(190, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(150, 80, 200),
                ForeColor = Color.White
            };
            btnSaveResults.Click += BtnSaveResults_Click;

            // ── Hint label (regex cheatsheet) ────────────────────
            var lblHint = new Label
            {
                Text = "Подсказка RegEx: \\d = цифра, \\w = буква/цифра, ^ = начало, $ = конец, " +
                       "[a-z] = диапазон, {n,m} = кол-во, .* = любые символы",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.Gray,
                Location = new Point(8, 310),
                Size = new Size(810, 20),
                AutoEllipsis = true
            };

            // ── Results ──────────────────────────────────────────
            lblResultCount = new Label
            {
                Text = "Найдено: 0",
                Font = new Font("Segoe UI Semibold", 10F),
                Location = new Point(8, 336),
                AutoSize = true,
                ForeColor = Color.DarkBlue
            };

            lvResults = new ListView
            {
                Location = new Point(8, 358),
                Size = new Size(810, 280),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            lvResults.Columns.Add("№",   42);
            lvResults.Columns.Add("Тип", 115);
            lvResults.Columns.Add("CPU", 175);
            lvResults.Columns.Add("GPU", 135);
            lvResults.Columns.Add("ОЗУ",  85);
            lvResults.Columns.Add("Диск",  65);
            lvResults.Columns.Add("Дата",  92);
            lvResults.Columns.Add("Цена ₽", 115);

            // ── Form ─────────────────────────────────────────────
            SuspendLayout();
            ClientSize = new Size(832, 655);
            Text = "Поиск по базе";
            BackColor = Color.FromArgb(240, 245, 255);
            MinimumSize = new Size(848, 690);
            FormBorderStyle = FormBorderStyle.Sizable;
            StartPosition = FormStartPosition.CenterParent;

            Controls.AddRange(new Control[]
            {
                lblTitle, gbCriteria, btnAddCriteria, btnSearch,
                btnClear, btnSaveResults, lblHint, lblResultCount, lvResults
            });

            ResumeLayout(false);
        }

        private FlowLayoutPanel flowCriteria = null!;
        private ListView lvResults = null!;
        private Label lblResultCount = null!;
    }
}
