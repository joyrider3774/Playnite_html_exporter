using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HtmlExporterPlugin
{
    static class Constants
    {
        private static IResourceProvider resources = new ResourceProvider();
        
        public static string ImageOptionsText = resources.GetString("LOC_HTMLEXPORTER_ImageOptions");
        public static string EnterValidValuesText = resources.GetString("LOC_HTMLEXPORTER_EnterValidValues");
        public static string ProcessingImagesText = resources.GetString("LOC_HTMLEXPORTER_ProcessingImages");

        public static string FailedImagesProcessText = resources.GetString("LOC_HTMLEXPORTER_FailedImagesProcess");
        public static string SuccessImagesProcessText = resources.GetString("LOC_HTMLEXPORTER_SuccessImagesProcess");
        public static string TotalImagesProcessText = resources.GetString("LOC_HTMLEXPORTER_TotalImagesProcess");


        public static string NotGroupedFieldSanitizedName = resources.GetString("LOC_HTMLEXPORTER_NotGroupedSanitezedName");
        public static string UndefinedString = resources.GetString("LOC_HTMLEXPORTER_UndefinedString"); 
        public static string NotPlayedText = resources.GetString("LOC_HTMLEXPORTER_NotPlayedText");
        public static string NeverText = resources.GetString("LOC_HTMLEXPORTER_NeverText"); 
        public static string NoneText = resources.GetString("LOC_HTMLEXPORTER_NoneText"); 
        public static string AscendingText = resources.GetString("LOC_HTMLEXPORTER_AscendingText");
        public static string DescendingText = resources.GetString("LOC_HTMLEXPORTER_DescendingText");
        public static string EraseQuestionText = resources.GetString("LOC_HTMLEXPORTER_EraseQuestion");


        public const string AppName = "HTML Exporter";
        public const string NumberSign = "#";
        public static string HTMLExportError = resources.GetString("LOC_HTMLEXPORTER_HTMLExportError");

        public const string MediaBackgroundText = "Background";
        public const string MediaIconText = "Icon";
        public const string MediaCoverText = "Cover";

        //fields, declare once, so we don't have to keep repeating the string, just in case they ever change
        public static string NotGroupedField = resources.GetString("LOC_HTMLEXPORTER_NotGroupedField");
        public const string AddedField = "Added";
        public const string AddedSegmentField = "AddedSegment";
        public const string AgeRatingField = "AgeRating";
        public const string CategoryField = "Category";
        public const string CommunityScoreField = "CommunityScore";
        public const string CommunityScoreGroupField = "CommunityScoreGroup";
        public const string CommunityScoreRatingField = "CommunityScoreRating";
        public const string CompletionStatusField = "CompletionStatus";
        public const string CriticScoreField = "CriticScore";
        public const string CriticScoreGroupField = "CriticScoreGroup";
        public const string CriticScoreRatingField = "CriticScoreRating";
        public const string DeveloperField = "Developer";
        public const string FavoriteField = "Favorite";
        public const string FeatureField = "Feature";
        public const string GenreField = "Genre";
        public const string HiddenField = "Hidden";
        public const string InstallationStatusField = "InstallationStatus";
        public const string IsCustomGameField = "IsCustomGame";
        public const string LastActivityField = "LastActivity";
        public const string LastActivitySegmentField = "LastActivitySegment";
        public const string ModifiedField = "Modified";
        public const string ModifiedSegmentField = "ModifiedSegment";
        public const string NameField = "Name";
        public const string PlatformField = "Platform";
        public const string PlayCountField = "PlayCount";
        public const string PlaytimeCategoryField = "PlaytimeCategory";
        public const string PlaytimeField = "Playtime";
        public const string PublisherField = "Publisher";
        public const string RegionField = "Region";
        public const string ReleaseDateField = "ReleaseDate";
        public const string ReleaseYearField = "ReleaseYear";
        public const string SerieField = "Serie";
        public const string SourceField = "Source";
        public const string UserScoreField = "UserScore";
        public const string UserScoreGroupField = "UserScoreGroup";
        public const string UserScoreRatingField = "UserScoreRating";
        public const string LibraryField = "Library";
        public const string TagField = "Tag";

        //Texts for fields
        public static string RegionFieldText = resources.GetString("LOC_HTMLEXPORTER_RegionFieldText");
        public static string AddedFieldText = resources.GetString("LOC_HTMLEXPORTER_AddedFieldText");
        public static string CategoryFieldText = resources.GetString("LOC_HTMLEXPORTER_CategoryFieldText");
        public static string DeveloperFieldText = resources.GetString("LOC_HTMLEXPORTER_DeveloperFieldText");
        public static string FavoriteFieldText = resources.GetString("LOC_HTMLEXPORTER_FavoriteFieldText");
        public static string FeatureFieldText = resources.GetString("LOC_HTMLEXPORTER_FeatureFieldText");
        public static string GenreFieldText = resources.GetString("LOC_HTMLEXPORTER_GenreFieldText");
        public static string HiddenFieldText = resources.GetString("LOC_HTMLEXPORTER_HiddenFieldText");
        public static string ModifiedFieldText = resources.GetString("LOC_HTMLEXPORTER_ModifiedFieldText");
        public static string NameFieldText = resources.GetString("LOC_HTMLEXPORTER_NameFieldText");
        public static string PlatformFieldText = resources.GetString("LOC_HTMLEXPORTER_PlatformFieldText");
        public static string PlaytimeFieldText = resources.GetString("LOC_HTMLEXPORTER_PlaytimeFieldText");
        public static string PublisherFieldText = resources.GetString("LOC_HTMLEXPORTER_PublisherFieldText");
        public static string SerieFieldText = resources.GetString("LOC_HTMLEXPORTER_SerieFieldText");
        public static string SourceFieldText = resources.GetString("LOC_HTMLEXPORTER_SourceFieldText");
        public static string ReleaseYearFieldText = resources.GetString("LOC_HTMLEXPORTER_ReleaseYearFieldText");
        public static string AgeRatingFieldText = resources.GetString("LOC_HTMLEXPORTER_AgeRatingFieldText");
        public static string LastActivitySegmentFieldText = resources.GetString("LOC_HTMLEXPORTER_LastActivitySegmentFieldText");
        public static string PlaytimeCategoryFieldText = resources.GetString("LOC_HTMLEXPORTER_PlaytimeCategoryFieldText");
        public static string CompletionStatusFieldText = resources.GetString("LOC_HTMLEXPORTER_CompletionStatusFieldText");
        public static string UserScoreFieldText = resources.GetString("LOC_HTMLEXPORTER_UserScoreFieldText");
        public static string UserScoreGroupFieldText = resources.GetString("LOC_HTMLEXPORTER_UserScoreGroupFieldText");
        public static string UserScoreRatingFieldText = resources.GetString("LOC_HTMLEXPORTER_UserScoreRatingFieldText");
        public static string CommunityScoreFieldText = resources.GetString("LOC_HTMLEXPORTER_CommunityScoreFieldText");
        public static string CommunityScoreGroupFieldText = resources.GetString("LOC_HTMLEXPORTER_CommunityScoreGroupFieldText");
        public static string CommunityScoreRatingFieldText = resources.GetString("LOC_HTMLEXPORTER_CommunityScoreRatingFieldText");
        public static string CriticScoreFieldText = resources.GetString("LOC_HTMLEXPORTER_CriticScoreFieldText");
        public static string CriticScoreGroupFieldText = resources.GetString("LOC_HTMLEXPORTER_CriticScoreGroupFieldText");
        public static string CriticScoreRatingFieldText = resources.GetString("LOC_HTMLEXPORTER_CriticScoreRatingFieldText");
        public static string AddedSegmentFieldText = resources.GetString("LOC_HTMLEXPORTER_AddedSegmentFieldText");
        public static string InstallationStatusFieldText = resources.GetString("LOC_HTMLEXPORTER_InstallationStatusFieldText");
        public static string IsCustomGameFieldText = resources.GetString("LOC_HTMLEXPORTER_IsCustomGameFieldText");
        public static string ModifiedSegmentFieldText = resources.GetString("LOC_HTMLEXPORTER_ModifiedSegmentFieldText");
        public static string PlayCountFieldText = resources.GetString("LOC_HTMLEXPORTER_PlayCountFieldText");
        public static string ReleaseDateFieldText = resources.GetString("LOC_HTMLEXPORTER_ReleaseDateFieldText");
        public static string LastActivityFieldText = resources.GetString("LOC_HTMLEXPORTER_LastActivityFieldText");
        public static string LibraryFieldText = resources.GetString("LOC_HTMLEXPORTER_LibraryFieldText");
        public static string TagFieldText = "Tag";

        //Completionstatustext
        public static string CompletionStatusAbandoned = resources.GetString("LOC_HTMLEXPORTER_CompletionStatusAbandoned");
        public static string CompletionStatusBeaten = resources.GetString("LOC_HTMLEXPORTER_CompletionStatusBeaten");
        public static string CompletionStatusCompleted = resources.GetString("LOC_HTMLEXPORTER_CompletionStatusCompleted");
        public static string CompletionStatusNotPlayed = resources.GetString("LOC_HTMLEXPORTER_CompletionStatusNotPlayed");
        public static string CompletionStatusOnHold = resources.GetString("LOC_HTMLEXPORTER_CompletionStatusOnHold");
        public static string CompletionStatusPlayed = resources.GetString("LOC_HTMLEXPORTER_CompletionStatusPlayed");
        public static string CompletionStatusPlaying = resources.GetString("LOC_HTMLEXPORTER_CompletionStatusPlaying");
        public static string CompletionStatusPlanToPlay = resources.GetString("LOC_HTMLEXPORTER_CompletionStatusPlanToPlay");

        //LOCPlaytimeCategory
        public static string PlaytimeCategoryO1000plus = resources.GetString("LOC_HTMLEXPORTER_PlaytimeCategoryO1000plus");
        public static string PlaytimeCategoryO500_1000 = resources.GetString("LOC_HTMLEXPORTER_PlaytimeCategoryO500_1000");
        public static string PlaytimeCategoryO100_500 = resources.GetString("LOC_HTMLEXPORTER_PlaytimeCategoryO100_500");
        public static string PlaytimeCategoryO10_100 = resources.GetString("LOC_HTMLEXPORTER_PlaytimeCategoryO10_100");
        public static string PlaytimeCategoryO1_10 = resources.GetString("LOC_HTMLEXPORTER_PlaytimeCategoryO1_10");
        public static string PlaytimeCategoryNotPlayed = resources.GetString("LOC_HTMLEXPORTER_PlaytimeCategoryNotPlayed");
        public static string PlaytimeCategoryLessThenHour = resources.GetString("LOC_HTMLEXPORTER_PlaytimeCategoryLessThenHour");

        //DateSegMentField
        public static string SegmentFieldMoreThenYear = resources.GetString("LOC_HTMLEXPORTER_SegmentFieldMoreThenYear");
        public static string SegmentFieldPastMonth = resources.GetString("LOC_HTMLEXPORTER_SegmentFieldPastMonth");
        public static string SegmentFieldPastWeek = resources.GetString("LOC_HTMLEXPORTER_SegmentFieldPastWeek");
        public static string SegmentFieldPastYear = resources.GetString("LOC_HTMLEXPORTER_SegmentFieldPastYear");
        public static string SegmentFieldNever = resources.GetString("LOC_HTMLEXPORTER_SegmentFieldNever");
        public static string SegmentFieldToday = resources.GetString("LOC_HTMLEXPORTER_SegmentFieldToday");
        public static string SegmentFieldYesterday = resources.GetString("LOC_HTMLEXPORTER_SegmentFieldYesterday");
       
        //InstallationStatus
        public static string InstallationStatusInstalled = resources.GetString("LOC_HTMLEXPORTER_InstallationStatusInstalled");
        public static string InstallationStatusUninstalled = resources.GetString("LOC_HTMLEXPORTER_InstallationStatusUninstalled");
        
        //scoregroup
        public static string ScoreGroupO0x = resources.GetString("LOC_HTMLEXPORTER_ScoreGroupO0x");
        public static string ScoreGroupO1x = resources.GetString("LOC_HTMLEXPORTER_ScoreGroupO1x");
        public static string ScoreGroupO2x = resources.GetString("LOC_HTMLEXPORTER_ScoreGroupO2x");
        public static string ScoreGroupO3x = resources.GetString("LOC_HTMLEXPORTER_ScoreGroupO3x");
        public static string ScoreGroupO4x = resources.GetString("LOC_HTMLEXPORTER_ScoreGroupO4x");
        public static string ScoreGroupO5x = resources.GetString("LOC_HTMLEXPORTER_ScoreGroupO5x");
        public static string ScoreGroupO6x = resources.GetString("LOC_HTMLEXPORTER_ScoreGroupO6x");
        public static string ScoreGroupO7x = resources.GetString("LOC_HTMLEXPORTER_ScoreGroupO7x");
        public static string ScoreGroupO8x = resources.GetString("LOC_HTMLEXPORTER_ScoreGroupO8x");
        public static string ScoreGroupO9x = resources.GetString("LOC_HTMLEXPORTER_ScoreGroupO9x");

        //ScoreRating 
        public static string ScoreRatingPositive = resources.GetString("LOC_HTMLEXPORTER_ScoreRatingPositive");
        public static string ScoreRatingNone = resources.GetString("LOC_HTMLEXPORTER_ScoreRatingNone");
        public static string ScoreRatingNegative = resources.GetString("LOC_HTMLEXPORTER_ScoreRatingNegative");
        public static string ScoreRatingMixed = resources.GetString("LOC_HTMLEXPORTER_ScoreRatingMixed");

        //progressstatus
        public static string ErasingOutputFolder = resources.GetString("LOC_HTMLEXPORTER_ErasingOutputFolder");
        public static string ErasingPreviousHTML = resources.GetString("LOC_HTMLEXPORTER_ErasingPreviousHTML");
        public static string PreparingGenerateHTML = resources.GetString("LOC_HTMLEXPORTER_PreparingGenerateHTML");
        public static string GeneratatingHTML = resources.GetString("LOC_HTMLEXPORTER_GeneratatingHTML");
        public static string GeneratatingHTMLOf = resources.GetString("LOC_HTMLEXPORTER_GeneratatingHTMLOf");
        public static string GeneratatingHTMLEntry = resources.GetString("LOC_HTMLEXPORTER_GeneratatingHTMEntry");

        //finish text
        public static string FinishedExportingHTML = resources.GetString("LOC_HTMLEXPORTER_FinishedExportingHTML");
        public static string FinishedExportingHTMLElapsedtime = resources.GetString("LOC_HTMLEXPORTER_FinishedExportingHTMLElapsedtime");
        public static string FinishedExportingHTMLSuccess = resources.GetString("LOC_HTMLEXPORTER_FinishedExportingHTMLSuccess");
        public static string FinishedExportingHTMLErrors = resources.GetString("LOC_HTMLEXPORTER_FinishedExportingHTMLErrors");
        public static string FinishedExportingHTMLErrorsTemplate = resources.GetString("LOC_HTMLEXPORTER_FinishedExportingHTMLErrorsTemplate");

        
        //errors
        public static string ErrorHTMLExporterNoOutputFolder = resources.GetString("LOC_HTMLEXPORTER_ErrorHTMLExpoterNoOutputFolder");
        public static string ErrorHTMLExporterDetectedRootDriveErase = resources.GetString("LOC_HTMLEXPORTER_DetectedRootDrive");

        //settingsview
        public static string RevertPagesQuestion1 = resources.GetString("LOC_HTMLEXPORTER_RevertPagesQuestion1");
        public static string RevertPagesQuestion2 = resources.GetString("LOC_HTMLEXPORTER_RevertPagesQuestion2");

        //True / False Values
        public static string HTMLExporterTrue = resources.GetString("LOC_HTMLEXPORTER_HTMLExporterTrue");
        public static string HTMLExporterFalse = resources.GetString("LOC_HTMLEXPORTER_HTMLExporterFalse");

        //HTML Constants
        public static string HTMLMainMenu = resources.GetString("LOC_HTMLEXPORTER_HTMLMainMenu");
        public static string HTMLQuickLinks = resources.GetString("LOC_HTMLEXPORTER_HTMLQuickLinks");
        public static string HTMLLinks = resources.GetString("LOC_HTMLEXPORTER_HTMLLinks");
        public static string HTMLDescription = resources.GetString("LOC_HTMLEXPORTER_HTMLDescription");
        public static string HTMLDetails = resources.GetString("LOC_HTMLEXPORTER_HTMLDetails");
        public static string HTMLField = resources.GetString("LOC_HTMLEXPORTER_HTMLField");
        public static string HTMLStartsWith = resources.GetString("LOC_HTMLEXPORTER_HTMLStartsWith");
        public static string HTMLContains = resources.GetString("LOC_HTMLEXPORTER_HTMLContains");
        public static string HTMLClearFilter = resources.GetString("LOC_HTMLEXPORTER_HTMLClearFilter");
        public static string HTMLValueToFilter = resources.GetString("LOC_HTMLEXPORTER_HTMLValueToFilter");
        public static string HTMLSearch = resources.GetString("LOC_HTMLEXPORTER_HTMLSearch");
        public static string HTMLTypeInAGameNameToSearch = resources.GetString("LOC_HTMLEXPORTER_HTMLTypeInAGameNameToSearch");

        //playnite menus
        public static string HTMLExporterMenu = resources.GetString("LOC_HTMLEXPORTER_HTMLExporterMenu");
        public static string HTMLExporterSubMenu = resources.GetString("LOC_HTMLEXPORTER_HTMLExporterSubMenu");
        public static string HTMLExporterSubMenuFilter = resources.GetString("LOC_HTMLEXPORTER_HTMLExporterSubMenuFilter");

        //lists
        public static readonly ReadOnlyCollection<string> AvailableGroupFields = new ReadOnlyCollection<string>(new List<String> {NameField, SourceField, LibraryField, PlatformField, ReleaseYearField, AgeRatingField, FavoriteField,
            LastActivitySegmentField, PlaytimeCategoryField, RegionField, CompletionStatusField, UserScoreField, UserScoreGroupField,
            UserScoreRatingField, CommunityScoreField, CommunityScoreGroupField, CommunityScoreRatingField, CriticScoreField, CriticScoreGroupField,
            CriticScoreRatingField, CategoryField, GenreField, AddedSegmentField, DeveloperField, FeatureField, InstallationStatusField, IsCustomGameField,
            ModifiedSegmentField, PlayCountField, PublisherField, SerieField, HiddenField, AddedField, LastActivityField, ModifiedField, ReleaseDateField,
            PlaytimeField, TagField, NotGroupedField});

        public static readonly ReadOnlyCollection<string> AvailableSortFields  = new ReadOnlyCollection<string>(new List<String> {NameField, SourceField, ReleaseYearField, FavoriteField,
            LastActivitySegmentField, PlaytimeCategoryField, CompletionStatusField, UserScoreField, UserScoreGroupField,
            UserScoreRatingField, CommunityScoreField, CommunityScoreGroupField, CommunityScoreRatingField, CriticScoreField, CriticScoreGroupField,
            CriticScoreRatingField, AddedSegmentField, InstallationStatusField, IsCustomGameField,
            ModifiedSegmentField, PlayCountField, HiddenField, AddedField, LastActivityField, ModifiedField, ReleaseDateField, PlaytimeField});


        public static readonly ReadOnlyCollection<string> DateFields = new ReadOnlyCollection<string> (new List<String> {
          AddedField , ModifiedField, LastActivityField });

        public static readonly ReadOnlyCollection<string> DefaultDescGroupFields = new ReadOnlyCollection<string>(new List<String> {
         AddedField , ReleaseDateField, ModifiedField, LastActivityField, PlayCountField, PlaytimeField, ReleaseYearField, UserScoreField, CriticScoreField, CommunityScoreField});


        public static readonly ReadOnlyCollection<string> FakeGameFields = new ReadOnlyCollection<string>(new List<String> {
            CategoryField, DeveloperField, GenreField, FeatureField, SerieField, PublisherField, LibraryField, PlatformField, AgeRatingField, RegionField, TagField });

        public static Dictionary<Guid, string> LibraryList { get; } = new Dictionary<Guid, string>
        {
            { Guid.Parse("E3C26A3D-D695-4CB7-A769-5FF7612C7EDD"), "Battle.net" },
            { Guid.Parse("0E2E793E-E0DD-4447-835C-C44A1FD506EC"), "Bethesda" },
            { Guid.Parse("00000002-DBD1-46C6-B5D0-B1BA559D10E4"), "Epic" },
            { Guid.Parse("AEBE8B7C-6DC3-4A66-AF31-E7375C6B5E9E"), "Gog" },
            { Guid.Parse("00000001-EBB2-4EEC-ABCB-7C89937A42BB"), "Itch.io" },
            { Guid.Parse("85DD7072-2F20-4E76-A007-41035E390724"), "Origin" },
            { Guid.Parse("CB91DFC9-B977-43BF-8E70-55F46E410FAB"), "Steam" },
            { Guid.Parse("E2A7D494-C138-489D-BB3F-1D786BEEB675"), "Twitch" },
            { Guid.Parse("C2F038E5-8B92-4877-91F1-DA9094155FC5"), "Ubisoft Connect" },
            { Guid.Parse("96e8c4bc-ec5c-4c8b-87e7-18ee5a690626"), "Humble"},
            { Guid.Parse("7e4fbb5e-2ae3-48d4-8ba0-6b30e7a4e287"), "Xbox" },
            { Guid.Parse("402674cd-4af6-4886-b6ec-0e695bfa0688"), "Amazon Games" },
            { Guid.Parse("e4ac81cb-1b1a-4ec9-8639-9a9633989a71"), "PlayStation" },
            { Guid.Parse("f7da6eb0-17d7-497c-92fd-347050914954"), "Indiegala" },                                //by lacro59
            { Guid.Parse("77346DD6-B0CC-4F7D-80F0-C1D138CCAE58"), "Occulus" },                                  //by Jesibu & Ransel
            { Guid.Parse("88409022-088a-4de8-805a-fdbac291f00a"), "Rockstar" },                                 //by crow
            { Guid.Parse("2366fb38-bf25-45ea-9a78-dcc797ee83c3"), "Importer for AniList" },                     //by darklinkpower
            { Guid.Parse("5befaf7a-a0af-4ce2-992c-bc048d94e71b"), "XCloud" },                                   //by joyrider3774
            { Guid.Parse("e4ac81cb-1b1a-4ec9-8639-9a9633989a72"), "Nintendo" },                                 //by Xenor
            { Guid.Parse("d0217e44-0df5-45f7-8515-478bdf21a883"), "Battlestate Games (Escape from Tarkov)" },   //by Aliaksandr Dzik (DrinkFromTheCup)
            { Guid.Parse("317a5e2e-eac1-48bc-adb3-fb9e321afd3f"), "Riot Launcher" },                            //by Aliaksandr Dzik (DrinkFromTheCup)
            { Guid.Parse("3bf61d13-9730-442a-b9c0-f7228c8cd529"), "Space Station 13 (BYOND)" },                 //by Aliaksandr Dzik (DrinkFromTheCup)
            { Guid.Parse("ef6a8a8d-7d89-4a26-be54-a6584e33b478"), "Last Epoch" },                               //by Aliaksandr Dzik (DrinkFromTheCup)
            { Guid.Parse("a21f1ec0-9a7f-4adb-b8a5-78587ecb5394"), "Path of Exile" },                            //by Aliaksandr Dzik (DrinkFromTheCup)
            { Guid.Parse("5a25b200-7283-40a2-ad36-51abf4eb7ca0"), "Paradox" },                                  //by Aliaksandr Dzik (DrinkFromTheCup)
            { Guid.Parse("ef17cc27-95d4-45e7-bd49-214ba2e5f4b2"), "Fanatical" },                                //by NosyN00b
            { Guid.Parse("6ab2531e-4800-404b-a938-4421b28a9f3e"), "MultiMC" },                                  //by Maxine
            { Guid.Parse("62ac4052-e08a-4a1a-b70a-c2c0c3673bb9"), "Humble Keys Library" },                      //by Fierce Punch Studios
            { Guid.Parse("41e49490-0583-4148-94d2-940c7c74f1d9"), "EmuLibrary" },                               //by Nicholas Hastings (psychonic)
            { Guid.Parse("34c3178f-6e1d-4e27-8885-99d4f031b168"), "Legacy Games" },                             //by Jesibu
            { Guid.Parse("5f2dfd12-5f13-46fe-bcdd-64eb53ace26a"), "NVIDIA GeForce NOW" },                       //by darklinkpower
            { Guid.Parse("d407a620-5953-4ca4-a25c-8194c8559381"), "JAST USA" },                                 //by darklinkpower
            { Guid.Parse("42ff71aa-34dc-4b12-86b6-1328136c958f"), "Groupees" }                                  //by Jesibu
        };

        public static string SantizeValue(string value, string field)
        {
            if (String.IsNullOrEmpty(value))
            {
                switch (field)
                {
                    case Constants.PlaytimeField:
                    case Constants.PlaytimeCategoryField:
                    case Constants.CompletionStatusField:
                    case Constants.PlayCountField:
                        return Constants.NotPlayedText;
                    case Constants.InstallationStatusField:
                        return Constants.InstallationStatusInstalled;
                    case Constants.ModifiedField:
                    case Constants.LastActivityField:
                        return Constants.NeverText;
                    default:
                        return Constants.NoneText;
                }
            }
            else
            {
                if ((field != Constants.UserScoreGroupField) &&
                    (field != Constants.CommunityScoreGroupField) &&
                    (field != Constants.CriticScoreGroupField) &&
                    (field != Constants.LastActivitySegmentField) &&
                    (field != Constants.AddedSegmentField) &&
                    (field != Constants.ModifiedSegmentField) &&
                    (field != Constants.CompletionStatusField) &&
                    (field != Constants.PlaytimeCategoryField)&&
                    (field != Constants.UserScoreRatingField) &&
                    (field != Constants.CommunityScoreRatingField) &&
                    (field != Constants.CriticScoreRatingField) &&
                    (field != Constants.InstallationStatusField) &&
                    (field != Constants.FavoriteField) &&
                    (field != Constants.HiddenField) &&
                    (field != Constants.IsCustomGameField))
                {
                    return value;
                }
                else
                {
                    switch (field)
                    {

                        case Constants.FavoriteField:
                        case Constants.HiddenField:
                        case Constants.IsCustomGameField:
                            switch (value)
                            {
                                case "True":
                                    return Constants.HTMLExporterTrue;
                                case "False":
                                    return Constants.HTMLExporterFalse;
                                default:
                                    return value;
                            }

                        case Constants.InstallationStatusField:
                            switch (value)
                            {
                                case "Uninstalled":
                                    return Constants.InstallationStatusUninstalled;
                                case "Installed":
                                    return Constants.InstallationStatusInstalled;
                                default:
                                    return value;
                            }    

                        case Constants.UserScoreRatingField:
                        case Constants.CommunityScoreRatingField:
                        case Constants.CriticScoreRatingField:
                            switch (value)
                            {
                                case "Positive":
                                    return Constants.ScoreRatingPositive;
                                case "None":
                                    return Constants.ScoreRatingNone;
                                case "Negative":
                                    return Constants.ScoreRatingNegative;
                                case "Mixed":
                                    return Constants.ScoreRatingMixed;
                                default:
                                    return value;
                            }

                        case Constants.UserScoreGroupField:
                        case Constants.CommunityScoreGroupField:
                        case Constants.CriticScoreGroupField:
                            switch (value)
                            {
                                case "O0x":
                                    return Constants.ScoreGroupO1x;
                                case "O1x":
                                    return Constants.ScoreGroupO1x;
                                case "O2x":
                                    return Constants.ScoreGroupO2x;
                                case "O3x":
                                    return Constants.ScoreGroupO3x;
                                case "O4x":
                                    return Constants.ScoreGroupO4x;
                                case "O5x":
                                    return Constants.ScoreGroupO5x;
                                case "O6x":
                                    return Constants.ScoreGroupO6x;
                                case "O7x":
                                    return Constants.ScoreGroupO7x;
                                case "O8x":
                                    return Constants.ScoreGroupO8x;
                                case "O9x":
                                    return Constants.ScoreGroupO9x;
                                case "None":
                                    return Constants.NoneText;
                                default:
                                    return value;
                            }


                        case Constants.LastActivitySegmentField:
                        case Constants.AddedSegmentField:
                        case Constants.ModifiedSegmentField:
                            switch (value)
                            {
                                case "MoreThenYear":
                                    return Constants.SegmentFieldMoreThenYear;
                                case "PastMonth":
                                    return Constants.SegmentFieldPastMonth;
                                case "PastWeek":
                                    return Constants.SegmentFieldPastWeek;
                                case "PastYear":
                                    return Constants.SegmentFieldPastYear;
                                case "Never":
                                    return Constants.SegmentFieldNever;
                                case "Today":
                                    return Constants.SegmentFieldToday;
                                case "Yesterday":
                                    return Constants.SegmentFieldYesterday;
                                default:
                                    return value;
                            }



                        case Constants.PlaytimeCategoryField:
                            switch (value)
                            {
                                case "O500_1000":
                                    return Constants.PlaytimeCategoryO500_1000;
                                case "O1000plus":
                                    return Constants.PlaytimeCategoryO1000plus;
                                case "O100_500":
                                    return Constants.PlaytimeCategoryO100_500;
                                case "O10_100":
                                    return Constants.PlaytimeCategoryO10_100;
                                case "O1_10":
                                    return Constants.PlaytimeCategoryO1_10;
                                case "NotPlayed":
                                    return Constants.PlaytimeCategoryNotPlayed;
                                case "LessThenHour":
                                    return Constants.PlaytimeCategoryLessThenHour;
                                default:
                                    return value;
                            }


                        case Constants.CompletionStatusField:
                            switch (value)
                            {
                                case "PlanToPlay":
                                    return Constants.CompletionStatusPlanToPlay;
                                case "OnHold":
                                    return Constants.CompletionStatusOnHold;
                                case "NotPlayed":
                                    return Constants.CompletionStatusNotPlayed;
                                case "Abandoned":
                                    return Constants.CompletionStatusAbandoned;
                                case "Beaten":
                                    return Constants.CompletionStatusBeaten;
                                case "Completed":
                                    return Constants.CompletionStatusCompleted;
                                case "Played":
                                    return Constants.CompletionStatusPlayed;
                                case "Playing":
                                    return Constants.CompletionStatusPlaying;
                                default:
                                    return value;

                            }

                        default:
                            return value;
                    }
                }
            }
        }

        public static string GetNameFromField(string Field, bool convertNotGrouped = true)
        {
            if (convertNotGrouped && (Field == Constants.NotGroupedField))
            {
                return Constants.NotGroupedFieldSanitizedName;
            }

            switch (Field)
            {
                case Constants.ReleaseYearField:
                    return Constants.ReleaseYearFieldText;
                case Constants.AgeRatingField:
                    return Constants.AgeRatingFieldText;
                case Constants.LastActivitySegmentField:
                    return Constants.LastActivitySegmentFieldText;
                case Constants.PlaytimeCategoryField:
                    return Constants.PlaytimeCategoryFieldText;
                case Constants.CompletionStatusField:
                    return Constants.CompletionStatusFieldText;
                case Constants.UserScoreField:
                    return Constants.UserScoreFieldText;
                case Constants.UserScoreGroupField:
                    return Constants.UserScoreGroupFieldText;
                case Constants.UserScoreRatingField:
                    return Constants.UserScoreRatingFieldText;
                case Constants.CommunityScoreField:
                    return Constants.CommunityScoreFieldText;
                case Constants.CommunityScoreGroupField:
                    return Constants.CommunityScoreGroupFieldText;
                case Constants.CommunityScoreRatingField:
                    return Constants.CommunityScoreRatingFieldText;
                case Constants.CriticScoreField:
                    return Constants.CriticScoreFieldText;
                case Constants.CriticScoreGroupField:
                    return Constants.CriticScoreGroupFieldText;
                case Constants.CriticScoreRatingField:
                    return Constants.CriticScoreRatingFieldText;
                case Constants.AddedSegmentField:
                    return Constants.AddedSegmentFieldText;
                case Constants.InstallationStatusField:
                    return Constants.InstallationStatusFieldText;
                case Constants.IsCustomGameField:
                    return Constants.IsCustomGameFieldText;
                case Constants.ModifiedSegmentField:
                    return Constants.ModifiedSegmentFieldText;
                case Constants.PlayCountField:
                    return Constants.PlayCountFieldText;
                case Constants.ReleaseDateField:
                    return Constants.ReleaseDateFieldText;
                case Constants.LastActivityField:
                    return Constants.LastActivityFieldText;
                case Constants.RegionField:
                    return Constants.RegionFieldText;
                case Constants.AddedField:
                    return Constants.AddedFieldText;
                case Constants.CategoryField:
                    return Constants.CategoryFieldText;
                case Constants.DeveloperField:
                    return Constants.DeveloperFieldText;
                case Constants.FavoriteField:
                    return Constants.FavoriteFieldText;
                case Constants.FeatureField:
                    return Constants.FeatureFieldText;
                case Constants.GenreField:
                    return Constants.GenreFieldText;
                case Constants.HiddenField:
                    return Constants.HiddenFieldText;
                case Constants.ModifiedField:
                    return Constants.ModifiedFieldText;
                case Constants.NameField:
                    return Constants.NameFieldText;
                case Constants.PlatformField:
                    return Constants.PlatformFieldText;
                case Constants.PlaytimeField:
                    return Constants.PlaytimeFieldText;
                case Constants.PublisherField:
                    return Constants.PublisherFieldText;
                case Constants.SerieField:
                    return Constants.SerieFieldText;
                case Constants.SourceField:
                    return Constants.SourceFieldText;
                case Constants.LibraryField:
                    return Constants.LibraryFieldText;
                case Constants.TagField:
                    return Constants.TagFieldText;


                default:
                    return Field;
            }
        }

        public static string GetFieldFromName(string FieldText, bool convertNotGrouped = true)
        {
            if (convertNotGrouped && (FieldText == Constants.NotGroupedFieldSanitizedName))
            {
                return Constants.NotGroupedField;
            }
            if (FieldText == Constants.ReleaseYearFieldText)
            {
                return Constants.ReleaseYearField;
            }
            if (FieldText == Constants.AgeRatingFieldText)
            {
                return Constants.AgeRatingField;
            }
            if (FieldText == Constants.LastActivitySegmentFieldText)
            {
                return Constants.LastActivitySegmentField;
            }
            if (FieldText == Constants.PlaytimeCategoryFieldText)
            {
                return Constants.PlaytimeCategoryField;
            }
            if (FieldText == Constants.CompletionStatusFieldText)
            {
                return Constants.CompletionStatusField;
            }
            if (FieldText == Constants.UserScoreFieldText)
            {
                return Constants.UserScoreField;
            }
            if (FieldText == Constants.UserScoreGroupFieldText)
            {
                return Constants.UserScoreGroupField;
            }
            if (FieldText == Constants.UserScoreRatingFieldText)
            {
                return Constants.UserScoreRatingField;
            }
            if (FieldText == Constants.CommunityScoreFieldText)
            {
                return Constants.CommunityScoreField;
            }
            if (FieldText == Constants.CommunityScoreGroupFieldText)
            {
                return Constants.CommunityScoreGroupField;
            }
            if (FieldText == Constants.CommunityScoreRatingFieldText)
            {
                return Constants.CommunityScoreRatingField;
            }
            if (FieldText == Constants.CriticScoreFieldText)
            {
                return Constants.CriticScoreField;
            }
            if (FieldText == Constants.CriticScoreGroupFieldText)
            {
                return Constants.CriticScoreGroupField;
            }
            if (FieldText == Constants.CriticScoreRatingFieldText)
            {
                return Constants.CriticScoreRatingField;
            }
            if (FieldText == Constants.AddedSegmentFieldText)
            {
                return Constants.AddedSegmentField;
            }
            if (FieldText == Constants.InstallationStatusFieldText)
            {
                return Constants.InstallationStatusField;
            }
            if (FieldText == Constants.IsCustomGameFieldText)
            {
                return Constants.IsCustomGameField;
            }
            if (FieldText == Constants.ModifiedSegmentFieldText)
            {
                return Constants.ModifiedSegmentField;
            }
            if (FieldText == Constants.PlayCountFieldText)
            {
                return Constants.PlayCountField;
            }
            if (FieldText == Constants.ReleaseDateFieldText)
            {
                return Constants.ReleaseDateField;
            }
            if (FieldText == Constants.LastActivityFieldText)
            {
                return Constants.LastActivityField;
            }
            if (FieldText == Constants.RegionFieldText)
            {
                return Constants.RegionField;
            }
            if (FieldText == Constants.AddedFieldText)
            {
                return Constants.AddedField;
            }
            if (FieldText == Constants.CategoryFieldText)
            {
                return Constants.CategoryField;
            }
            if (FieldText == Constants.DeveloperFieldText)
            {
                return Constants.DeveloperField;
            }
            if (FieldText == Constants.FavoriteFieldText)
            {
                return Constants.FavoriteField;
            }
            if (FieldText == Constants.FeatureFieldText)
            {
                return Constants.FeatureField;
            }
            if (FieldText == Constants.GenreFieldText)
            {
                return Constants.GenreField;
            }
            if (FieldText == Constants.HiddenFieldText)
            {
                return Constants.HiddenField;
            }
            if (FieldText == Constants.ModifiedFieldText)
            {
                return Constants.ModifiedField;
            }
            if (FieldText == Constants.NameFieldText)
            {
                return Constants.NameField;
            }
            if (FieldText == Constants.PlatformFieldText)
            {
                return Constants.PlatformField;
            }
            if (FieldText == Constants.PlaytimeFieldText)
            {
                return Constants.PlaytimeField;
            }
            if (FieldText == Constants.PublisherFieldText)
            {
                return Constants.PublisherField;
            }
            if (FieldText == Constants.SerieFieldText)
            {
                return Constants.SerieField;
            }
            if (FieldText == Constants.SourceFieldText)
            {
                return Constants.SourceField;
            }
            if (FieldText == Constants.LibraryFieldText)
            {
                return Constants.LibraryField;
            }
            if (FieldText == Constants.TagFieldText)
            {
                return Constants.TagField;
            }
            return FieldText;
        }
    }
}
