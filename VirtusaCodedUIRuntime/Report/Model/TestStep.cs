using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodedUI.Virtusa.Report.Model
{
    class TestStep
    {

        public String Time
        {
            get;
            set;
        }

        public String Category
        {
            get;
            set;
        }

        public String ErrImg
        {
            get;
            set;
        }

        public String ErrThumb
        {
            get;
            set;
        }

        public String Message
        {
            get;
            set;
        }

        public String Stacktrace
        {
            get;
            set;
        }

        public String CodeFile
        {
            get;
            set;
        }

        public String CodeLine
        {
            get;
            set;
        }

        public String Loglvl
        {
            get;
            set;
        }

        public bool Passed
        {
            get;
            set;
        }

        /**
         * Success Test Step
         */
        public TestStep(bool isPass, String time, String category, String message,
                String codefile, String codeline, String loglvl)
        {
            //super();
            this.Passed = isPass;
            this.Time = time;
            this.Category = category;
            this.Message = message;
            this.CodeFile = codefile;
            this.CodeLine = codeline;
            this.Loglvl = loglvl;
        }


        /**
         * Failed Test Step
         */
        public TestStep(bool isPassed, String time, String category, String errimg,
                String errthumb, String message, String stacktrace,
                String codefile, String codeline, String loglvl)
        {
            //super();
            this.Passed = isPassed;
            this.Time = time;
            this.Category = category;
            this.ErrImg = errimg;
            this.ErrThumb = errthumb;
            this.Message = message;
            this.Stacktrace = stacktrace;
            this.CodeFile = codefile;
            this.CodeLine = codeline;
            this.Loglvl = loglvl;
        }
 
    }

}
