using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class IniFile {
  Dictionary<string, Dictionary<string, string>> ini = new Dictionary<string, Dictionary<string, string>>(StringComparer.InvariantCultureIgnoreCase);

  public IniFile(string file) {
    var txt = "";
    if (File.Exists(file)) 
      txt = File.ReadAllText(file);

    Dictionary<string, string> currentSection = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

    ini[""] = currentSection;

    foreach (var line in txt.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                           .Where(t => !string.IsNullOrWhiteSpace(t))
                           .Select(t => t.Trim())) {

      if (line.StartsWith(";") || line.StartsWith("#")) continue;

      if (line.StartsWith("[") && line.EndsWith("]")) {
        currentSection = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        ini[line.Substring(1, line.LastIndexOf("]") - 1)] = currentSection;
        continue;
      }

      var idx = line.IndexOf("=");
      if (idx == -1)
        currentSection[line] = "";
      else
        currentSection[line.Substring(0, idx).Trim()] = line.Substring(idx + 1).Trim();
    }
  }

  public string GetValue(string key) {
    return GetValue("", key, "");
  }

  public string GetValue(string key, string section) {
    return GetValue(section, key, "");
  }

  public string GetValue(string section, string key, string @default) {
    if (!ini.ContainsKey(section))
      return @default;

    if (!ini[section].ContainsKey(key))
      return @default;

    return ini[section][key];
  }

  public string[] GetKeys(string section) {
    if (!ini.ContainsKey(section))
      return new string[0];

    return ini[section].Keys.ToArray();
  }

  public string[] GetSections() {
    return ini.Keys.Where(t => t != "").ToArray();
  }

  public void SetValue(string section, string key, string value) {
    if (!ini.ContainsKey(section)) {
      Dictionary<string, string> currentSection = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
      ini.Add(section, currentSection);
    }
    ini[section][key] = value;
  }

  public void SaveSettings(string newFilePath) {
    string strToSave = "";

    foreach (string section in ini.Keys) {
      if (section == "" && ini[section].Keys.Count == 0) continue;
      strToSave += ("[" + section + "]\r\n");

      foreach (string key  in ini[section].Keys) {
          strToSave += (key + "=" + ini[section][key] + "\r\n");
      }
    }

    File.WriteAllText(newFilePath, strToSave);
  }
}