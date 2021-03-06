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
using System.IO;
using ClosedXML.Excel;

namespace SEOMacroscope
{

  public partial class MacroscopeExcelDataExtractorReport : MacroscopeExcelReports
  {

    /**************************************************************************/

    private MacroscopeDataExtractorCssSelectors DataExtractorCssSelectors;
    private MacroscopeDataExtractorRegexes DataExtractorRegexes;
    private MacroscopeDataExtractorXpaths DataExtractorXpaths;

    /**************************************************************************/

    public MacroscopeExcelDataExtractorReport (
      MacroscopeDataExtractorCssSelectors NewDataExtractorCssSelectors,
      MacroscopeDataExtractorRegexes NewDataExtractorRegexes,
      MacroscopeDataExtractorXpaths NewDataExtractorXpaths
    )
    {

      this.DataExtractorCssSelectors = NewDataExtractorCssSelectors;
      this.DataExtractorRegexes = NewDataExtractorRegexes;
      this.DataExtractorXpaths = NewDataExtractorXpaths;

    }

    /**************************************************************************/

    public void WriteXslx (
      MacroscopeJobMaster JobMaster,
      string OutputFilename
    )
    {

      XLWorkbook Workbook = new XLWorkbook();

      this.BuildWorksheetCssSelectors( JobMaster, Workbook, "CSS Selectors" );
      this.BuildWorksheetRegularExpressions( JobMaster, Workbook, "Regular Expressions" );
      this.BuildWorksheetXpaths( JobMaster, Workbook, "XPaths" );

      try
      {
        Workbook.SaveAs( OutputFilename );
      }
      catch ( IOException )
      {
        MacroscopeSaveExcelFileException CannotSaveExcelFileException;
        CannotSaveExcelFileException = new MacroscopeSaveExcelFileException(
          string.Format( "Cannot write to Excel file at {0}", OutputFilename )
        );
        throw CannotSaveExcelFileException;
      }

    }

    /**************************************************************************/

  }

}
