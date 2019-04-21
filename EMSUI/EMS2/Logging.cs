using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EMS2
{
    class Logging
    {
        //File to save logs to
        private readonly string logFile = "../../../../logs/LogFile.txt";

        public enum ErrorLevel
        {
            OFF,
            DEBUG,
            INFO,
            WARN,
            ERROR,
            FATAL,
            ALL
        }

        //Max error level to log
        private readonly ErrorLevel MaxLevel = ErrorLevel.ALL;

        //Min error level to log
        private readonly ErrorLevel MinLevel = ErrorLevel.OFF;





        /**
          * FUNCTION    : Logging
          * DESCRIPTION : Initalizes a logging class with no parameters, 
          *                 the default max is ALL and min is OFF
          * PARAMETERS  : NONE
          * RETURNS     : NONE
          */
        public Logging()
        {
        }





        /**
          * FUNCTION    : Logging
          * DESCRIPTION : Initializes a logging class with two parameters 
          *                 only error levels equal to the max(first parameter) 
          *                 and min(second parameter) are logged. If the 
          *                 max is greater than the min then throw an exception
          * PARAMETERS  : ErrorLevel max : The max error level to be logged
          *               ErrorLevel min : The min error level to be logged
          * RETURNS     : NONE
          */
        public Logging(ErrorLevel max, ErrorLevel min)
        {
            if (max < min)
            {
                throw new System.ArgumentException("The first argument value cannot be greater than the second argument, see LoggingInfo for ErrorLevel values.");
            }

            MaxLevel = max;

            MinLevel = min;
        }





        /**
          * FUNCTION    : Logging
          * DESCRIPTION : Initalizes a loggin class with one parameter, only that 
          *                 error level will be logged.
          * PARAMETERS  : ErrorLevel single : The only error level to be logged
          * RETURNS     : NONE
          */
        public Logging(ErrorLevel single)
        {
            MaxLevel = single;

            MinLevel = single;
        }





        /**
          * FUNCTION    : CheckFile
          * DESCRIPTION : Check if the file path exists, if not then create it
          * PARAMETERS  : NONE
          * RETURNS     : NONE
          */
        private void CheckFile()
        {
            if (!File.Exists(logFile))
            {
                File.Create(logFile);
            }
        }






        /**
          * FUNCTION    : Log
          * DESCRIPTION : Log when there is an exception, only required parameters are the error level, 
          *                 string and the exception, the other parameters should be left alone.
          *                 The method will not log if the message is blank or spaces.
          * PARAMETERS  : ErrorLevel errorLevel : The error level of the log to be logged
                          string message : The messeage of the log
                          Exception ex : The exception being logged
                          [CallerLineNumber] int lineNumber = 0 : The line number of where the log is being called
                          [CallerMemberName] string caller = null : The method that called the log
          * RETURNS     : NONE
          */
        public void Log(ErrorLevel errorLevel,
                        string message,
                        Exception ex,
                        [CallerLineNumber] int lineNumber = 0,
                        [CallerMemberName] string caller = null)
        {
            if (string.IsNullOrWhiteSpace(message) == false)
            {
                Log(errorLevel,
                    message + Environment.NewLine + "  Exception caught:" + Environment.NewLine + "   " + ex.ToString(),
                    lineNumber,
                    caller);
            }
        }






        /**
          * FUNCTION    : Log
          * DESCRIPTION : Log a message, only required parameters are the error level, 
          *                 and the string, the other parameters should be left alone.
          *                 The method will not log if the message is blank or spaces.
          * PARAMETERS  : ErrorLevel errorLevel : The error level of the log to be logged
                          string message : The messeage of the log
                          [CallerLineNumber] int lineNumber = 0 : The line number of where the log is being called
                          [CallerMemberName] string caller = null : The method that called the log
          * RETURNS     : NONE
          */
        public void Log(ErrorLevel errorLevel,
                        string message,
                        [CallerLineNumber] int lineNumber = 0,
                        [CallerMemberName] string caller = null)
        {
            if (string.IsNullOrWhiteSpace(message) == false && CheckLevel(errorLevel) == true)
            {
                string logMessage = (GetErrorLevelString(errorLevel) + " [" + DateTime.Now + "] Logged at line [" + lineNumber + "] in " + caller + " - " + message + Environment.NewLine);
                SaveToFile(logMessage);
            }
        }





        /**
          * FUNCTION    : SaveToFile
          * DESCRIPTION : Save the message to the log file
          * PARAMETERS  : NONE
          * RETURNS     : NONE
          */
        private void SaveToFile(string logMessage)
        {
            using (StreamWriter sw = File.AppendText(logFile))
            {
                sw.WriteLine(logMessage);
            }
        }





        /**
          * FUNCTION    : CheckLevel
          * DESCRIPTION : Check that the error level is within the desired capture levels
          * PARAMETERS  : ErrorLevel level : The error level to be tested against
          *                 the max and min error levels. If the logs error level is ALL or
          *                 OFF then the method will throw an exception
          * RETURNS     : bool : true if the error level is between or equal to the max and min error levels
          *                    : false if it is outside the max and min values
          */
        private bool CheckLevel(ErrorLevel level)

        {
            bool returnValue = false;

            if (level == ErrorLevel.ALL || level == ErrorLevel.OFF)
            {
                throw new System.ArgumentException("A message being logged can not have an error level equal to OFF or ALL.");
            }

            else if (MinLevel <= level && level <= MaxLevel)
            {
                returnValue = true;
            }

            return returnValue;
        }






        /**
          * FUNCTION    : GetErrorLevelString
          * DESCRIPTION : Used for generating the log message, gets a string for the corresponding error level
          * PARAMETERS  : ErrorLevel errorLevel : The error level that is to be converted to a string
          * RETURNS     : String : The string version of errorLevel
          */
        private string GetErrorLevelString(ErrorLevel errorLevel)
        {
            string returnString = "";

            int errorIntValue = (int)errorLevel;

            switch (errorIntValue)
            {
                case 1:
                    returnString = "DEBUG";
                    break;
                case 2:
                    returnString = "INFO ";
                    break;
                case 3:
                    returnString = "WARN ";
                    break;
                case 4:
                    returnString = "ERROR";
                    break;
                case 5:
                    returnString = "FATAL";
                    break;
            }

            return returnString;
        }
    }
}
