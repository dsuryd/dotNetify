using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;
using DotNetify;

namespace ViewModels
{
   /// <summary>
   /// This class demonstrates the application of dynamic LINQ to perform incremental search on a list,
   /// and pagination tehcnique on the results.
   /// </summary>
   public class AFITop100VM : BaseVM
   {
      public List<MovieRecord> Movies
      {
         get
         {
            IEnumerable<MovieRecord> results;

            if (!String.IsNullOrEmpty(Query))
               results = _model.AllRecords.AsQueryable().Where(Query);
            else
               results = _model.AllRecords;

            return Paginate(results);
         }
      }

      public string Query
      {
         get { return Get<string>(); }
         set
         {
            if (EnableAutoComplete)
               AutoComplete(ref value);

            Set(value);

            if (IsQueryValid(value))
               Changed(() => Movies);
         }
      }

      public string QueryError
      {
         get { return Get<string>(); }
         set { Set(value); }
      }

      public bool EnableAutoComplete
      {
         get { return Get<bool>(); }
         set { Set(value); }
      }

      public string AutoCompleteCaption
      {
         get { return Get<string>(); }
         set { Set(value); }
      }

      public int[] Pagination
      {
         get { return Get<int[]>(); }
         set
         {
            Set(value);
            Page = 1;
         }
      }

      public int Page
      {
         get { return Get<int>(); }
         set
         {
            Set(value);
            Changed(() => Movies);
         }
      }

      private static int _recordsPerPage = 10;
      private static readonly List<string> _propertyNames = typeof(MovieRecord).GetTypeInfo().GetProperties().ToList().Select(i => i.Name).ToList();
      private AFITop100Model _model;
      private List<MovieRecord> _queryTest = new List<MovieRecord>();
      private int _errorPos;

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="model">Model containing AFI Top 100 movie records.</param>
      public AFITop100VM(AFITop100Model model)
      {
         _model = model;
         EnableAutoComplete = true;
         AutoCompleteCaption = "Auto-complete";
      }

      /// <summary>
      /// Returns whether a query expression is valid.
      /// If not, it will set the QueryError property.
      /// </summary>
      private bool IsQueryValid(string iQuery)
      {
         QueryError = String.Empty;
         if (String.IsNullOrEmpty(iQuery))
            return true;

         try
         {
            _queryTest.AsQueryable().Where(iQuery);
            return true;
         }
         catch (ParseException ex)
         {
            QueryError = ex.Message;
            _errorPos = ex.Position;
            return false;
         }
      }

      /// <summary>
      /// Paginates the query results.
      /// It sets the Pagination and Page properties.
      /// </summary>
      private List<MovieRecord> Paginate(IEnumerable<MovieRecord> iQueryResults)
      {
         IEnumerable<MovieRecord> results;

         // ChangedProperties is a base class property that contains a list of changed properties.
         // Here it's used to check whether user has changed the Page property value through clicking a pagination link.
         if (ChangedProperties.ContainsKey("Page"))
            results = iQueryResults.Skip(_recordsPerPage * (Page - 1)).Take(_recordsPerPage);
         else
         {
            var pageCount = (int)Math.Ceiling(iQueryResults.Count() / (double)_recordsPerPage);
            Pagination = Enumerable.Range(1, pageCount).ToArray();
            results = iQueryResults.Take(_recordsPerPage);
         }
         return results.ToList();
      }

      /// <summary>
      /// Auto-completes a query expression in response to user typing the first letter of
      /// a known model property or a LINQ method at the end of the expression.
      /// </summary>
      private void AutoComplete(ref string iQuery)
      {
         if (AutoCompleteCaption != "Auto-complete")
            AutoCompleteCaption = "Auto-complete";

         // If user isn't typing new letter, don't do auto-complete.
         if (Query != null && Query.Length > iQuery.Length)
            return;

         if (!IsQueryValid(iQuery))
         {
            if (QueryError.StartsWith("No property or field") && _errorPos == iQuery.Length - 1 && (_errorPos == 0 || iQuery[_errorPos - 1] != '.'))
            {
               var firstLetter = iQuery[_errorPos].ToString();
               var propertyName = _propertyNames.FirstOrDefault(i => i.StartsWith(firstLetter, StringComparison.OrdinalIgnoreCase));
               if (!String.IsNullOrEmpty(propertyName))
                  iQuery = iQuery.Remove(_errorPos).Insert(_errorPos, propertyName);
            }
            else if (QueryError.StartsWith("No property or field") && _errorPos > 0 && iQuery[_errorPos - 1] == '.')
            {
               var firstLetter = iQuery[_errorPos].ToString().ToUpper();
               if (firstLetter == "C")
                  iQuery = iQuery.Remove(_errorPos).Insert(_errorPos, "contains(\"\")");
               else if (firstLetter == "S")
                  iQuery = iQuery.Remove(_errorPos).Insert(_errorPos, "startsWith(\"\")");
               else if (firstLetter == "E")
                  iQuery = iQuery.Remove(_errorPos).Insert(_errorPos, "endsWith(\"\")");
               else if (firstLetter == "L")
                  iQuery = iQuery.Remove(_errorPos).Insert(_errorPos, "toLower()");
               else if (firstLetter == "U")
                  iQuery = iQuery.Remove(_errorPos).Insert(_errorPos, "toUpper()");
            }
            else if (QueryError.StartsWith("Identifier expected") && iQuery.Last() == '.')
            {
               AutoCompleteCaption = "Auto-complete: <b>c</b>ontains, <b>s</b>tartsWith, <b>e</b>ndsWith, to<b>L</b>ower, to<b>U</b>pper";
            }
            else if (QueryError.StartsWith("Syntax error") && _errorPos == iQuery.Length - 1)
            {
               var firstLetter = iQuery[_errorPos].ToString().ToUpper();
               if (firstLetter == "A")
                  iQuery = iQuery.Remove(_errorPos).Insert(_errorPos, "AND ");
               else if (firstLetter == "O")
                  iQuery = iQuery.Remove(_errorPos).Insert(_errorPos, "OR ");
            }
         }
      }
   }
}
