using Latest_Staff_Portal.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class SalaryAdvanceModel
    {

        public string Loan_No { get; set; }
        public string Loan_Product_Type { get; set; }
        public string Description { get; set; }
        public string Employee_No { get; set; }
        public string Employee_Name { get; set; }
        public DateTime Application_Date { get; set; }
        public decimal Amount_Requested { get; set; }
        public string Reason { get; set; }
        public string Loan_Status { get; set; }
        public decimal Instalment { get; set; }
        public decimal Repayment { get; set; }
        public decimal Total_Repayment { get; set; }
        public decimal Period_Repayment { get; set; }
        public string External_Document_No { get; set; }
        public int Receipts { get; set; }
        public decimal Flat_Rate_Principal { get; set; }
        public decimal Flat_Rate_Interest { get; set; }
        public decimal Interest_Rate { get; set; }
        public string Interest_Calculation_Method { get; set; }
        public string Payroll_Group { get; set; }
        public bool Opening_Loan { get; set; }
        public string HELB_No { get; set; }
        public string University_Name { get; set; }
        public bool Stop_Loan { get; set; }
        public string Refinanced_From_Loan { get; set; }
        public string Date_filter { get; set; }
    }

    public class NewSalaryAdvanceViewModel
    {

        public string Employeeno { get; set; }

        public decimal Amountrequested { get; set; }

        public int Installment { get; set; }

        public string Reasontext { get; set; }

    }
}