using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab2
{
    /// <summary>
    /// Окно с подробной разбивкой формирования цены.
    /// Получает готовый частичный объект Computer и разбивает его цену по компонентам.
    /// </summary>
    public class PriceDetailsForm : Form
    {
        public PriceDetailsForm(Computer c)
        {
            Text = "Из чего складывается цена";
            ClientSize = new Size(480, 420);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 248, 255);

            // ── Вычисляем вклад каждого компонента ────────────────
            // Все расчёты идут до финального ÷3, чтобы показать "сырые" части
            decimal cpuRaw = c.CPU != null ? c.CPU.Price() / 10m : 0m;
            decimal gpuRaw = c.VideoCard != null ? c.VideoCard.Price() : 0m;

            decimal ramTypeMult = c.RAMtype switch
            {
                "DDR3" => 1.1m,
                "DDR4" => 1.7m,
                "DDR5" => 2.8m,
                _ => 1m
            };
            decimal ramRaw = c.RAMsizeGB * ramTypeMult * 10m;
            decimal hddRaw = c.HDDtype == "SSD" ? c.HDDsizeGB * 6m : c.HDDsizeGB * 1m;
            decimal periphRaw = (c.HasMonitor ? 9_000m : 0m)
                              + (c.HasKeyboard ? 1_500m : 0m)
                              + (c.HasMouse ? 1_000m : 0m);

            decimal subtotal = cpuRaw + gpuRaw + ramRaw + hddRaw + periphRaw;

            decimal typeMult = c.Type switch
            {
                "Рабочая станция" => 1.3m,
                "Сервер" => 1.6m,
                "Ноутбук" => 1.4m,
                "Моноблок" => 1.2m,
                "Мини-ПК" => 1.1m,
                _ => 1.0m
            };

            decimal afterMult = subtotal * typeMult;
            decimal total = Math.Round(afterMult / 3m, 2);

            // ── UI ────────────────────────────────────────────────
            var titleLabel = new Label
            {
                Text = "Разбивка цены компьютера",
                Font = new Font("Segoe UI Semibold", 13F),
                Location = new Point(16, 12),
                AutoSize = true
            };

            // ListView с тремя колонками: компонент / базовая стоимость / после коэфф.
            var lv = new ListView
            {
                Location = new Point(16, 44),
                Size = new Size(448, 280),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                Font = new Font("Segoe UI", 10F)
            };
            lv.Columns.Add("Компонент", 180);
            lv.Columns.Add("До коэф.", 110);
            lv.Columns.Add($"×{typeMult:F1} ÷ 3", 120);

            void AddRow(string name, decimal raw, Color? color = null)
            {
                decimal final = Math.Round(raw * typeMult / 3m, 2);
                var item = new ListViewItem(name);
                item.SubItems.Add(raw > 0 ? $"{raw:N0} ₽" : "—");
                item.SubItems.Add(raw > 0 ? $"{final:N0} ₽" : "—");
                if (color.HasValue) item.ForeColor = color.Value;
                lv.Items.Add(item);
            }

            string cpuName = c.CPU?.Model is { Length: > 0 } m ? $"CPU  {m}" : "CPU  (не выбран)";
            string gpuName = c.VideoCard?.Model is { Length: > 0 } gm ? $"GPU  {gm}" : "GPU  (не выбрана)";
            string ramName = c.RAMsizeGB > 0 ? $"ОЗУ  {c.RAMsizeGB} ГБ {c.RAMtype}" : "ОЗУ  (не выбрана)";
            string hddName = c.HDDsizeGB > 0 ? $"Диск  {c.HDDsizeGB} ГБ {c.HDDtype}" : "Диск  (не задан)";

            AddRow(cpuName, cpuRaw, cpuRaw > 0 ? Color.FromArgb(0, 100, 180) : Color.Gray);
            AddRow(gpuName, gpuRaw, gpuRaw > 0 ? Color.FromArgb(0, 130, 80) : Color.Gray);
            AddRow(ramName, ramRaw, ramRaw > 0 ? Color.FromArgb(150, 60, 180) : Color.Gray);
            AddRow(hddName, hddRaw, hddRaw > 0 ? Color.FromArgb(180, 100, 0) : Color.Gray);

            // Периферия — только если есть хоть что-то
            if (periphRaw > 0)
            {
                string periphDesc = string.Join(" + ",
                    c.HasMonitor ? new[] { "Монитор" } : Array.Empty<string>());
                // строим список включённой периферии
                var parts = new System.Collections.Generic.List<string>();
                if (c.HasMonitor) parts.Add("Монитор");
                if (c.HasKeyboard) parts.Add("Клавиатура");
                if (c.HasMouse) parts.Add("Мышь");
                AddRow($"Периферия  ({string.Join(", ", parts)})", periphRaw, Color.FromArgb(160, 40, 40));
            }

            // Разделитель
            var sepItem = new ListViewItem("─────────────────");
            sepItem.SubItems.Add(""); sepItem.SubItems.Add("");
            sepItem.ForeColor = Color.LightGray;
            lv.Items.Add(sepItem);

            // Итоговая строка
            var totalItem = new ListViewItem("Итого");
            totalItem.SubItems.Add($"{subtotal:N0} ₽");
            totalItem.SubItems.Add($"{total:N0} ₽");
            totalItem.Font = new Font("Segoe UI Semibold", 10F);
            totalItem.ForeColor = Color.DarkGreen;
            lv.Items.Add(totalItem);

            // ── Пояснение формулы ─────────────────────────────────
            string typeNote = typeMult != 1.0m
                ? $"Тип «{c.Type}» даёт коэффициент ×{typeMult:F1}. "
                : "";

            var lblFormula = new Label
            {
                Text = $"{typeNote}Финальная цена = (сумма компонентов × {typeMult:F1}) ÷ 3",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(16, 332),
                Size = new Size(448, 36),
                AutoEllipsis = false
            };

            var btnClose = new Button
            {
                Text = "Закрыть",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(360, 376),
                Size = new Size(104, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                DialogResult = DialogResult.OK
            };

            Controls.AddRange(new Control[] { titleLabel, lv, lblFormula, btnClose });
            AcceptButton = btnClose;
        }
    }
}
