using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using Playnite.SDK.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Controls;
using System.Security.Cryptography;
using Playnite.SDK.Events;
//using ZetaProducerHtmlCompressor.Internal;

namespace HtmlExporterPlugin
{
    

    public class HtmlExporterPlugin : GenericPlugin
    {
        public static IResourceProvider resources = new ResourceProvider();
        private HtmlExporterPluginSettingsViewModel Settings { get; set; }
        public HtmlExporterPluginSettingsView SettingsView { get; private set; }
        private static readonly ILogger logger = LogManager.GetLogger();
        public static string pluginFolder;
        public List<DirectoryInfo> TemplateFolders = new List<DirectoryInfo>();
        public override Guid Id { get; } = Guid.Parse("14bd031a-a1ff-4754-a586-0b9c23a6f557");

        private void UpdateTemplateFolders()
        {
            TemplateFolders.Clear();
            var templateSearchPaths = new List<string>() { Path.Combine(pluginFolder, "Templates"), Path.Combine(GetPluginUserDataPath(), "Templates")};
            foreach (string searchPath in templateSearchPaths)
            {
                Directory.CreateDirectory(searchPath);
                foreach (string dir in Directory.GetDirectories(searchPath, "*", SearchOption.TopDirectoryOnly))
                {

                    bool found = false;
                    DirectoryInfo dirinfo = new DirectoryInfo(dir);
                    foreach (DirectoryInfo founddir in TemplateFolders)
                    {
                        found = founddir.Name.ToLower().Equals(dirinfo.Name.ToLower());
                        if (found)
                        {
                            break;
                        }
                    }
                    if (!found)
                    {
                        TemplateFolders.Add(dirinfo);
                    }
                }
            }
        }

        public HtmlExporterPlugin(IPlayniteAPI api) : base(api)
        {
            pluginFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Localization.SetPluginLanguage(pluginFolder, api.ApplicationSettings.Language);
            UpdateTemplateFolders();
            Settings = new HtmlExporterPluginSettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
        }

        static readonly Regex re = new Regex(@"\%(\w+)\%", RegexOptions.Compiled);  //does not include empty string
        static readonly Regex re2 = new Regex(@"\%(\w*)\%", RegexOptions.Compiled); //includes empty string
        public string ReplaceDictionary(string text, Dictionary<string, string> keywords)
        {
            //2 times so user can generate variables in 1st pass using other variables for example to get groupfield value %%groupfieldrealnamelower%% or
            //  %%groupfieldrealnamelower%_filename%
            string temp = re.Replace(text, match => keywords.TryGetValue(match.Groups[1].Value, out var val) ? val : "%" + match.Groups[1].Value + "%");
            //2nd time we replace any leftover %% with empty string for %%groupfieldlowerexceptname%% in case of name wil become above %%. This will also replace any %% in orignal
            //values that are not identifiers but that should not happen much
            return re2.Replace(temp, match => match.Groups[1].Value == string.Empty ? String.Empty : keywords.TryGetValue(match.Groups[1].Value, out var val) ? val : "%" + match.Groups[1].Value + "%");
        }

