using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab2
{
    public class PriceDetailsForm : Form
    {
        public PriceDetailsForm(Computer c)
        {
            Text            = "Разбивка стоимости";
            ClientSize      = new Size(520, 380);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            MinimizeBox     = false;
            StartPosition   = FormStartPosition.CenterParent;
            BackColor       = Color.FromArgb(245, 248, 255);
            Font            = new Font("Segoe UI", 10F);

            // ── Заголовок ──────────────────────────────────────
            var lblTitle = new Label
            {
                Text      = $"Конфигурация: {(string.IsNullOrEmpty(c.Type) ? "не указан тип" : c.Type)}",
                Font      = new Font("Segoe UI Semibold", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 80, 180),
                Location  = new Point(16, 14),
                Size      = new Size(488, 26),
                AutoSize  = false
            };

            // ── Таблица компонентов ────────────────────────────
            var lv = new ListView
            {
                Location              = new Point(16, 48),
                Size                  = new Size(488, 228),
                View                  = View.Details,
                FullRowSelect         = true,
                GridLines             = true,
                HeaderStyle           = ColumnHeaderStyle.Nonclickable,
                UseCompatibleStateImageBehavior = false,
                Font                  = new Font("Segoe UI", 10F),
                BorderStyle           = BorderStyle.FixedSingle
            };
            lv.Columns.Add("Компонент", 160);
            lv.Columns.Add("Описание",  200);
            lv.Columns.Add("Стоимость", 118);

            decimal typeMult = c.Type switch
            {
                "Рабочая станция" => 1.3m,
                "Сервер"          => 1.6m,
                "Ноутбук"         => 1.4m,
                "Моноблок"        => 1.2m,
                "Мини-ПК"         => 1.1m,
                _                 => 1.0m
            };

            decimal cpuRaw  = c.CPU?.MaxFrequency > 0 ? c.CPU.Price() : 0m;
            decimal gpuRaw  = c.VideoCard?.BasePrice > 0 ? c.VideoCard.Price() : 0m;
            decimal ramPerGB = (c.RAMtype ?? "") switch { "DDR3" => 50m, "DDR4" => 80m, "DDR5" => 140m, _ => 60m };
            decimal ramRaw  = c.RAMsizeGB > 0 ? c.RAMsizeGB * ramPerGB : 0m;
            decimal hddRaw  = c.HDDsizeGB > 0
                              ? (c.HDDtype == "SSD" ? c.HDDsizeGB * 8m : c.HDDsizeGB * 2m)
                              : 0m;
            decimal mon     = c.HasMonitor  ?  9_000m : 0m;
            decimal kb      = c.HasKeyboard ?  1_500m : 0m;
            decimal mouse   = c.HasMouse    ?  1_000m : 0m;

            void AddRow(string component, string desc, decimal raw, Color? bg = null)
            {
                if (raw <= 0) return;
                var item = new ListViewItem(component);
                item.SubItems.Add(desc);
                item.SubItems.Add($"{raw * typeMult:N0} ₽");
                if (bg.HasValue) item.BackColor = bg.Value;
                lv.Items.Add(item);
            }

            Color rowA = Color.White;
            Color rowB = Color.FromArgb(235, 242, 255);

            if (cpuRaw > 0) AddRow("🖥  Процессор",
                $"{c.CPU.Manufacturer} {c.CPU.Model}", cpuRaw,
                lv.Items.Count % 2 == 0 ? rowA : rowB);

            if (gpuRaw > 0) AddRow("🎮  Видеокарта",
                $"{c.VideoCard.Model} {c.VideoCard.MemoryGB}GB", gpuRaw,
                lv.Items.Count % 2 == 0 ? rowA : rowB);

            if (ramRaw > 0) AddRow("💾  Оперативная память",
                $"{c.RAMsizeGB} ГБ {c.RAMtype}", ramRaw,
                lv.Items.Count % 2 == 0 ? rowA : rowB);

            if (hddRaw > 0) AddRow("💿  Накопитель",
                $"{c.HDDsizeGB} ГБ {c.HDDtype}", hddRaw,
                lv.Items.Count % 2 == 0 ? rowA : rowB);

            if (mon > 0)   AddRow("🖥  Монитор",    "+9 000₽", mon,   lv.Items.Count % 2 == 0 ? rowA : rowB);
            if (kb > 0)    AddRow("⌨  Клавиатура", "+1 500₽", kb,    lv.Items.Count % 2 == 0 ? rowA : rowB);
            if (mouse > 0) AddRow("🖱  Мышь",       "+1 000₽", mouse, lv.Items.Count % 2 == 0 ? rowA : rowB);

            // ── Разделитель ────────────────────────────────────
            var sep = new Label
            {
                Location  = new Point(16, 284),
                Size      = new Size(488, 1),
                BackColor = Color.FromArgb(180, 180, 200),
                BorderStyle = BorderStyle.None
            };

            // ── Строка коэффициента ────────────────────────────
            var lblMult = new Label
            {
                Text      = typeMult != 1.0m
                            ? $"Коэффициент «{c.Type}»: × {typeMult:F1}"
                            : "",
                Font      = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(80, 80, 120),
                Location  = new Point(16, 292),
                Size      = new Size(300, 20)
            };

            // ── Итого ──────────────────────────────────────────
            var lblTotal = new Label
            {
                Text      = $"ИТОГО:  {c.Price():N0} ₽",
                Font      = new Font("Segoe UI Semibold", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 140, 60),
                Location  = new Point(16, 316),
                Size      = new Size(488, 26),
                TextAlign = ContentAlignment.MiddleRight
            };

            // ── Кнопка закрыть ─────────────────────────────────
            var btnClose = new Button
            {
                Text        = "Закрыть",
                DialogResult = DialogResult.OK,
                Location    = new Point(406, 346),
                Size        = new Size(98, 28),
                FlatStyle   = FlatStyle.Flat,
                BackColor   = Color.FromArgb(0, 120, 215),
                ForeColor   = Color.White,
                Font        = new Font("Segoe UI", 10F)
            };

            Controls.AddRange(new System.Windows.Forms.Control[]
                { lblTitle, lv, sep, lblMult, lblTotal, btnClose });

            AcceptButton = btnClose;
        }
    }
}
