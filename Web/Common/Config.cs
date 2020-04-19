using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Web.Common
{
    public interface IConfig
    {
        string successMessage();
        string errorMessage();
        string getCreatedInnerMessage();
        string getUpdatedInnerMessage();
        string getDeletedInnerMessage();
    }
    public class Config : IConfig
    {
        public Config()
        {

        }      

        public string getCreatedInnerMessage()
        {            
            return "The record is created";
        }

        public string getDeletedInnerMessage()
        {
            return "The record is deleted";
        }

        public string errorMessage()
        {            
            return "<button id='btn-error' type='button' class='btn btn-success collapse' data-toggle='modal' data-target='#modalMessageError' data-dismiss='modal'><i class='fa fa-check'></i>&nbsp;</button>";
        }

        public string successMessage()
        {            
            return "<button id='btn-success' type='button' class='btn btn-success collapse' data-toggle='modal' data-target='#modalMessage' data-dismiss='modal'><i class='fa fa-check'></i>&nbsp;</button>";
        }

        public string getUpdatedInnerMessage()
        {
            return "The record is updated";
        }
    }
}