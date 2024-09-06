using System.Data;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;

namespace Base {
  public class Start {
    static void Main(string[] args) {
      SearchSettings settings = getSearchSetting(args);
      
      List<string> paths = findPaths.handle(settings);

      printPaths.handle(paths);
    }

    
    private static SearchSettings getSearchSetting(string[] args) {
      
      SearchSettings settings = new SearchSettings();
      if (args.Length < 1) {return settings;} 

      Dictionary<string, Func<SearchSettings, string[], int, SearchSettings>> commands = new Dictionary<string, Func<SearchSettings, string[], int, SearchSettings>>()
      {
        {"-d", (ss, ar, p) => {
            ss.baseDir = ar[p + 1];
            return ss;
          }},
        {"-f", (ss, ar, p) => {
          ss.nameForFind = ar[p + 1];
          return ss;
        }},
        {"--not-case-distinction", (ss, ar, p) => {
            ss.searchParameters.MatchCasing = MatchCasing.CaseInsensitive;
            return ss;
        }},
        {"--block-hidden", (ss, ar, p) => {
            ss.searchParameters.AttributesToSkip = FileAttributes.Hidden | FileAttributes.System;
            return ss;
        }},
        {"--local-search", (ss, ar, p) => {
            ss.searchParameters.RecurseSubdirectories = false;
            return ss;
        }},
        {"--block", (ss, ar, p) => {
            ss.blockedDirectories = ar[p + 1].Split(';');
            return ss;
        }},
        {"--find-folders", (ss, ar, p) => {
          ss.itemTypeToSearch = ItemOptions.folders;
          return ss;
        }},
        {"--find-all", (ss, ar, p) => {
          ss.itemTypeToSearch = ItemOptions.all;
          return ss;
        }}
      };


      for(int c = 0; c < args.Length; c++) {
        try {
          settings = commands[args[c]](settings, args, c);
        } catch {}
      }

      return settings;
    }
  }

  enum ItemOptions {
    files = 0,
    folders = 1,
    all = 2
  }

  class SearchSettings {
    public string baseDir { get; set; } = Directory.GetCurrentDirectory();
    public string nameForFind { get; set; } = ".git";

    public string[] blockedDirectories { get; set; } = new string[]{};

    public ItemOptions itemTypeToSearch = ItemOptions.files;

    public EnumerationOptions searchParameters { get; set; } = new EnumerationOptions() {
      IgnoreInaccessible = true, 
      RecurseSubdirectories = true,
      MatchCasing = MatchCasing.CaseSensitive, 
      AttributesToSkip = FileAttributes.None
    };
  }

  class findPaths {
    static public List<string> handle(SearchSettings settings) {

      List<string> files = Directory.EnumerateFiles(settings.baseDir, settings.nameForFind, settings.searchParameters).ToList();

      if (settings.itemTypeToSearch == ItemOptions.folders) {
        files = Directory.GetDirectories(settings.baseDir, settings.nameForFind, settings.searchParameters).ToList();
      } else if (settings.itemTypeToSearch == ItemOptions.all) {
        files.AddRange(Directory.GetDirectories(settings.baseDir, settings.nameForFind, settings.searchParameters).ToList());
      }

      List<string> usablePaths = filterPaths(files, settings.blockedDirectories);
      
      return usablePaths;
    }

    
    static private List<string> filterPaths(List<string> paths, string[] invalidDirectories) {
      invalidDirectories = invalidDirectories ?? new string[0];

      paths = paths.OrderByDescending(x => x).Reverse().ToList<string>();

      foreach(string path in paths.ToList()) {
        if (hasInvalidDirectory(invalidDirectories, path)) {
          paths.Remove(path);
          continue;
        }
      }
      
      return paths;
    }
    
    static private bool hasInvalidDirectory(string[] invalidNames, string strForFind) {    
      foreach(string invalid in invalidNames) {
        if (strForFind.Contains(invalid)) {
          return true;
        }
      }

      return false;
    }
  }

  class printPaths {
    
    static public void handle(List<string> paths) {      
      if (paths.Count > 1) {
        Console.WriteLine(paths[0]);
        for (int c = 1; c < paths.Count; c++) { //loop for paths

          List<string> filesArray = paths[c].Split(@"\").ToList(); // array with directories of this path
          List<string> previousFilesArray = paths[c-1].Split(@"\").ToList(); // array with directories of previus path to compare

          printOnlyUniqueFiles(filesArray, previousFilesArray);
        }
      } else {
        Console.WriteLine("not have files");
      }

    }

    static private void printOnlyUniqueFiles(List<string> filesPath, List<string> previousFilesPath) {
      for(int i = 0; i < filesPath.Count; i++) { // loop for directories of a path
        bool differentDirectory;

        try {
          differentDirectory = (filesPath[i] != previousFilesPath[i]);
        } catch {
          differentDirectory = true;
        }

        if (differentDirectory) {
          for(int l = i; l < filesPath.Count; l++) { // print rest directories of this path, and jump for next path
            Console.Write(@"\" + filesPath[l]);
          }
          break;
        } 

        turnIntoSpaces(filesPath[i], testForIncrement: (i != 0));
      }
      Console.WriteLine(); // break line for nest path
    }

    static private void turnIntoSpaces(string str, int extra = 1, bool testForIncrement = true) {
      if (!testForIncrement) {
        extra = 0;
      }
      
      for(int v = 0; v < str.Length + extra; v++) {
        Console.Write(" ");
      }
    }
  }
}