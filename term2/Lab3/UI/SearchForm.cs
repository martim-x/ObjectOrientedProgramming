using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Lab2
{
    /// <summary>
    /// Окно конструктора поисковых запросов.
    /// Поддерживает множественные критерии и регулярные выражения.
    /// </summary>
    public partial class SearchForm : Form
    {
        private readonly List<Computer> _source;

        // Уникальный счётчик для имён контролов каждой строки
        private int _rowCounter = 0;

        public List<Computer> Results { get; private set; } = new();

        private readonly string[] _fields =
        {
            "Тип компьютера", "Производитель CPU", "Серия CPU", "Модель CPU",
            "Производитель GPU", "Модель GPU", "Тип ОЗУ", "Тип диска"
        };

        public SearchForm(List<Computer> source)
        {
            _source = source;
            InitializeComponent();
            AddCriteriaRow();   // первая строка
        }

        // ── Добавить строку критерия ──────────────────────────────
        private void BtnAddCriteria_Click(object sender, EventArgs e)
        {
            if (flowCriteria.Controls.Count >= 6)
            {
                MessageBox.Show("Максимум 6 критериев.", "Поиск",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            AddCriteriaRow();
        }

        /// <summary>
        /// FIX: каждой строке присваивается уникальный ID (_rowCounter),
        /// а не порядковый индекс в FlowPanel — это устраняет баг, когда
        /// после удаления промежуточной строки CollectCriteria не находила
        /// контролы по имени.
        /// </summary>
        private void AddCriteriaRow()
        {
            int id = _rowCounter++;
            bool isFirst = (flowCriteria.Controls.Count == 0);

            var panel = new Panel { Width = 780, Height = 36 };
            // Храним ID в Tag, чтобы CollectCriteria мог его читать
            panel.Tag = id;

            int xOff = 0;

            if (!isFirst)
            {
                var cbLogic = new ComboBox
                {
                    Width = 60, DropDownStyle = ComboBoxStyle.DropDownList,
                    Location = new Point(0, 6), Name = $"cbLogic_{id}"
                };
                cbLogic.Items.AddRange(new object[] { "AND", "OR" });
                cbLogic.SelectedIndex = 0;
                panel.Controls.Add(cbLogic);
                xOff = 68;
            }

            var cbField = new ComboBox
            {
                Width = 170, DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(xOff, 6), Name = $"cbField_{id}"
            };
            cbField.Items.AddRange(_fields);
            cbField.SelectedIndex = 0;
            panel.Controls.Add(cbField);

            var txtVal = new TextBox
            {
                Width = 260, Location = new Point(xOff + 178, 6),
                Name = $"txtVal_{id}", PlaceholderText = "Значение / RegEx"
            };
            panel.Controls.Add(txtVal);

            var chkRx = new CheckBox
            {
                Text = "RegEx", Location = new Point(xOff + 446, 8),
                Name = $"chkRx_{id}", AutoSize = true
            };
            panel.Controls.Add(chkRx);

            if (!isFirst)
            {
                var btnDel = new Button
                {
                    Text = "✕", Width = 30, Height = 26,
                    Location = new Point(xOff + 520, 4),
                    FlatStyle = FlatStyle.Flat, ForeColor = Color.Red
                };
                btnDel.Click += (s, e) => flowCriteria.Controls.Remove(panel);
                panel.Controls.Add(btnDel);
            }

            flowCriteria.Controls.Add(panel);
        }

        // ── Выполнить поиск ───────────────────────────────────────
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                var criteria = CollectCriteria();
                if (criteria.Count == 0)
                {
                    MessageBox.Show("Введите хотя бы один критерий.", "Поиск",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                IEnumerable<Computer> result = _source;
                bool firstDone = false;

                foreach (var (logic, field, value, useRegex) in criteria)
                {
                    var predicate = BuildPredicate(field, value, useRegex);

                    if (!firstDone)
                    {
                        result    = result.Where(predicate);
                        firstDone = true;
                    }
                    else
                    {
                        result = logic == "OR"
                            ? result.Union(_source.Where(predicate))
                            : result.Where(predicate);
                    }
                }

                Results = result.Distinct().ToList();
                DisplayResults(Results);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка поиска:\n" + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static Func<Computer, bool> BuildPredicate(string field, string value, bool useRegex)
        {
            string GetVal(Computer c) => field switch
            {
                "Тип компьютера"    => c.Type,
                "Производитель CPU" => c.CPU.Manufacturer,
                "Серия CPU"         => c.CPU.Series,
                "Модель CPU"        => c.CPU.Model,
                "Производитель GPU" => c.VideoCard.Manufacturer,
                "Модель GPU"        => c.VideoCard.Model,
                "Тип ОЗУ"           => c.RAMtype,
                "Тип диска"         => c.HDDtype,
                _                  => ""
            };

            if (useRegex)
            {
                // Проверяем RegEx заранее, чтобы получить понятное исключение
                var rx = new Regex(value, RegexOptions.IgnoreCase);
                return c => rx.IsMatch(GetVal(c));
            }
            return c => GetVal(c).Contains(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// FIX: читаем ID из panel.Tag, а не из счётчика цикла,
        /// поэтому удалённые строки не ломают сбор критериев.
        /// </summary>
        private List<(string logic, string field, string value, bool useRegex)> CollectCriteria()
        {
            var list = new List<(string, string, string, bool)>();

            foreach (Panel panel in flowCriteria.Controls)
            {
                int id = (int)panel.Tag;

                string logic    = "AND";
                string field    = _fields[0];
                string value    = "";
                bool   useRegex = false;

                foreach (Control ctrl in panel.Controls)
                {
                    if (ctrl.Name == $"cbLogic_{id}" && ctrl is ComboBox cbL) logic    = cbL.Text;
                    if (ctrl.Name == $"cbField_{id}" && ctrl is ComboBox cbF) field    = cbF.Text;
                    if (ctrl.Name == $"txtVal_{id}"  && ctrl is TextBox   tb) value    = tb.Text.Trim();
                    if (ctrl.Name == $"chkRx_{id}"   && ctrl is CheckBox chk) useRegex = chk.Checked;
                }

                if (!string.IsNullOrWhiteSpace(value))
                    list.Add((logic, field, value, useRegex));
            }
            return list;
        }

        private void DisplayResults(List<Computer> found)
        {
            lvResults.Items.Clear();
            lblResultCount.Text = $"Найдено: {found.Count}";

            for (int i = 0; i < found.Count; i++)
            {
                var c    = found[i];
                var item = new ListViewItem((i + 1).ToString());
                item.SubItems.Add(c.Type);
                item.SubItems.Add($"{c.CPU.Manufacturer} {c.CPU.Model}");
                item.SubItems.Add(c.VideoCard.Model);
                item.SubItems.Add($"{c.RAMsizeGB} ГБ {c.RAMtype}");
                item.SubItems.Add(c.HDDtype);
                item.SubItems.Add(c.PurchaseDate.ToString("dd.MM.yy"));
                item.SubItems.Add($"{c.Price():N0} ₽");
                item.BackColor = i % 2 == 0 ? Color.White : Color.AliceBlue;
                lvResults.Items.Add(item);
            }
        }

        // ── Сохранить результаты ──────────────────────────────────
        private void BtnSaveResults_Click(object sender, EventArgs e)
        {
            if (Results.Count == 0)
            {
                MessageBox.Show("Нет результатов для сохранения.", "Поиск",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dlg = new SaveFileDialog
            {
                Title    = "Сохранить результаты поиска",
                Filter   = "JSON файл (*.json)|*.json|XML файл (*.xml)|*.xml",
                FileName = "search_results"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                if (dlg.FilterIndex == 1)
                {
                    var opts = new JsonSerializerOptions { WriteIndented = true };
                    File.WriteAllText(dlg.FileName, JsonSerializer.Serialize(Results, opts));
                }
                else
                {
                    var xml = new System.Xml.Serialization.XmlSerializer(typeof(List<Computer>));
                    using var fs = File.Create(dlg.FileName);
                    xml.Serialize(fs, Results);
                }
                MessageBox.Show($"Сохранено {Results.Count} записей.", "Готово",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения:\n" + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            flowCriteria.Controls.Clear();
            _rowCounter = 0;
            AddCriteriaRow();
            lvResults.Items.Clear();
            lblResultCount.Text = "Найдено: 0";
            Results.Clear();
        }
    }
}
