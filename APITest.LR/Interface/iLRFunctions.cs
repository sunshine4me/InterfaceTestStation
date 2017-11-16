using System;
using System.Collections.Generic;
using System.Text;

namespace APITest.LR.Interface
{
    public interface iLRFunctions
    {
        void lr_save_string(string value, string name);
        void lr_output_message(string text,params object[] attrs);
    }
}
