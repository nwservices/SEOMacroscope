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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using ExCSS;
using StylesheetParser = ExCSS.Parser; // Alias class name, remove when upgrading to ExCSS 3.0
using Stylesheet = ExCSS.StyleSheet; // Alias class name, remove when upgrading to ExCSS 3.0

namespace SEOMacroscope
{

  public partial class MacroscopeDocument : Macroscope
  {

    /**************************************************************************/

    private void ConfigureCssPageRequestHeadersCallback ( HttpRequestMessage Request )
    {
      this.AuthenticateRequest( Request: Request );
    }

    /** -------------------------------------------------------------------- **/

    private async Task ProcessCssPage ()
    {

      Stopwatch TimeDuration = new Stopwatch();
      long FinalDuration;

      TimeDuration.Start();

      try
      {
        await this._ProcessCssPage();
      }
      catch ( Exception ex )
      {
        this.DebugMsg( string.Format( "ProcessCssPage :: Exception: {0}", ex.Message ) );
      }

      TimeDuration.Stop();

      FinalDuration = TimeDuration.ElapsedMilliseconds;

      if ( FinalDuration > 0 )
      {
        this.Duration = FinalDuration;
      }
      else
      {
        this.Duration = 0;
      }

    }

    /** -------------------------------------------------------------------- **/

    private async Task _ProcessCssPage ()
    {

      MacroscopeHttpTwoClient Client = this.DocCollection.GetJobMaster().GetHttpClient();
      MacroscopeHttpTwoClientResponse Response = null;
      string ResponseErrorCondition = null;

      DebugMsg( string.Format( "ProcessCssPage: {0}", "" ) );

      try
      {

        Response = await Client.Get(
          this.GetUri(),
          this.ConfigureCssPageRequestHeadersCallback,
          this.PostProcessRequestHttpHeadersCallback
        );

      }
      catch ( MacroscopeDocumentException ex )
      {
        this.DebugMsg( string.Format( "_ProcessCssPage :: MacroscopeDocumentException: {0}", ex.Message ) );
        ResponseErrorCondition = ex.Message;
        this.SetStatusCode( HttpStatusCode.BadRequest );
        this.AddRemark( "_ProcessCssPage", ex.Message );
      }
      catch ( Exception ex )
      {
        this.DebugMsg( string.Format( "_ProcessCssPage :: Exception: {0}", ex.Message ) );
        ResponseErrorCondition = ex.Message;
        this.SetStatusCode( HttpStatusCode.BadRequest );
        this.AddRemark( "_ProcessCssPage", ex.Message );
      }

      if ( Response != null )
      {

        string RawData = "";

        this.ProcessResponseHttpHeaders( Response: Response );

        /** Get Response Body ---------------------------------------------- **/

        try
        {

          DebugMsg( string.Format( "MIME TYPE: {0}", this.MimeType ) );

          RawData = Response.GetContentAsString();

          this.SetContentLength( Length: RawData.Length ); // May need to find bytes length

          this.SetWasDownloaded( true );

        }
        catch ( Exception ex )
        {
          DebugMsg( string.Format( "Exception: {0}", ex.Message ) );
          this.SetStatusCode( HttpStatusCode.Ambiguous );
          this.SetContentLength( Length: 0 );
        }

        if ( !string.IsNullOrEmpty( RawData ) )
        {

          try
          {
            StylesheetParser CssParser = new StylesheetParser();
            Stylesheet CssStylesheet = CssParser.Parse( RawData );
            this.ProcessCssOutlinks( CssStylesheet: CssStylesheet );
          }
          catch ( Exception ex )
          {
            this.DebugMsg( string.Format( "ProcessHtmlAttributeCssLinks: {0}", ex.Message ) );
            this.AddRemark( "ProcessHtmlAttributeCssLinks", ex.Message );
          }

        }
        else
        {
          DebugMsg( string.Format( "ProcessCssPage: ERROR: {0}", this.GetUrl() ) );
        }

        /** Custom Filters ------------------------------------------------- **/

        if ( !string.IsNullOrEmpty( RawData ) )
        {

          if (
            MacroscopePreferencesManager.GetCustomFiltersEnable()
            && MacroscopePreferencesManager.GetCustomFiltersApplyToCss() )
          {

            MacroscopeCustomFilters CustomFilter = this.DocCollection.GetJobMaster().GetCustomFilter();

            if ( ( CustomFilter != null ) && ( CustomFilter.IsEnabled() ) )
            {
              this.ProcessGenericCustomFiltered(
                CustomFilter: CustomFilter,
                GenericText: RawData
              );
            }

          }

        }

        /** Data Extractors ------------------------------------------------ **/

        if ( !string.IsNullOrEmpty( RawData ) )
        {

          if (
            MacroscopePreferencesManager.GetDataExtractorsEnable()
            && MacroscopePreferencesManager.GetDataExtractorsApplyToCss() )
          {
            this.ProcessGenericDataExtractors( GenericText: RawData );
          }

        }

        /** Title ---------------------------------------------------------- **/

        {
          MatchCollection reMatches = Regex.Matches( this.DocUrl, "/([^/]+)$" );
          string DocumentTitle = null;
          foreach ( Match match in reMatches )
          {
            if ( match.Groups[ 1 ].Value.Length > 0 )
            {
              DocumentTitle = match.Groups[ 1 ].Value.ToString();
              break;
            }
          }
          if ( DocumentTitle != null )
          {
            this.SetTitle( DocumentTitle, MacroscopeConstants.TextProcessingMode.NO_PROCESSING );
            DebugMsg( string.Format( "TITLE: {0}", this.GetTitle() ) );
          }
          else
          {
            DebugMsg( string.Format( "TITLE: {0}", "MISSING" ) );
          }
        }

      }

      if ( ResponseErrorCondition != null )
      {
        this.ProcessErrorCondition( ResponseErrorCondition );
      }

    }

