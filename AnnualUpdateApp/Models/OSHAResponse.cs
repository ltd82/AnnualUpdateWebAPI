using Pics.IGR.Models;
public class OSHAResponse
{
    public bool IsError { get; set; }
    public string ErrorMessage { get; set; }

    public OSHA IGRForm { get; set; }

    public OSHAResponse()
    {
        this.IsError = false;
        this.ErrorMessage = "";
        IGRForm = new OSHA();
    }
}