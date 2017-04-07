﻿/*

	This file is part of SEOMacroscope.

	Copyright 2017 Jason Holland.

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
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace SEOMacroscope
{

  /// <summary>
  /// Description of Macroscope.
  /// </summary>

  public class Macroscope
  {

    /**************************************************************************/

    // Override SuppressDebugMsg to enable/disable debugging output statically.
    public static Boolean SuppressStaticDebugMsg;

    // Set SuppressDebugMsg to enable/disable debugging output in object.
    public Boolean SuppressDebugMsg;

    private string UserAgentString;
    
    /**************************************************************************/

    static Macroscope ()
    {
      SuppressStaticDebugMsg = false;
    }

    public Macroscope ()
    {
      this.SuppressDebugMsg = false;
      
      this.UserAgentString = this._UserAgent();
    }

    /**************************************************************************/
    
    private static string GetVersion ()
    {
      string Location = Assembly.GetExecutingAssembly().Location;
      string Version = FileVersionInfo.GetVersionInfo( Location ).ProductVersion;
      return( Version );
    }

    /**************************************************************************/
    
    public string UserAgent ()
    {
      return( this.UserAgentString );
    }
    
    /**************************************************************************/
        
    private string _UserAgent ()
    {
      #if (DEBUG)
      const string MyUserAgent = "BOT";
      #else
      string Location = Assembly.GetExecutingAssembly().Location;
      string Name = FileVersionInfo.GetVersionInfo( Location ).ProductName;
      string Version = FileVersionInfo.GetVersionInfo( Location ).ProductVersion;
      string MyUserAgent = string.Format( "{0}/{1}", Name, Version );
      #endif
      return( MyUserAgent );
    }

    /**************************************************************************/
		
    [Conditional( "DEVMODE" )]
    public static void DebugMsg ( string Msg, Boolean Flag )
    {
      if( !SuppressStaticDebugMsg )
      {
        Debug.WriteLine( Msg );
      }
    }

    [Conditional( "DEVMODE" )]
    public void DebugMsg ( string Msg )
    {
      if( !SuppressDebugMsg )
      {
        Debug.WriteLine(
          string.Format(
            "TID:{0} :: {1} :: {2}",
            Thread.CurrentThread.ManagedThreadId,
            this.GetType(),
            Msg
          )
        );
      }
    }
    
    [Conditional( "DEVMODE" )]
    public void DebugMsgForced ( string Msg )
    {
      Debug.WriteLine(
        string.Format(
          "TID:{0} :: {1} :: {2}",
          Thread.CurrentThread.ManagedThreadId,
          this.GetType(),
          Msg
        )
      );
    }

    /**************************************************************************/

  }

}
