﻿/*

  This file is part of SEOMacroscope.

  Copyright 2018 Jason Holland.

  The GitHub repository may be found at:

    https://github.com/nazuke/SEOMacroscope

  Foobar is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  Foobar is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Collections.Generic;
using CsvHelper;

namespace SEOMacroscope
{

  public partial class MacroscopeCsvOverviewReport : MacroscopeCsvReports
  {

    /**************************************************************************/

    private void BuildWorksheetOverview (
      MacroscopeJobMaster JobMaster,
      CsvWriter ws
    )
    {

      MacroscopeDocumentCollection DocCollection = JobMaster.GetDocCollection();

      {

        ws.WriteField( "URL" );
        ws.WriteField( "Status Code" );
        ws.WriteField( "Status" );
        ws.WriteField( "Redirect" );
        ws.WriteField( "Robots" );
        ws.WriteField( "Duration" );
        ws.WriteField( "Crawled Date" );
        ws.WriteField( "Server Date" );
        ws.WriteField( "Modified Date" );
        ws.WriteField( "Expires Date" );
        ws.WriteField( "Content-Type" );
        ws.WriteField( "Charset" );
        ws.WriteField( "Locale" );
        ws.WriteField( "Language" );
        ws.WriteField( "Canonical" );
        ws.WriteField( "Page Depth" );
        ws.WriteField( "Links In" );
        ws.WriteField( "Links Out" );
        ws.WriteField( "Hyperlinks In" );
        ws.WriteField( "Hyperlinks Out" );
        ws.WriteField( "Ration In" );
        ws.WriteField( "Ratio Out" );
        ws.WriteField( "Author" );
        ws.WriteField( "Title" );
        ws.WriteField( "Title Length" );
        ws.WriteField( "Description" );
        ws.WriteField( "Description Length" );
        ws.WriteField( "Keywords" );
        ws.WriteField( "Keywords Length" );
        ws.WriteField( "Keywords Count" );
        ws.WriteField( "Error Condition" );

        ws.NextRecord();

      }

      foreach( MacroscopeDocument msDoc in DocCollection.IterateDocuments() )
      {

        List<decimal> HyperlinkRatio = DocCollection.GetDocumentHyperlinksRatio( Url: msDoc.GetUrl() );

        this.InsertAndFormatUrlCell( ws, msDoc );

        this.InsertAndFormatStatusCodeCell( ws, msDoc );

        this.InsertAndFormatContentCell( ws, msDoc.GetStatusCode().ToString() );

        this.InsertAndFormatRedirectCell( ws, msDoc );

        this.InsertAndFormatRobotsCell( ws, msDoc );

        this.InsertAndFormatContentCell( ws, msDoc.GetDurationInSecondsFormatted() );

        this.InsertAndFormatContentCell( ws, this.FormatIfMissing( msDoc.GetCrawledDate() ) );

        this.InsertAndFormatContentCell( ws, this.FormatIfMissing( msDoc.GetDateServer() ) );

        this.InsertAndFormatContentCell( ws, this.FormatIfMissing( msDoc.GetDateModified() ) );

        this.InsertAndFormatContentCell( ws, this.FormatIfMissing( msDoc.GetDateExpires() ) );

        this.InsertAndFormatContentCell( ws, this.FormatIfMissing( msDoc.GetMimeType() ) );

        this.InsertAndFormatContentCell( ws, this.FormatIfMissing( msDoc.GetCharacterSet() ) );

        this.InsertAndFormatContentCell( ws, this.FormatIfMissing( msDoc.GetLocale() ) );

        this.InsertAndFormatContentCell( ws, this.FormatIfMissing( msDoc.GetIsoLanguageCode() ) );

        this.InsertAndFormatContentCell( ws, this.FormatIfMissing( msDoc.GetCanonical() ) );

        this.InsertAndFormatContentCell( ws, msDoc.GetDepth().ToString() );

        this.InsertAndFormatContentCell( ws, msDoc.CountInlinks().ToString() );
        this.InsertAndFormatContentCell( ws, msDoc.CountOutlinks().ToString() );

        this.InsertAndFormatContentCell( ws, msDoc.CountHyperlinksIn().ToString() );
        this.InsertAndFormatContentCell( ws, msDoc.CountHyperlinksOut().ToString() );

        this.InsertAndFormatContentCell( ws, this.FormatIfMissing( string.Format( "{0:0.00}%", HyperlinkRatio[ 0 ] ) ) );
        this.InsertAndFormatContentCell( ws, this.FormatIfMissing( string.Format( "{0:0.00}%", HyperlinkRatio[ 1 ] ) ) );

        this.InsertAndFormatContentCell( ws, msDoc.GetAuthor() );

        this.InsertAndFormatContentCell( ws, this.FormatIfMissing( msDoc.GetTitle() ) );

        this.InsertAndFormatContentCell( ws, msDoc.GetTitleLength().ToString() );

        this.InsertAndFormatContentCell( ws, this.FormatIfMissing( msDoc.GetDescription() ) );
        this.InsertAndFormatContentCell( ws, msDoc.GetDescriptionLength().ToString() );

        this.InsertAndFormatContentCell( ws, this.FormatIfMissing( msDoc.GetKeywords() ) );
        this.InsertAndFormatContentCell( ws, msDoc.GetKeywordsLength().ToString() );
        this.InsertAndFormatContentCell( ws, msDoc.GetKeywordsCount().ToString() );

        this.InsertAndFormatContentCell( ws, this.FormatIfMissing( msDoc.GetErrorCondition() ) );

        ws.NextRecord();

      }

    }

    /**************************************************************************/

  }

}