        public void DoExportToHtml()
        {
            ImageProcessRunner ConvertImagesRunner = new ImageProcessRunner();
            var progressOptions = new GlobalProgressOptions(String.Empty, true)
            {
                IsIndeterminate = false
            };
            _ = PlayniteApi.Dialogs.ActivateGlobalProgress((progressAction) =>
              {
                  try
                  {
                      Stopwatch sw = new Stopwatch();
                      sw.Start();
                      UpdateTemplateFolders();
                      Settings.Settings.ConvertImageOptions.CheckForImageMagick();
                      //trick to get real storagepath i looked at sources what GetFullFilePath did and it combines the root
                      //folder with the supplied string
                      string StoragePath = PlayniteApi.Database.GetFullFilePath(String.Empty);
                      IItemCollection<Game> gameslist = PlayniteApi.Database.Games;
                      Dictionary<string, string> GameMediaHashtable = new Dictionary<string, string>();
                      Dictionary<string, bool> GameMediaCopyDoneDict = new Dictionary<string, bool>();
                      Dictionary<string, string> FirstGroupFieldFileNames = new Dictionary<string, string>();
                      FileDeDuplicator DeDuplicator = new FileDeDuplicator();
                      int pagecount = Settings.Settings.Pages.Count;
                      int PageNr = 0;
                      int Errors = 0;
                      int FaultyTemplate = 0;
                      int Succes = 0;
                      /*HtmlCompressor Compressor = new HtmlCompressor();
                      Compressor.setCompressCss(true);
                      Compressor.setRemoveComments(true);
                      Compressor.setCompressJavaScript(true);*/
                      string AllMenuEntries = String.Empty;


                      OriginalImageDataFile PreviousImageDataFile = new OriginalImageDataFile();
                      if (PreviousImageDataFile.LoadFromFile(Path.Combine(GetPluginUserDataPath(), "PreviousImagesUsed.json")))
                      {
                          if (!PreviousImageDataFile.SameSettingsUsed(Settings.Settings))
                              PreviousImageDataFile.Clear();
                      }


                      progressAction.ProgressMaxValue = pagecount;
                      string outputfolder = Settings.Settings.OutputFolder;
                      if (!Directory.Exists(outputfolder))
                      {
                          Directory.CreateDirectory(outputfolder);
                      }

                      System.IO.DirectoryInfo di = new DirectoryInfo(outputfolder);
                      progressAction.Text = Constants.ErasingPreviousHTML;
                      foreach (FileInfo file in di.GetFiles("*.html", SearchOption.TopDirectoryOnly))
                      {
                          file.Delete();
                      }
                      progressAction.Text = Constants.PreparingGenerateHTML;

                      Dictionary<string, bool> PagesGenerated = new Dictionary<string, bool>();
                      foreach (PageObject page in Settings.Settings.Pages)
                      {
                          try
                          {
                              PageNr++;
                              progressAction.CurrentProgressValue = PageNr;
                              if (progressAction.CancelToken.IsCancellationRequested)
                              {
                                  break;
                              }

                              if (PagesGenerated.ContainsKey(page.Pagefilename))
                              {
                                  continue;
                              }

                              DirectoryInfo TemplateFolderDir = TemplateFolders.Where(o => o.Name.ToLower().Equals(page.Templatefoldername.ToLower())).FirstOrDefault();
                              if (TemplateFolderDir == null)
                              {
                                  FaultyTemplate++;
                                  continue;
                              }
                              string FullTemplateFolder = TemplateFolderDir.FullName;
                              string PageOutputFilename = Path.Combine(outputfolder, page.Pagefilename);
                              string PageFilename = Path.Combine(FullTemplateFolder, "page.txt");
                              string GameCardFilename = Path.Combine(FullTemplateFolder, "gamecard.txt");
                              string GameCardsHeaderFilename = Path.Combine(FullTemplateFolder, "gamecardsheader.txt");
                              string QuicklinkFileName = Path.Combine(FullTemplateFolder, "quicklink.txt");
                              string GameCardsHeaderGroupedFilename = Path.Combine(FullTemplateFolder, "gamecardsheadergrouped.txt");
                              string MenuEntryFilename = Path.Combine(FullTemplateFolder, "menuentry.txt");
                              string MenuEntryHeaderFilename = Path.Combine(FullTemplateFolder, "menuentryheader.txt");
                              string QuickLinksHeaderFilename = Path.Combine(FullTemplateFolder, "quicklinksheader.txt");
                              string GameCardDetailsFileName = Path.Combine(FullTemplateFolder, "gamedetails.txt");
                              string GameCardDetailsCategoryFileName = Path.Combine(FullTemplateFolder, "gamedetailscategory.txt");
                              string GameCardDetailsDeveloperFileName = Path.Combine(FullTemplateFolder, "gamedetailsdeveloper.txt");
                              string GameCardDetailsFeatureFileName = Path.Combine(FullTemplateFolder, "gamedetailsfeature.txt");
                              string GameCardDetailsGenreFileName = Path.Combine(FullTemplateFolder, "gamedetailsgenre.txt");
                              string GameCardDetailsPublisherFileName = Path.Combine(FullTemplateFolder, "gamedetailspublisher.txt");
                              string GameCardDetailsPlatformFileName = Path.Combine(FullTemplateFolder, "gamedetailsplatform.txt");
                              string GameCardDetailsRegionFileName = Path.Combine(FullTemplateFolder, "gamedetailsregion.txt");
                              string GameCardDetailsAgeRatingFileName = Path.Combine(FullTemplateFolder, "gamedetailsagerating.txt");
                              string GameCardDetailsWeblinkFileName = Path.Combine(FullTemplateFolder, "gamedetailsweblink.txt");
                              string GameCardDetailsTagFilename = Path.Combine(FullTemplateFolder, "gamedetailstag.txt");

                              if ((!File.Exists(PageFilename)) || (!File.Exists(GameCardFilename)) || (!File.Exists(GameCardsHeaderFilename)) ||
                                  (!File.Exists(GameCardsHeaderGroupedFilename)) || (!File.Exists(QuickLinksHeaderFilename)) ||
                                  (!File.Exists(MenuEntryFilename)) || (!File.Exists(MenuEntryHeaderFilename)) ||
                                  (!File.Exists(QuicklinkFileName)) || (!File.Exists(GameCardDetailsFileName)) ||
                                  (!File.Exists(GameCardDetailsCategoryFileName)) || (!File.Exists(GameCardDetailsGenreFileName)) ||
                                  (!File.Exists(GameCardDetailsFeatureFileName)) || (!File.Exists(GameCardDetailsDeveloperFileName)) ||
                                  (!File.Exists(GameCardDetailsPlatformFileName)) || (!File.Exists(GameCardDetailsWeblinkFileName)) ||
                                  (!File.Exists(GameCardDetailsPublisherFileName)) || (!File.Exists(GameCardDetailsRegionFileName)) ||
                                  (!File.Exists(GameCardDetailsAgeRatingFileName)) || (!File.Exists(GameCardDetailsTagFilename)))

                              {
                                  FaultyTemplate++;
                                  continue;
                              }

                              string pageindex = File.ReadAllText(PageFilename);
                              string GameCard = File.ReadAllText(GameCardFilename);
                              string GameCardsHeader = File.ReadAllText(GameCardsHeaderFilename);
                              string GameCardsHeaderGrouped = File.ReadAllText(GameCardsHeaderGroupedFilename);
                              string QuickLinksHeader = File.ReadAllText(QuickLinksHeaderFilename);
                              string MenuEntry = File.ReadAllText(MenuEntryFilename);
                              string MenuEntryHeader = File.ReadAllText(MenuEntryHeaderFilename);
                              string QuickLink = File.ReadAllText(QuicklinkFileName);
                              string GameCardDetails = File.ReadAllText(GameCardDetailsFileName);

                              string GameCardDetailsCategory = File.ReadAllText(GameCardDetailsCategoryFileName);
                              string GameCardDetailsGenre = File.ReadAllText(GameCardDetailsGenreFileName);
                              string GameCardDetailsFeature = File.ReadAllText(GameCardDetailsFeatureFileName);
                              string GameCardDetailsDeveloper = File.ReadAllText(GameCardDetailsDeveloperFileName);
                              string GameCardDetailsPublisher = File.ReadAllText(GameCardDetailsPublisherFileName);
                              string GameCardDetailsWeblink = File.ReadAllText(GameCardDetailsWeblinkFileName);
                              string GameCardDetailsPlatform = File.ReadAllText(GameCardDetailsPlatformFileName);
                              string GameCardDetailsAgeRating = File.ReadAllText(GameCardDetailsAgeRatingFileName);
                              string GameCardDetailsRegion = File.ReadAllText(GameCardDetailsRegionFileName);
                              string GameCardDetailsTag = File.ReadAllText(GameCardDetailsTagFilename);

                              string PageGroupFieldName = Constants.GetNameFromField(page.Groupfield);
                              string PageSortFieldName = Constants.GetNameFromField(page.Sortfield);
                              string PageGroupFieldRealNameLower = page.Groupfield.ToLower();
                              string PageSortFieldRealNameLower = Constants.FakeGameFields.Contains(page.Sortfield) ? Constants.NameField.ToLower() : page.Sortfield.ToLower();
                              string sortField = Constants.FakeGameFields.Contains(page.Sortfield) ? Constants.NameField : page.Sortfield;

                              Dictionary<string, string> CurrentPageValuesDict = new Dictionary<string, string>
                              {
                                  ["groupfield"] = PageGroupFieldName,
                                  ["groupfieldexceptname"] = page.Groupfield == Constants.NameField ? "" : PageGroupFieldName,
                                  ["groupfieldrealnamelower"] = HttpUtility.HtmlEncode(PageGroupFieldRealNameLower),
                                  ["groupfieldexceptname"] = page.Groupfield == Constants.NameField ? "" : PageGroupFieldName,
                                  ["groupfieldrealnamelowerexceptname"] = HttpUtility.HtmlEncode(page.Groupfield == Constants.NameField ? "" : PageGroupFieldRealNameLower),
                                  ["sortfield"] = HttpUtility.HtmlEncode(PageSortFieldName),
                                  ["sortfieldrealnamelower"] = HttpUtility.HtmlEncode(PageSortFieldRealNameLower),
                                  ["pagefilename"] = Uri.EscapeDataString(page.Pagefilename),
                                  ["pagetitle"] = HttpUtility.HtmlEncode(page.Pagetitle),
                                  ["groupfieldsort"] = HttpUtility.HtmlEncode(page.GroupAscending ? Constants.AscendingText : Constants.DescendingText),
                                  ["groupfieldsortexceptname"] = page.Groupfield == Constants.NameField ? "" : HttpUtility.HtmlEncode(page.GroupAscending ? Constants.AscendingText : Constants.DescendingText),
                                  ["sortfieldsort"] = HttpUtility.HtmlEncode(page.SortAscending ? Constants.AscendingText : Constants.DescendingText),
                                  ["sortfieldsortsymbol"] = HttpUtility.HtmlEncode(page.SortAscending ? "▲" : "▼"),
                                  ["groupfieldsortsymbol"] = HttpUtility.HtmlEncode(page.GroupAscending ? "▲" : "▼"),
                                  ["groupfieldsortsymbolexceptname"] = page.Groupfield == Constants.NameField ? "" : HttpUtility.HtmlEncode(page.GroupAscending ? "▲" : "▼"),
                                  ["mainmenu_name"] = HttpUtility.HtmlEncode(Constants.HTMLMainMenu),
                                  ["quicklinks_name"] = HttpUtility.HtmlEncode(Constants.HTMLQuickLinks),
                                  ["links_name"] = HttpUtility.HtmlEncode(Constants.HTMLLinks),
                                  ["description_name"] = HttpUtility.HtmlEncode(Constants.HTMLDescription),
                                  ["details_name"] = HttpUtility.HtmlEncode(Constants.HTMLDetails)
                              };

                              string MenuEntryHeaderoutput = ReplaceDictionary(MenuEntryHeader, CurrentPageValuesDict);
                              string QuickLinkHeaderOutput = ReplaceDictionary(QuickLinksHeader, CurrentPageValuesDict);

                              if (String.IsNullOrEmpty(AllMenuEntries))
                              {
                                  Dictionary<string, bool> filesdone = new Dictionary<string, bool>();
                                  foreach (PageObject pagemenu in Settings.Settings.Pages)
                                  {
                                      if (!filesdone.ContainsKey(pagemenu.Pagefilename))
                                      {
                                          Dictionary<string, string> MenuEntryDict = new Dictionary<string, string>
                                          {
                                              ["groupfieldrealnamelower"] = HttpUtility.HtmlEncode(pagemenu.Groupfield.ToLower()),
                                              ["sortfieldrealnamelower"] = HttpUtility.HtmlEncode(Constants.FakeGameFields.Contains(pagemenu.Sortfield) ? Constants.NameField.ToLower() : pagemenu.Sortfield.ToLower()),
                                              ["groupfieldexceptname"] = pagemenu.Groupfield == Constants.NameField ? "" : Constants.GetNameFromField(pagemenu.Groupfield),
                                              ["groupfieldrealnamelowerexceptname"] = HttpUtility.HtmlEncode(pagemenu.Groupfield == Constants.NameField ? "" : pagemenu.Groupfield.ToLower()),
                                              ["groupfield"] = HttpUtility.HtmlEncode(Constants.GetNameFromField(pagemenu.Groupfield)),
                                              ["groupfieldexceptname"] = pagemenu.Groupfield == Constants.NameField ? "" : HttpUtility.HtmlEncode(Constants.GetNameFromField(pagemenu.Groupfield)),
                                              ["sortfield"] = HttpUtility.HtmlEncode(Constants.GetNameFromField(pagemenu.Sortfield)),
                                              ["pagefilename"] = Uri.EscapeDataString(pagemenu.Pagefilename),
                                              ["pagetitle"] = HttpUtility.HtmlEncode(pagemenu.Pagetitle),
                                              ["groupfieldsort"] = HttpUtility.HtmlEncode(pagemenu.GroupAscending ? Constants.AscendingText : Constants.DescendingText),
                                              ["groupfieldsortexceptname"] = pagemenu.Groupfield == Constants.NameField ? "" : HttpUtility.HtmlEncode(pagemenu.GroupAscending ? Constants.AscendingText : Constants.DescendingText),
                                              ["sortfieldsort"] = HttpUtility.HtmlEncode(pagemenu.SortAscending ? Constants.AscendingText : Constants.DescendingText),
                                              ["groupfieldsortsymbol"] = HttpUtility.HtmlEncode(pagemenu.GroupAscending ? "▲" : "▼"),
                                              ["groupfieldsortsymbolexceptname"] = pagemenu.Groupfield == Constants.NameField ? "" : HttpUtility.HtmlEncode(pagemenu.GroupAscending ? "▲" : "▼"),
                                              ["sortfieldsortsymbol"] = HttpUtility.HtmlEncode(pagemenu.SortAscending ? "▲" : "▼"),
                                              ["mainmenu_name"] = HttpUtility.HtmlEncode(Constants.HTMLMainMenu),
                                              ["quicklinks_name"] = HttpUtility.HtmlEncode(Constants.HTMLQuickLinks),
                                              ["links_name"] = HttpUtility.HtmlEncode(Constants.HTMLLinks),
                                              ["description_name"] = HttpUtility.HtmlEncode(Constants.HTMLDescription),
                                              ["details_name"] = HttpUtility.HtmlEncode(Constants.HTMLDetails)
                                          };
                                          string menuentryoutput = ReplaceDictionary(MenuEntry, MenuEntryDict);
                                          AllMenuEntries += menuentryoutput;

                                          if (!FirstGroupFieldFileNames.ContainsKey(pagemenu.Groupfield))
                                          {
                                              FirstGroupFieldFileNames[pagemenu.Groupfield] = Uri.EscapeDataString(pagemenu.Pagefilename);
                                          }
                                          filesdone[pagemenu.Pagefilename] = true;
                                      }
                                  }
                              }

                              List<FakeGame> fakegames;
                              if (page.Groupfield == Constants.TagField)
                              {
                                  List<FakeGame> list = new List<FakeGame>();
                                  foreach (Game game in gameslist)
                                  {
                                      List<Platform> platforms = new List<Platform>();

                                      bool include = false;
                                      if ((game.Platforms == null) || (game.Platforms.Count == 0))
                                      {
                                          include = !Settings.Settings.ExcludePlatforms.Contains(Constants.UndefinedString);
                                      }
                                      else
                                      {
                                          foreach (Platform tmpplatform in game.Platforms)
                                          {
                                              include = include || !Settings.Settings.ExcludePlatforms.Contains(tmpplatform.Name);
                                              if (include)
                                              {
                                                  break;
                                              }
                                          }
                                      }

                                      if (include &&
                                         (!Settings.Settings.ExcludeSources.Contains(game.Source == null ? Constants.UndefinedString : game.Source.Name)) &&
                                         (!(Settings.Settings.ExcludeHiddenGames && game.Hidden)))
                                      {
                                          if ((game.Tags == null) || (game.Tags.Count == 0))
                                          {
                                              FakeGame newgame = new FakeGame(game)
                                              {
                                                  Tag = String.Empty,
                                                  Platform = GetGamePlatform(game.Platforms),
                                                  Library = GetGameLibrary(game.PluginId)
                                              };
                                              list.Add(newgame);
                                          }
                                          else
                                          {
                                              foreach (Tag tag in game.Tags)
                                              {
                                                  FakeGame newgame = new FakeGame(game)
                                                  {
                                                      Tag = tag.Name,
                                                      Platform = GetGamePlatform(game.Platforms),
                                                      Library = GetGameLibrary(game.PluginId)
                                                  };
                                                  list.Add(newgame);
                                              }
                                          }
                                      }
                                  }


                                  if (page.SortAscending)
                                  {
                                      if (page.GroupAscending)
                                      {
                                          fakegames = list.AsQueryable().OrderBy(o => o.Tag).ThenBy(
                                              o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                      }
                                      else
                                      {
                                          fakegames = list.AsQueryable().OrderByDescending(o => o.Tag).ThenBy(
                                              o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                      }

                                  }
                                  else
                                  {
                                      if (page.GroupAscending)
                                      {
                                          fakegames = list.AsQueryable().OrderBy(o => o.Tag).ThenByDescending(
                                              o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                      }
                                      else
                                      {
                                          fakegames = list.AsQueryable().OrderByDescending(o => o.Tag).ThenByDescending(
                                              o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                      }
                                  }
                              }
                              else
                              {
                                  if (page.Groupfield == Constants.PlatformField)
                                  {
                                      List<FakeGame> list = new List<FakeGame>();
                                      foreach (Game game in gameslist)
                                      {
                                          List<Platform> platforms = new List<Platform>();

                                          bool include = false;
                                          if ((game.Platforms == null) || (game.Platforms.Count == 0))
                                          {
                                              include = !Settings.Settings.ExcludePlatforms.Contains(Constants.UndefinedString);
                                          }
                                          else
                                          {
                                              foreach (Platform tmpplatform in game.Platforms)
                                              {
                                                  include = include || !Settings.Settings.ExcludePlatforms.Contains(tmpplatform.Name);
                                                  if (include)
                                                  {
                                                      break;
                                                  }
                                              }
                                          }

                                          if (include &&
                                             (!Settings.Settings.ExcludeSources.Contains(game.Source == null ? Constants.UndefinedString : game.Source.Name)) &&
                                             (!(Settings.Settings.ExcludeHiddenGames && game.Hidden)))
                                          {
                                              if ((game.Platforms == null) || (game.Platforms.Count == 0))
                                              {
                                                  FakeGame newgame = new FakeGame(game)
                                                  {
                                                      Platform = String.Empty,
                                                      Library = GetGameLibrary(game.PluginId)
                                                  };
                                                  list.Add(newgame);
                                              }
                                              else
                                              {
                                                  foreach (Platform platform in game.Platforms)
                                                  {
                                                      FakeGame newgame = new FakeGame(game)
                                                      {
                                                          Platform = platform.Name,
                                                          Library = GetGameLibrary(game.PluginId)
                                                      };
                                                      list.Add(newgame);
                                                  }
                                              }
                                          }
                                      }


                                      if (page.SortAscending)
                                      {
                                          if (page.GroupAscending)
                                          {
                                              fakegames = list.AsQueryable().OrderBy(o => o.Platform).ThenBy(
                                                  o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                          }
                                          else
                                          {
                                              fakegames = list.AsQueryable().OrderByDescending(o => o.Platform).ThenBy(
                                                  o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                          }

                                      }
                                      else
                                      {
                                          if (page.GroupAscending)
                                          {
                                              fakegames = list.AsQueryable().OrderBy(o => o.Platform).ThenByDescending(
                                                  o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                          }
                                          else
                                          {
                                              fakegames = list.AsQueryable().OrderByDescending(o => o.Platform).ThenByDescending(
                                                  o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                          }
                                      }
                                  }
                                  else
                                  {
                                      if (page.Groupfield == Constants.AgeRatingField)
                                      {
                                          List<FakeGame> list = new List<FakeGame>();
                                          foreach (Game game in gameslist)
                                          {
                                              bool include = false;
                                              if ((game.Platforms == null) || (game.Platforms.Count == 0))
                                              {
                                                  include = !Settings.Settings.ExcludePlatforms.Contains(Constants.UndefinedString);
                                              }
                                              else
                                              {
                                                  foreach (Platform tmpplatform in game.Platforms)
                                                  {
                                                      include = include || !Settings.Settings.ExcludePlatforms.Contains(tmpplatform.Name);
                                                      if (include)
                                                      {
                                                          break;
                                                      }
                                                  }
                                              }

                                              if (include &&
                                                 (!Settings.Settings.ExcludeSources.Contains(game.Source == null ? Constants.UndefinedString : game.Source.Name)) &&
                                                 (!(Settings.Settings.ExcludeHiddenGames && game.Hidden)))
                                              {
                                                  if ((game.AgeRatings == null) || (game.AgeRatings.Count == 0))
                                                  {
                                                      FakeGame newgame = new FakeGame(game)
                                                      {
                                                          AgeRating = String.Empty,
                                                          Platform = GetGamePlatform(game.Platforms),
                                                          Library = GetGameLibrary(game.PluginId)
                                                      };
                                                      list.Add(newgame);
                                                  }
                                                  else
                                                  {
                                                      foreach (AgeRating agerating in game.AgeRatings)
                                                      {
                                                          FakeGame newgame = new FakeGame(game)
                                                          {
                                                              AgeRating = agerating.Name,
                                                              Platform = GetGamePlatform(game.Platforms),
                                                              Library = GetGameLibrary(game.PluginId)
                                                          };
                                                          list.Add(newgame);
                                                      }
                                                  }

                                              }
                                          }


                                          if (page.SortAscending)
                                          {
                                              if (page.GroupAscending)
                                              {
                                                  fakegames = list.AsQueryable().OrderBy(o => o.AgeRating).ThenBy(
                                                      o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                              }
                                              else
                                              {
                                                  fakegames = list.AsQueryable().OrderByDescending(o => o.AgeRating).ThenBy(
                                                      o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                              }

                                          }
                                          else
                                          {
                                              if (page.GroupAscending)
                                              {
                                                  fakegames = list.AsQueryable().OrderBy(o => o.AgeRating).ThenByDescending(
                                                      o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                              }
                                              else
                                              {
                                                  fakegames = list.AsQueryable().OrderByDescending(o => o.AgeRating).ThenByDescending(
                                                      o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                              }
                                          }
                                      }
                                      else
                                      {
                                          if (page.Groupfield == Constants.RegionField)
                                          {
                                              List<FakeGame> list = new List<FakeGame>();
                                              foreach (Game game in gameslist)
                                              {
                                                  bool include = false;
                                                  if ((game.Platforms == null) || (game.Platforms.Count == 0))
                                                  {
                                                      include = !Settings.Settings.ExcludePlatforms.Contains(Constants.UndefinedString);
                                                  }
                                                  else
                                                  {
                                                      foreach (Platform tmpplatform in game.Platforms)
                                                      {
                                                          include = include || !Settings.Settings.ExcludePlatforms.Contains(tmpplatform.Name);
                                                          if (include)
                                                          {
                                                              break;
                                                          }
                                                      }
                                                  }

                                                  if (include &&
                                                     (!Settings.Settings.ExcludeSources.Contains(game.Source == null ? Constants.UndefinedString : game.Source.Name)) &&
                                                     (!(Settings.Settings.ExcludeHiddenGames && game.Hidden)))
                                                  {
                                                      if ((game.Regions == null) || (game.Regions.Count == 0))
                                                      {
                                                          FakeGame newgame = new FakeGame(game)
                                                          {
                                                              Region = String.Empty,
                                                              Platform = GetGamePlatform(game.Platforms),
                                                              Library = GetGameLibrary(game.PluginId)
                                                          };
                                                          list.Add(newgame);
                                                      }
                                                      else
                                                      {
                                                          foreach (Region region in game.Regions)
                                                          {
                                                              FakeGame newgame = new FakeGame(game)
                                                              {
                                                                  Region = region.Name,
                                                                  Platform = GetGamePlatform(game.Platforms),
                                                                  Library = GetGameLibrary(game.PluginId)
                                                              };
                                                              list.Add(newgame);
                                                          }
                                                      }
                                                  }
                                              }

                                              if (page.SortAscending)
                                              {
                                                  if (page.GroupAscending)
                                                  {
                                                      fakegames = list.AsQueryable().OrderBy(o => o.Region).ThenBy(
                                                          o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                  }
                                                  else
                                                  {
                                                      fakegames = list.AsQueryable().OrderByDescending(o => o.Region).ThenBy(
                                                          o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                  }

                                              }
                                              else
                                              {
                                                  if (page.GroupAscending)
                                                  {
                                                      fakegames = list.AsQueryable().OrderBy(o => o.Region).ThenByDescending(
                                                          o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                  }
                                                  else
                                                  {
                                                      fakegames = list.AsQueryable().OrderByDescending(o => o.Region).ThenByDescending(
                                                          o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                  }
                                              }
                                          }
                                          else
                                          {
                                              if (page.Groupfield == Constants.SerieField)
                                              {
                                                  List<FakeGame> list = new List<FakeGame>();
                                                  foreach (Game game in gameslist)
                                                  {
                                                      bool include = false;
                                                      if ((game.Platforms == null) || (game.Platforms.Count == 0))
                                                      {
                                                          include = !Settings.Settings.ExcludePlatforms.Contains(Constants.UndefinedString);
                                                      }
                                                      else
                                                      {
                                                          foreach (Platform tmpplatform in game.Platforms)
                                                          {
                                                              include = include || !Settings.Settings.ExcludePlatforms.Contains(tmpplatform.Name);
                                                              if (include)
                                                              {
                                                                  break;
                                                              }
                                                          }
                                                      }

                                                      if (include &&
                                                         (!Settings.Settings.ExcludeSources.Contains(game.Source == null ? Constants.UndefinedString : game.Source.Name)) &&
                                                         (!(Settings.Settings.ExcludeHiddenGames && game.Hidden)))
                                                      {
                                                          if (game.Series == null)
                                                          {
                                                              FakeGame newgame = new FakeGame(game)
                                                              {
                                                                  Serie = String.Empty,
                                                                  Platform = GetGamePlatform(game.Platforms),
                                                                  Library = GetGameLibrary(game.PluginId)
                                                              };
                                                              list.Add(newgame);
                                                          }
                                                          else
                                                          {
                                                              foreach (Series serie in game.Series)
                                                              {
                                                                  FakeGame newgame = new FakeGame(game)
                                                                  {
                                                                      Serie = serie.Name,
                                                                      Platform = GetGamePlatform(game.Platforms),
                                                                      Library = GetGameLibrary(game.PluginId)
                                                                  };
                                                                  list.Add(newgame);
                                                              }
                                                          }


                                                      }
                                                  }

                                                  if (page.SortAscending)
                                                  {
                                                      if (page.GroupAscending)
                                                      {
                                                          fakegames = list.AsQueryable().OrderBy(o => o.Serie).ThenBy(
                                                              o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                      }
                                                      else
                                                      {
                                                          fakegames = list.AsQueryable().OrderByDescending(o => o.Serie).ThenBy(
                                                              o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                      }

                                                  }
                                                  else
                                                  {
                                                      if (page.GroupAscending)
                                                      {
                                                          fakegames = list.AsQueryable().OrderBy(o => o.Serie).ThenByDescending(
                                                              o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                      }
                                                      else
                                                      {
                                                          fakegames = list.AsQueryable().OrderByDescending(o => o.Serie).ThenByDescending(
                                                              o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                      }
                                                  }
                                              }
                                              else
                                              {
                                                  if (page.Groupfield == Constants.LibraryField)
                                                  {
                                                      List<FakeGame> list = new List<FakeGame>();
                                                      foreach (Game game in gameslist)
                                                      {
                                                          bool include = false;
                                                          if ((game.Platforms == null) || (game.Platforms.Count == 0))
                                                          {
                                                              include = !Settings.Settings.ExcludePlatforms.Contains(Constants.UndefinedString);
                                                          }
                                                          else
                                                          {
                                                              foreach (Platform tmpplatform in game.Platforms)
                                                              {
                                                                  include = include || !Settings.Settings.ExcludePlatforms.Contains(tmpplatform.Name);
                                                                  if (include)
                                                                  {
                                                                      break;
                                                                  }
                                                              }
                                                          }

                                                          if (include &&
                                                             (!Settings.Settings.ExcludeSources.Contains(game.Source == null ? Constants.UndefinedString : game.Source.Name)) &&
                                                             (!(Settings.Settings.ExcludeHiddenGames && game.Hidden)))
                                                          {
                                                              FakeGame newgame = new FakeGame(game)
                                                              {
                                                                  Platform = GetGamePlatform(game.Platforms),
                                                                  Library = GetGameLibrary(game.PluginId)
                                                              };
                                                              list.Add(newgame);
                                                          }
                                                      }


                                                      if (page.SortAscending)
                                                      {
                                                          if (page.GroupAscending)
                                                          {
                                                              fakegames = list.AsQueryable().OrderBy(o => o.Library).ThenBy(
                                                                  o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                          }
                                                          else
                                                          {
                                                              fakegames = list.AsQueryable().OrderByDescending(o => o.Library).ThenBy(
                                                                      o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                          }
                                                      }
                                                      else
                                                      {
                                                          if (page.GroupAscending)
                                                          {
                                                              fakegames = list.AsQueryable().OrderBy(o => o.Library).ThenByDescending(
                                                                  o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                          }
                                                          else
                                                          {
                                                              fakegames = list.AsQueryable().OrderByDescending(o => o.Library).ThenByDescending(
                                                                  o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                          }
                                                      }
                                                  }
                                                  else
                                                  if (page.Groupfield == Constants.PublisherField)
                                                  {
                                                      List<FakeGame> list = new List<FakeGame>();
                                                      foreach (Game game in gameslist)
                                                      {
                                                          bool include = false;
                                                          if ((game.Platforms == null) || (game.Platforms.Count == 0))
                                                          {
                                                              include = !Settings.Settings.ExcludePlatforms.Contains(Constants.UndefinedString);
                                                          }
                                                          else
                                                          {
                                                              foreach (Platform tmpplatform in game.Platforms)
                                                              {
                                                                  include = include || !Settings.Settings.ExcludePlatforms.Contains(tmpplatform.Name);
                                                                  if (include)
                                                                  {
                                                                      break;
                                                                  }
                                                              }
                                                          }

                                                          if (include &&
                                                             (!Settings.Settings.ExcludeSources.Contains(game.Source == null ? Constants.UndefinedString : game.Source.Name)) &&
                                                             (!(Settings.Settings.ExcludeHiddenGames && game.Hidden)))
                                                          {
                                                              if ((game.Publishers == null) || (game.Publishers.Count == 0))
                                                              {
                                                                  FakeGame newgame = new FakeGame(game)
                                                                  {
                                                                      Publisher = String.Empty,
                                                                      Platform = GetGamePlatform(game.Platforms),
                                                                      Library = GetGameLibrary(game.PluginId)
                                                                  };
                                                                  list.Add(newgame);
                                                              }
                                                              else
                                                              {
                                                                  foreach (Company publisher in game.Publishers)
                                                                  {
                                                                      FakeGame newgame = new FakeGame(game)
                                                                      {
                                                                          Publisher = publisher.Name,
                                                                          Platform = GetGamePlatform(game.Platforms),
                                                                          Library = GetGameLibrary(game.PluginId)
                                                                      };
                                                                      list.Add(newgame);
                                                                  }
                                                              }
                                                          }
                                                      }


                                                      if (page.SortAscending)
                                                      {
                                                          if (page.GroupAscending)
                                                          {
                                                              fakegames = list.AsQueryable().OrderBy(o => o.Publisher).ThenBy(
                                                                  o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                          }
                                                          else
                                                          {
                                                              fakegames = list.AsQueryable().OrderByDescending(o => o.Publisher).ThenBy(
                                                                      o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                          }
                                                      }
                                                      else
                                                      {
                                                          if (page.GroupAscending)
                                                          {
                                                              fakegames = list.AsQueryable().OrderBy(o => o.Publisher).ThenByDescending(
                                                                  o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                          }
                                                          else
                                                          {
                                                              fakegames = list.AsQueryable().OrderByDescending(o => o.Publisher).ThenByDescending(
                                                                  o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                          }
                                                      }
                                                  }
                                                  else
                                                  {
                                                      if (page.Groupfield == Constants.FeatureField)
                                                      {
                                                          List<FakeGame> list = new List<FakeGame>();

                                                          foreach (Game game in gameslist)
                                                          {
                                                              bool include = false;
                                                              if ((game.Platforms == null) || (game.Platforms.Count == 0))
                                                              {
                                                                  include = !Settings.Settings.ExcludePlatforms.Contains(Constants.UndefinedString);
                                                              }
                                                              else
                                                              {
                                                                  foreach (Platform tmpplatform in game.Platforms)
                                                                  {
                                                                      include = include || !Settings.Settings.ExcludePlatforms.Contains(tmpplatform.Name);
                                                                      if (include)
                                                                      {
                                                                          break;
                                                                      }
                                                                  }
                                                              }

                                                              if (include &&
                                                                 (!Settings.Settings.ExcludeSources.Contains(game.Source == null ? Constants.UndefinedString : game.Source.Name)) &&
                                                                 (!(Settings.Settings.ExcludeHiddenGames && game.Hidden)))
                                                              {
                                                                  if ((game.Features == null) || (game.Features.Count == 0))
                                                                  {
                                                                      FakeGame newgame = new FakeGame(game)
                                                                      {
                                                                          Feature = String.Empty,
                                                                          Platform = GetGamePlatform(game.Platforms),
                                                                          Library = GetGameLibrary(game.PluginId)
                                                                      };
                                                                      list.Add(newgame);
                                                                  }
                                                                  else
                                                                  {
                                                                      foreach (GameFeature Feature in game.Features)
                                                                      {
                                                                          FakeGame newgame = new FakeGame(game)
                                                                          {
                                                                              Feature = Feature.Name,
                                                                              Platform = GetGamePlatform(game.Platforms),
                                                                              Library = GetGameLibrary(game.PluginId)
                                                                          };
                                                                          list.Add(newgame);
                                                                      }
                                                                  }
                                                              }
                                                          }

                                                          if (page.SortAscending)
                                                          {
                                                              if (page.GroupAscending)
                                                              {
                                                                  fakegames = list.AsQueryable().OrderBy(o => o.Feature).ThenBy(
                                                                          o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                              }
                                                              else
                                                              {
                                                                  fakegames = list.AsQueryable().OrderByDescending(o => o.Feature).ThenBy(
                                                                      o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                              }

                                                          }
                                                          else
                                                          {
                                                              if (page.GroupAscending)
                                                              {
                                                                  fakegames = list.AsQueryable().OrderBy(o => o.Feature).ThenByDescending(
                                                                      o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                              }
                                                              else
                                                              {
                                                                  fakegames = list.AsQueryable().OrderByDescending(o => o.Feature).ThenByDescending(
                                                                      o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();

                                                              }
                                                          }
                                                      }
                                                      else
                                                      {
                                                          if (page.Groupfield == Constants.GenreField)
                                                          {
                                                              List<FakeGame> list = new List<FakeGame>();

                                                              foreach (Game game in gameslist)
                                                              {
                                                                  bool include = false;
                                                                  if ((game.Platforms == null) || (game.Platforms.Count == 0))
                                                                  {
                                                                      include = !Settings.Settings.ExcludePlatforms.Contains(Constants.UndefinedString);
                                                                  }
                                                                  else
                                                                  {
                                                                      foreach (Platform tmpplatform in game.Platforms)
                                                                      {
                                                                          include = include || !Settings.Settings.ExcludePlatforms.Contains(tmpplatform.Name);
                                                                          if (include)
                                                                          {
                                                                              break;
                                                                          }
                                                                      }
                                                                  }

                                                                  if (include &&
                                                                     (!Settings.Settings.ExcludeSources.Contains(game.Source == null ? Constants.UndefinedString : game.Source.Name)) &&
                                                                     (!(Settings.Settings.ExcludeHiddenGames && game.Hidden)))
                                                                  {
                                                                      if ((game.Genres == null) || (game.Genres.Count == 0))
                                                                      {
                                                                          FakeGame newgame = new FakeGame(game)
                                                                          {
                                                                              Genre = String.Empty,
                                                                              Platform = GetGamePlatform(game.Platforms),
                                                                              Library = GetGameLibrary(game.PluginId)
                                                                          };
                                                                          list.Add(newgame);
                                                                      }
                                                                      else
                                                                      {
                                                                          foreach (Genre genre in game.Genres)
                                                                          {
                                                                              FakeGame newgame = new FakeGame(game)
                                                                              {
                                                                                  Genre = genre.Name,
                                                                                  Platform = GetGamePlatform(game.Platforms),
                                                                                  Library = GetGameLibrary(game.PluginId)
                                                                              };
                                                                              list.Add(newgame);
                                                                          }
                                                                      }
                                                                  }
                                                              }


                                                              if (page.SortAscending)
                                                              {
                                                                  if (page.GroupAscending)
                                                                  {
                                                                      fakegames = list.AsQueryable().OrderBy(o => o.Genre).ThenBy(
                                                                          o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                  }
                                                                  else
                                                                  {
                                                                      fakegames = list.AsQueryable().OrderByDescending(o => o.Genre).ThenBy(
                                                                          o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                  }
                                                              }
                                                              else
                                                              {
                                                                  if (page.GroupAscending)
                                                                  {
                                                                      fakegames = list.AsQueryable().OrderBy(o => o.Genre).ThenByDescending(
                                                                          o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                  }
                                                                  else
                                                                  {
                                                                      fakegames = list.AsQueryable().OrderByDescending(o => o.Genre).ThenByDescending(
                                                                          o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                  }
                                                              }
                                                          }
                                                          else
                                                          {
                                                              if (page.Groupfield == Constants.DeveloperField)
                                                              {
                                                                  List<FakeGame> list = new List<FakeGame>();

                                                                  foreach (Game game in gameslist)
                                                                  {
                                                                      bool include = false;
                                                                      if ((game.Platforms == null) || (game.Platforms.Count == 0))
                                                                      {
                                                                          include = !Settings.Settings.ExcludePlatforms.Contains(Constants.UndefinedString);
                                                                      }
                                                                      else
                                                                      {
                                                                          foreach (Platform tmpplatform in game.Platforms)
                                                                          {
                                                                              include = include || !Settings.Settings.ExcludePlatforms.Contains(tmpplatform.Name);
                                                                              if (include)
                                                                              {
                                                                                  break;
                                                                              }
                                                                          }
                                                                      }

                                                                      if (include &&
                                                                         (!Settings.Settings.ExcludeSources.Contains(game.Source == null ? Constants.UndefinedString : game.Source.Name)) &&
                                                                         (!(Settings.Settings.ExcludeHiddenGames && game.Hidden)))
                                                                      {
                                                                          if ((game.Developers == null) || (game.Developers.Count == 0))
                                                                          {
                                                                              FakeGame newgame = new FakeGame(game)
                                                                              {
                                                                                  Developer = String.Empty,
                                                                                  Platform = GetGamePlatform(game.Platforms),
                                                                                  Library = GetGameLibrary(game.PluginId)
                                                                              };
                                                                              list.Add(newgame);
                                                                          }
                                                                          else
                                                                          {
                                                                              foreach (Company developer in game.Developers)
                                                                              {
                                                                                  FakeGame newgame = new FakeGame(game)
                                                                                  {
                                                                                      Developer = developer.Name,
                                                                                      Platform = GetGamePlatform(game.Platforms),
                                                                                      Library = GetGameLibrary(game.PluginId)
                                                                                  };
                                                                                  list.Add(newgame);
                                                                              }
                                                                          }
                                                                      }
                                                                  }

                                                                  if (page.SortAscending)
                                                                  {
                                                                      if (page.GroupAscending)
                                                                      {
                                                                          fakegames = list.AsQueryable().OrderBy(o => o.Developer).ThenBy(
                                                                              o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                      }
                                                                      else
                                                                      {
                                                                          fakegames = list.AsQueryable().OrderByDescending(o => o.Developer).ThenBy(
                                                                              o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                      }
                                                                  }
                                                                  else
                                                                  {
                                                                      if (page.GroupAscending)
                                                                      {
                                                                          fakegames = list.AsQueryable().OrderBy(o => o.Developer).ThenByDescending(
                                                                              o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                      }
                                                                      else
                                                                      {
                                                                          fakegames = list.AsQueryable().OrderByDescending(o => o.Developer).ThenByDescending(
                                                                              o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                      }
                                                                  }
                                                              }
                                                              else
                                                              {
                                                                  if (page.Groupfield == Constants.CategoryField)
                                                                  {
                                                                      List<FakeGame> list = new List<FakeGame>();

                                                                      foreach (Game game in gameslist)
                                                                      {
                                                                          bool include = false;
                                                                          if ((game.Platforms == null) || (game.Platforms.Count == 0))
                                                                          {
                                                                              include = !Settings.Settings.ExcludePlatforms.Contains(Constants.UndefinedString);
                                                                          }
                                                                          else
                                                                          {
                                                                              foreach (Platform tmpplatform in game.Platforms)
                                                                              {
                                                                                  include = include || !Settings.Settings.ExcludePlatforms.Contains(tmpplatform.Name);
                                                                                  if (include)
                                                                                  {
                                                                                      break;
                                                                                  }
                                                                              }
                                                                          }

                                                                          if (include &&
                                                                             (!Settings.Settings.ExcludeSources.Contains(game.Source == null ? Constants.UndefinedString : game.Source.Name)) &&
                                                                             (!(Settings.Settings.ExcludeHiddenGames && game.Hidden)))
                                                                          {
                                                                              {
                                                                                  if ((game.Categories == null) || (game.Categories.Count == 0))
                                                                                  {
                                                                                      FakeGame newgame = new FakeGame(game)
                                                                                      {
                                                                                          Category = String.Empty,
                                                                                          Platform = GetGamePlatform(game.Platforms),
                                                                                          Library = GetGameLibrary(game.PluginId)
                                                                                      };
                                                                                      list.Add(newgame);
                                                                                  }
                                                                                  else
                                                                                  {
                                                                                      foreach (Category category in game.Categories)
                                                                                      {
                                                                                          FakeGame newgame = new FakeGame(game)
                                                                                          {
                                                                                              Category = category.Name,
                                                                                              Platform = GetGamePlatform(game.Platforms),
                                                                                              Library = GetGameLibrary(game.PluginId)
                                                                                          };
                                                                                          list.Add(newgame);
                                                                                      }
                                                                                  }
                                                                              }
                                                                          }
                                                                      }

                                                                      if (page.SortAscending)
                                                                      {
                                                                          if (page.GroupAscending)
                                                                          {
                                                                              fakegames = list.AsQueryable().OrderBy(o => o.Category).ThenBy(
                                                                                  o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                          }
                                                                          else
                                                                          {
                                                                              fakegames = list.AsQueryable().OrderByDescending(o => o.Category).ThenBy(
                                                                                  o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                          }

                                                                      }
                                                                      else
                                                                      {
                                                                          if (page.GroupAscending)
                                                                          {
                                                                              fakegames = list.AsQueryable().OrderBy(o => o.Category).ThenByDescending(
                                                                                  o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                          }
                                                                          else
                                                                          {
                                                                              fakegames = list.AsQueryable().OrderByDescending(o => o.Category).ThenByDescending(
                                                                                  o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                          }
                                                                      }
                                                                  }
                                                                  else

                                                                  {
                                                                      List<FakeGame> list = new List<FakeGame>();

                                                                      foreach (Game game in gameslist)
                                                                      {
                                                                          bool include = false;
                                                                          if ((game.Platforms == null) || (game.Platforms.Count == 0))
                                                                          {
                                                                              include = !Settings.Settings.ExcludePlatforms.Contains(Constants.UndefinedString);
                                                                          }
                                                                          else
                                                                          {
                                                                              foreach (Platform tmpplatform in game.Platforms)
                                                                              {
                                                                                  include = include || !Settings.Settings.ExcludePlatforms.Contains(tmpplatform.Name);
                                                                                  if (include)
                                                                                  {
                                                                                      break;
                                                                                  }
                                                                              }
                                                                          }

                                                                          if (include &&
                                                                             (!Settings.Settings.ExcludeSources.Contains(game.Source == null ? Constants.UndefinedString : game.Source.Name)) &&
                                                                             (!(Settings.Settings.ExcludeHiddenGames && game.Hidden)))
                                                                          {
                                                                              FakeGame newgame = new FakeGame(game)
                                                                              {
                                                                                  Library = GetGameLibrary(game.PluginId),
                                                                                  Platform = GetGamePlatform(game.Platforms),
                                                                              };
                                                                              list.Add(newgame);
                                                                          }
                                                                      }


                                                                      if (Constants.DateFields.Contains(page.Groupfield))
                                                                      {
                                                                          if (page.SortAscending)
                                                                          {
                                                                              if (page.GroupAscending)
                                                                              {
                                                                                  fakegames = list.AsQueryable().OrderBy(o
                                                                                      => ((DateTime?)(o.OriginalGame.GetType().GetProperty(page.Groupfield).GetValue(o.OriginalGame))).HasValue ?
                                                                                          ((DateTime)(o.OriginalGame.GetType().GetProperty(page.Groupfield).GetValue(o.OriginalGame))).Date : DateTime.MinValue)
                                                                                          .ThenBy(o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                              }
                                                                              else
                                                                              {
                                                                                  fakegames = list.AsQueryable().OrderByDescending(o
                                                                                      => ((DateTime?)(o.OriginalGame.GetType().GetProperty(page.Groupfield).GetValue(o.OriginalGame))).HasValue ?
                                                                                          ((DateTime)(o.OriginalGame.GetType().GetProperty(page.Groupfield).GetValue(o.OriginalGame))).Date : DateTime.MinValue)
                                                                                      .ThenBy(o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                              }
                                                                          }
                                                                          else
                                                                          {
                                                                              if (page.GroupAscending)
                                                                              {
                                                                                  fakegames = list.AsQueryable().OrderBy(o
                                                                                  => ((DateTime?)(o.OriginalGame.GetType().GetProperty(page.Groupfield).GetValue(o.OriginalGame))).HasValue ?
                                                                                          ((DateTime)(o.OriginalGame.GetType().GetProperty(page.Groupfield).GetValue(o.OriginalGame))).Date : DateTime.MinValue)
                                                                                  .ThenByDescending(o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                              }
                                                                              else
                                                                              {
                                                                                  fakegames = list.AsQueryable().OrderByDescending(o
                                                                                  => ((DateTime?)(o.OriginalGame.GetType().GetProperty(page.Groupfield).GetValue(o.OriginalGame))).HasValue ?
                                                                                          ((DateTime)(o.OriginalGame.GetType().GetProperty(page.Groupfield).GetValue(o.OriginalGame))).Date : DateTime.MinValue)
                                                                                      .ThenByDescending(o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                              }
                                                                          }
                                                                      }

                                                                      if (page.Groupfield == Constants.NotGroupedField)
                                                                      {
                                                                          if (Constants.DateFields.Contains(page.Groupfield))
                                                                          {
                                                                              if (page.SortAscending)
                                                                              {
                                                                                  fakegames = list.AsQueryable().OrderBy(o
                                                                                      => ((DateTime?)(o.OriginalGame.GetType().GetProperty(page.Sortfield).GetValue(o.OriginalGame))).HasValue ?
                                                                                          ((DateTime)(o.OriginalGame.GetType().GetProperty(page.Sortfield).GetValue(o.OriginalGame))).Date : DateTime.MinValue).ToList();
                                                                              }
                                                                              else
                                                                              {
                                                                                  fakegames = list.AsQueryable().OrderByDescending(o =>
                                                                                    ((DateTime?)(o.OriginalGame.GetType().GetProperty(page.Sortfield).GetValue(o.OriginalGame))).HasValue ?
                                                                                     ((DateTime)(o.OriginalGame.GetType().GetProperty(page.Sortfield).GetValue(o.OriginalGame))).Date : DateTime.MinValue).ToList();
                                                                              }
                                                                          }
                                                                          else
                                                                          {
                                                                              if (page.SortAscending)
                                                                              {
                                                                                  fakegames = list.AsQueryable().OrderBy(o => o.OriginalGame.GetType().GetProperty(page.Sortfield).GetValue(o.OriginalGame)).ToList();
                                                                              }
                                                                              else
                                                                              {
                                                                                  fakegames = list.AsQueryable().OrderByDescending(o => o.OriginalGame.GetType().GetProperty(page.Sortfield).GetValue(o.OriginalGame)).ToList();
                                                                              }
                                                                          }
                                                                      }
                                                                      else
                                                                      {
                                                                          if (page.Groupfield == Constants.NameField)
                                                                          {
                                                                              if (page.SortAscending)
                                                                              {
                                                                                  if (page.GroupAscending)
                                                                                  {
                                                                                      fakegames = list.AsQueryable().OrderBy(
                                                                                          o => o.OriginalGame.Name, new NameComparer())
                                                                                      .ThenBy(
                                                                                          o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                                  }
                                                                                  else
                                                                                  {
                                                                                      fakegames = list.AsQueryable().OrderByDescending(
                                                                                          o => o.OriginalGame.Name, new NameComparer())
                                                                                      .ThenBy(
                                                                                          o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                                  }
                                                                              }
                                                                              else
                                                                              {
                                                                                  if (page.GroupAscending)
                                                                                  {
                                                                                      fakegames = list.AsQueryable().OrderBy(
                                                                                          o => o.OriginalGame.Name, new NameComparer())
                                                                                      .ThenByDescending(
                                                                                          o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                                  }
                                                                                  else
                                                                                  {
                                                                                      fakegames = list.AsQueryable().OrderByDescending(
                                                                                          o => o.OriginalGame.Name, new NameComparer())
                                                                                      .ThenByDescending(
                                                                                          o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                                  }
                                                                              }
                                                                          }
                                                                          else
                                                                          {
                                                                              if (page.SortAscending)
                                                                              {
                                                                                  if (page.GroupAscending)
                                                                                  {
                                                                                      fakegames = list.AsQueryable().OrderBy(
                                                                                          o => o.OriginalGame.GetType().GetProperty(page.Groupfield).GetValue(o.OriginalGame))
                                                                                      .ThenBy(
                                                                                          o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                                  }
                                                                                  else
                                                                                  {
                                                                                      fakegames = list.AsQueryable().OrderByDescending(
                                                                                          o => o.OriginalGame.GetType().GetProperty(page.Groupfield).GetValue(o.OriginalGame))
                                                                                      .ThenBy(
                                                                                          o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                                  }
                                                                              }
                                                                              else
                                                                              {
                                                                                  if (page.GroupAscending)
                                                                                  {
                                                                                      fakegames = list.AsQueryable().OrderBy(
                                                                                          o => o.OriginalGame.GetType().GetProperty(page.Groupfield).GetValue(o.OriginalGame))
                                                                                      .ThenByDescending(
                                                                                          o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                                  }
                                                                                  else
                                                                                  {
                                                                                      fakegames = list.AsQueryable().OrderByDescending(
                                                                                          o => o.OriginalGame.GetType().GetProperty(page.Groupfield).GetValue(o.OriginalGame))
                                                                                      .ThenByDescending(
                                                                                          o => o.OriginalGame.GetType().GetProperty(sortField).GetValue(o.OriginalGame)).ToList();
                                                                                  }
                                                                              }
                                                                          }
                                                                      }
                                                                  }
                                                              }
                                                          }
                                                      }
                                                  }
                                              }
                                          }
                                      }
                                  }
                              }
                              int groupcount = 0;
                              int count = 0;
                              string localgroupfield = String.Empty;
                              string AllGameCardsWithHeaders = String.Empty;
                              string prevlocalgroupfield = String.Empty;

                              StringBuilder GameCardsOutputBuilder = new StringBuilder();
                              StringBuilder AllGameCardsWithHeadersBuilder = new StringBuilder();
                              StringBuilder AllQuicklinksBuilder = new StringBuilder();

                              if (fakegames.Count != 0)
                              {
                                  FakeGame FirstGame = fakegames[0];
                                  if (page.Groupfield == Constants.NotGroupedField)
                                  {
                                      prevlocalgroupfield = Constants.NotGroupedFieldSanitizedName;
                                  }
                                  else
                                  {
                                      if (page.Groupfield == Constants.NameField)
                                      {
                                          prevlocalgroupfield = FirstGame.OriginalGame.Name.Substring(0, Math.Min(1, FirstGame.OriginalGame.Name.Length)).ToUpper();
                                          if ((prevlocalgroupfield.Length > 0) && (!Char.IsLetter(prevlocalgroupfield[0])))
                                          {
                                              prevlocalgroupfield = Constants.NumberSign;
                                          }
                                      }
                                      else
                                      {
                                          if (Constants.DateFields.Contains(page.Groupfield))
                                          {
                                              var tmp = FirstGame.OriginalGame.GetType().GetProperty(page.Groupfield).GetValue(FirstGame.OriginalGame);
                                              if (tmp != null)
                                              {
                                                  prevlocalgroupfield = ((DateTime)tmp).ToShortDateString();
                                              }
                                          }
                                          else
                                          {
                                              if (page.Groupfield == Constants.PlaytimeField)
                                              {

                                                  var tmp = FirstGame.OriginalGame.GetType().GetProperty(page.Groupfield).GetValue(FirstGame.OriginalGame);
                                                  if (tmp != null)
                                                  {
                                                      prevlocalgroupfield = ConvertFromSeconds((ulong)tmp);
                                                  }
                                              }
                                              else
                                              {
                                                  if (Constants.FakeGameFields.Contains(page.Groupfield))
                                                  {
                                                      var tmp = FirstGame.GetType().GetProperty(page.Groupfield).GetValue(FirstGame);
                                                      if (tmp != null)
                                                      {
                                                          prevlocalgroupfield = tmp.ToString();
                                                      }
                                                  }
                                                  else
                                                  {
                                                      var tmp = FirstGame.OriginalGame.GetType().GetProperty(page.Groupfield).GetValue(FirstGame.OriginalGame);
                                                      if (tmp != null)
                                                      {
                                                          prevlocalgroupfield = tmp.ToString();
                                                      }
                                                  }
                                              }
                                          }
                                      }
                                  }
                              }

                              prevlocalgroupfield = Constants.SantizeValue(prevlocalgroupfield, page.Groupfield);
                              int gamescount = fakegames.Count;
                              foreach (FakeGame fakegame in fakegames)
                              {
                                  Dictionary<string, string> CurrentGameValuesDict = CurrentPageValuesDict;
                                  Game realgame = fakegame.OriginalGame;
                                  count += 1;
                                  if ((count % 50 == 0) || (count == 1))
                                  {
                                      progressAction.Text = Constants.GeneratatingHTML + " " + PageNr.ToString() + " " + Constants.GeneratatingHTMLOf + " " +
                                        pagecount.ToString() + " " + Constants.GeneratatingHTMLEntry + " " + count.ToString() + " " + Constants.GeneratatingHTMLOf + " " +
                                        gamescount.ToString();
                                  }
                                  if (progressAction.CancelToken.IsCancellationRequested)
                                  {
                                      break;
                                  }

                                  if (page.Groupfield == Constants.NotGroupedField)
                                  {
                                      localgroupfield = Constants.NotGroupedFieldSanitizedName;
                                  }
                                  else
                                  {
                                      if (page.Groupfield == Constants.NameField)
                                      {
                                          localgroupfield = fakegame.OriginalGame.Name.Substring(0, Math.Min(1, fakegame.OriginalGame.Name.Length)).ToUpper();
                                          if ((localgroupfield.Length > 0) && (!Char.IsLetter(localgroupfield[0])))
                                          {
                                              localgroupfield = Constants.NumberSign;
                                          }
                                      }
                                      else
                                      {
                                          if (Constants.DateFields.Contains(page.Groupfield))
                                          {
                                              localgroupfield = String.Empty;
                                              var tmp = fakegame.OriginalGame.GetType().GetProperty(page.Groupfield).GetValue(fakegame.OriginalGame);
                                              if (tmp != null)
                                              {
                                                  localgroupfield = ((DateTime)tmp).ToShortDateString();
                                              }
                                          }
                                          else
                                          {
                                              if (page.Groupfield == Constants.PlaytimeField)
                                              {
                                                  localgroupfield = String.Empty;
                                                  var tmp = fakegame.OriginalGame.GetType().GetProperty(page.Groupfield).GetValue(fakegame.OriginalGame);
                                                  if (tmp != null)
                                                  {
                                                      localgroupfield = ConvertFromSeconds((ulong)tmp);
                                                  }
                                              }
                                              else
                                              {
                                                  if (Constants.FakeGameFields.Contains(page.Groupfield))
                                                  {
                                                      localgroupfield = String.Empty;
                                                      var tmp = fakegame.GetType().GetProperty(page.Groupfield).GetValue(fakegame);
                                                      if (tmp != null)
                                                      {
                                                          localgroupfield = tmp.ToString();
                                                      }
                                                  }
                                                  else
                                                  {
                                                      localgroupfield = String.Empty;
                                                      var tmp = fakegame.OriginalGame.GetType().GetProperty(page.Groupfield).GetValue(fakegame.OriginalGame);
                                                      if (tmp != null)
                                                      {
                                                          localgroupfield = tmp.ToString();
                                                      }
                                                  }
                                              }
                                          }
                                      }
                                      localgroupfield = Constants.SantizeValue(localgroupfield, page.Groupfield);
                                  }

                                  if (prevlocalgroupfield != localgroupfield)
                                  {
                                      Dictionary<string, string> CurrentDict = CurrentPageValuesDict;
                                      CurrentDict["groupname"] = HttpUtility.HtmlEncode(prevlocalgroupfield);
                                      CurrentDict["groupcount"] = groupcount.ToString();
                                      CurrentDict["count"] = count.ToString();
                                      //optimize
                                      string QuickLinkOutput = ReplaceDictionary(QuickLink, CurrentDict);
                                      string GameCardsHeaderOutput = ReplaceDictionary(GameCardsHeader, CurrentDict);

                                      AllQuicklinksBuilder.Append(QuickLinkOutput);

                                      CurrentDict["gamecardsheader"] = GameCardsHeaderOutput;
                                      CurrentDict["gamecards"] = GameCardsOutputBuilder.ToString();

                                      string GameCardsHeaderGroupedOutput = ReplaceDictionary(GameCardsHeaderGrouped, CurrentDict);

                                      AllGameCardsWithHeaders += GameCardsHeaderGroupedOutput;

                                      GameCardsOutputBuilder.Clear();
                                      prevlocalgroupfield = localgroupfield;
                                      groupcount = 0;
                                  }

                                  string gameicon = String.Empty;
                                  if (GameMediaHashtable.ContainsKey(Constants.MediaIconText + realgame.Id.ToString()))
                                  {
                                      gameicon = (string)GameMediaHashtable[Constants.MediaIconText + realgame.Id.ToString()];
                                  }
                                  else
                                  {
                                      string filespathicon = String.Empty;
                                      if (!String.IsNullOrEmpty(realgame.Icon))
                                      {
                                          filespathicon = realgame.Icon;
                                      }

                                      if (filespathicon.ToLower().StartsWith("https://") || filespathicon.ToLower().StartsWith("http://"))
                                      {
                                          gameicon = filespathicon;
                                      }
                                      else
                                      {
                                          if ((filespathicon != String.Empty) && File.Exists(PlayniteApi.Database.GetFullFilePath(filespathicon)))
                                          {
                                              gameicon = realgame.Icon.Replace("\\", "/");
                                          }
                                          else
                                          {
                                              var tmpplatform = realgame.Platforms?.FirstOrDefault(o => !String.IsNullOrEmpty(o.Icon));
                                              if (!String.IsNullOrEmpty(tmpplatform?.Icon))
                                              {
                                                  gameicon = tmpplatform.Icon.Replace("\\", "/");
                                              }
                                          }
                                          gameicon = DeDuplicator.GetUniqueFile(PlayniteApi.Database.GetFullFilePath(gameicon), gameicon, Settings.Settings.ConvertImageOptions.DetectDuplicates);
                                      }
                                      GameMediaHashtable[Constants.MediaIconText + realgame.Id.ToString()] = gameicon;
                                  }

                                  string coverimage = String.Empty;

                                  if (GameMediaHashtable.ContainsKey(Constants.MediaCoverText + realgame.Id.ToString()))
                                  {
                                      coverimage = (string)GameMediaHashtable[Constants.MediaCoverText + realgame.Id.ToString()];
                                  }
                                  else
                                  {
                                      string filespathcover = String.Empty;
                                      if (!String.IsNullOrEmpty(realgame.CoverImage))
                                      {
                                          filespathcover = realgame.CoverImage;
                                      }

                                      if (filespathcover.ToLower().StartsWith("https://") || filespathcover.ToLower().StartsWith("http://"))
                                      {
                                          coverimage = filespathcover;
                                      }
                                      else
                                      {
                                          if ((filespathcover != String.Empty) && File.Exists(PlayniteApi.Database.GetFullFilePath(filespathcover)))
                                          {
                                              coverimage = realgame.CoverImage.Replace("\\", "/");
                                          }
                                          else
                                          {
                                              var tmpplatform = realgame.Platforms?.FirstOrDefault(o => !String.IsNullOrEmpty(o.Cover));
                                              if (!String.IsNullOrEmpty(tmpplatform?.Cover))
                                              {
                                                  gameicon = tmpplatform.Cover.Replace("\\", "/");
                                              }
                                          }

                                          coverimage = DeDuplicator.GetUniqueFile(PlayniteApi.Database.GetFullFilePath(coverimage), coverimage, Settings.Settings.ConvertImageOptions.DetectDuplicates);
                                      }
                                      GameMediaHashtable[Constants.MediaCoverText + realgame.Id.ToString()] = coverimage;
                                  }

                                  string backgroundimage = String.Empty;
                                  if (GameMediaHashtable.ContainsKey(Constants.MediaBackgroundText + realgame.Id.ToString()))
                                  {
                                      backgroundimage = (string)GameMediaHashtable[Constants.MediaBackgroundText + realgame.Id.ToString()];
                                  }
                                  else
                                  {
                                      string filespathbackground = String.Empty;
                                      if (!String.IsNullOrEmpty(realgame.BackgroundImage))
                                      {
                                          filespathbackground = realgame.BackgroundImage;
                                      }

                                      if (filespathbackground.ToLower().StartsWith("https://") || filespathbackground.ToLower().StartsWith("http://"))
                                      {
                                          backgroundimage = filespathbackground;
                                      }
                                      else
                                      {
                                          if ((filespathbackground != String.Empty) && File.Exists(PlayniteApi.Database.GetFullFilePath(filespathbackground)))
                                          {
                                              backgroundimage = realgame.BackgroundImage.Replace("\\", "/");
                                          }
                                          else
                                          {
                                              var tmpplatform = realgame.Platforms?.FirstOrDefault(o => !String.IsNullOrEmpty(o.Background));
                                              if (!String.IsNullOrEmpty(tmpplatform?.Background))
                                              {
                                                  gameicon = tmpplatform.Background.Replace("\\", "/");
                                              }
                                          }
                                          backgroundimage = DeDuplicator.GetUniqueFile(PlayniteApi.Database.GetFullFilePath(backgroundimage), backgroundimage, Settings.Settings.ConvertImageOptions.DetectDuplicates);
                                      }
                                      GameMediaHashtable[Constants.MediaBackgroundText + realgame.Id.ToString()] = backgroundimage;
                                  }


                                  string AddedDateDate = String.Empty;
                                  string AddedDateTime = String.Empty;
                                  if (realgame.Added.HasValue)
                                  {
                                      AddedDateDate = ((DateTime)(realgame.Added)).Date.ToShortDateString();
                                      TimeSpan TmpAdded = ((DateTime)(realgame.Added)).TimeOfDay;
                                      if (!((TmpAdded.Days == 0) && (TmpAdded.Hours == 0) &&
                                          (TmpAdded.Minutes == 0) && (TmpAdded.Seconds == 0)))
                                      {
                                          AddedDateTime = TmpAdded.ToString(@"h\:mm\:ss");
                                      }
                                  }

                                  string ReleaseDateDate = String.Empty;
                                  string ReleaseDateTime = String.Empty;
                                  if (realgame.ReleaseDate.HasValue)
                                  {
                                      ReleaseDateDate = realgame.ReleaseDate.ToString();
                                  }

                                  string LastModifieddDate = Constants.NeverText;
                                  string LastModifiedTime = String.Empty;
                                  if (realgame.Modified.HasValue)
                                  {
                                      LastModifieddDate = ((DateTime)(realgame.Modified)).Date.ToShortDateString();
                                      TimeSpan TmpModified = ((DateTime)(realgame.Modified)).TimeOfDay;
                                      if (!((TmpModified.Days == 0) && (TmpModified.Hours == 0) &&
                                          (TmpModified.Minutes == 0) && (TmpModified.Seconds == 0)))
                                      {
                                          LastModifiedTime = TmpModified.ToString(@"h\:mm\:ss");
                                      }
                                  }

                                  string LastPlayedDate = Constants.NeverText;
                                  string LastPlayedTime = String.Empty;
                                  if (realgame.LastActivity.HasValue)
                                  {
                                      LastPlayedDate = ((DateTime)(realgame.LastActivity)).Date.ToShortDateString();
                                      TimeSpan TmpLastActivity = ((DateTime)(realgame.LastActivity)).TimeOfDay;
                                      if (!((TmpLastActivity.Days == 0) && (TmpLastActivity.Hours == 0) &&
                                          (TmpLastActivity.Minutes == 0) && (TmpLastActivity.Seconds == 0)))
                                      {
                                          LastPlayedTime = TmpLastActivity.ToString(@"h\:mm\:ss");
                                      }
                                  }

                                  string LastActivitySegment = Constants.SantizeValue(realgame.LastActivitySegment.ToString(), "LastActivitySegment");
                                  string PlaytimeCategory = Constants.SantizeValue(realgame.PlaytimeCategory.ToString(), "PlaytimeCategory");
                                  string CompletionStatus = Constants.SantizeValue(realgame.CompletionStatus?.ToString(), "CompletionStatus");
                                  string UserScoreGroup = Constants.SantizeValue(realgame.UserScoreGroup.ToString(), "UserScoreGroup");
                                  string CommunityScoreGroup = Constants.SantizeValue(realgame.CommunityScoreGroup.ToString(), "CommunityScoreGroup");
                                  string CriticScoreGroup = Constants.SantizeValue(realgame.CriticScoreGroup.ToString(), "CriticScoreGroup");
                                  string AddedSegment = Constants.SantizeValue(realgame.AddedSegment.ToString(), "AddedSegment");
                                  string InstallationStatus = Constants.SantizeValue(realgame.InstallationStatus.ToString(), "InstallationStatus");
                                  string ModifiedSegment = Constants.SantizeValue(realgame.ModifiedSegment.ToString(), "ModifiedSegment");
                                  string PlayCount = Constants.SantizeValue(realgame.PlayCount.ToString(), "PlayCount");
                                  string Playtime = ConvertFromSeconds(realgame.Playtime);
                                  string UserScoreRating = Constants.SantizeValue(realgame.UserScoreRating.ToString(), "UserScoreRating");
                                  string CriticScoreRating = Constants.SantizeValue(realgame.CriticScoreRating.ToString(), "CriticScoreRating");
                                  string CommunityScoreRating = Constants.SantizeValue(realgame.CommunityScoreRating.ToString(), "CommunityScoreRating");
                                  string FavoriteField = Constants.SantizeValue(realgame.Favorite.ToString(), "Favorite");
                                  string HiddenField = Constants.SantizeValue(realgame.Hidden.ToString(), "Hidden");
                                  string IsCustomGameField = Constants.SantizeValue(realgame.IsCustomGame.ToString(), "IsCustomGame");

                                  CurrentGameValuesDict["gamename"] = HttpUtility.HtmlEncode(realgame.Name);
                                  CurrentGameValuesDict["name"] = HttpUtility.HtmlEncode(realgame.Name);
                                  CurrentGameValuesDict["groupname"] = HttpUtility.HtmlEncode(prevlocalgroupfield);
                                  CurrentGameValuesDict["sortfield"] = HttpUtility.HtmlEncode(PageSortFieldName);
                                  CurrentGameValuesDict["groupcount"] = groupcount.ToString();
                                  CurrentGameValuesDict["groupfield"] = HttpUtility.HtmlEncode(PageGroupFieldName); ;
                                  CurrentGameValuesDict["groupfieldrealnamelower"] = HttpUtility.HtmlEncode(PageGroupFieldRealNameLower);
                                  CurrentGameValuesDict["sortfieldrealnamelower"] = HttpUtility.HtmlEncode(PageSortFieldRealNameLower);
                                  CurrentGameValuesDict["pagefilename"] = Uri.EscapeDataString(page.Pagefilename);
                                  CurrentGameValuesDict["pagetitle"] = HttpUtility.HtmlEncode(page.Pagetitle);
                                  CurrentGameValuesDict["count"] = count.ToString();

                                  CurrentGameValuesDict["coverimage"] = Settings.Settings.ConvertImageOptions.CoverDestFilename(coverimage);
                                  CurrentGameValuesDict["icon"] = Settings.Settings.ConvertImageOptions.IconDestFilename(gameicon);
                                  CurrentGameValuesDict["backgroundimage"] = Settings.Settings.ConvertImageOptions.BackgroundDestFilename(backgroundimage);

                                  CurrentGameValuesDict["lastactivitydate"] = HttpUtility.HtmlEncode(LastPlayedDate);
                                  CurrentGameValuesDict["lastactivitytime"] = HttpUtility.HtmlEncode(LastPlayedTime);
                                  CurrentGameValuesDict["addeddate"] = HttpUtility.HtmlEncode(AddedDateDate);
                                  CurrentGameValuesDict["addedtime"] = HttpUtility.HtmlEncode(AddedDateTime);
                                  CurrentGameValuesDict["releasedatedate"] = HttpUtility.HtmlEncode(ReleaseDateDate);
                                  CurrentGameValuesDict["releasedatetime"] = HttpUtility.HtmlEncode(ReleaseDateTime);
                                  CurrentGameValuesDict["modifieddate"] = HttpUtility.HtmlEncode(LastModifieddDate);
                                  CurrentGameValuesDict["modifiedtime"] = HttpUtility.HtmlEncode(LastModifiedTime);
                                  CurrentGameValuesDict["lastactivity"] = (HttpUtility.HtmlEncode(LastPlayedDate) + " " + HttpUtility.HtmlEncode(LastPlayedTime)).Trim();
                                  CurrentGameValuesDict["added"] = (HttpUtility.HtmlEncode(AddedDateDate) + " " + HttpUtility.HtmlEncode(AddedDateTime)).Trim();
                                  CurrentGameValuesDict["releasedate"] = (HttpUtility.HtmlEncode(ReleaseDateDate) + " " + HttpUtility.HtmlEncode(ReleaseDateTime)).Trim();
                                  CurrentGameValuesDict["modified"] = (HttpUtility.HtmlEncode(LastModifieddDate) + " " + HttpUtility.HtmlEncode(LastModifiedTime)).Trim();
                                  CurrentGameValuesDict["notes"] = HttpUtility.HtmlEncode(realgame.Notes);
                                  CurrentGameValuesDict["version"] = HttpUtility.HtmlEncode(realgame.Version);
                                  CurrentGameValuesDict["playtime"] = HttpUtility.HtmlEncode(Playtime);
                                  CurrentGameValuesDict["source"] = HttpUtility.HtmlEncode(realgame.Source);
                                  CurrentGameValuesDict["platform"] = HttpUtility.HtmlEncode(fakegame.Platform);
                                  CurrentGameValuesDict["releaseyear"] = HttpUtility.HtmlEncode(realgame.ReleaseYear);
                                  CurrentGameValuesDict["agerating"] = HttpUtility.HtmlEncode(fakegame.AgeRating);
                                  CurrentGameValuesDict["favorite"] = HttpUtility.HtmlEncode(FavoriteField);
                                  CurrentGameValuesDict["lastactivitysegment"] = HttpUtility.HtmlEncode(LastActivitySegment);
                                  CurrentGameValuesDict["playtimecategory"] = HttpUtility.HtmlEncode(PlaytimeCategory);
                                  CurrentGameValuesDict["region"] = HttpUtility.HtmlEncode(fakegame.Region);
                                  CurrentGameValuesDict["completionstatus"] = HttpUtility.HtmlEncode(CompletionStatus);
                                  CurrentGameValuesDict["userscore"] = HttpUtility.HtmlEncode(realgame.UserScore);
                                  CurrentGameValuesDict["userscoregroup"] = HttpUtility.HtmlEncode(UserScoreGroup);
                                  CurrentGameValuesDict["userscorerating"] = HttpUtility.HtmlEncode(UserScoreRating);
                                  CurrentGameValuesDict["communityscore"] = HttpUtility.HtmlEncode(realgame.CommunityScore);
                                  CurrentGameValuesDict["communityscoregroup"] = HttpUtility.HtmlEncode(CommunityScoreGroup);
                                  CurrentGameValuesDict["communityscorerating"] = HttpUtility.HtmlEncode(CommunityScoreRating);
                                  CurrentGameValuesDict["criticscore"] = HttpUtility.HtmlEncode(realgame.CriticScore);
                                  CurrentGameValuesDict["criticscoregroup"] = HttpUtility.HtmlEncode(CriticScoreGroup);
                                  CurrentGameValuesDict["criticscorerating"] = HttpUtility.HtmlEncode(CriticScoreRating);
                                  CurrentGameValuesDict["addedsegment"] = HttpUtility.HtmlEncode(AddedSegment);
                                  CurrentGameValuesDict["installationstatus"] = HttpUtility.HtmlEncode(InstallationStatus);
                                  CurrentGameValuesDict["iscustomgame"] = HttpUtility.HtmlEncode(IsCustomGameField);
                                  CurrentGameValuesDict["modifiedsegment"] = HttpUtility.HtmlEncode(ModifiedSegment);
                                  CurrentGameValuesDict["playcount"] = HttpUtility.HtmlEncode(PlayCount);
                                  CurrentGameValuesDict["description"] = realgame.Description;
                                  CurrentGameValuesDict["detailspagefilename"] = Uri.EscapeDataString(realgame.Id.ToString() + ".html");
                                  CurrentGameValuesDict["category"] = HttpUtility.HtmlEncode(fakegame.Category);
                                  CurrentGameValuesDict["genre"] = HttpUtility.HtmlEncode(fakegame.Genre);
                                  CurrentGameValuesDict["developer"] = HttpUtility.HtmlEncode(fakegame.Developer);
                                  CurrentGameValuesDict["feature"] = HttpUtility.HtmlEncode(fakegame.Feature);
                                  CurrentGameValuesDict["publisher"] = HttpUtility.HtmlEncode(fakegame.Publisher);
                                  CurrentGameValuesDict["tag"] = HttpUtility.HtmlEncode(fakegame.Tag);
                                  CurrentGameValuesDict["serie"] = HttpUtility.HtmlEncode(fakegame.Serie);
                                  CurrentGameValuesDict["hidden"] = HttpUtility.HtmlEncode(HiddenField);
                                  //CurrentGameValuesDict["pluginid"] = HttpUtility.HtmlEncode(fakegame.Library);
                                  CurrentGameValuesDict["library"] = HttpUtility.HtmlEncode(fakegame.Library);

                                  foreach (string agroupfield in Constants.AvailableGroupFields)
                                  {
                                      if (agroupfield == Constants.NotGroupedField)
                                      {
                                          continue;
                                      }

                                      string filename = String.Empty;
                                      if (FirstGroupFieldFileNames.ContainsKey(agroupfield))
                                      {
                                          filename = (string)FirstGroupFieldFileNames[agroupfield];
                                      }
                                      CurrentGameValuesDict["" + agroupfield.ToLower() + "_filename"] = filename;
                                      CurrentGameValuesDict["" + agroupfield.ToLower() + "_name"] = Constants.GetNameFromField(agroupfield);
                                  }

                                  string GameCardOutPut = ReplaceDictionary(GameCard, CurrentGameValuesDict);
                                  GameCardsOutputBuilder.Append(GameCardOutPut);

                                  if (PageNr == 1)
                                  {
                                      //uses hash table as we return platform images as well and they can be processes multiple times.
                                      //also if fakefield groupby was used we can encounter same game multiple times and no need
                                      //to copy the files again.
                                      if (Settings.Settings.CopyImages)
                                      {
                                          OriginalGameImageData ImageData = PreviousImageDataFile.GetOriginalGameImageData(fakegame.OriginalGame.Id.ToString());

                                          if (!GameMediaCopyDoneDict.ContainsKey(fakegame.OriginalGame.Id.ToString()))
                                          {
                                              if (!String.IsNullOrEmpty(backgroundimage) &&
                                                (!(backgroundimage.ToLower().StartsWith("https://") || backgroundimage.ToLower().StartsWith("http://"))))
                                              {
                                                  if (!GameMediaCopyDoneDict.ContainsKey(Constants.MediaBackgroundText + backgroundimage))
                                                  {
                                                      string fullbackgroundimage = Path.Combine(StoragePath, backgroundimage.Replace("/", "\\"));
                                                      string fullbackgroundimagedest = Path.Combine(outputfolder, backgroundimage.Replace("/", "\\"));
                                                      try
                                                      {
                                                          string DestFile = Settings.Settings.ConvertImageOptions.BackgroundDestFilename(fullbackgroundimagedest);
                                                          if (Settings.Settings.ConvertImageOptions.AlwaysProcess || !ImageData.BackgroundImageSame(fullbackgroundimage) || !File.Exists(DestFile))
                                                          {
                                                              if (File.Exists(fullbackgroundimage))
                                                              {
                                                                  bool needsConversion = Settings.Settings.ConvertImageOptions.BackgroundNeedsConversion(fullbackgroundimage);

                                                                  ConvertImagesRunner.addImageProcess(!needsConversion, fullbackgroundimage, fullbackgroundimagedest, DestFile, Settings.Settings.ConvertImageOptions.ImageMagickLocation,
                                                                        "\"" + fullbackgroundimage + "\" " + Settings.Settings.ConvertImageOptions.BackgroundOptions(Path.GetExtension(fullbackgroundimage), Path.GetExtension(DestFile)) + " \"" + DestFile + "\"",
                                                                        File.Exists(Settings.Settings.ConvertImageOptions.ImageMagickLocation) ? Path.GetDirectoryName(Settings.Settings.ConvertImageOptions.ImageMagickLocation) : "", true);
                                                              }
                                                          }
                                                          ImageData.SetBackgroundImageData(fullbackgroundimage);
                                                      }
                                                      catch { };
                                                      GameMediaCopyDoneDict[Constants.MediaBackgroundText + backgroundimage] = true;
                                                  }
                                              }

                                              if (!String.IsNullOrEmpty(coverimage) &&
                                                (!(coverimage.ToLower().StartsWith("https://") || coverimage.ToLower().StartsWith("http://"))))
                                              {
                                                  if (!GameMediaCopyDoneDict.ContainsKey(Constants.MediaCoverText + coverimage))
                                                  {
                                                      string fullcoverimage = Path.Combine(StoragePath, coverimage.Replace("/", "\\"));
                                                      string fullcoverimagedest = Path.Combine(outputfolder, coverimage.Replace("/", "\\"));
                                                      try
                                                      {
                                                          string DestFile = Settings.Settings.ConvertImageOptions.CoverDestFilename(fullcoverimagedest);
                                                          if (Settings.Settings.ConvertImageOptions.AlwaysProcess || !ImageData.CoverImageSame(fullcoverimage) || !File.Exists(DestFile))
                                                          {
                                                              if (File.Exists(fullcoverimage))
                                                              {
                                                                  bool needsConversion = Settings.Settings.ConvertImageOptions.CoverNeedsConversion(fullcoverimage);

                                                                  ConvertImagesRunner.addImageProcess(!needsConversion, fullcoverimage, fullcoverimagedest, DestFile, Settings.Settings.ConvertImageOptions.ImageMagickLocation,
                                                                        "\"" + fullcoverimage + "\" " + Settings.Settings.ConvertImageOptions.CoverOptions(Path.GetExtension(fullcoverimage), Path.GetExtension(DestFile)) + " \"" + DestFile + "\"",
                                                                        File.Exists(Settings.Settings.ConvertImageOptions.ImageMagickLocation) ? Path.GetDirectoryName(Settings.Settings.ConvertImageOptions.ImageMagickLocation) : "", true);
                                                              }
                                                          }
                                                          ImageData.SetCoverImageData(fullcoverimage);
                                                      }
                                                      catch { };
                                                      GameMediaCopyDoneDict[Constants.MediaCoverText + fakegame.OriginalGame.Id.ToString()] = true;
                                                  }
                                              }

                                              if (!String.IsNullOrEmpty(gameicon) &&
                                                (!(gameicon.ToLower().StartsWith("https://") || gameicon.ToLower().StartsWith("http://"))))
                                              {
                                                  if (!GameMediaCopyDoneDict.ContainsKey(Constants.MediaIconText + gameicon))
                                                  {
                                                      string fulliconimage = Path.Combine(StoragePath, gameicon.Replace("/", "\\"));
                                                      string fulliconimagedest = Path.Combine(outputfolder, gameicon.Replace("/", "\\"));
                                                      try
                                                      {
                                                          string DestFile = Settings.Settings.ConvertImageOptions.IconDestFilename(fulliconimagedest);
                                                          if (Settings.Settings.ConvertImageOptions.AlwaysProcess || !ImageData.IconImageSame(fulliconimage) || !File.Exists(DestFile))
                                                          {
                                                              if (File.Exists(fulliconimage))
                                                              {
                                                                  bool needsConversion = Settings.Settings.ConvertImageOptions.IconNeedsConversion(fulliconimage);

                                                                  ConvertImagesRunner.addImageProcess(!needsConversion, fulliconimage, fulliconimagedest, DestFile, Settings.Settings.ConvertImageOptions.ImageMagickLocation,
                                                                            "\"" + fulliconimage + "\" " + Settings.Settings.ConvertImageOptions.IconOptions(Path.GetExtension(fulliconimage), Path.GetExtension(DestFile)) + " \"" + DestFile + "\"",
                                                                            File.Exists(Settings.Settings.ConvertImageOptions.ImageMagickLocation) ? Path.GetDirectoryName(Settings.Settings.ConvertImageOptions.ImageMagickLocation) : "", true);

                                                              }
                                                          }
                                                          ImageData.SetIconImageData(fulliconimage);
                                                      }
                                                      catch { };
                                                      GameMediaCopyDoneDict[Constants.MediaIconText + gameicon] = true;
                                                  }
                                              }
                                              GameMediaCopyDoneDict[fakegame.OriginalGame.Id.ToString()] = true;
                                          }

                                          PreviousImageDataFile.SetOriginalGameImageData(fakegame.OriginalGame.Id.ToString(), ImageData);

                                      }

                                      StringBuilder CategoriesOutput = new StringBuilder();

                                      if (fakegame.OriginalGame.Categories != null)
                                      {
                                          string filename = String.Empty;
                                          if (FirstGroupFieldFileNames.ContainsKey(Constants.CategoryField))
                                          {
                                              filename = (string)FirstGroupFieldFileNames[Constants.CategoryField];
                                          }

                                          foreach (Category category in fakegame.OriginalGame.Categories)
                                          {
                                              string GameCardDetailsCategoryOutput = GameCardDetailsCategory.Replace("%category%", category.Name);
                                              GameCardDetailsCategoryOutput = GameCardDetailsCategoryOutput.Replace("%category_filename%", filename);
                                              CategoriesOutput.Append(GameCardDetailsCategoryOutput);
                                          }
                                      }
                                      CurrentGameValuesDict["categories"] = CategoriesOutput.ToString();

                                      StringBuilder GenresOutput = new StringBuilder();
                                      if (fakegame.OriginalGame.Genres != null)
                                      {
                                          string filename = String.Empty;
                                          if (FirstGroupFieldFileNames.ContainsKey(Constants.GenreField))
                                          {
                                              filename = (string)FirstGroupFieldFileNames[Constants.GenreField];
                                          }

                                          foreach (Genre genre in fakegame.OriginalGame.Genres)
                                          {
                                              string GameCardDetailsGenreOutput = GameCardDetailsGenre.Replace("%genre%", genre.Name);
                                              GameCardDetailsGenreOutput = GameCardDetailsGenreOutput.Replace("%genre_filename%", filename);
                                              GenresOutput.Append(GameCardDetailsGenreOutput);
                                          }
                                      }
                                      CurrentGameValuesDict["genres"] = GenresOutput.ToString();

                                      StringBuilder FeaturesOutput = new StringBuilder();
                                      if (fakegame.OriginalGame.Features != null)
                                      {
                                          string filename = String.Empty;
                                          if (FirstGroupFieldFileNames.ContainsKey(Constants.FeatureField))
                                          {
                                              filename = (string)FirstGroupFieldFileNames[Constants.FeatureField];
                                          }
                                          foreach (GameFeature feature in fakegame.OriginalGame.Features)
                                          {
                                              string GameCardDetailsFeatureOutput = GameCardDetailsFeature.Replace("%feature%", feature.Name);
                                              GameCardDetailsFeatureOutput = GameCardDetailsFeatureOutput.Replace("%feature_filename%", filename);
                                              FeaturesOutput.Append(GameCardDetailsFeatureOutput);
                                          }
                                      }
                                      CurrentGameValuesDict["features"] = FeaturesOutput.ToString();

                                      StringBuilder DevelopersOutput = new StringBuilder();
                                      if (fakegame.OriginalGame.Developers != null)
                                      {
                                          string filename = String.Empty;
                                          if (FirstGroupFieldFileNames.ContainsKey(Constants.DeveloperField))
                                          {
                                              filename = (string)FirstGroupFieldFileNames[Constants.DeveloperField];
                                          }
                                          foreach (Company developer in fakegame.OriginalGame.Developers)
                                          {
                                              string GameCardDetailsDeveloperOutput = GameCardDetailsDeveloper.Replace("%developer%", developer.Name);
                                              GameCardDetailsDeveloperOutput = GameCardDetailsDeveloperOutput.Replace("%developer_filename%", filename);
                                              DevelopersOutput.Append(GameCardDetailsDeveloperOutput);
                                          }
                                      }
                                      CurrentGameValuesDict["developers"] = DevelopersOutput.ToString();

                                      StringBuilder PublishersOutput = new StringBuilder();
                                      if (fakegame.OriginalGame.Publishers != null)
                                      {
                                          string filename = String.Empty;
                                          if (FirstGroupFieldFileNames.ContainsKey(Constants.PublisherField))
                                          {
                                              filename = (string)FirstGroupFieldFileNames[Constants.PublisherField];
                                          }
                                          foreach (Company publisher in fakegame.OriginalGame.Publishers)
                                          {
                                              string GameCardDetailsPublisherOutput = GameCardDetailsPublisher.Replace("%publisher%", publisher.Name);
                                              GameCardDetailsPublisherOutput = GameCardDetailsPublisherOutput.Replace("%publisher_filename%", filename);
                                              PublishersOutput.Append(GameCardDetailsPublisherOutput);
                                          }
                                      }
                                      CurrentGameValuesDict["publishers"] = PublishersOutput.ToString();

                                      StringBuilder TagsOutput = new StringBuilder();
                                      if (fakegame.OriginalGame.Tags != null)
                                      {
                                          string filename = String.Empty;
                                          if (FirstGroupFieldFileNames.ContainsKey(Constants.TagField))
                                          {
                                              filename = (string)FirstGroupFieldFileNames[Constants.TagField];
                                          }
                                          foreach (Tag tag in fakegame.OriginalGame.Tags)
                                          {
                                              string GameCardDetailsTagOutput = GameCardDetailsTag.Replace("%tag%", tag.Name);
                                              GameCardDetailsTagOutput = GameCardDetailsTagOutput.Replace("%tag_filename%", filename);
                                              TagsOutput.Append(GameCardDetailsTagOutput);
                                          }
                                      }
                                      CurrentGameValuesDict["tags"] = TagsOutput.ToString();

                                      StringBuilder PlatformOutput = new StringBuilder();
                                      if (fakegame.OriginalGame.Platforms != null)
                                      {
                                          string filename = String.Empty;
                                          if (FirstGroupFieldFileNames.ContainsKey(Constants.PlatformField))
                                          {
                                              filename = (string)FirstGroupFieldFileNames[Constants.PlatformField];
                                          }
                                          foreach (Platform platform in fakegame.OriginalGame.Platforms)
                                          {
                                              string GameCardDetailsPlatformOutput = GameCardDetailsPlatform.Replace("%platform%", platform.Name);
                                              GameCardDetailsPlatformOutput = GameCardDetailsPlatformOutput.Replace("%platform_filename%", filename);
                                              PlatformOutput.Append(GameCardDetailsPlatformOutput);
                                          }
                                      }
                                      CurrentGameValuesDict["platforms"] = PlatformOutput.ToString();

                                      StringBuilder RegionOutput = new StringBuilder();
                                      if (fakegame.OriginalGame.Regions != null)
                                      {
                                          string filename = String.Empty;
                                          if (FirstGroupFieldFileNames.ContainsKey(Constants.RegionField))
                                          {
                                              filename = (string)FirstGroupFieldFileNames[Constants.RegionField];
                                          }
                                          foreach (Region region in fakegame.OriginalGame.Regions)
                                          {
                                              string GameCardDetailsRegionOutput = GameCardDetailsRegion.Replace("%region%", region.Name);
                                              GameCardDetailsRegionOutput = GameCardDetailsRegionOutput.Replace("%region_filename%", filename);
                                              RegionOutput.Append(GameCardDetailsRegionOutput);
                                          }
                                      }
                                      CurrentGameValuesDict["regions"] = RegionOutput.ToString();

                                      StringBuilder AgeRatingOutput = new StringBuilder();
                                      if (fakegame.OriginalGame.AgeRatings != null)
                                      {
                                          string filename = String.Empty;
                                          if (FirstGroupFieldFileNames.ContainsKey(Constants.AgeRatingField))
                                          {
                                              filename = (string)FirstGroupFieldFileNames[Constants.AgeRatingField];
                                          }
                                          foreach (AgeRating ageRating in fakegame.OriginalGame.AgeRatings)
                                          {
                                              string GameCardDetailsAgeRatingOutput = GameCardDetailsAgeRating.Replace("%agerating%", ageRating.Name);
                                              GameCardDetailsAgeRatingOutput = GameCardDetailsAgeRatingOutput.Replace("%agerating_filename%", filename);
                                              AgeRatingOutput.Append(GameCardDetailsAgeRatingOutput);
                                          }
                                      }
                                      CurrentGameValuesDict["ageratings"] = AgeRatingOutput.ToString();

                                      StringBuilder WebLinksOutput = new StringBuilder();
                                      if (fakegame.OriginalGame.Links != null)
                                      {
                                          foreach (Link link in fakegame.OriginalGame.Links)
                                          {
                                              if (link != null)
                                              {
                                                  string GameCardDetailsWeblinkOutput = GameCardDetailsWeblink.Replace("%linkname%", link.Name);
                                                  GameCardDetailsWeblinkOutput = GameCardDetailsWeblinkOutput.Replace("%linkurl%", link.Url);
                                                  WebLinksOutput.Append(GameCardDetailsWeblinkOutput);
                                              }
                                          }
                                      }
                                      CurrentGameValuesDict["links"] = WebLinksOutput.ToString();

                                      string GameDetailsOutPut = ReplaceDictionary(GameCardDetails, CurrentGameValuesDict);
                                      string GameCardDetailsOutPutFilename = Path.Combine(outputfolder, realgame.Id.ToString() + ".html");
                                      //GameDetailsOutPut = Compressor.compress(GameDetailsOutPut);
                                      File.WriteAllText(GameCardDetailsOutPutFilename, GameDetailsOutPut, Encoding.UTF8);
                                  }
                                  groupcount += 1;
                              }

                              if (fakegames.Count != 0)
                              {
                                  Dictionary<string, string> CurrentDict = CurrentPageValuesDict;
                                  CurrentDict["groupname"] = HttpUtility.HtmlEncode(prevlocalgroupfield);
                                  CurrentDict["groupcount"] = groupcount.ToString();
                                  CurrentDict["count"] = count.ToString();

                                  string QuickLinkOutput = ReplaceDictionary(QuickLink, CurrentDict);
                                  string GameCardsHeaderOutput = ReplaceDictionary(GameCardsHeader, CurrentDict);
                                  AllQuicklinksBuilder.Append(QuickLinkOutput);

                                  CurrentDict["gamecardsheader"] = GameCardsHeaderOutput;
                                  CurrentDict["gamecards"] = GameCardsOutputBuilder.ToString();
                                  string GameCardsHeaderGroupedOutput = ReplaceDictionary(GameCardsHeaderGrouped, CurrentDict);

                                  AllGameCardsWithHeaders += GameCardsHeaderGroupedOutput;
                              }

                              Dictionary<string, string> IndexOutPutDict = CurrentPageValuesDict;
                              IndexOutPutDict["menuentryheader"] = MenuEntryHeaderoutput;
                              IndexOutPutDict["allmenuentries"] = AllMenuEntries;
                              IndexOutPutDict["quicklinksheader"] = QuickLinkHeaderOutput;
                              IndexOutPutDict["allquicklinks"] = AllQuicklinksBuilder.ToString();
                              IndexOutPutDict["allgamecardswithheaders"] = AllGameCardsWithHeaders;
                              IndexOutPutDict["count"] = count.ToString();
                              IndexOutPutDict["groupcount"] = groupcount.ToString();

                              foreach (string agroupfield in Constants.AvailableGroupFields)
                              {
                                  if (agroupfield == Constants.NotGroupedField)
                                  {
                                      continue;
                                  }
                                  string filename = String.Empty;
                                  if (FirstGroupFieldFileNames.ContainsKey(agroupfield))
                                  {
                                      filename = (string)FirstGroupFieldFileNames[agroupfield];
                                  }
                                  IndexOutPutDict["" + agroupfield.ToLower() + "_filename"] = filename;
                                  IndexOutPutDict["" + agroupfield.ToLower() + "_name"] = Constants.GetNameFromField(agroupfield);
                              }
                              string IndexOutput = ReplaceDictionary(pageindex, IndexOutPutDict);

                              //IndexOutput = Compressor.compress(IndexOutput);
                              File.WriteAllText(PageOutputFilename, IndexOutput, Encoding.UTF8);


                              PagesGenerated[page.Pagefilename] = true;
                              Succes++;
                          }
                          catch (Exception E)
                          {
                              logger.Error(E, Constants.HTMLExportError);
                              Errors++;
                          }
                      }

                      string WebIconFile = Path.Combine(pluginFolder, "webdata", "web-icon.png");
                      if (File.Exists(WebIconFile))
                      {
                          File.Copy(WebIconFile, Path.Combine(Settings.Settings.OutputFolder, "web-icon.png"), true);
                      }

                      string WebManifestFile = Path.Combine(pluginFolder, "webdata", "manifest.webmanifest");
                      if (File.Exists(WebManifestFile))
                      {
                          File.Copy(WebManifestFile, Path.Combine(Settings.Settings.OutputFolder, "manifest.webmanifest"), true);
                      }

                      if (Settings.Settings.CopyImages && !progressAction.CancelToken.IsCancellationRequested)
                      {
                          if (ConvertImagesRunner.Count() > 0)
                          {
                              try
                              {
                                  progressAction.Text = Constants.ProcessingImagesText + " (0/" + ConvertImagesRunner.Count().ToString() + ")";
                                  progressAction.CurrentProgressValue = 0;
                                  progressAction.ProgressMaxValue = 1;

                                  int UpdateProgressCount = ConvertImagesRunner.Count() > 1000 ? 50 : ConvertImagesRunner.Count() > 100 ? 5 : 1;

                                  int MaxTasks = 1;
                                  int.TryParse(Settings.Settings.ConvertImageOptions.MaxTasks, out MaxTasks);

                                  ConvertImagesRunner.Start(MaxTasks,
                                      (int count, int max) =>
                                      {
                                          if (count % UpdateProgressCount == 0)
                                          {
                                              if (progressAction.CancelToken.IsCancellationRequested)
                                                  ConvertImagesRunner.Stop();
                                              progressAction.CurrentProgressValue = count;
                                              progressAction.ProgressMaxValue = max;
                                              progressAction.Text = Constants.ProcessingImagesText + " (" + count.ToString() + "/" + max.ToString() + ")";
                                          }
                                      });

                                  if (!progressAction.CancelToken.IsCancellationRequested)
                                  {
                                      PreviousImageDataFile.SaveToFile(Path.Combine(GetPluginUserDataPath(), "PreviousImagesUsed.json"), Settings.Settings);
                                  }

                                  foreach (string error in ConvertImagesRunner.FailedFiles)
                                  {
                                      logger.Warn("Failed processing image: " + error);
                                  }
                              }
                              catch (Exception E)
                              {
                                  logger.Error(E, Constants.HTMLExportError);
                              }
                          }
                      }

                      sw.Stop();

                      string sMsg = Constants.FinishedExportingHTML + "\n\n" +
                          Constants.FinishedExportingHTMLElapsedtime + sw.Elapsed.ToString() + "\n\n" +
                          Constants.FinishedExportingHTMLSuccess + Succes.ToString() + "\n" +
                          Constants.FinishedExportingHTMLErrors + Errors.ToString() + "\n" +
                          Constants.FinishedExportingHTMLErrorsTemplate + FaultyTemplate.ToString();

                      if (Settings.Settings.CopyImages)
                      {
                          sMsg += "\n\n" +
                            Constants.TotalImagesProcessText + ConvertImagesRunner.Count().ToString() + "\n" +
                            Constants.SuccessImagesProcessText + ConvertImagesRunner.SuccesFullConverts.ToString() + "\n" +
                            Constants.FailedImagesProcessText + ConvertImagesRunner.FailedConverts.ToString() + "\n";
                      }
                      PlayniteApi.Dialogs.ShowMessage(sMsg, Constants.AppName);
                  }
                  catch (Exception E)
                  {
                      logger.Error(E, Constants.HTMLExportError);
                  }
              }, progressOptions);


        }

        public delegate void ImageProgress(string message);

        public override IEnumerable<MainMenuItem> GetMainMenuItems(GetMainMenuItemsArgs args)
        {
            List<MainMenuItem> MainMenuItems = new List<MainMenuItem>
            {
                new MainMenuItem {
                    MenuSection = "@" + Constants.HTMLExporterMenu,
                    Icon = Path.Combine(pluginFolder, "icon.png"),
                    Description = Constants.HTMLExporterSubMenu,
                    Action = (MainMenuItem) =>
                    {
                        List<string> ErrorList = new List<string>();
                        if(Settings.VerifySettings(out ErrorList))
                        {
                            DoExportToHtml();
                        }
                        else
                        {
                            StringBuilder errorbuilder = new StringBuilder();
                            foreach(string error in ErrorList)
                            {
                                errorbuilder.Append(error + "\n");
                            }
                            PlayniteApi.Dialogs.ShowMessage(errorbuilder.ToString(), Constants.AppName);
                        }
                    }
                }
             };
            return MainMenuItems;
        }

        public static string GetGamePlatform(List<Platform> Platforms)
        {
            if (Platforms == null || Platforms.Count == 0)
            {
                return String.Empty;
            }
            else
            {
                return String.Join(", ", Platforms);
            }
        }

        public static string GetGameLibrary(Guid PluginId)
        {
            if ((PluginId == null) || (PluginId == Guid.Empty))
            {
                return "Playnite";
            }
            else
            {
                if (Constants.LibraryList.ContainsKey(PluginId))
                {
                    return Constants.LibraryList[PluginId];
                }
                else
                {
                    return "Unknown";
                }
            }
        }

        public string GetPageFilename(string GroupField, bool GroupAscending, string SortField, bool SortAscending)
        {
            string tmpGroup = GroupAscending ? Constants.AscendingText : Constants.DescendingText;
            string tmpSort = SortAscending ? Constants.DescendingText : Constants.DescendingText;

            return GroupField.ToLower() + "_" + tmpGroup + "_" + SortField.ToLower() + "_" + tmpSort + ".html";
        }

        public PageObject CreatePageObject(string templatefoldername, string groupfield, bool groupascending, string sortfield, bool sortascending, bool usegroupfieldasname)
        {
            return new PageObject(usegroupfieldasname ? Constants.GetNameFromField(groupfield) : Constants.GetNameFromField(groupfield) + " / " + Constants.GetNameFromField(sortfield), templatefoldername, groupfield, groupascending, sortfield, sortascending);

        }


        public string ConvertFromSeconds(ulong PlayTimeSeconds)
        {
            string playtime = String.Empty;

            if (PlayTimeSeconds > 0)
            {
                double playtimetemp = PlayTimeSeconds;
                ulong day = (ulong)Math.Truncate((double)playtimetemp / (24 * 3600));
                playtimetemp %= (24 * 3600);
                ulong hour = (ulong)Math.Truncate((double)playtimetemp / 3600);
                playtimetemp %= 3600;
                ulong minutes = (ulong)Math.Truncate((double)playtimetemp / 60);
                playtimetemp %= 60;
                ulong seconds = (ulong)playtimetemp;

                if (day > 0)
                {
                    playtime = playtime + day + "d ";
                }

                if ((hour > 0) || (day > 0))
                {
                    playtime = playtime + hour + "h ";

                }

                if ((minutes > 0) || (hour > 0) || (day > 0))
                {
                    playtime = playtime + minutes + "m ";
                }

                if ((seconds > 0) || (minutes > 0) || (hour > 0) || (day > 0))
                {
                    playtime = playtime + seconds + "s";

                }
            }
            if (String.IsNullOrEmpty(playtime))
            {
                playtime = Constants.NotPlayedText;
            }
            return playtime;
        }


        public override void OnGameInstalled(OnGameInstalledEventArgs args)
        {
            // Add code to be executed when game is finished installing.
        }

        public override void OnGameStarted(OnGameStartedEventArgs args)
        {
            // Add code to be executed when game is started running.
        }

        public override void OnGameStarting(OnGameStartingEventArgs args)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameStopped(OnGameStoppedEventArgs args)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameUninstalled(OnGameUninstalledEventArgs args)
        {
            // Add code to be executed when game is uninstalled.
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            // Add code to be executed when Playnite is initialized.
        }

        public override void OnApplicationStopped(OnApplicationStoppedEventArgs args)
        {
            // Add code to be executed when Playnite is shutting down.
        }

        public override void OnLibraryUpdated(OnLibraryUpdatedEventArgs args)
        {
            // Add code to be executed when library is updated.
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return Settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            UpdateTemplateFolders();
            SettingsView = new HtmlExporterPluginSettingsView(this);
            return SettingsView;
        }
    }
}