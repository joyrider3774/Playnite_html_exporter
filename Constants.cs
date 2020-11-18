using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HtmlExporterPlugin
{
    static class Constants
    {
        private static IResourceProvider resources = new ResourceProvider();


        
        public static string NotGroupedFieldSanitizedName = resources.GetString("LOCNotGroupedSanitezedName");
        public static string UndefinedString = resources.GetString("LOCUndefinedString"); 
        public static string NotPlayedText = resources.GetString("LOCNotPlayedText");
        public static string NeverText = resources.GetString("LOCNeverText"); 
        public static string NoneText = resources.GetString("LOCNoneText"); 
        public static string AscendingText = resources.GetString("LOCAscendingText");
        public static string DescendingText = resources.GetString("LOCDescendingText");
       

        public const string AppName = "HTML Exporter";
        public const string NumberSign = "#";
        public static string HTMLExportError = resources.GetString("LOCHTMLExportError");

        public const string MediaBackgroundText = "Background";
        public const string MediaIconText = "Icon";
        public const string MediaCoverText = "Cover";

        //fields, declare once, so we don't have to keep repeating the string, just in case they ever change
        public static string NotGroupedField = resources.GetString("LOCNotGroupedField");
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

        //Texts for fields
        public static string RegionFieldText = resources.GetString("LOCRegionFieldText");
        public static string AddedFieldText = resources.GetString("LOCAddedFieldText");
        public static string CategoryFieldText = resources.GetString("LOCCategoryFieldText");
        public static string DeveloperFieldText = resources.GetString("LOCDeveloperFieldText");
        public static string FavoriteFieldText = resources.GetString("LOCFavoriteFieldText");
        public static string FeatureFieldText = resources.GetString("LOCFeatureFieldText");
        public static string GenreFieldText = resources.GetString("LOCGenreFieldText");
        public static string HiddenFieldText = resources.GetString("LOCHiddenFieldText");
        public static string ModifiedFieldText = resources.GetString("LOCModifiedFieldText");
        public static string NameFieldText = resources.GetString("LOCNameFieldText");
        public static string PlatformFieldText = resources.GetString("LOCPlatformFieldText");
        public static string PlaytimeFieldText = resources.GetString("LOCPlaytimeFieldText");
        public static string PublisherFieldText = resources.GetString("LOCPublisherFieldText");
        public static string SerieFieldText = resources.GetString("LOCSerieFieldText");
        public static string SourceFieldText = resources.GetString("LOCSourceFieldText");
        public static string ReleaseYearFieldText = resources.GetString("LOCReleaseYearFieldText");
        public static string AgeRatingFieldText = resources.GetString("LOCAgeRatingFieldText");
        public static string LastActivitySegmentFieldText = resources.GetString("LOCLastActivitySegmentFieldText");
        public static string PlaytimeCategoryFieldText = resources.GetString("LOCPlaytimeCategoryFieldText");
        public static string CompletionStatusFieldText = resources.GetString("LOCCompletionStatusFieldText");
        public static string UserScoreFieldText = resources.GetString("LOCUserScoreFieldText");
        public static string UserScoreGroupFieldText = resources.GetString("LOCUserScoreGroupFieldText");
        public static string UserScoreRatingFieldText = resources.GetString("LOCUserScoreRatingFieldText");
        public static string CommunityScoreFieldText = resources.GetString("LOCCommunityScoreFieldText");
        public static string CommunityScoreGroupFieldText = resources.GetString("LOCCommunityScoreGroupFieldText");
        public static string CommunityScoreRatingFieldText = resources.GetString("LOCCommunityScoreRatingFieldText");
        public static string CriticScoreFieldText = resources.GetString("LOCCriticScoreFieldText");
        public static string CriticScoreGroupFieldText = resources.GetString("LOCCriticScoreGroupFieldText");
        public static string CriticScoreRatingFieldText = resources.GetString("LOCCriticScoreRatingFieldText");
        public static string AddedSegmentFieldText = resources.GetString("LOCAddedSegmentFieldText");
        public static string InstallationStatusFieldText = resources.GetString("LOCInstallationStatusFieldText");
        public static string IsCustomGameFieldText = resources.GetString("LOCIsCustomGameFieldText");
        public static string ModifiedSegmentFieldText = resources.GetString("LOCModifiedSegmentFieldText");
        public static string PlayCountFieldText = resources.GetString("LOCPlayCountFieldText");
        public static string ReleaseDateFieldText = resources.GetString("LOCReleaseDateFieldText");
        public static string LastActivityFieldText = resources.GetString("LOCLastActivityFieldText");

        //Completionstatustext
        public static string CompletionStatusAbandoned = resources.GetString("LOCCompletionStatusAbandoned");
        public static string CompletionStatusBeaten = resources.GetString("LOCCompletionStatusBeaten");
        public static string CompletionStatusCompleted = resources.GetString("LOCCompletionStatusCompleted");
        public static string CompletionStatusNotPlayed = resources.GetString("LOCCompletionStatusNotPlayed");
        public static string CompletionStatusOnHold = resources.GetString("LOCCompletionStatusOnHold");
        public static string CompletionStatusPlayed = resources.GetString("LOCCompletionStatusPlayed");
        public static string CompletionStatusPlaying = resources.GetString("LOCCompletionStatusPlaying");
        public static string CompletionStatusPlanToPlay = resources.GetString("LOCCompletionStatusPlanToPlay");

        //LOCPlaytimeCategory
        public static string PlaytimeCategoryO1000plus = resources.GetString("LOCPlaytimeCategoryO1000plus");
        public static string PlaytimeCategoryO500_1000 = resources.GetString("LOCPlaytimeCategoryO500_1000");
        public static string PlaytimeCategoryO100_500 = resources.GetString("LOCPlaytimeCategoryO100_500");
        public static string PlaytimeCategoryO10_100 = resources.GetString("LOCPlaytimeCategoryO10_100");
        public static string PlaytimeCategoryO1_10 = resources.GetString("LOCPlaytimeCategoryO1_10");
        public static string PlaytimeCategoryNotPlayed = resources.GetString("LOCPlaytimeCategoryO1_10");
        public static string PlaytimeCategoryLessThenHour = resources.GetString("LOCPlaytimeCategoryLessThenHour");

        //DateSegMentField
        public static string SegmentFieldMoreThenYear = resources.GetString("LOCSegmentFieldMoreThenYear");
        public static string SegmentFieldPastMonth = resources.GetString("LOCSegmentFieldPastMonth");
        public static string SegmentFieldPastWeek = resources.GetString("LOCSegmentFieldPastWeek");
        public static string SegmentFieldPastYear = resources.GetString("LOCSegmentFieldPastYear");
        public static string SegmentFieldNever = resources.GetString("LOCSegmentFieldNever");
        public static string SegmentFieldToday = resources.GetString("LOCSegmentFieldToday");
        public static string SegmentFieldYesterday = resources.GetString("LOCSegmentFieldYesterday");
       
        //InstallationStatus
        public static string InstallationStatusInstalled = resources.GetString("LOCInstallationStatusInstalled");
        public static string InstallationStatusUninstalled = resources.GetString("LOCInstallationStatusUninstalled");
        
        //scoregroup
        public static string ScoreGroupO0x = resources.GetString("LOCScoreGroupO0x");
        public static string ScoreGroupO1x = resources.GetString("LOCScoreGroupO1x");
        public static string ScoreGroupO2x = resources.GetString("LOCScoreGroupO2x");
        public static string ScoreGroupO3x = resources.GetString("LOCScoreGroupO3x");
        public static string ScoreGroupO4x = resources.GetString("LOCScoreGroupO4x");
        public static string ScoreGroupO5x = resources.GetString("LOCScoreGroupO5x");
        public static string ScoreGroupO6x = resources.GetString("LOCScoreGroupO6x");
        public static string ScoreGroupO7x = resources.GetString("LOCScoreGroupO7x");
        public static string ScoreGroupO8x = resources.GetString("LOCScoreGroupO8x");
        public static string ScoreGroupO9x = resources.GetString("LOCScoreGroupO9x");

        //ScoreRating 
        public static string ScoreRatingPositive = resources.GetString("LOCScoreRatingPositive");
        public static string ScoreRatingNone = resources.GetString("LOCScoreRatingNone");
        public static string ScoreRatingNegative = resources.GetString("LOCScoreRatingNegative");
        public static string ScoreRatingMixed = resources.GetString("LOCScoreRatingMixed");

        //progressstatus
        public static string ErasingPreviousHTML = resources.GetString("LOCErasingPreviousHTML");
        public static string PreparingGenerateHTML = resources.GetString("LOCPreparingGenerateHTML");
        public static string GeneratatingHTML = resources.GetString("LOCGeneratatingHTML");
        public static string GeneratatingHTMLOf = resources.GetString("LOCGeneratatingHTMLOf");
        public static string GeneratatingHTMLEntry = resources.GetString("LOCGeneratatingHTMEntry");

        //finish text
        public static string FinishedExportingHTML = resources.GetString("LOCFinishedExportingHTML");
        public static string FinishedExportingHTMLElapsedtime = resources.GetString("LOCFinishedExportingHTMLElapsedtime");
        public static string FinishedExportingHTMLSuccess = resources.GetString("LOCFinishedExportingHTMLSuccess");
        public static string FinishedExportingHTMLErrors = resources.GetString("LOCFinishedExportingHTMLErrors");
        public static string FinishedExportingHTMLErrorsTemplate = resources.GetString("LOCFinishedExportingHTMLErrorsTemplate");

        
        //errors
        public static string ErrorHTMLExpoterNoOutputFolder = resources.GetString("LOCErrorHTMLExpoterNoOutputFolder");

        //settingsview
        public static string RevertPagesQuestion1 = resources.GetString("LOCRevertPagesQuestion1");
        public static string RevertPagesQuestion2 = resources.GetString("LOCRevertPagesQuestion2");

        //True / False Values
        public static string HTMLExporterTrue = resources.GetString("LOCHTMLExporterTrue");
        public static string HTMLExporterFalse = resources.GetString("LOCHTMLExporterFalse");

        //HTML Constants
        public static string HTMLMainMenu = resources.GetString("LOCHTMLMainMenu");
        public static string HTMLQuickLinks = resources.GetString("LOCHTMLQuickLinks");
        public static string HTMLLinks = resources.GetString("LOCHTMLLinks");
        public static string HTMLDescription = resources.GetString("LOCHTMLDescription");
        public static string HTMLDetails = resources.GetString("LOCHTMLDetails");

        //playnite menus
        public static string HTMLExporterMenu = resources.GetString("LOCHTMLExporterMenu");
        public static string HTMLExporterSubMenu = resources.GetString("LOCHTMLExporterSubMenu");

        //lists
        public static readonly ReadOnlyCollection<string> AvailableGroupFields = new ReadOnlyCollection<string>(new List<String> {NameField, SourceField, PlatformField, ReleaseYearField, AgeRatingField, FavoriteField,
            LastActivitySegmentField, PlaytimeCategoryField, RegionField, CompletionStatusField, UserScoreField, UserScoreGroupField,
            UserScoreRatingField, CommunityScoreField, CommunityScoreGroupField, CommunityScoreRatingField, CriticScoreField, CriticScoreGroupField,
            CriticScoreRatingField, CategoryField, GenreField, AddedSegmentField, DeveloperField, FeatureField, InstallationStatusField, IsCustomGameField,
            ModifiedSegmentField, PlayCountField, PublisherField, SerieField, HiddenField, AddedField, LastActivityField, ModifiedField, ReleaseDateField, PlaytimeField, NotGroupedField});

        public static readonly ReadOnlyCollection<string> AvailableSortFields  = new ReadOnlyCollection<string>(new List<String> {NameField, SourceField, PlatformField, ReleaseYearField, AgeRatingField, FavoriteField,
            LastActivitySegmentField, PlaytimeCategoryField, RegionField, CompletionStatusField, UserScoreField, UserScoreGroupField,
            UserScoreRatingField, CommunityScoreField, CommunityScoreGroupField, CommunityScoreRatingField, CriticScoreField, CriticScoreGroupField,
            CriticScoreRatingField, AddedSegmentField, InstallationStatusField, IsCustomGameField,
            ModifiedSegmentField, PlayCountField, HiddenField, AddedField, LastActivityField, ModifiedField, ReleaseDateField, PlaytimeField});


        public static readonly ReadOnlyCollection<string> DateFields = new ReadOnlyCollection<string> (new List<String> {
          AddedField , ReleaseDateField, ModifiedField, LastActivityField });

        public static readonly ReadOnlyCollection<string> DefaultDescGroupFields = new ReadOnlyCollection<string>(new List<String> {
         AddedField , ReleaseDateField, ModifiedField, LastActivityField, PlayCountField, PlaytimeField, ReleaseYearField, UserScoreField, CriticScoreField, CommunityScoreField});


        public static readonly ReadOnlyCollection<string> FakeGameFields = new ReadOnlyCollection<string>(new List<String> {
            CategoryField, DeveloperField, GenreField, FeatureField, SerieField, PublisherField });



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

        public static string GetNameFromField(string Field)
        {
            if (Field == Constants.NotGroupedField)
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


                default:
                    return Field;
            }
        }

    }
}