    /**************************************************************************/

    private void ProcessCssOutlinks ( Stylesheet CssStylesheet )
    {

      if( this.GetIsExternal() )
      {
        return;
      }

      foreach( var CssRule in CssStylesheet.StyleRules )
      {

        int iRule = CssStylesheet.StyleRules.IndexOf( CssRule );

        foreach( Property pProp in CssStylesheet.StyleRules[ iRule ].Declarations.Properties )
        {

          string BackgroundImageUrl;
          string LinkUrlAbs;

          switch( pProp.Name.ToLower() )
          {

            case "background-image":

              if( pProp.Term != null )
              {

                BackgroundImageUrl = pProp.Term.ToString();
                LinkUrlAbs = this.ProcessCssBackImageUrl( BackgroundImageUrl );

                DebugMsg( string.Format( "ProcessCssHyperlinksOut: (background-image): {0}", BackgroundImageUrl ) );
                DebugMsg( string.Format( "ProcessCssHyperlinksOut: (background-image): {0}", LinkUrlAbs ) );

                if( LinkUrlAbs != null )
                {

                  MacroscopeHyperlinkOut HyperlinkOut = null;
                  MacroscopeLink Outlink = null;

                  HyperlinkOut = this.HyperlinksOut.Add(
                    LinkType: MacroscopeConstants.HyperlinkType.CSS,
                    UrlTarget: LinkUrlAbs
                  );

                  Outlink = this.AddDocumentOutlink(
                    AbsoluteUrl: LinkUrlAbs,
                    LinkType: MacroscopeConstants.InOutLinkType.IMAGE,
                    Follow: true
                  );

                  if( Outlink != null )
                  {
                    Outlink.SetRawTargetUrl( BackgroundImageUrl );
                  }

                }

              }

              break;

            case "background":

              if( pProp.Term != null )
              {

                BackgroundImageUrl = pProp.Term.ToString();
                LinkUrlAbs = this.ProcessCssBackImageUrl( BackgroundImageUrl );

                DebugMsg( string.Format( "ProcessCssHyperlinksOut: (background): {0}", BackgroundImageUrl ) );
                DebugMsg( string.Format( "ProcessCssHyperlinksOut: (background): {0}", LinkUrlAbs ) );

                if( LinkUrlAbs != null )
                {

                  MacroscopeHyperlinkOut HyperlinkOut = null;
                  MacroscopeLink Outlink = null;

                  HyperlinkOut = this.HyperlinksOut.Add(
                    LinkType: MacroscopeConstants.HyperlinkType.CSS,
                    UrlTarget: LinkUrlAbs
                  );

                  Outlink = this.AddDocumentOutlink(
                    AbsoluteUrl: LinkUrlAbs,
                    LinkType: MacroscopeConstants.InOutLinkType.IMAGE,
                    Follow: true
                  );

                  if( Outlink != null )
                  {
                    Outlink.SetRawTargetUrl( BackgroundImageUrl );
                  }

                }

              }

              break;

            default:
              break;

          }

        }

      }

    }

    /**************************************************************************/

    private string ProcessCssBackImageUrl ( string BackgroundImageUrl )
    {

      string LinkUrlAbs = null;
      string LinkUrlCleaned = MacroscopeHttpUrlUtils.CleanUrlCss( BackgroundImageUrl );

      if ( LinkUrlCleaned != null )
      {

        try
        {
          LinkUrlAbs = MacroscopeHttpUrlUtils.MakeUrlAbsolute(
            BaseUrl: this.DocUrl,
            Url: LinkUrlCleaned
          );
        }
        catch ( MacroscopeUriFormatException ex )
        {
          DebugMsg( string.Format( "ProcessCssBackImageUrl: {0}", ex.Message ) );
        }

        DebugMsg( string.Format( "ProcessCssBackImageUrl: {0}", LinkUrlCleaned ) );
        DebugMsg( string.Format( "ProcessCssBackImageUrl: this.DocUrl: {0}", this.DocUrl ) );
        DebugMsg( string.Format( "ProcessCssBackImageUrl: LinkUrlAbs: {0}", LinkUrlAbs ) );

      }

      return ( LinkUrlAbs );

    }

    /**************************************************************************/

  }

}
