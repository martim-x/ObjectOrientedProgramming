using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Lab2
{
    public static class FileManager
    {
        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

        public static void Save(List<Computer> computers, string path)
        {
            string json = JsonSerializer.Serialize(computers, Options);
            File.WriteAllText(path, json);
        }

        public static List<Computer> Load(string path)
        {
            if (!File.Exists(path)) return new List<Computer>();
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<Computer>>(json, Options) ?? new List<Computer>();
        }
    }
}
