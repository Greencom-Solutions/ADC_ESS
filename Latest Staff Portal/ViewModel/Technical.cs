using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class LabSampleManagement
    {

        public string Sample_ID { get; set; }
        public string Month_Date { get; set; }
        public string Month_Name { get; set; }
        public string Global_Dimension_1_Code { get; set; }
        public string Global_Dimension_2_Code { get; set; }
        public string Description { get; set; }
        public string Received_At { get; set; }
        public string Analysed_At { get; set; }
        public string Exported_At { get; set; }
        public string TurnAround_Time { get; set; }
        public string Status { get; set; }
        public string Created_By { get; set; }
        public string Staff_No { get; set; }
        public string Staff_Name { get; set; }
        public string Time_Created { get; set; }
        public string Date_Created { get; set; }
    }

    public class LabSampleMgtLines
    {

        public int Line_No { get; set; }
        public string Sample_ID { get; set; }
        public string Sample_Code { get; set; }
        public string Type_of_Sampling { get; set; }
        public string Sample_Description { get; set; }
        public string Sample_Type { get; set; }
        public string Source { get; set; }
        public string Storage_Location { get; set; }
        public string Sampling_Date { get; set; }
        public string Received_At { get; set; }
        public bool Tested { get; set; }
        public string TestedBy { get; set; }
        public string Analysed_At { get; set; }
        public string Results { get; set; }
        public string Exported_At { get; set; }
        public string TurnAround_Time { get; set; }
        public string Staff_No { get; set; }
        public string Staff_Name { get; set; }
        public string Quantity_ml { get; set; }
        public string Notes { get; set; }
        public List<SelectListItem> ListOfRegions { get; set; }
        public List<SelectListItem> ListOfEmployees { get; set; }
    }
}