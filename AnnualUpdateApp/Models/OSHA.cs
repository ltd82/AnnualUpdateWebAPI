
namespace Pics.IGR.Models
{
    public class OSHA
    {
        public string token { get; set; }
        public string NumberOfEmployees { get; set; }
        public string NumOfHWorked { get; set; }
        public string NumOfFatalities { get; set; }
        public string NumOfCasesDayFWork { get; set; }
        public string NumOfDayFWork { get; set; }
        public string NumOfCasesTranferRestriction { get; set; }
        public string NumOfDayTranferRestriction { get; set; }
        public string NumOfOtherRecordCases { get; set; }
        public string NAICSCode { get; set; }

        public string getValues(int i)
        {

            if (i == 1) { return NumberOfEmployees; }
            else if (i == 2) { return NumOfHWorked; }
            else if (i == 3) { return NumOfFatalities; }
            else if (i == 4) { return NumOfCasesDayFWork; }
            else if (i == 5) { return NumOfDayFWork; }
            else if (i == 6) { return NumOfCasesTranferRestriction; }
            else if (i == 7) { return NumOfDayTranferRestriction; }
            else if (i == 8) { return NumOfOtherRecordCases; }
            else if (i == 9) { return NAICSCode; }
            else { return ""; }

        }

    }
}